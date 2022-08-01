using AutoMapper;
using MessageService.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using Prometheus;

namespace MessageService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly ITracer _tracer;

    Counter counter = Metrics.CreateCounter("message_service_counter", "message service");

    public MessageController(IMessageService messageService, IMapper mapper, ITracer tracer)
    {
        _messageService = messageService;
        _mapper = mapper;
        _tracer = tracer;
    }
}

