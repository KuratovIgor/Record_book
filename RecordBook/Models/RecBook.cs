using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecordBook
{
    public class RecBook : INotifyPropertyChanged
    {
        protected SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

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
            _sqlConnection.Open();
            FIO = fio;
            Number = number;
            Course = course;
            Group = group;
            NameDeputyHead = nameDeputyHead;
        }

        public void UpdateRecords()
        {
            Records.Clear();

            string command = $"select * from Marks where [Номер зачетки] = '{Number}'";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string numberRB = reader.GetValue(0) as string;
                string term = reader.GetValue(1) as string;
                string nameSub = reader.GetValue(2) as string;
                int countHours = Convert.ToInt32(reader.GetValue(3));
                int mark = Convert.ToInt32(reader.GetValue(4));
                DateTime date = Convert.ToDateTime(reader.GetValue(5) as string);
                string type = reader.GetValue(6) as string;
                string teacher = reader.GetValue(7) as string;

                Records.Add(new Record(numberRB, term, nameSub, countHours, mark, date, type, teacher));
            }
            CalculateAvg();

            reader.Close();
        }

        public void  AddMark(string term, string nameSubject, int countHours, string mark, string _date, string _type, string teacher)
        {
            try
            {
                string command;

                DateTime date = default;
                if (_date != null && _date != "")
                    date = DateTime.Parse(_date);

                if (String.IsNullOrEmpty(_date) && String.IsNullOrEmpty(mark))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Тип сдачи], [Фамилия преподавателя]) values (N'{Number}', N'{term}', N'{nameSubject}', {countHours}, N'{_type}', N'{teacher}')";
                else if (String.IsNullOrEmpty(_date))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], Оценка, [Тип сдачи], [Фамилия преподавателя]) values (N'{Number}', N'{term}', N'{nameSubject}', {countHours}, {mark}, N'{_type}', N'{teacher}')";
                else if (String.IsNullOrEmpty(mark))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Дата сдачи], [Тип сдачи], [Фамилия преподавателя]) values (N'{Number}', N'{term}', N'{nameSubject}', {countHours}, '{date.Day}.{date.Month}.{date.Year}', N'{_type}', N'{teacher}')";
                else
                    command = $"insert into Marks values (N'{Number}', N'{term}', N'{nameSubject}', {countHours}, {mark}, '{date.Day}.{date.Month}.{date.Year}', N'{_type}', N'{teacher}')";


                SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);
                sqlCommand.ExecuteNonQuery();

                UpdateRecords();

                MessageBox.Show("Оценка занесена в зачетную книжку!");

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            } 
        }

        public void EditMark(string mark, string subject, string term)
        {
            string command = $"update Marks set Оценка = {mark} where [Название предмета] = N'{subject}' and Семестр = N'{term}' and [Номер зачетки] = N'{Number}'";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);
            UpdateRecords();
            MessageBox.Show($"Исправление оценки({sqlCommand.ExecuteNonQuery().ToString()})");
        }

        public void EditDate(string _date, string subject, string term)
        {
            DateTime date = default;
            if (_date != null && _date != "")
                date = DateTime.Parse(_date);

            string command = $"update Marks set [Дата сдачи] = '{date.Day}.{date.Month}.{date.Year}' where [Название предмета] = N'{subject}' and Семестр = N'{term}' and [Номер зачетки] = N'{Number}'";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);
            UpdateRecords();
            MessageBox.Show($"Исправление даты({sqlCommand.ExecuteNonQuery().ToString()})");
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
