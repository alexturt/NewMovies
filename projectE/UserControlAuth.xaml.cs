using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace projectE
{
    /// <summary>
    /// Interaction logic for UserControlAuth.xaml
    /// </summary>
    public partial class UserControlAuth : Window
    {
        public UserControlAuth()
        {
            InitializeComponent();
        }
        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            
            //Process.GetCurrentProcess().Kill();
            Close();
        }
        //нажали кнопку "во весь экран" или "вернуть к нормальному размеру"
        private void button_maximazing_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)//если сейчас максимальный
            {
                WindowState = WindowState.Normal;//то возвращаем к нормальному
            }
            else
            {
                WindowState = WindowState.Maximized;//иначе делаем во весь экран
            }
        }
        //нажали кнопку скрыть в трей
        private void button_hide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        //перетаскивание окна за верхнюю панельку
        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_one_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_two_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
