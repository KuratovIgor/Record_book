using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Annotations;

namespace RecordBook
{
    public class MainViewModel : DependencyObject, INotifyPropertyChanged
    {
        private static DependencyProperty ChoosenRecordBookProperty = DependencyProperty.Register("ChoosenRecordBook", typeof(string), typeof(MainViewModel));
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






        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private RecBook _current;
        private string _currentTerm;

        public ObservableCollection<RecBook> RecordBooks { get; set; } = 
            new ObservableCollection<RecBook> { };

        public ObservableCollection<DataTable> RBTable { get; set; } =
            new ObservableCollection<DataTable> { };

        private RelayCommand _createRecordBookCommand;
        private RelayCommand _addMarkCommand;
        private RelayCommand _editCommand;
        private RelayCommand _calculateProbability;

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

        public RecBook CurrentRecordBook
        {
            get => _current;
            set
            {
                _current = value;

                UpdateRecords();
                UpdateDataGrid();

                OnPropertyChanged(nameof(CurrentRecordBook));
            }
        }

        public string CurrentTerm
        {
            get => _currentTerm;
            set
            {
                _currentTerm = value;

                UpdateDataGrid();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public RelayCommand CreateRecordBookCommand { get => _createRecordBookCommand ?? (_createRecordBookCommand = new RelayCommand(obj => CreateRecordBook())); }
        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => AddMark())); }
        public RelayCommand EditCommand { get => _editCommand ?? (_editCommand = new RelayCommand(obj => EditMark())); }
        public RelayCommand CalculateProbabilityCommand { get => _calculateProbability ?? (_calculateProbability = new RelayCommand(obj => RedDiplomaCalculator.CalculateProbabilityRedDiploma(CurrentRecordBook))); }

        public MainViewModel()
        {
            _sqlConnection.Open();

            UpdateRecordBooks();
        }

        private void UpdateDataGrid()
        {
            DataTable table = new DataTable();

            string command = $"select [Название предмета], [Кол-во часов], Оценка, [Дата сдачи], [Тип сдачи], [Фамилия преподавателя] from Marks where [Номер зачетки] = N'{CurrentRecordBook.Number}' and [Семестр] = N'{CurrentTerm}'";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command, _sqlConnection);
            dataAdapter.Fill(table);

            RBTable.Clear();
            RBTable.Add(table);
        }

        private void UpdateRecordBooks()
        {
            RecordBooks.Clear();
            RecordBooksChoosen.Clear();

            string command = "select * from Student";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
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
            }
        }

        private void CreateRecordBook()
        {
            CreateRecordBookWindow w = new CreateRecordBookWindow();
            w.ShowDialog();

            UpdateRecordBooks();
        }

        private void AddMark()
        {
            AddMarkWindow w = new AddMarkWindow(RecordBooks, ChoosenRecordBookProperty);
            w.ShowDialog();

            if (CurrentRecordBook != null)
            {
                UpdateRecords();
                UpdateDataGrid();
            }
        }

        private void EditMark()
        {
            EditingWindow w = new EditingWindow(RecordBooks, ChoosenRecordBookProperty);
            w.ShowDialog();

            if (CurrentRecordBook != null)
            {
                UpdateRecords();
                UpdateDataGrid();
            }
        }

        private void UpdateRecords()
        {
            string command = $"select * from Marks where [Номер зачетки] = '{CurrentRecordBook.Number}'";
            SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
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

                    Record record = new Record(numberRB, term, nameSub, countHours, mark, date, type, teacher);

                    foreach (var item in RecordBooks)
                    {
                        if (item.Number == numberRB)
                        {
                            item.AddRecord(record);
                            item.CalculateAvg();
                            CurrentRecordBook.Avg = item.Avg;

                            CurrentRecordBook.Records.Clear();
                            foreach (var rec in item.Records)
                            {
                                CurrentRecordBook.Records.Add(rec);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
