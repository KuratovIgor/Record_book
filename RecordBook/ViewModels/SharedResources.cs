using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public abstract class SharedResources : DependencyObject, INotifyPropertyChanged
    {
        protected SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private RelayCommand _filterRecordBookCommand;

        public ObservableCollection<RecBook> RecordBooks { get; set; } =
            new ObservableCollection<RecBook> { };

        public ObservableCollection<RecBook> RecordBooksChoosen { get; set; } =
            new ObservableCollection<RecBook> { };

        public ObservableCollection<string> Terms { get; set; } 
            = new ObservableCollection<string> { };

        private string _choosen;
        public string ChoosenRecordBook
        {
            get => _choosen;
            set
            {
                _choosen = value;
                OnPropertyChanged(nameof(ChoosenRecordBook));
            }
        }

        protected void UpdateRecordBooks()
        {
            RecordBooks.Clear();
            RecordBooksChoosen.Clear();

            string command = "select * from Student";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string numberRB = reader.GetValue(0) as string;
                string name = reader.GetValue(1) as string;
                int course = Convert.ToInt32(reader.GetValue(2));
                string nameZam = reader.GetValue(3) as string;
                string group = reader.GetValue(4) as string;

                RecordBooks.Add(new RecBook(numberRB, name, course, group, nameZam));
                RecordBooksChoosen.Add(new RecBook(numberRB, name, course, group, nameZam));
            }

            reader.Close();
        }

        private void FilterRecordBook(object obj)
        {
            string searchedString = obj as string;

            if (!string.IsNullOrWhiteSpace(searchedString) || !string.IsNullOrWhiteSpace(ChoosenRecordBook))
            {
                if (string.IsNullOrWhiteSpace(searchedString))
                    searchedString = ChoosenRecordBook;

                RecordBooksChoosen.Clear();
                Terms.Clear();

                try
                {
                    foreach (var item in RecordBooks)
                    {
                        if (item.FIO.ToLower().Contains(searchedString.ToLower()) ||
                            item.Group.ToLower().Contains(searchedString.ToLower()) ||
                            item.Number.ToLower().Contains(searchedString.ToLower()))
                        {
                            RecordBooksChoosen.Add(item);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                ChoosenRecordBook = "";
            }
            else
            {
                UpdateRecordBooks();
            }
        }

        public RelayCommand FilterRecordBookCommand { get => _filterRecordBookCommand ?? (_filterRecordBookCommand = new RelayCommand(obj => FilterRecordBook(obj))); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
