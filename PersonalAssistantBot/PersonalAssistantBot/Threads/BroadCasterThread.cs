using PersonalAssistantBot.Services;
using PersonalAssistantBot.SubjectHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Extensions.Directory;
using Takenet.MessagingHub.Client.Sender;

namespace PersonalAssistantBot.Threads
{
    class BroadCasterThread 
    {
        private readonly IMessagingHubSender _sender;
        private readonly CommomExpressionsHandler commom;
        private IDirectoryExtension _directory;
        private Settings _settings;
        private IStateManager _state;
        private static bool isActive = false;
        DocumentService _service;

        public BroadCasterThread(IMessagingHubSender sender, Settings settings, IDirectoryExtension directory, IStateManager state)
        {
            _sender = sender;
            _settings = settings;
            _directory = directory;
            _state = state;
            commom = new CommomExpressionsHandler();
            _service = new DocumentService();

        }

        public async Task Stop()
        {
            isActive = false;
        }

        public async Task Start()
        {
         
        }

        //http://www.c-sharpcorner.com/UploadFile/f9f215/parallel-programming-part-1-introducing-task-programming-l/
    }
}
