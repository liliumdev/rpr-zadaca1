using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace rpr1
{
    public partial class Program
    {

        static SelOption Menu_Student()
        {
            Menu m = new Menu(ProgramTitle);
            return m.AddOption(strings.MenuOptionTakeExams, Menu_TakeExams)
             .AddOption(strings.MenuOptionViewStudentExams, Menu_ViewStudentExams)
             .AddOption(strings.MenuBack, Menu_Main)
             .Draw();
        }

        static SelOption Menu_ViewStudentExams()
        {
            Console.Clear();
            Menu_DrawTitle();

            // First ask for student key
            string studentKey = ui.InputString(strings.EnterStudentKey);
            if (Students.Find(x => x.Key == studentKey) == null)
            {
                ui.Alert(strings.NoSuchKey, ConsoleColor.White, ConsoleColor.DarkGreen);
               // return Menu_Student();
            }

            Menu m = new Menu(ProgramTitle);
            foreach (Exam e in Exams)
            {
                foreach (var tuple in e.TakenExams)
                {
                    if (tuple.Item1 == studentKey)
                    {
                        m.AddOption(
                            tuple.Item1 + " : " + e.Key + " - " + (e is ExamB ? "A" : "B") + " / " + 
                            (tuple.Item2.Passed ? strings.PassedWord : strings.NotPassedWord), 
                            () =>
                            {
                                return Menu_ViewSingleExamResults(e, tuple.Item2, false);
                            });
                    }
                }
            }
            m.AddOption(strings.MenuBack, Menu_Student);
            return m.Draw();
        }

        static SelOption Menu_TakeExams()
        {
            Menu m = new Menu(ProgramTitle);
            foreach (Exam e in Exams)
            {
                m.AddOption(e.Key + (e is ExamA ? " - A" : " - B"), () =>
                {
                    return TakeExam(e);
                });
            }
            m.AddOption(strings.MenuBack, Menu_Student);
            return m.Draw();
        }
    }
}
