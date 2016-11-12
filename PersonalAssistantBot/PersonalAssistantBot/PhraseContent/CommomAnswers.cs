using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAssistantBot.PhraseContent
{
    public class CommomAnswers
    {
        
        string[] Hi = {"Ola!","E ai ,beleza?","kole", "Bão ou não?","oi oi" };
        string[] HoWd = {"Beleza demais!","Bom sim!","Bão!","Joia sim","Sussa","Bão demás!" };
        string[] BadWords = { "Por favor, não fale palavras de baixo calão", "Sem linguagem xula por favor", "você beija sua mãe com essa boca?","Tenho poucos meses de idade, não fale assim comigo", "Mais educação, menos palavrão" };
        Random rand;

        public CommomAnswers()
        {
            rand = new Random();
        }
        public string getRandomHi()
        {
            return Hi[rand.Next() % Hi.Length];
        }

        public string getRandomHowD()
        {
            return HoWd[rand.Next() % HoWd.Length];
        }

        public string getRandomBadWords()
        {
            return BadWords[rand.Next() % BadWords.Length];
        }
    }
}
