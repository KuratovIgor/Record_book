using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace RecordBook
{
    public class EditingViewModel : DependencyObject
    {
        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private static readonly DependencyProperty TermProperty = DependencyProperty.Register("Term", typeof(string), typeof(EditingViewModel));
        private static readonly DependencyProperty NameSubjectProperty = DependencyProperty.Register("NameSubject", typeof(string), typeof(EditingViewModel));
        private static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(string), typeof(EditingViewModel));
        private static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(EditingViewModel));

        private RelayCommand _editCommand;

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

        public string Mark
        {
            get => (string)GetValue(MarkProperty);
            set => SetValue(MarkProperty, (value));
        }

        public string Date
        {
            get => (string)GetValue(DateProperty);
            set => SetValue(DateProperty, (value));
        }

        public RelayCommand EditCommand { get => _editCommand ?? (_editCommand = new RelayCommand(obj => Edit())); }

        public EditingViewModel(ICollection<RecBook> recordBooks)
        {
            _sqlConnection.Open();

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
            }
        }

        private void Edit()
        {
            try
            {
                SqlCommand sqlCommand;
                string command = "";
                DateTime date = default;
                if (Date != null && Date != "")
                    date = DateTime.Parse(Date);

                if (String.IsNullOrEmpty(Date) && String.IsNullOrEmpty(Mark))
                    throw new Exception("Введите данные!");

                if (!String.IsNullOrEmpty(Mark))
                {
                    command = $"update Marks set Оценка = {Mark} where [Название предмета] = N'{NameSubject}' and Семестр = N'{Term}' and [Номер зачетки] = N'{SelectedRB.Number}'";
                    sqlCommand = new SqlCommand(command, _sqlConnection);
                    MessageBox.Show($"Исправление оценки({sqlCommand.ExecuteNonQuery().ToString()})");
                }

                if (!String.IsNullOrEmpty(Date))
                {
                    command = $"update Marks set [Дата сдачи] = '{Date}' where [Название предмета] = N'{NameSubject}' and Семестр = N'{Term}' and [Номер зачетки] = N'{SelectedRB.Number}'";
                    sqlCommand = new SqlCommand(command, _sqlConnection);
                    MessageBox.Show($"Исправление даты({sqlCommand.ExecuteNonQuery().ToString()})");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
