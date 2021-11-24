using System;
using System.CodeDom;
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
using RecordBook.Views;

namespace RecordBook
{
    public class MainViewModel : SharedResources
    {
        private AgreeWindow _agreeWindow;

        private RecBook _current;
        private string _currentTerm = "1 семестр";
        private int _termNumber = 1;

        private RelayCommand _createRecordBookCommand;
        private RelayCommand _addMarkCommand;
        private RelayCommand _editCommand;
        private RelayCommand _calculateProbability;
        private RelayCommand _openNextTermCommand;
        private RelayCommand _openPrevTermCommand;
        private RelayCommand _toNextCourseCommand;

        private RelayCommand _agreeCommand;
        private RelayCommand _notAgreeCommand;

        public ObservableCollection<DataTable> RBTable { get; set; } =
            new ObservableCollection<DataTable> { };


        public RecBook CurrentRecordBook
        {
            get => _current;
            set
            {
                _current = value;

                //CurrentTerm = "1 семестр";

                CurrentRecordBook.UpdateRecords();
                UpdateDataGrid();
                SetTerms();

                OnPropertyChanged(nameof(CurrentRecordBook));
            }
        }

        public string CurrentTerm
        {
            get => _currentTerm;
            set
            {
                if (value != null)
                {
                    _currentTerm = value;

                    if (_termNumber != 10)
                        _termNumber = Convert.ToInt32(_currentTerm[0]) - '0';

                    UpdateDataGrid();
                    OnPropertyChanged(nameof(CurrentTerm));
                }
            }
        }

        public MainViewModel()
        {
            _sqlConnection.Open();

            UpdateRecordBooks();
        }

        private void UpdateDataGrid()
        {
            try
            {
                DataTable table = new DataTable();

                string command = $"select [Название предмета], [Кол-во часов], Оценка, [Дата сдачи], [Тип сдачи], [Фамилия преподавателя] from Marks where [Номер зачетки] = N'{CurrentRecordBook.Number}' and [Семестр] = N'{CurrentTerm}'";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command, _sqlConnection);
                dataAdapter.Fill(table);

                RBTable.Clear();
                RBTable.Add(table);
            }
            catch(Exception){}
        }

        private void SetTerms()
        {
            Terms.Clear();
            for (int i = 1; i <= CurrentRecordBook.Course*2; i++)
            {
                Terms.Add($"{i} семестр");
            }

            CurrentTerm = Terms[0];
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

        private void OpenNextTerm()
        {
            if (CurrentTerm != Terms[Terms.Count - 1])
            {
                _termNumber++;
                CurrentTerm = $"{_termNumber} семестр";
            }
        }

        private void OpenPrevTerm()
        {
            if (CurrentTerm != Terms[0])
            {
                _termNumber--;
                CurrentTerm = $"{_termNumber} семестр";
            }
        }

        private void ToNextCourse()
        {
            if (CurrentRecordBook != null)
            {
                _agreeWindow = new AgreeWindow(this);
                _agreeWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите студента!");
            }
        }

        private void Agree()
        {
            _agreeWindow.Close();

            CurrentRecordBook.ToNextCourse();
            SetTerms();
        }

        private void NotAgree()
        {
            _agreeWindow.Close();
        }

        public RelayCommand CreateRecordBookCommand { get => _createRecordBookCommand ?? (_createRecordBookCommand = new RelayCommand(obj => CreateRecordBook())); }
        public RelayCommand AddMarkCommand { get => _addMarkCommand ?? (_addMarkCommand = new RelayCommand(obj => AddMark())); }
        public RelayCommand EditCommand { get => _editCommand ?? (_editCommand = new RelayCommand(obj => EditMark())); }
        public RelayCommand CalculateProbabilityCommand { get => _calculateProbability ?? (_calculateProbability = new RelayCommand(obj => RedDiplomaCalculator.CalculateProbabilityRedDiploma(CurrentRecordBook))); }
        public RelayCommand OpenNextTermCommand { get => _openNextTermCommand ?? (_openNextTermCommand = new RelayCommand(obj => OpenNextTerm())); }
        public RelayCommand OpenPrevTermCommand { get => _openPrevTermCommand ?? (_openPrevTermCommand = new RelayCommand(obj => OpenPrevTerm())); }
        public RelayCommand ToNextCourseCommand {get => _toNextCourseCommand ?? (_toNextCourseCommand = new RelayCommand(obj => ToNextCourse())); }

        public RelayCommand AgreeCommand { get => _agreeCommand ?? (_agreeCommand = new RelayCommand(obj => Agree())); }
        public RelayCommand NotAgreeCommand { get => _notAgreeCommand ?? (_notAgreeCommand = new RelayCommand(obj => NotAgree())); }
    }
}
