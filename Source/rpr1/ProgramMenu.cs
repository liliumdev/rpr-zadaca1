using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UI;

namespace rpr1
{
    public partial class Program
    {
        public static string ProgramTitle = "Exams 1.0a";

        static SelOption Menu_Main()
        {
            Menu m = new Menu(ProgramTitle);
            return m.AddNote(strings.MenuNoteChooseSubinterface)
             .AddOption(strings.MenuOptionProfessor, Menu_Professor)
             .AddOption(strings.MenuOptionStudent, Menu_Student)
             .AddOption(strings.MenuBack, Menu_Language)
             .AddOption(strings.MenuExit, Menu_Exit)
             .Draw();
        }

        static SelOption Menu_Exit()
        {
            System.Environment.Exit(1);

            return null;
        }
        

        static SelOption Menu_EnglishLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            return Menu_Main();
        }

        static SelOption Menu_BosnianLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("bs-Latn-BA");
            return Menu_Main();
        }

        static SelOption Menu_Language()
        {            
            Menu m = new Menu(ProgramTitle + " - Welcome / Dobro dosli");
            return m.AddOption("English language", Menu_EnglishLanguage)
             .AddOption("Bosanski jezik", Menu_BosnianLanguage)
             .AddNote("")
             .AddNote("Use arrows up and down, and the Enter key to make a choice.")
             .AddNote("Koristite strelice gore i dolje, te tipku Enter da nacinite izbor.")
             .Draw();
        }

        static void Menu_DrawTitle()
        {
            int t;
            t.AddRow(new object[] { ProgramTitle }).SetCellColor(0, ConsoleColor.Red, ConsoleColor.Blue);
            t.Draw();
        }
    }
}
