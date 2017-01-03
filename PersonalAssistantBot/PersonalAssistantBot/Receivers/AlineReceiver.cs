using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using PersonalAssistantBot.Services;

namespace PersonalAssistantBot
{
    public class AlineReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private Settings _settings;
        private DocumentService _service;

        public AlineReceiver(IMessagingHubSender sender, Settings settings)
        {
            _sender = sender;
            _settings = settings;
            _service = new DocumentService();
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            await _sender.SendMessageAsync("Pong!", message.From, cancellationToken);
        }

        private Document AlineCarrossel()
        {

        }
    }
}
