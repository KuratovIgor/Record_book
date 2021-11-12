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

        private static readonly DependencyProperty NameSubjectProperty = DependencyProperty.Register("NameSubject", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty CountHoursProperty = DependencyProperty.Register("CountHours", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(AddMarkViewModel));
        private static readonly DependencyProperty TeacherProperty = DependencyProperty.Register("Teacher", typeof(string), typeof(AddMarkViewModel));

        private RelayCommand _addMarkCommand;

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

        public AddMarkViewModel(ICollection<RecBook> recordBooks)
        {
            _sqlConnection.Open();

            foreach (var item in recordBooks)
            {
                RecordBooks.Add(item);
                RecordBooksChoosen.Add(item);
            }
        }

        public void Add()
        {
            SelectedRB.AddMark(CurrentTerm, NameSubject, Convert.ToInt32(CountHours), Mark, Date, Type, Teacher);

            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            window.Close();
        }

        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => Add())); }
    }
}
