using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecordBook
{
    public class RecBook : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private double _avg;

        public string FIO { get; set; }
        public string Number { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public string NameDeputyHead { get; set; }

        public double Avg
        {
            get => _avg;
            set
            {
                _avg = value;
                OnPropertyChanged(nameof(Avg));
            }
        }

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

        public void CalculateAvg()
        {
            double value = 0;
            int count = 0;

            foreach (var item in Records)
            {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(item.Mark)))
                {
                    value += item.Mark;
                    count++;
                }
            }

            Avg = Math.Round(value / count, 2);
        }
    }
}
