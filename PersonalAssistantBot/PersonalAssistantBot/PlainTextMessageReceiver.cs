﻿using System;
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
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 5));
                if (commom.IsHiMessage(message.Content.ToString()) || commom.IsHowdMessage(message.Content.ToString()))
                {
                    await _sender.SendMessageAsync(new PlainText { Text = "eu sou o Assistente Pessoal do Cristiano Guerra,😎\n\n o que deseja saber sobre ele?" }, message.From, cancellationToken);
                    await _sender.SendMessageAsync(await CreateFirstCarrossel(message, cancellationToken), message.From, cancellationToken);
                }
                else if (message.Content.ToString().ToLower().Contains("agenda"))
                {

                    await _sender.SendMessageAsync(GetAgendaQuickReply(), message.From, cancellationToken);
                    _state.SetState(message.From.ToIdentity(), "agenda");
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
                else if (message.Content.ToString().Contains("Hobbies"))
                {
                    await _sender.SendMessageAsync(CreateHobbiesCarrossel(), message.From, cancellationToken);
                    //  _state.SetState(message.From.ToIdentity(), "hobbies");
                }
                else
                {
                    await _sender.SendMessageAsync("Ainda estou aprendendo sobre alguns assuntos,\nmas estou aqui para falar do Cristiano.\n\nEscolha o que deseja saber.", message.From, cancellationToken);
                    await _sender.SendMessageAsync(await CreateFirstCarrossel(message, cancellationToken), message.From, cancellationToken);
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

        public async Task<Document> CreateFirstCarrossel(Message message, CancellationToken cancellationToken)
        {
            List<CarrosselCard> carrosselData = new List<CarrosselCard>();
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "Cristiano faz muitas coisas bacanas e diferentes.\nTenho muito pra mostrar!", Title = "Quer conhecer meus Hobbies?", Type = new MediaType("image", "jpeg"), Uri = new Uri(_settings.HobbiesImages[0]) }, CardContent = "Faço muitas coisas bacanas e tenho muito pra mostrar! Clique e conheça!", options = new List<CarrosselOptions> { new CarrosselOptions { value = "Hobbies", label = "Hobbies" } } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "De aplicativos, plataformas, Chatbots e websites.", Title = "Trabalho, Trabalho e Trabalho", Type = new MediaType("image", "jpeg"), Uri = new Uri(_settings.WorkImages[0]) }, CardContent = "De aplicativos, plataformas, Chatbots e websites.", options = new List<CarrosselOptions> { new CarrosselOptions { value = "Trabalhos", label = "Trabalhos" } } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "Marque um compromisso com Cristiano!", Title = "Agenda!", Uri = new Uri(_settings.AgendaImages[0]) }, CardContent = "Marque um compromisso com Cristiano!", options = new List<CarrosselOptions> { new CarrosselOptions { value = "Agenda", label = "Agenda" } } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });
            Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);
            if (account.FullName.Contains("Aline") && account.FullName.Contains("Lara"))
            {
                carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "Meu amor! Aqui está uma surpresinha de aniversário!", Title = "Surpresa de Aniversário!", Uri = new Uri("https://cdn.mensagenscomamor.com/content/images/img/a/aniversario01.jpg") }, CardContent = "Meu amor! Aqui está uma surpresinha de aniversário!", options = new List<CarrosselOptions> { new CarrosselOptions { value = "Docinho", label = "Docinho" } } });
            }

            return _service.CreateCarrossel(carrosselData);
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


        private Document AlineCarrossel()
        {

            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://i.ytimg.com/vi/TmQ5cSVAQ6Q/maxresdefault.jpg"), Text = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", Title = "PARABENS, MEU AMOR!", Type = new MediaType("image", "jpeg") }, CardContent = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://erikvanslyke.files.wordpress.com/2011/05/read-instructions-caution-sign-s-2655.gif"), Text = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", Title = "Instruções!", Type = new MediaType("image", "jpeg") }, CardContent = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://www.torontoprosupershow.com/sites/default/files//ticket.jpg"), Text = "Possui alguns vales para você usar comigo! Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", Title = "Vale tudo de bom!", Type = new MediaType("image", "jpeg") }, CardContent = "Possui alguns vales para você usar comigo! Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://dicasdesaude.blog.br/wp-content/uploads/2015/08/4-Dicas-de-Cuidados-com-o-Cora%C3%A7%C3%A3o-Que-Voc%C3%AA-Pode-e-deve-Tomar-A-Partir-de-Hoje-Interna.jpg"), Text = "Pequenas declarações de mim para você. Todas minhas mesmo.", Title = "Te amos!", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas declarações de mim para você. Todas minhas mesmo.", options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://i.imgur.com/iGUdXht.jpg"), Text = "Pequenas confissões, de mim para você.", Title = "Segredos e Segredos", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas confissões, de mim para você.", options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://images.all-free-download.com/images/graphicthumb/love_box_vector_559244.jpg"), Text = "Quer começar? ", Title = "Começar", Type = new MediaType("image", "jpeg") }, CardContent = "Quer começar?", options = new List<CarrosselOptions> { new CarrosselOptions { label = "Começar", value = "Comecar" } } });

            Document alineCarrossel = _service.CreateCarrossel(cards);
            return alineCarrossel;

        }

        private Document CreateHobbiesCarrossel()
        {
            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s23.postimg.org/blawuy4yj/hobbies.jpg"), Text = "Uma das melhores coisas do mundo:\n\nAndar de bicicleta a toda velocidade!", Title = "Bicicleta!", Type = new MediaType("image", "jpeg") }, CardContent = "Uma das melhores coisas do mundo:\n\nAndar de bicicleta a toda velocidade!", options = new List<CarrosselOptions>() { new CarrosselOptions { label = new WebLink { Text = "Strava", Title = "Strava", Uri = new Uri("https://www.strava.com/athletes/12908959") } } } });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s23.postimg.org/vtizekisr/bow_and_arrow.jpg"), Text = "É praticamente uma terapia. Não pensar em nada mais do que seu alvo.", Title = "Arco e Flecha", Type = new MediaType("image", "jpeg") }, CardContent = "É praticamente uma terapia. Não pensar em nada mais do que seu alvo.", options = new List<CarrosselOptions>() { new CarrosselOptions { label = "Arco e Flecha", value = "arco e flecha" } } });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s30.postimg.org/q1fib25g1/games.jpg"), Text = "Digitais ou não, os jogos são uma das melhores formas de interação com pessoas!", Title = "Games e BoardGames", Type = new MediaType("image", "jpeg") }, CardContent = "Digitais ou não, os jogos são uma das melhores formas de interação com pessoas!", options = new List<CarrosselOptions>() { new CarrosselOptions { value = "Games", label = "games" } } });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s23.postimg.org/swdrbanvf/programming.png"), Text = "Pequenos programas feitos por hobbie e aplicativos!", Title = "Programação", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenos programas feitos por hobbies e aplicativos!", options = new List<CarrosselOptions>() { new CarrosselOptions { label = new WebLink { Title = "Google Play", Text = "Google Play", Uri = new Uri("https://play.google.com/store/apps/developer?id=Cristiano%20Guerra&hl=pt_BR") } } } });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s23.postimg.org/kjqg5lzej/graal_11.jpg"), Text = "Grupo de SwordPlay que participei por muitos anos.", Title = "Graal MG", Type = new MediaType("image", "jpeg") }, CardContent = "Grupo de SwordPlay que participei por muitos anos.", options = new List<CarrosselOptions>() { new CarrosselOptions { label = "Graal MG", value = "graal mg " } } });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://s30.postimg.org/7msfv4ywh/quadrinhos.jpg"), Text = "Colecionei de 2000 a 2015, mas ainda sou apaixonado!", Title = "Quadrinhos", Type = new MediaType("image", "jpeg") }, CardContent = "Colecionei de 2000 a 2015, mas ainda sou apaixonado!", options = new List<CarrosselOptions> { new CarrosselOptions { label = "Quadrinhos", value = "quadrinhos" } } });

            Document hobbiesCarrossel = _service.CreateCarrossel(cards);
            return hobbiesCarrossel;
        }

    }
}
