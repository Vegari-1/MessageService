using Microsoft.AspNetCore.SignalR;

using MessageService.Cache;
using MessageService.Model;
using MessageService.Service.Interface;

using AutoMapper;
using MessageService.Dto;
using MongoDB.Driver.Core.Connections;
using static OpenTracing.Contrib.NetCore.Configuration.MicrosoftSqlClientDiagnosticOptions;
using BusService;
using Polly;

namespace MessageService.Hubs
{
    public class ConversationHub : Hub
    {
        private static readonly ConnectionCache<string> _userConnections = new();
        private static readonly Dictionary<string, string> _connectionUser = new();
        private static readonly object _lock = new();

        private readonly ILogger<ConversationHub> _logger;
        private readonly string MSG_RECEIVED = "messageReceived";
        private readonly string MSG_SENT = "messageSent";
        private readonly string CONVERSATION_CREATED = "conversationCreated";

        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;
        private readonly IMessageSyncService _messageSyncService;

        public ConversationHub(IConversationService conversationService, ILogger<ConversationHub> logger, 
            IMapper mapper, IMessageSyncService messageSyncService)
        {
            _conversationService = conversationService;
            _logger = logger;
            _mapper = mapper;
            _messageSyncService = messageSyncService;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()!.Request.Query["profileId"].ToString();
            _logger.LogInformation($"Received profile id: {userId}");
            lock (_lock)
            {
                _connectionUser[Context.ConnectionId] = userId;
                _userConnections.Add(userId, Context.ConnectionId);
            }
            return Task.CompletedTask;
        }

        public async Task SendMessage(string conversationId, string senderId, string content)
        {
            var conversation = await _conversationService.Get(conversationId);
            await SendMessageInternalAsync(conversation, senderId, content);
        }

        public async Task StartConversation(Model.Profile sender, Model.Profile receiver, string message)
        {
            var conversation = await _conversationService.StartConversation(sender, receiver);

            foreach (var connectionId in _userConnections.GetConnections(sender.GlobalId.ToString()))
                await Clients.Client(connectionId).SendAsync(CONVERSATION_CREATED, _mapper.Map<ConversationResponse>(conversation));

            foreach (var connectionId in _userConnections.GetConnections(receiver.GlobalId.ToString()))
                await Clients.Client(connectionId).SendAsync(CONVERSATION_CREATED, _mapper.Map<ConversationResponse>(conversation));

            await SendMessageInternalAsync(conversation, sender.GlobalId.ToString(), message);
        }

        public Task Terminate()
        {
            var userId = Context.GetHttpContext()!.Request.Query["profileId"].ToString();
            _logger.LogInformation($"Will remove profile id: {userId}");

            lock (_lock)
                _userConnections.Remove(userId, Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_lock)
            {
                var userId = _connectionUser[Context.ConnectionId];
                var connections = _userConnections.GetConnections(userId);
                _userConnections.Remove(userId, Context.ConnectionId);
                _connectionUser.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        private async Task SendMessageInternalAsync(Conversation conversation, string senderId, string content)
        {
            var message = await _conversationService.SendMessage(conversation, senderId, content);

            await _messageSyncService.PublishAsync(
                new MessageInfo(Guid.Parse(senderId), conversation.Participants.Find(x => x.GlobalId.ToString() != senderId).GlobalId),
                Events.Created);

            foreach (var user in conversation.Participants)
            {
                var userId = user.GlobalId.ToString();
                foreach (var connectionId in _userConnections.GetConnections(userId))
                {
                    var eventName = MSG_RECEIVED;
                    if (userId == senderId)
                        eventName = MSG_SENT;
                    _logger.LogInformation($"Sending: {eventName} for message {message.Id} - {message.Sender} - {message.Content}");
                    await Clients.Client(connectionId).SendAsync(eventName, conversation.Id.ToString(), _mapper.Map<MessageResponse>(message));
                }
            }
        }
    }
}
