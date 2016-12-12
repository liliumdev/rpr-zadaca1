using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    [Serializable]
    public class ExamA : Exam, IExamQuestionAdder
    {
        public ExamB PrerequisiteExam = null;
        
        public ExamA(string key, ExamB prerequisite) : base(key)
        {
            PrerequisiteExam = prerequisite;
        }

        public override bool HasQuestion(string question)
        {
            return PrerequisiteExam.HasQuestion(question) || base.HasQuestion(question);
        }

        public bool AddQuestion(Question question)
        {
            if (!PrerequisiteExam.HasQuestion(question.Text) && !HasQuestion(question.Text) && !HasSimilarQuestion(question.Text) &&
                Utility.NumberOfWords(question.Text) <= 10)
            {
                Questions.Add(question);
                return true;
            }

            return false;
        }
    }
}
