using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Documents;

namespace RecordBook
{
    public class CreateRecodBookViewModel : DependencyObject
    {
        private SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private static readonly DependencyProperty FioProperty = DependencyProperty.Register("Fio", typeof(string), typeof(CreateRecodBookViewModel));
        private static readonly DependencyProperty NumberRBProperty = DependencyProperty.Register("NumberRecordBook", typeof(string), typeof(CreateRecodBookViewModel));
        private static readonly DependencyProperty CourseProperty = DependencyProperty.Register("Course", typeof(string), typeof(CreateRecodBookViewModel));
        private static readonly DependencyProperty GroupProperty = DependencyProperty.Register("FioZam", typeof(string), typeof(CreateRecodBookViewModel));
        private static readonly DependencyProperty FioZamProperty = DependencyProperty.Register("Group", typeof(string), typeof(CreateRecodBookViewModel));

        private RelayCommand _createCommand;

        public string Fio
        {
            get => (string) GetValue(FioProperty);
            set => SetValue(FioProperty, (value));
        }

        public string NumberRecordBook
        {
            get => (string)GetValue(NumberRBProperty);
            set => SetValue(NumberRBProperty, (value));
        }

        public string Course
        {
            get => (string)GetValue(CourseProperty);
            set => SetValue(CourseProperty, (value));
        }

        public string FioZam
        {
            get => (string)GetValue(FioZamProperty);
            set => SetValue(FioZamProperty, (value));
        }

        public string Group
        {
            get => (string)GetValue(GroupProperty);
            set => SetValue(GroupProperty, (value));
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
                MessageBox.Show(e.Message);
            }
        }
    }
}
