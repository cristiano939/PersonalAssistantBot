using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using PersonalAssistantBot.Services;
using PersonalAssistantBot.Models;
using System.Collections.Generic;
using Lime.Messaging.Contents;

namespace PersonalAssistantBot
{
    public class AlineReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private Settings _settings;
        private Random _random;
        private IStateManager _state;
        private DocumentService _service;

        public AlineReceiver(IMessagingHubSender sender, Settings settings, IStateManager state)
        {
            _sender = sender;
            _settings = settings;
            _state = state;
            _service = new DocumentService();
            _random = new Random();
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Document doc = null;
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (message.Content.ToString().ToLower().Contains("voltar"))
            {
                doc = new PlainText { Text = "Ok, Lindona! \nQuando quiser voltar já sabe o que fazer!😎 " };
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                _state.SetState(message.From.ToIdentity(), "default");
            }
            else if (message.Content.ToString().ToLower().Contains("valezinhos"))
            {
                doc = GetValezinhos();
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));
                await _sender.SendMessageAsync(CreateAlineSelect(), message.From, cancellationToken);
            }
            else if (message.Content.ToString().ToLower().Contains("comecar"))
            {
                doc = CreateAlineSelect();
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
            }
            else if (message.Content.ToString().ToLower().Contains("segredos"))
            {
                doc = new PlainText {Text = _settings.Secrets[_random.Next(0, _settings.Secrets.Length-1)] };
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));
                await _sender.SendMessageAsync(CreateAlineSelect(), message.From, cancellationToken);
            }
            else if (message.Content.ToString().ToLower().Contains("declaracao"))
            {
                doc = new PlainText { Text = _settings.LoveUSays[_random.Next(0, _settings.LoveUSays.Length - 1)] };
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));
                await _sender.SendMessageAsync(CreateAlineSelect(), message.From, cancellationToken);
            }
            else if (message.Content.ToString().ToLower().Contains("instrucoes"))
            {
                doc = AlineCarrossel();
                await _sender.SendMessageAsync(doc, message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));
                await _sender.SendMessageAsync(CreateAlineSelect(), message.From, cancellationToken);
            }
            
        }

        private Document AlineCarrossel()
        {

            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://i.ytimg.com/vi/TmQ5cSVAQ6Q/maxresdefault.jpg"), Text = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", Title = "PARABENS, MEU AMOR!", Type = new MediaType("image", "jpeg") }, CardContent = "Amor! Hoje é seu aniversário, então preparei essa surpresa para você", options = new List<string>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://erikvanslyke.files.wordpress.com/2011/05/read-instructions-caution-sign-s-2655.gif"), Text = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", Title = "Instruções!", Type = new MediaType("image", "jpeg") }, CardContent = "Antes de prosseguir é preciso saber o que essa brincadeira faz!", options = new List<string>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://www.torontoprosupershow.com/sites/default/files//ticket.jpg"), Text = "Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", Title = "Vale tudo de bom!", Type = new MediaType("image", "jpeg") }, CardContent = "Possui alguns vales para você usar comigo! Selecione os vales e vamos ver o que faremos nos proximos 10 minutos.", options = new List<string>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://dicasdesaude.blog.br/wp-content/uploads/2015/08/4-Dicas-de-Cuidados-com-o-Cora%C3%A7%C3%A3o-Que-Voc%C3%AA-Pode-e-deve-Tomar-A-Partir-de-Hoje-Interna.jpg"), Text = "Pequenas declarações de mim para você. Todas minhas mesmo.", Title = "Te amos!", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas declarações de mim para você. Todas minhas mesmo.", options = new List<string>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("http://i.imgur.com/iGUdXht.jpg"), Text = "Pequenas confissões, de mim para você.", Title = "Segredos e Segredos", Type = new MediaType("image", "jpeg") }, CardContent = "Pequenas confissões, de mim para você.", options = new List<string>() });
            cards.Add(new CarrosselCard { CardMediaHeader = new MediaLink { Uri = new Uri("https://image.freepik.com/icones-gratis/curva-de-ponto-de-seta-para-a-esquerda_318-10099.jpg"), Text = "Voltar para o Bot normal.", Title = "Voltar", Type = new MediaType("image", "jpeg") }, CardContent = "Voltar para o Bot normal.", options = new List<string>() });

            Document alineCarrossel = _service.CreateCarrossel(cards);
            return alineCarrossel;

        }

        private Document CreateAlineSelect()
        {
            Select select = new Select { Scope = SelectScope.Immediate, Text = "O que você quer fazer, meu amor?" };
            var Options = new SelectOption[5];
            Options[0] = new SelectOption { Text = "Valezinhos", Value = new PlainText { Text = "Valezinhos" } };
            Options[1] = new SelectOption { Text = "Declaração", Value = new PlainText { Text = "declaracao" } };
            Options[2] = new SelectOption { Text = "Segredos", Value = new PlainText { Text = "Segredos" } };
            Options[3] = new SelectOption { Text = "Instruções", Value = new PlainText { Text = "instrucoes" } };
            Options[4] = new SelectOption { Text = "Voltar", Value = new PlainText { Text = "Voltar" } };


            select.Options = Options;
            return select;
        }

        private Document GetValezinhos()
        {
            return new MediaLink
            {
                Text = "Esse vale é para ser usado por 10 min para brincarmos mais!",
                Title = "Valezinho!",
                Type = new MediaType("image", "jpeg"),
                Uri = new Uri(_settings.TicketsImages[_random.Next(0, _settings.TicketsImages.Length - 1)])
            };
        }
    }
}
