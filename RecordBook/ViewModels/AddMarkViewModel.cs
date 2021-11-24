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
    public class AddMarkViewModel : SharedResources
    {
        public List<string> Marks { get; set; } = new List<string>
        {
            "2", "3", "4", "5"
        };

        public List<string> Types { get; set; } = new List<string>
        {
            "Зачет", "Диф.зачет", "Экзамен", "Курсовой проект"
        };

        public string CurrentTerm { get; set; }
        public string Mark { get; set; }
        public string Type { get; set; }

        private RelayCommand _addMarkCommand;

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

        private string _name;
        private string _countHours;
        private string _date;
        private string _teacher;

        public string NameSubject
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(NameSubject));
            }
        }

        public string CountHours
        {
            get => _countHours;
            set
            {
                _countHours = value;
                OnPropertyChanged(nameof(CountHours));
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Teacher
        {
            get => _teacher;
            set
            {
                _teacher = value;
                OnPropertyChanged(nameof(Teacher));
            }
        }

        public AddMarkViewModel(ICollection<RecBook> recordBooks)
        {
            _sqlConnection.Open();

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
                RecordBooksChoosen.Add(item);
            }
        }
        private void SetTerms()
        {
            Terms.Clear();
            for (int i = 1; i <= SelectedRB.Course * 2; i++)
            {
                Terms.Add($"{i} семестр");
            }
        }

        public void Add()
        {
            try
            {
                SelectedRB.AddMark(CurrentTerm, NameSubject, Convert.ToInt32(CountHours), Mark, Date, Type, Teacher);

                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Введите данные!");
            }
        }

        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => Add())); }
    }
}
