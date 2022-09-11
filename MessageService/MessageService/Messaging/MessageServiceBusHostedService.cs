using Polly;

using BusService;
using BusService.Routing;

using MessageService.Service.Interface;

namespace MessageService.JobOfferMessaging;

public class MessageServiceBusHostedService : MessageBusHostedService
{
    public MessageServiceBusHostedService(IMessageBusService serviceBus, IServiceScopeFactory serviceScopeFactory) : base(serviceBus, serviceScopeFactory)
    {
    }

    protected override void ConfigureSubscribers()
    {
        var policy = BuildPolicy();

        // Add MessageSubscriber subscribers to the list of subscribers
        Subscribers.Add(new MessageBusSubscriber(policy, SubjectBuilder.Build(Topics.Profile), typeof(IProfileSyncService)));
    }

    private Policy BuildPolicy()
    {
        return Policy
                .Handle<Exception>()
                .WaitAndRetry(5, _ => TimeSpan.FromSeconds(5), (exception, _, _, _) =>
                {
                    // TODO: here we should log unsuccessful try to handle event
                });
    }
}
