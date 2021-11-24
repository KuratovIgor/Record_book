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

namespace RecordBook.Views
{
    /// <summary>
    /// Логика взаимодействия для AgreeWindow.xaml
    /// </summary>
    public partial class AgreeWindow : Window
    {
        public AgreeWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
