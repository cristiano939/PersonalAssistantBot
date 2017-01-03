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

namespace PersonalAssistantBot
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly CommomExpressionsHandler commom;
        private Settings _settings;

        public PlainTextMessageReceiver(IMessagingHubSender sender, Settings settings)
        {
            _sender = sender;
            _settings = settings;
            commom = new CommomExpressionsHandler();
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
                else
                {
                }
            }
            else
            {
                await _sender.SendMessageAsync("Ainda estou aprendendo sobre alguns assuntos,\nmas estou aqui para falar do Cristiano.\n\nEscolha o que deseja saber.", message.From, cancellationToken);
                await _sender.SendMessageAsync(CreateFirstCarrossel(), message.From, cancellationToken);
            }

        }

        public Document CreateFirstCarrossel()
        {
            List<CarrosselCard> carrosselData = new List<CarrosselCard>();
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink {Text="Cristiano faz muitas coisas bacanas e diferentes.\nTenho muito pra mostrar!",Title= "Quer conhecer meus Hobbies?", Type= new MediaType("image","jpeg"), Uri = new Uri(_settings.HobbiesImages[0]) }, CardContent = "Faço muitas coisas bacanas e tenho muito pra mostrar! Clique e conheça!", options = new List<string> {"Hobbies!" } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Text = "De aplicativos, plataformas, Chatbots e websites.", Title = "Trabalho, Trabalho e Trabalho", Type = new MediaType("image","jpeg"),Uri=new Uri(_settings.WorkImages[0]) }, CardContent = "De aplicativos, plataformas, Chatbots e websites.", options = new List<string> {"Trabalhos" } });
            carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink {Text="Marque um compromisso com Cristiano!", Title="Agenda!",Uri=new Uri(_settings.AgendaImages[0]) }, CardContent = "Marque um compromisso com Cristiano!", options = new List<string> {"Agenda" } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });
            //carrosselData.Add(new CarrosselCard { CardMediaHeader = new MediaLink { }, CardContent = "", options = new List<string> { } });


            DocumentService _service = new DocumentService();
            return _service.CreateCarrossel(carrosselData);
        }

    }
}
