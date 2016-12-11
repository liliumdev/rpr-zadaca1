using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    [Serializable]
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Faculty { get; set; }
        public int StudyYear { get; set; }
        public string Key { get; set; }

        public Student(string first, string last, DateTime birth, string faculty, int studyYear)
        {
            FirstName = first; LastName = last; Birthdate = birth;
            Faculty = faculty; StudyYear = studyYear;
            Key = Utility.RandomString(5);
        }
    }
}
