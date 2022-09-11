using Microsoft.Extensions.Options;

using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;
using Prometheus;
using Jaeger.Reporters;
using Jaeger;
using Jaeger.Senders.Thrift;
using Jaeger.Samplers;

using BusService;

using MessageService.Middlwares;
using MessageService.Repository;
using MessageService.Repository.Interface;
using MessageService.Service.Interface;
using MessageService.JobOfferMessaging;
using MessageService.Hubs;
using MessageService.Middlewares.Events;

var builder = WebApplication.CreateBuilder(args);

// Default Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Event middleware deps
builder.Services.Configure<AppConfig>(
    builder.Configuration.GetSection("AppConfig"));

// Nats
builder.Services.Configure<MessageBusSettings>(builder.Configuration.GetSection("Nats"));
builder.Services.AddSingleton<IMessageBusSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MessageBusSettings>>().Value);
builder.Services.AddSingleton<IMessageBusService, MessageBusService>();
builder.Services.AddHostedService<MessageServiceBusHostedService>();

// Mongo
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

// Repositories
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

// Services
builder.Services.AddScoped<IConversationService, MessageService.Service.ConversationService>();

// Sync services
builder.Services.AddScoped<IProfileSyncService, MessageService.Service.ProfileSyncService>();
builder.Services.AddScoped<IEventSyncService, MessageService.Service.EventSyncService>();
builder.Services.AddScoped<IMessageSyncService, MessageService.Service.MessageSyncService>();

// Controllers
builder.Services.AddControllers();

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// SignarR
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MessageService", Version = "v1" });
});

// Tracing
builder.Services.AddOpenTracing();
builder.Services.AddSingleton<ITracer>(sp =>
{
    var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender("host.docker.internal", 6831, 0))
                    .Build();
    var tracer = new Tracer.Builder(serviceName)
        // The constant sampler reports every span.
        .WithSampler(new ConstSampler(true))
        // LoggingReporter prints every reported span to the logging framework.
        .WithLoggerFactory(loggerFactory)
        .WithReporter(reporter)
        .Build();

    GlobalTracer.Register(tracer);

    return tracer;
});

builder.Services.Configure<HttpHandlerDiagnosticOptions>(options =>
        options.OperationNameResolver =
            request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseEventSenderMiddleware();

// Prometheus metrics
app.UseMetricServer();

// SignalR
app.MapHub<ConversationHub>("/conversationHub");

app.Run();

namespace MessageService
{
    public partial class Program { }
}
