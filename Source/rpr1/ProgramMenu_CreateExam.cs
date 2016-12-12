using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace rpr1
{
    public class Program
    {
        static SelOption Menu_CreateNewExamA()
        {
            string key = "";
            Exam e = null;
            while (key == "")
            {
                Console.Clear();
                Menu_DrawTitle();
                key = ui.InputString(strings.ExamKey);

                bool hasB = false;
                for (int k = 0; k <= Exams.Count; k++)
                {
                    if (Exams[k].Key == key && Exams[k] is ExamA)
                    {
                        ui.Alert(strings.ExamExists);
                        Console.Clear();
                        return Menu_Professor();
                    }
                    else if (Exams[k].Key == key && Exams[k] is ExamB)
                    {
                        hasB = true;
                        e = Exams[k];
                    }
                }
                if (!hasB)
                {
                    ui.Alert(strings.ExamNoB);
                    return Menu_Professor();
                }
            }

            ExamA exam = new ExamA(key, e as ExamB);

            Console.WriteLine("\n");

            int i = 1;
            for (i = 1; i <= 8; i++)
            {
                Question q = new RegularQuestion();
                if (i > 6)
                    q = new FillInQuestion();

                q.Text = ui.InputString(strings.Question + i,
                                        x => Utility.NumberOfWords(x) <= 10 &&
                                               !exam.HasSimilarQuestion(x) &&
                                               !exam.HasQuestion(x),
                                        strings.QuestionNotValidatedA);

                if (i <= 6)
                {
                    for (int j = 1; j <= 3; j++)
                        (q as RegularQuestion).Answers.Add(ui.InputString(strings.GivenAnswer + j));


                    q.CorrectAnswer = (q as RegularQuestion).Answers[int.Parse(
                                                        ui.InputString(strings.CorrectAnswer,
                                                                        x =>
                                                                        {
                                                                            int y = -1;
                                                                            if (int.TryParse(x, out y))
                                                                                return y > 0 && y < (i > 6 ? 6 : 4);
                                                                            else return false;
                                                                        },
                                                                        String.Format(strings.CorrectAnswerMustBe, (i > 6 ? 5 : 3))
                                                                        )
                                                                    ) - 1];
                }
                else
                {
                    q.CorrectAnswer = ui.InputString(strings.CorrectFillInAnswer);
                }


                Console.WriteLine("\n");
                exam.AddQuestion(q);
            }

            ui.Alert(strings.ExamAdded, ConsoleColor.White, ConsoleColor.Blue);
            Exams.Add(exam);
            return Menu_Professor();
        }

        static SelOption Menu_CreateNewExamB()
        {
            string key = "";
            while (key == "")
            {
                Console.Clear();
                Menu_DrawTitle();
                key = ui.InputString(strings.ExamKey);
                Exam e = Exams.Find(x => x.Key == key);
                if (e != null)
                {
                    if (e is ExamB)
                    {
                        ui.Alert(strings.ExamExists);
                        return Menu_Professor();
                    }
                }
            }

            ExamB exam = new ExamB(key);

            Console.WriteLine("\n");

            int i = 1;
            for (i = 1; i <= 8; i++)
            {
                RegularQuestion q = new RegularQuestion();
                q.Text = ui.InputString(strings.Question + i,
                                        x => Utility.NumberOfWords(x) <= 10 &&
                                               !exam.HasSimilarQuestion(x) &&
                                               !exam.HasQuestion(x),
                                        strings.QuestionNotValidated);

                for (int j = 1; j <= (i > 6 ? 5 : 3); j++)
                    q.Answers.Add(ui.InputString(strings.GivenAnswer + j));

                q.CorrectAnswer = q.Answers[int.Parse(
                                                ui.InputString(strings.CorrectAnswer,
                                                                x => {
                                                                    int y = -1;
                                                                    if (int.TryParse(x, out y))
                                                                        return y > 0 && y < (i > 6 ? 6 : 4);
                                                                    else return false;
                                                                },
                                                                String.Format(strings.CorrectAnswerMustBe, (i > 6 ? 5 : 3))
                                                                )
                                                            ) - 1];
                if (i > 6)
                {
                    q.CorrectAnswer2 = q.Answers[int.Parse(
                                                ui.InputString(strings.CorrectAnswer2,
                                                                x => {
                                                                    int y = -1;
                                                                    if (int.TryParse(x, out y))
                                                                        return y > 0 && y < (i > 6 ? 6 : 4);
                                                                    else return false;
                                                                },
                                                                 String.Format(strings.CorrectAnswerMustBe, (i > 6 ? 5 : 3))
                                                                )
                                                            ) - 1];
                }

                Console.WriteLine("\n");
                exam.AddQuestion(q);
            }

            ui.Alert(strings.ExamAdded, ConsoleColor.White, ConsoleColor.Blue);
            Exams.Add(exam);
            return Menu_Professor();
        }
    }
}
