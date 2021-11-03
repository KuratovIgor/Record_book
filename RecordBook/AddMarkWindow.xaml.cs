using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecordBook
{
    /// <summary>
    /// Логика взаимодействия для AddMarkWindow.xaml
    /// </summary>
    public partial class AddMarkWindow : Window
    {
        public AddMarkWindow(ICollection<RecBook> recordBooks)
        {
            InitializeComponent();
            DataContext = new AddMarkViewModel(recordBooks);
        }
    }
}
