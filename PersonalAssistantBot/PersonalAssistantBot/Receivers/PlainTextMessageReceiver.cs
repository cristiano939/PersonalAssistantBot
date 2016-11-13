using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using PersonalAssistantBot.PhraseContent;
using PersonalAssistantBot.SubjectHandlers;
using Lime.Messaging.Contents;

namespace PersonalAssistantBot
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly CommomExpressionsHandler commom;

        public PlainTextMessageReceiver(IMessagingHubSender sender)
        {
            _sender = sender;
            commom = new CommomExpressionsHandler();
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            Document doc = commom.GetCommomAnser(message.Content.ToString());
            if (doc != null)
            {
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync("Ainda estou aprendendo", message.From, cancellationToken);
            }
            
        }


        public DocumentCollection CreateIntroCarrossel()
        {
            DocumentCollection carrossel = new DocumentCollection();
            carrossel.Items = new DocumentSelect[8];

        }
    }
}
