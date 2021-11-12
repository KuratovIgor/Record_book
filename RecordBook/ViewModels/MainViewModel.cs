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
    public class MainViewModel : SharedResources
    {
        private RecBook _current;
        private string _currentTerm;

        private RelayCommand _createRecordBookCommand;
        private RelayCommand _addMarkCommand;
        private RelayCommand _editCommand;
        private RelayCommand _calculateProbability;

        public ObservableCollection<DataTable> RBTable { get; set; } =
            new ObservableCollection<DataTable> { };


        public RecBook CurrentRecordBook
        {
            get => _current;
            set
            {
                _current = value;

                CurrentRecordBook.UpdateRecords();
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

        private void CreateRecordBook()
        {
            CreateRecordBookWindow w = new CreateRecordBookWindow();
            w.ShowDialog();

            UpdateRecordBooks();
        }

        private void AddMark()
        {
            AddMarkWindow w = new AddMarkWindow(RecordBooks);
            w.ShowDialog();
            
            if (CurrentRecordBook != null)
                UpdateDataGrid();
        }

        private void EditMark()
        {
            EditingWindow w = new EditingWindow(RecordBooks);
            w.ShowDialog();

            if (CurrentRecordBook != null)
                UpdateDataGrid();
        }

        public RelayCommand CreateRecordBookCommand { get => _createRecordBookCommand ?? (_createRecordBookCommand = new RelayCommand(obj => CreateRecordBook())); }
        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => AddMark())); }
        public RelayCommand EditCommand { get => _editCommand ?? (_editCommand = new RelayCommand(obj => EditMark())); }
        public RelayCommand CalculateProbabilityCommand { get => _calculateProbability ?? (_calculateProbability = new RelayCommand(obj => RedDiplomaCalculator.CalculateProbabilityRedDiploma(CurrentRecordBook))); }
    }
}
