using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client.Listener;
using System.Diagnostics;
using System;
using Takenet.MessagingHub.Client.Extensions.Broadcast;

namespace PersonalAssistantBot
{
    public class Startup : IStartable
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDictionary<string, object> _settings;
        private readonly IBroadcastExtension _broadcast;

        public Startup(IMessagingHubSender sender, IDictionary<string, object> settings, IBroadcastExtension broadcast)
        {
            _sender = sender;
            _settings = settings;
            _broadcast = broadcast;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _broadcast.CreateDistributionListAsync("SubscriptionList");
            _broadcast.CreateDistributionListAsync("FreeUsersList");

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            return Task.CompletedTask;
        }
    }
}
