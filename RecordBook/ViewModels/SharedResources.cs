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

        protected static DependencyProperty ChoosenRecordBookProperty = DependencyProperty.Register("ChoosenRecordBook", typeof(string), typeof(MainViewModel));

        private RelayCommand _filterRecordBookCommand;

        public ObservableCollection<RecBook> RecordBooks { get; set; } =
            new ObservableCollection<RecBook> { };

        public ObservableCollection<RecBook> RecordBooksChoosen { get; set; } =
            new ObservableCollection<RecBook> { };

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


        public string ChoosenRecordBook
        {
            get => GetValue(ChoosenRecordBookProperty) as string;
            set => SetValue(ChoosenRecordBookProperty, value);
        }

        private void FilterRecordBook()
        {
            RecordBooksChoosen.Clear();

            try
            {
                foreach (var item in RecordBooks)
                {
                    if (item.FIO.Contains(ChoosenRecordBook) || 
                        item.Group.Contains(ChoosenRecordBook) ||
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

        public RelayCommand FilterRecordBookCommand { get => _filterRecordBookCommand ?? (_filterRecordBookCommand = new RelayCommand(obj => FilterRecordBook())); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
