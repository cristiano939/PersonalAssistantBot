using Lime.Messaging.Contents;
using PersonalAssistantBot.PhraseContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonalAssistantBot.SubjectHandlers
{
    class CommomExpressionsHandler
    {
        Regex hiRegex;
        Regex howdRegex;
        Regex badWordsRegex;
        Regex byeRegex;
        Regex whoAreURegex;
        CommomAnswers commom;

        public CommomExpressionsHandler()
        {
            hiRegex = new Regex(@"\b^(olá|ola|oi|opa|e ai|kole|cole|koe)\b");
            howdRegex = new Regex(@"\b^(beleza|blz|tudo bom|bão|bao|belez|bom)\b");
            badWordsRegex = new Regex(@"\b.*(burro|caralho|porra|pau|cu|cú|bunda|buceta|vsf|foder|foda-se|fodase|imbecil|idiota)\b");
            byeRegex = new Regex(@"\b(tchau|flw|bye|adeus|flws|ate logo|ate breve|até logo|até breve)\b");
            whoAreURegex = new Regex(@"\b(quem é voce|quem é você|que é voce|que é você|que eh voce|que eh você|que he vc|quem eh vc|q eh vc|quem he voce|o que he voce|que he voce|quem é vc)\b");
            commom = new CommomAnswers();
        }

        public PlainText GetCommomAnser(string message)
        {
            string answer = CreateAnswer(message);
            if (answer != null)
            {
                PlainText text = new PlainText { Text = answer };
                return text;
            }
            else
            {
                return null;
            }

        }

        private string CreateAnswer(string message)
        {
            message = message.ToLower();
            if (badWordsRegex.IsMatch(message))
            {
                return commom.getRandomBadWords();
            }
            else if (hiRegex.IsMatch(message))
            {
                return commom.getRandomHi();
            }
            else if (howdRegex.IsMatch(message))
            {
                return commom.getRandomHowD();
            }
            else if (byeRegex.IsMatch(message))
            {
                return commom.getRandomBye();
            } else if (whoAreURegex.IsMatch(message))
            {
                return commom.getRandomWhoAreU();
            }
            else
            {
                return null;
            }

        }
    }
}
