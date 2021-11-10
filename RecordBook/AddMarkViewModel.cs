using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecordBook
{
    public class AddMarkViewModel : DependencyObject
    {
        private static DependencyProperty ChoosenRecordBookProperty;

        public ObservableCollection<RecBook> RecordBooksChoosen { get; set; } =
            new ObservableCollection<RecBook> { };

        public string ChoosenRecordBook
        {
            get => GetValue(ChoosenRecordBookProperty) as string;
            set => SetValue(ChoosenRecordBookProperty, value);
        }

        private RelayCommand _filterRecordBookCommand;
        public RelayCommand FilterRecordBookCommand { get => _filterRecordBookCommand ?? (_filterRecordBookCommand = new RelayCommand(obj => FilterRecordBook())); }


        private void FilterRecordBook()
        {
            RecordBooksChoosen.Clear();

            try
            {
                foreach (var item in RecordBooks)
                {
                    if (item.FIO.Contains(ChoosenRecordBook) || item.Group.Contains(ChoosenRecordBook) ||
                        item.Number.Contains(ChoosenRecordBook))
                    {
                        RecordBooksChoosen.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }





        public List<string> Terms { get; set; } = new List<string>
        {
            "1 семестр",
            "2 семестр",
            "3 семестр",
            "4 семестр",
            "5 семестр",
            "6 семестр",
            "7 семестр",
            "8 семестр",
            "9 семестр",
            "10 семестр"
        };

        public List<string> Marks { get; set; } = new List<string>
        {
            "2", "3", "4", "5"
        };

        public List<string> Types { get; set; } = new List<string>
        {
            "Зачет", "Диф.зачет", "Экзамен", "Курсовой проект"
        };

        private string _currentTerm;
        public string CurrentTerm
        {
            get => _currentTerm;
            set => _currentTerm = value;
        }

        private string _mark;
        public string Mark
        {
            get => _mark;
            set => _mark = value;
        }

        private string _type;

        public string Type
        {
            get => _type;
            set => _type = value;
        }

        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private static readonly DependencyProperty NameSubjectProperty = DependencyProperty.Register("NameSubject", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty CountHoursProperty = DependencyProperty.Register("CountHours", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty TeacherProperty = DependencyProperty.Register("Teacher", typeof(string), typeof(AddMarkViewModel));

        private RelayCommand _addMarkCommand;

        public ObservableCollection<RecBook> RecordBooks { get; set; } = 
            new ObservableCollection<RecBook> { };

        private RecBook _selectedRB;
        public RecBook SelectedRB
        {
            get => _selectedRB;
            set => _selectedRB = value;
        }

        public string NameSubject
        {
            get => (string)GetValue(NameSubjectProperty);
            set => SetValue(NameSubjectProperty, (value));
        }

        public string CountHours
        {
            get => (string)GetValue(CountHoursProperty);
            set => SetValue(CountHoursProperty, (value));
        }

        public string Date
        {
            get => (string) GetValue(DateProperty);
            set => SetValue(DateProperty, (value));
        }

        public string Teacher
        {
            get => (string)GetValue(TeacherProperty);
            set => SetValue(TeacherProperty, (value));
        }

        public AddMarkViewModel(ICollection<RecBook> recordBooks, DependencyProperty prop)
        {
            _sqlConnection.Open();
            ChoosenRecordBookProperty = prop;

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
                RecordBooksChoosen.Add(item);
            }
        }

        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => Add())); }
        
        public void Add()
        {
            try
            {
                string command;

                DateTime date = default;
                if (Date != null && Date != "")
                    date = DateTime.Parse(Date);

                if (String.IsNullOrEmpty(Date) && String.IsNullOrEmpty(Mark))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{CurrentTerm}', N'{NameSubject}', {CountHours}, N'{Type}', N'{Teacher}')";
                else if (String.IsNullOrEmpty(Date))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], Оценка, [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{CurrentTerm}', N'{NameSubject}', {CountHours}, {Mark}, N'{Type}', N'{Teacher}')";
                else if (String.IsNullOrEmpty(Mark))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Дата сдачи], [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{CurrentTerm}', N'{NameSubject}', {CountHours}, '{date.Day}.{date.Month}.{date.Year}', N'{Type}', N'{Teacher}')";
                else
                    command = $"insert into Marks values (N'{SelectedRB.Number}', N'{CurrentTerm}', N'{NameSubject}', {CountHours}, {Mark}, '{date.Day}.{date.Month}.{date.Year}', N'{Type}', N'{Teacher}')";


                SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Оценка занесена в зачетную книжку!");

                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
