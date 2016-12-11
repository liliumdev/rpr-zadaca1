using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    [Serializable]
    public class Question
    {
        public string Text { get; set; }
        public string CorrectAnswer { get; set; }
        public string CorrectAnswer2 { get; set; }

        public Question()
        {
            Text = ""; CorrectAnswer = ""; CorrectAnswer2 = "";
        }

        public virtual bool IsCorrectAnswer(string answer)
        {
            return answer == CorrectAnswer || answer == CorrectAnswer2;
        }
    }

    [Serializable]
    public class RegularQuestion : Question
    {
        public List<string> Answers = new List<string>();
    }

    [Serializable]
    public class FillInQuestion : Question
    {
    }
}
