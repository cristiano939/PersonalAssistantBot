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
using PersonalAssistantBot.Models;
using System.Collections.Generic;
using PersonalAssistantBot.Services;
using Lime.Messaging.Resources;
using Takenet.MessagingHub.Client.Extensions.Directory;

namespace PersonalAssistantBot
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly CommomExpressionsHandler commom;
        private IDirectoryExtension _directory;
        private Settings _settings;
        private IStateManager _state;
        DocumentService _service;
        public PlainTextMessageReceiver(IMessagingHubSender sender, Settings settings, IDirectoryExtension directory, IStateManager state)
        {
            _sender = sender;
            _settings = settings;
            _directory = directory;
            _state = state;
            commom = new CommomExpressionsHandler();
            _service = new DocumentService();
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
                    await _sender.SendMessageAsync(new PlainText { Text = "eu sou o Assistente Pessoal do Cristiano Guerra,😎\n\n o que deseja saber sobre ele?" }, message.From, cancellationToken);
                    await _sender.SendMessageAsync(CreateFirstCarrossel(), message.From, cancellationToken);
                }
                else { }
            }
            else
            {
                if (await IsALine(message, cancellationToken))
                {
                    await _sender.SendMessageAsync(AlineCarrossel(), message.From, cancellationToken);
                    _state.SetState(message.From.ToIdentity(), "Aline");
                }
                else {
                    await _sender.SendMessageAsync("Ainda estou aprendendo sobre alguns assuntos,\nmas estou aqui para falar do Cristiano.\n\nEscolha o que deseja saber.", message.From, cancellationToken);
                    await _sender.SendMessageAsync(CreateFirstCarrossel(), message.From, cancellationToken);
                }
            }

        }

        private async Task<bool> IsALine(Message message, CancellationToken cancellationToken)
        {
            if (message.Content.ToString().ToLower().Contains("eu sou a aline")
                || message.Content.ToString().ToLower().Contains("sou eu amor")
                || message.Content.ToString().ToLower().Contains("aline lara")
                || message.Content.ToString().ToLower().Contains("docinho"))
            {
                return true;
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);
                if (account.FullName.Contains("Aline") && account.FullName.Contains("Lara"))
                {
                    return true;

                }
            }
            return false;
        }

        public Document CreateFirstCarrossel()
        {
            List<CarrosselCard> carrosselData = new List<CarrosselCard>();
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "Cristiano faz muitas coisas bacanas e diferentes.\nTenho muito pra mostrar!", Title = "Quer conhecer meus Hobbies?", Type = new MediaType("image", "jpeg"), Uri = new Uri(_settings.HobbiesImages[0]) }, CardContent = "Faço muitas coisas bacanas e tenho muito pra mostrar! Clique e conheça!", options = new List<string> { "Hobbies!" } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "De aplicativos, plataformas, Chatbots e websites.", Title = "Trabalho, Trabalho e Trabalho", Type = new MediaType("image", "jpeg"), Uri = new Uri(_settings.WorkImages[0]) }, CardContent = "De aplicativos, plataformas, Chatbots e websites.", options = new List<string> { "Trabalhos" } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "Marque um compromisso com Cristiano!", Title = "Agenda!", Uri = new Uri(_settings.AgendaImages[0]) }, CardContent = "Marque um compromisso com Cristiano!", options = new List<string> { "Agenda" } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });

            return _service.CreateCarrossel(carrosselData);
        }

        private Document AlineCarrossel()
        {

            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://i.ytimg.com/vi/TmQ5cSVAQ6Q/maxresdefault.jpg"), Text = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", Title = "PARABENS, MEU AMOR!", Type = new MediaType("image", "jpeg") }, CardContent = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", options = new List<string>() });
            cards.Add ( new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://erikvanslyke.files.wordpress.com/2011/05/read-instructions-caution-sign-s-2655.gif"), Text = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", Title = "Instruções!", Type = new MediaType("image", "jpeg") }, CardContent = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", options = new List<string>() });
            cards.Add ( new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://www.torontoprosupershow.com/sites/default/files//ticket.jpg"), Text = "Possui alguns vales para você usar comigo! Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", Title = "Vale tudo de bom!", Type = new MediaType("image", "jpeg") }, CardContent = "Possui alguns vales para você usar comigo! Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", options = new List<string>() });
            cards.Add ( new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://dicasdesaude.blog.br/wp-content/uploads/2015/08/4-Dicas-de-Cuidados-com-o-Cora%C3%A7%C3%A3o-Que-Voc%C3%AA-Pode-e-deve-Tomar-A-Partir-de-Hoje-Interna.jpg"), Text = "Pequenas declarações de mim para você. Todas minhas mesmo.", Title = "Te amos!", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas declarações de mim para você. Todas minhas mesmo.", options = new List<string>() });
            cards.Add ( new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://i.imgur.com/iGUdXht.jpg"), Text = "Pequenas confissões, de mim para você.", Title = "Segredos e Segredos", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas confissões, de mim para você.", options = new List<string>() });
            cards.Add ( new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://images.all-free-download.com/images/graphicthumb/love_box_vector_559244.jpg"), Text = "Quer começar? ", Title = "Começar", Type = new MediaType("image", "jpeg") }, CardContent = "Quer começar?", options = new List<string> { "Comecar" } });

            Document alineCarrossel = _service.CreateCarrossel(cards);
            return alineCarrossel;

        }

    }
}
