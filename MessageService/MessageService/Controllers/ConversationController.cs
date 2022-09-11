using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using OpenTracing;
using Prometheus;

using MessageService.Repository.Interface.Pagination;
using MessageService.Service.Interface;
using MessageService.Dto;

namespace MessageService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly ILogger<ConversationController> _logger;
    private readonly IConversationService _conversationService;
    private readonly IMapper _mapper;
    private readonly ITracer _tracer;

    readonly Counter counter = Metrics.CreateCounter("message_service_counter", "message service");

    public ConversationController(IConversationService conversationService, IMapper mapper, ITracer tracer, ILogger<ConversationController> logger)
    {
        _conversationService = conversationService;
        _mapper = mapper;
        _tracer = tracer;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = "profile-id")] Guid userId, [FromQuery] PaginationParams paginationParams, string? query)
    {
        _logger.LogInformation($"Received profile id: {userId}");

        var actionName = ControllerContext.ActionDescriptor.DisplayName;
        using var scope = _tracer.BuildSpan(actionName).StartActive(true);
        scope.Span.Log("get conversation");

        counter.Inc();

        var conversationList = await _conversationService.Filter(userId, paginationParams, query);
        return Ok(conversationList.ToPagedList(_mapper.Map<List<ConversationResponse>>(conversationList.Items)));
    }
}

