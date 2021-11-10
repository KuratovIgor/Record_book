using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecordBook
{
    public class RecordBookFilter : DependencyObject
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
        //public RelayCommand FilterRecordBookCommand { get => _filterRecordBookCommand ?? (_filterRecordBookCommand = new RelayCommand(obj => FilterRecordBook())); }


        //private void FilterRecordBook()
        //{
        //    RecordBooksChoosen.Clear();

        //    try
        //    {
        //        foreach (var item in RecordBooks)
        //        {
        //            if (item.FIO.Contains(ChoosenRecordBook) || item.Group.Contains(ChoosenRecordBook) ||
        //                item.Number.Contains(ChoosenRecordBook))
        //            {
        //                RecordBooksChoosen.Add(item);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }
        //}
    }
}
