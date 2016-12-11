using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    [Serializable]
    public class ExamB : Exam, IExamQuestionAdder
    {
        public ExamB(string key) : base(key)
        {
        }

        public bool AddQuestion(Question question)
        {
            if (Utility.NumberOfWords(question.Text) <= 10 && !HasQuestion(question.Text))
            {
                Questions.Add(question);
                return true;
            }

            return false;
        }
    }
}
