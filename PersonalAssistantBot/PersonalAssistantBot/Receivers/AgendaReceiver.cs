using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using PersonalAssistantBot.Integrations;
using PersonalAssistantBot.Services;
using Takenet.MessagingHub.Client.Extensions.Directory;
using PersonalAssistantBot.SubjectHandlers;
using Lime.Messaging.Contents;

namespace PersonalAssistantBot
{
    public class AgendaReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly CommomExpressionsHandler commom;
        private IDirectoryExtension _directory;
        private Settings _settings;
        private IStateManager _state;

        DocumentService _service;
        private GoogleCalendarIntegration _calendar;

        public AgendaReceiver(IMessagingHubSender sender, Settings settings, IDirectoryExtension directory, IStateManager state)
        {
            _sender = sender;
            _settings = settings;
            _directory = directory;
            _state = state;
            commom = new CommomExpressionsHandler();
            _service = new DocumentService();
            _calendar = new GoogleCalendarIntegration();
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            Document doc = commom.GetCommomAnser(message.Content.ToString());
            if (doc != null)
            {
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                if (commom.IsHiMessage(message.Content.ToString()) || commom.IsHowdMessage(message.Content.ToString()))
                {
                    await _sender.SendMessageAsync(GetHiQuickReply(), message.From, cancellationToken);

                }
            }
            else if (message.Content.ToString().ToLower().Contains("Voltar"))
            {

                PlainTextMessageReceiver firstReceiver = new PlainTextMessageReceiver(_sender, _settings, _directory, _state);
                await firstReceiver.ReceiveAsync(message, cancellationToken);
                _state.SetState(message.From.ToIdentity(), "default");
            }
            else if (message.Content.ToString().ToLower().Contains("disponibilidade"))
            {
                var result = _calendar.ReadAllEvents();
            }
            else if (message.Content.ToString().ToLower().Contains("marcar"))
            {
                var result = _calendar.SetEventOnCalendar();
            }
          
            else {
                await _sender.SendMessageAsync("Ainda estou aprendendo sobre alguns assuntos,\nmas estou aqui para falar do Cristiano.\n\nEscolha o que deseja saber.", message.From, cancellationToken);
        await _sender.SendMessageAsync(GetAgendaQuickReply(), message.From, cancellationToken);
            }


}

public Document GetHiQuickReply()
{
    Select agendaSelect = new Select { Text = "Acho que já nos comprimentamos antes, não? Deseja voltar para o menu inicial?", Scope = SelectScope.Immediate };
    SelectOption[] options = new SelectOption[2];
    options[0] = new SelectOption { Text = "Não, quero continuar aqui!", Value = "Continuar" };
    options[1] = new SelectOption { Text = "Voltar", Value = "Voltar" };

    agendaSelect.Options = options;
    return agendaSelect;
}


public Document GetAgendaQuickReply()
{
    Select agendaSelect = new Select { Text = "Que quer fazer com a agenda do Sr.Cristiano?", Scope = SelectScope.Immediate };
    SelectOption[] options = new SelectOption[3];
    options[0] = new SelectOption { Text = "Marcar", Value = "Marcar" };
    options[1] = new SelectOption { Text = "Disponibilidade", Value = "Disponibilidade" };
    options[2] = new SelectOption { Text = "Nada, quero voltar", Value = "Voltar" };
    agendaSelect.Options = options;
    return agendaSelect;
}

    }
}
