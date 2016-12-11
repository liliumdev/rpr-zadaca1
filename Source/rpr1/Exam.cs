using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    interface IExamQuestionAdder
    {
        bool AddQuestion(Question question);
    }

    [Serializable]
    public class TakenExam
    {
        public List<string> ChosenAnswers { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public bool Passed { get; set; }

        public TakenExam()
        {
            CorrectAnswers = WrongAnswers = 0;
            Passed = true;
            ChosenAnswers = new List<string>();
        }
    }

    [Serializable]
    public class Exam
    {
        public string Key { get; set; }
        public List<Question> Questions { get; set; }
        public List<Tuple<string, TakenExam>> TakenExams { get; set; }

        public Exam(string key)
        {
            Key = key;
            Questions = new List<Question>();
            TakenExams = new List<Tuple<string, TakenExam>>();
        }

        public virtual bool HasQuestion(string question)
        {
            return Questions.Find(x => x.Text == question) != null;
        }

        public bool HasSimilarQuestion(string question)
        {
            string[] words1 = Utility.WordsInString(question);
            for (int i = 0; i < Questions.Count; i++)
            {
                string[] words2 = Utility.WordsInString(Questions[i].Text);
                if (words1.Intersect(words2).Count() >= 3)
                    return true;
            }

            return false;
        }
    }

   

    
}
