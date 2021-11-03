using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecordBook
{
    public class RecBook
    {
        public string FIO { get; set; }
        public string Number { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public string NameDeputyHead { get; set; }

        public List<Record> Records = new List<Record> { };

        public RecBook(string number, string fio, int course, string group, string nameDeputyHead)
        {
            FIO = fio;
            Number = number;
            Course = course;
            Group = group;
            NameDeputyHead = nameDeputyHead;
        }

        public void AddRecord(Record record)
        {
            Records.Add(record);
        }
    }
}
