using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace rpr1
{
    public partial class Program
    {
        static SelOption Menu_Professor()
        {
            Menu m = new Menu(ProgramTitle);
            return m.AddOption(strings.MenuOptionCreateNewExamA, Menu_CreateNewExamA)
             .AddOption(strings.MenuOptionCreateNewExamB, Menu_CreateNewExamB)
             .AddOption(strings.MenuOptionViewExams, Menu_ViewExams)
             .AddOption(strings.MenuOptionViewExamResults, Menu_ViewExamResults)
             .AddOption(strings.MenuOptionViewGlobalStats, Menu_ViewGlobalStats)
             .AddOption(strings.MenuOptionLoadData, Menu_LoadData)
             .AddOption(strings.MenuOptionSaveData, Menu_SaveData)
             .AddOption(strings.MenuOptionStudentList, Menu_ViewStudents)
             .AddOption(strings.MenuBack, Menu_Main)
             .Draw();
        }

        static SelOption Menu_ViewExams()
        {
            Menu m = new Menu(ProgramTitle);
            foreach (Exam e in Exams)
            {
                m.AddOption(e.Key + (e is ExamA ? " - A" : " - B"), () =>
                {
                    return Menu_ViewExamQuestions(e);
                });
            }
            m.AddOption(strings.MenuBack, Menu_Professor);
            return m.Draw();
        }

        static SelOption Menu_ViewExamResults()
        {
            Menu m = new Menu(ProgramTitle);
            foreach (Exam e in Exams)
            {
                foreach (Tuple<string, TakenExam> t in e.TakenExams)
                {
                    m.AddOption(t.Item1 + " - " + e.Key + (e is ExamA ? " - A" : " - B"), () =>
                    {
                        return Menu_ViewSingleExamResults(e, t.Item2);
                    });
                }
            }

            m.AddOption(strings.MenuBack, Menu_Professor);
            return m.Draw();
        }

        static SelOption Menu_ViewGlobalStats()
        {
            Menu m = new Menu(ProgramTitle);
            foreach (Exam e in Exams)
            {
                m.AddOption(e.Key + (e is ExamA ? " - A" : " - B"), () =>
                {
                    return Menu_ViewSingleExamStats(e);
                });
            }
            m.AddOption(strings.MenuBack, Menu_Professor);
            return m.Draw();
        }

        static SelOption Menu_ViewExamQuestions(Exam e)
        {
            Console.Clear();
            Menu_DrawTitle();
            Table t = new Table();
            t.AddRow(new object[] { "--- " + e.Key + " - " + (e is ExamB ? "B" : "A") + "  ---" }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
            int i = 1;
            foreach (Question q in e.Questions)
            {
                t.AddRow(new object[] { i.ToString() + ". " + q.Text }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);

                if (q is RegularQuestion)
                {
                    List<string> answers = (q as RegularQuestion).Answers;
                    t.AddRow(answers.ToArray());
                    int answer1 = answers.FindIndex(x => x == q.CorrectAnswer);
                    int answer2 = answers.FindIndex(x => x == q.CorrectAnswer2);
                    if (answer1 != -1)
                        t.SetCellColor(answer1, ConsoleColor.White, ConsoleColor.DarkGreen);
                    if (answer2 != -1)
                        t.SetCellColor(answer2, ConsoleColor.White, ConsoleColor.DarkGreen);
                }
                else
                    t.AddRow(new object[] { (q as FillInQuestion).CorrectAnswer }).SetCellColor(0, ConsoleColor.White, ConsoleColor.DarkGreen);
                i++;
                t.AddRow(new object[] { " " });
            }
            t.Draw();

            return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_ViewExams).Draw(false);
        }

        static SelOption Menu_LoadData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("exams.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            Exams = (List<Exam>)formatter.Deserialize(stream);
            stream.Close();

            stream = new FileStream("students.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            Students = (List<Student>)formatter.Deserialize(stream);
            stream.Close();

            return Menu_Professor();
        }

        static SelOption Menu_SaveData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("exams.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Exams);
            stream.Close();

            stream = new FileStream("students.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Students);
            stream.Close();

            return Menu_Professor();
        }

        static SelOption Menu_ViewStudents()
        {
            Console.Clear();
            Menu_DrawTitle();
            Table t = new Table();
            t.AddRow(new object[] { "Ime i prezime", "Fakultet", "Godina studija" });
            foreach (Student s in Students)
                t.AddRow(new object[] { s.FirstName + " " + s.LastName, s.Faculty, s.StudyYear });

            t.Draw();

            return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_Professor).Draw(false);
        }

        static SelOption Menu_ViewSingleExamStats(Exam e)
        {
            Console.Clear();
            Menu_DrawTitle();

            if (e.TakenExams.Count == 0)
            {
                Table tt = new Table();
                tt.AddRow(new object[] { "--- " + e.Key + " - " + (e is ExamB ? "B" : "A") + " ---" })
                 .AddRow(new object[] { strings.NoOneTookExam })
                 .SetCellColor(0, ConsoleColor.White, ConsoleColor.Black)
                 .Draw();
                return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_ViewGlobalStats).Draw(false);
            }

            Table t = new Table();
            t.AddRow(new object[] { "--- " + e.Key + " - " + (e is ExamB ? "B" : "A") + " ---" })
             .SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);

            int passedTotal = 0;
            Dictionary<int, int> passedWithNCorrectAnswers = new Dictionary<int, int>();
            Dictionary<string, int> worstAnsweredQuestions = new Dictionary<string, int>();
            Dictionary<string, int> bestAnsweredQuestions = new Dictionary<string, int>();
            Dictionary<int, int> yearPassed = new Dictionary<int, int>();

            foreach (var tuple in e.TakenExams)
            {
                var te = tuple.Item2;

                if (te.Passed)
                {
                    passedTotal++;

                    if (passedWithNCorrectAnswers.ContainsKey(te.CorrectAnswers))
                        passedWithNCorrectAnswers[te.CorrectAnswers]++;
                    else
                        passedWithNCorrectAnswers[te.CorrectAnswers] = 1;

                    int studyYear = Students.Find(x => x.Key == tuple.Item1).StudyYear;
                    if (yearPassed.ContainsKey(studyYear))
                        yearPassed[studyYear]++;
                    else
                        yearPassed[studyYear] = 1;
                }

                int i = 0;
                foreach (string answer in te.ChosenAnswers)
                {
                    if (e.Questions[i].IsCorrectAnswer(answer))
                    {
                        if (bestAnsweredQuestions.ContainsKey(e.Questions[i].Text))
                            bestAnsweredQuestions[e.Questions[i].Text]++;
                        else
                            bestAnsweredQuestions[e.Questions[i].Text] = 1;
                    }
                    else
                    {
                        if (worstAnsweredQuestions.ContainsKey(e.Questions[i].Text))
                            worstAnsweredQuestions[e.Questions[i].Text]++;
                        else
                            worstAnsweredQuestions[e.Questions[i].Text] = 1;
                    }
                    i++;
                }
            }

            string worstAnsweredQuestion = worstAnsweredQuestions.FirstOrDefault(x => x.Value == worstAnsweredQuestions.Values.Max()).Key;
            string bestAnsweredQuestion = bestAnsweredQuestions.FirstOrDefault(x => x.Value == bestAnsweredQuestions.Values.Max()).Key;


            t.AddRow(new object[] { strings.TotalPassed, passedTotal }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
            foreach (var tuple in passedWithNCorrectAnswers)
            {
                t.AddRow(new object[] { String.Format(strings.PassedWithN, tuple.Key), tuple.Value }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
            }
            if (bestAnsweredQuestion != null)
            {
                t.AddRow(new object[] { strings.BestAnswered }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
                t.AddRow(new object[] { "(x" + bestAnsweredQuestions[bestAnsweredQuestion] + ") " + bestAnsweredQuestion });
            }
            if (worstAnsweredQuestion != null)
            {
                t.AddRow(new object[] { strings.WorstAnswered }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
                t.AddRow(new object[] { "(x" + worstAnsweredQuestions[worstAnsweredQuestion] + ") " + worstAnsweredQuestion });
            }
            t.Draw();

            Console.WriteLine("\n");

            t = new Table();
            t.AddRow(new object[] { strings.PercentagePerYear });
            List<object> years = new List<object>();
            List<object> values = new List<object>();
            foreach (var tuple in yearPassed)
            {
                years.Add(tuple.Key.ToString() + ". " + strings.YearWord);
                values.Add( Math.Round(((double)tuple.Value / passedTotal * 100.0), 2).ToString() + "%");
            }
            t.AddRow(years.ToArray());
            t.AddRow(values.ToArray());
            t.Draw();

            return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_ViewGlobalStats).Draw(false);
        }



        static SelOption Menu_ViewSingleExamResults(Exam e, TakenExam te, bool returnToProf = true)
        {
            Console.Clear();
            Menu_DrawTitle();
            Table t = new Table();
            t.AddRow(new object[] {
                    "--- " + e.Key + " - " + (e is ExamB ? "B" : "A") + " / " + (te.Passed ? strings.PassedWord : strings.NotPassedWord) + " ---"
                    }
                    ).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
            int i = 1;
            foreach (Question q in e.Questions)
            {
                t.AddRow(new object[] { i.ToString() + ". " + q.Text }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);

                if (q is RegularQuestion)
                {
                    List<string> answers = (q as RegularQuestion).Answers;
                    t.AddRow(answers.ToArray());
                    if (te.ChosenAnswers.Count > i - 1)
                    {
                        int chosenAnswer = answers.FindIndex(x => x == te.ChosenAnswers[i - 1]);
                        t.SetCellColor(chosenAnswer, ConsoleColor.White, ConsoleColor.Red);
                    }
                    int answer1 = answers.FindIndex(x => x == q.CorrectAnswer);
                    int answer2 = answers.FindIndex(x => x == q.CorrectAnswer2);
                    if (answer1 != -1)
                        t.SetCellColor(answer1, ConsoleColor.White, ConsoleColor.DarkGreen);
                    if (answer2 != -1)
                        t.SetCellColor(answer2, ConsoleColor.White, ConsoleColor.DarkGreen);
                }
                else
                {
                    if (te.ChosenAnswers.Count > i - 1)
                    {
                        string chosenAnswer = te.ChosenAnswers[i - 1];
                        t.AddRow(new object[] { chosenAnswer, (q as FillInQuestion).CorrectAnswer })
                          .SetCellColor(1, ConsoleColor.White, ConsoleColor.DarkGreen);
                        if (q.IsCorrectAnswer(chosenAnswer))
                            t.SetCellColor(0, ConsoleColor.White, ConsoleColor.DarkGreen);
                        else
                            t.SetCellColor(0, ConsoleColor.White, ConsoleColor.Red);
                    }
                    else
                    {
                        t.AddRow(new object[] { "", (q as FillInQuestion).CorrectAnswer })
                         .SetCellColor(0, ConsoleColor.White, ConsoleColor.Red)
                         .SetCellColor(1, ConsoleColor.White, ConsoleColor.DarkGreen);
                    }
                }
                i++;
                t.AddRow(new object[] { " " });
            }
            t.Draw();

            if (returnToProf)
                return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_ViewExamResults).Draw(false);
            else
                return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_Student).Draw(false);
        }

    }
}
