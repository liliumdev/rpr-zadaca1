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
        static string RegisterStudent()
        {
            string key = Utility.RandomString(10);
            List<object> studentData = ui.MultipleInputString(new string[] {
                                                                strings.StudentFirstName,
                                                                strings.StudentLastName,
                                                                strings.StudentBirthday,
                                                                strings.StudentFaculty,
                                                                strings.StudentStudyYear
                                                             }, new string[] {
                                                                 "string",
                                                                 "string",
                                                                 "date",
                                                                 "string",
                                                                 "int"
                                                             }, new Func<object, bool>[] {
                                                                 x => Utility.NumberOfWords(x.ToString()) == 1,  // Maximum one word in first 
                                                                 x => Utility.NumberOfWords(x.ToString()) == 1,  // and last name
                                                                 x => true,                                      // no validation here
                                                                 x => Utility.NumberOfWords(x.ToString()) < 10,  // max 10 words in faculty
                                                                 x => (int)x < 11 && (int)x > 0                  // study year in [1,10]
                                                             });

            DateTime bday = (DateTime)studentData[2];
            if(DateTime.Now.Year - bday.Year > 26)
            {
                ui.Alert(strings.LessThan26);
                return "";
            }

            Student s = new Student((string)studentData[0], (string)studentData[1], (DateTime)studentData[2], (string)studentData[3], (int)studentData[4]);


            // We should check first, but ... meh, what are the odds?
            s.Key = key;

            Students.Add(s);

            ui.Alert(String.Format(strings.StudentCreated, key), backgroundColor: ConsoleColor.DarkGreen);

            return key;
        }

        static SelOption TakeExam(Exam e)
        {
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Menu_DrawTitle();
            ui.Alert(strings.NoEntryKey, ConsoleColor.White, ConsoleColor.DarkGreen);
            bool retakingThisExam = false;
            Console.Clear();
            Console.Clear();
            Console.Clear();
            Console.Clear(); // kako bi se bilo sigurno da se konzola ocistila 
            // First ask for student key
            string studentKey = ui.InputString(strings.EnterStudentKey);
            if(Students.Find(x => x.Key == studentKey) == null)
            {
                if(ui.Alert(strings.WantToRegister, ConsoleColor.White, ConsoleColor.DarkGreen, true))
                    studentKey = RegisterStudent();
                else
                {
                    return Menu_Student();
                }
            }
            if(studentKey == "")
            {
                return Menu_Student();
            }

            // Did the student already take this exam
            if (Exams.Find(x => x == e && x.TakenExams.Find(y => y.Item1 == studentKey) != null) != null)
            {
                retakingThisExam = ui.Alert(strings.AlreadyTookExam, ConsoleColor.White, ConsoleColor.Red, true);
                if (!retakingThisExam)
                {
                    return Menu_Student();
                }
            }

            // If it's type A, did the student pass type B ?
            if (e is ExamA)
            {
                var takenBefore = (e as ExamA).PrerequisiteExam.TakenExams.Find(x => x.Item1 == studentKey);
                if (takenBefore == null)
                {
                    ui.Alert(strings.FirstTakeB);
                    return Menu_Student();
                }
                else if(!takenBefore.Item2.Passed)
                {
                    ui.Alert(strings.FirstTakeB);
                    return Menu_Student();
                }
            }

            TakenExam te = new TakenExam();
            Table t1 = new Table();
            t1.AddRow(new object[] { "--- " + e.Key + " - " + (e is ExamB ? "B" : "A") + "  ---" }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);
            t1.Draw();

            int i = 1;
            int wrongAnswersInRow = 0;
            foreach (Question q in e.Questions)
            { 
                Table t = new Table();
                t.AddRow(new object[] { i.ToString() + ". " + q.Text }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Black);

                if (q is RegularQuestion)
                {
                    List<string> answers = (q as RegularQuestion).Answers;
                    t.Draw();

                    // Choose answer
                    Menu m = new Menu();
                    foreach(string answer in answers)
                    {
                        m.AddOption(answer);
                    }
                    int chosenAnswer = m.Draw(false).SelectedIndex;
                    te.ChosenAnswers.Add(answers[chosenAnswer-1]);
                    if (q.IsCorrectAnswer(answers[chosenAnswer-1]))
                    {                        
                        te.CorrectAnswers++;
                        wrongAnswersInRow = 0;
                    }
                    else
                    {
                        te.WrongAnswers++;
                        wrongAnswersInRow++;
                        if(wrongAnswersInRow == 2)
                        {
                            ui.Alert(strings.FailedExam);
                            te.Passed = false;
                            break;
                        }
                    }
                }
                else
                {
                    t.Draw();
                    string chosenAnswer = ui.InputString(strings.EnterYourAnswer);
                    te.ChosenAnswers.Add(chosenAnswer);
                    if (q.IsCorrectAnswer(chosenAnswer))
                    {
                        te.CorrectAnswers++;
                        wrongAnswersInRow = 0;
                    }
                    else
                    {
                        te.WrongAnswers++;
                        wrongAnswersInRow++;
                        if (wrongAnswersInRow == 2)
                        {
                            ui.Alert(strings.FailedExam);
                            te.Passed = false;
                            break;
                        }
                    }
                }

                i++;
            }
            if(!retakingThisExam)
                e.TakenExams.Add(new Tuple<string, TakenExam>(studentKey, te));
            else
            {
                Exam eToUpdate = Exams.Find(x => (
                                                    ((x is ExamA) && (e is ExamA)) || ((x is ExamB) && (e is ExamB))
                                                  ) && x.TakenExams.Find(y => y.Item1 == studentKey) != null);
                eToUpdate.TakenExams[eToUpdate.TakenExams.FindIndex(y => y.Item1 == studentKey)] = new Tuple<string, TakenExam>(studentKey, te);
            }

            if (te.Passed)
            {
                if (ui.Alert(strings.PassedExam, ConsoleColor.White, ConsoleColor.DarkGreen, true))
                    return Menu_ViewSingleExamResults(e, te, false);
            }
            else
            {
                if (ui.Alert(strings.FailedExamDetails, ConsoleColor.White, ConsoleColor.DarkGreen, true))
                    return Menu_ViewSingleExamResults(e, te, false);
            }
            
            return new Menu(strings.ChooseOption).AddOption(strings.MenuBack, Menu_Student).Draw(false);
        }
    }
}
