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
        string[] bye = { "Flws", "flws!", "Tchau tchau", "até", "até breve","até logo","espero ter ajudado, tchau!","até a próxima" };
        string[] HoWd = {"Beleza demais!","Bom sim!","Bão!","Joia sim","Sussa","Bão demás!" };
        string[] BadWords = { "Por favor, não fale palavras de baixo calão", "Sem linguagem xula por favor", "você beija sua mãe com essa boca?","Tenho poucos meses de idade, não fale assim comigo", "Mais educação, menos palavrão" };
        string[] WhoAreU = {"Eu sou o Assistente Pessoal do Sr.Guerra!\nUma IA simples criada para cuidar das coisas dele.\n Atendo comandos padrões e simples." };
        Random rand;

        public CommomAnswers()
        {
            rand = new Random();
        }
        public string getRandomHi()
        {
            return Hi[rand.Next() % Hi.Length];
        }
        public string getRandomBye()
        {
            return bye[rand.Next() % bye.Length];
        }

        public string getRandomHowD()
        {
            return HoWd[rand.Next() % HoWd.Length];
        }
        public string getRandomWhoAreU()
        {
            return WhoAreU[rand.Next() % WhoAreU.Length];
        }

        public string getRandomBadWords()
        {
            return BadWords[rand.Next() % BadWords.Length];
        }
    }
}
