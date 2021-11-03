using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordBook
{
    public class Record
    {
        public string NumberRB { get; set; }
        public string Term { get; set; }
        public string NameSubject { get; set; }
        public int CountHours { get; set; }
        public int Mark { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Teacher { get; set; }

        public Record(string number, string term, string nameSub, int countHours, int mark, DateTime date, string type, string teacher)
        {
            NumberRB = number;
            Term = term;
            NameSubject = nameSub;
            CountHours = countHours;
            Mark = mark;
            Date = date;
            Type = type;
            Teacher = teacher;
        }
    }
}
