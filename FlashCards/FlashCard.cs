using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards
{
    class FlashCard
    {
        public FlashCard(string question, string answer)
        {
            this.Question = question;
            this.Answer = answer;
        }

        public string Question { get; private set; }
        public string Answer { get; private set; }
    }
}
