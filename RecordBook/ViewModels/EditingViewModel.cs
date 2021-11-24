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
    public class EditingViewModel : SharedResources
    {
        public List<string> Marks { get; set; } = new List<string>
        {
            "2", "3", "4", "5"
        };

        public ObservableCollection<string> Subjects { get; set; } = new ObservableCollection<string> { };

        private string _currentTerm;
        public string CurrentTerm
        {
            get => _currentTerm;
            set
            {
                _currentTerm = value;
                SetSubjects();
            }
        }

        private string _mark;
        public string Mark
        {
            get => _mark;
            set => _mark = value;
        }

        public string Subject { get; set; }

        private RelayCommand _editCommand;

        
        private RecBook _selectedRB;
        public RecBook SelectedRB
        {
            get => _selectedRB;
            set
            {
                _selectedRB = value;
                SetTerms();
                OnPropertyChanged(nameof(SelectedRB));
            }
        }

        private string _date;
        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public RelayCommand EditCommand { get => _editCommand ?? (_editCommand = new RelayCommand(obj => Edit())); }

        public EditingViewModel(ICollection<RecBook> recordBooks)
        {
            _sqlConnection.Open();

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
                RecordBooksChoosen.Add(item);
            }
        }

        private void SetSubjects()
        {
            string command = $"select distinct [Название предмета] from Marks where [Номер зачетки] = N'{SelectedRB.Number}' and Семестр=N'{CurrentTerm}'";

            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            Subjects.Clear();
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Subjects.Add(reader.GetValue(0) as string);
                }
            }
        }

        private void SetTerms()
        {
            string command = $"select distinct [Семестр] from Marks where [Номер зачетки] = N'{SelectedRB.Number}'";

            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            Terms.Clear();
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Terms.Add(reader.GetValue(0) as string);
                }
            }
        }

        private void Edit()
        {
            try
            {
                if (String.IsNullOrEmpty(Date) && String.IsNullOrEmpty(Mark))
                    throw new Exception("Введите данные!");

                if (!String.IsNullOrEmpty(Mark))
                   SelectedRB.EditMark(Mark, Subject, CurrentTerm);

                if (!String.IsNullOrEmpty(Date))
                    SelectedRB.EditDate(Date, Subject, CurrentTerm);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
