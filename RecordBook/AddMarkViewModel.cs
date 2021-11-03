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
        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private static readonly DependencyProperty TermProperty = DependencyProperty.Register("Term", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty NameSubjectProperty = DependencyProperty.Register("NameSubject", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty CountHoursProperty = DependencyProperty.Register("CountHours", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty TeacherProperty = DependencyProperty.Register("Teacher", typeof(string), typeof(AddMarkViewModel));

        private RelayCommand _addMarkCommand;

        public ObservableCollection<RecBook> RecordBooks { get; set; } = new ObservableCollection<RecBook> { };

        private RecBook _selectedRB;
        public RecBook SelectedRB
        {
            get => _selectedRB;
            set => _selectedRB = value;
        }

        public string Term
        {
            get => (string)GetValue(TermProperty);
            set => SetValue(TermProperty, (value));
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

        public string Mark
        {
            get => (string)GetValue(MarkProperty);
            set => SetValue(MarkProperty, (value));
        }

        public string Date
        {
            get => (string) GetValue(DateProperty);
            set => SetValue(DateProperty, (value));
        }

        public string Type
        {
            get => (string)GetValue(TypeProperty);
            set => SetValue(TypeProperty, (value));
        }

        public string Teacher
        {
            get => (string)GetValue(TeacherProperty);
            set => SetValue(TeacherProperty, (value));
        }

        public AddMarkViewModel(ICollection<RecBook> recordBooks)
        {
            _sqlConnection.Open();

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
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
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{Term}', N'{NameSubject}', {CountHours}, N'{Type}', N'{Teacher}')";
                else if (String.IsNullOrEmpty(Date))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], Оценка, [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{Term}', N'{NameSubject}', {CountHours}, {Mark}, N'{Type}', N'{Teacher}')";
                else if (String.IsNullOrEmpty(Mark))
                    command = $"insert into Marks ([Номер зачетки], Семестр, [Название предмета], [Кол-во часов], [Дата сдачи], [Тип сдачи], [Фамилия преподавателя]) values (N'{SelectedRB.Number}', N'{Term}', N'{NameSubject}', {CountHours}, '{date.Day}.{date.Month}.{date.Year}', N'{Type}', N'{Teacher}')";
                else
                    command = $"insert into Marks values (N'{SelectedRB.Number}', N'{Term}', N'{NameSubject}', {CountHours}, {Mark}, '{date.Day}.{date.Month}.{date.Year}', N'{Type}', N'{Teacher}')";


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
