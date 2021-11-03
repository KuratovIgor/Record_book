using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditingWindow.xaml
    /// </summary>
    public partial class EditingWindow : Window
    {
        public EditingWindow(ICollection<RecBook> recordBooks)
        {
            InitializeComponent();
            DataContext = new EditingViewModel(recordBooks);
        }
    }
}
