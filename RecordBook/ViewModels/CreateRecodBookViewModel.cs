using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace RecordBook
{
    public class CreateRecodBookViewModel : INotifyPropertyChanged
    {
        private string _fio;
        private string _numbrerRB;
        private string _course;
        private string _fioZam;
        private string _group;

        public List<string> Courses { get; set; } = new List<string>
        {
            "1", "2", "3", "4", "5"
        };

        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private RelayCommand _createCommand;

        public string Fio
        {
            get => _fio;
            set
            {
                _fio = value;
                OnPropertyChanged(nameof(Fio));
            }
        }

        public string NumberRecordBook
        {
            get => _numbrerRB;
            set
            {
                _numbrerRB = value;
                OnPropertyChanged(nameof(NumberRecordBook));
            }
        }

        public string Course
        {
            get => _course;
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        public string FioZam
        {
            get => _fioZam;
            set
            {
                _fioZam = value;
                OnPropertyChanged(nameof(FioZam));
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        public CreateRecodBookViewModel()
        {
            _sqlConnection.Open();
        }

        public RelayCommand CreateCommand { get => _createCommand ?? (_createCommand = new RelayCommand(obj => Create())); }

        private void Create()
        {
            try
            {
                string command = $"insert into Student values (N'{NumberRecordBook}', N'{Fio}', {Course}, N'{FioZam}', N'{Group}')";

                SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);
                sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Зачетная книжка создана!");

                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Невозможно создать зачетку! Возможно, данные не введены, или введены не корректно.");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
