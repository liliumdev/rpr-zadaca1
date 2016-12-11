using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI;

namespace rpr1
{
    public partial class Program
    {
        private static List<Exam> Exams = new List<Exam>();
        private static List<Student> Students = new List<Student>();
        private static UserInterface ui = new UserInterface();

        static void Main(string[] args)
        {
            SelOption opt = new SelOption(0, Menu_Language);
            while(opt.Func != null)
            {
                opt = opt.Func();
            }
        }
    }
}
