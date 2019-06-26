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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO;
//using System.Drawing;

namespace projectE
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        

        private void button_panel_Click(object sender, RoutedEventArgs e)
        {
            if (grid.ColumnDefinitions[2].IsEnabled)
                closePanel();
            else
                openPanel();
        }

        void openPanel()
        {
            grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions[2].IsEnabled = true;
            Width += grid.ColumnDefinitions[1].ActualWidth;
        }
        void closePanel()
        {
            double size = grid.ColumnDefinitions[2].ActualWidth;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            grid.ColumnDefinitions[2].IsEnabled = false;
            Width -= size;
        }

        BitmapImage noFavoriteImg = new BitmapImage(new Uri("/Resources/пустаязвезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage FavoriteImg = new BitmapImage(new Uri("/Resources/звезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage noWatchedImg = new BitmapImage(new Uri("/Resources/nowatched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage WatchedImg = new BitmapImage(new Uri("/Resources/watched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage posterImg = new BitmapImage(new Uri("/Resources/posterTEST.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DataTable dt = new DataTable();
        //    DataColumn dc = new DataColumn("poster")
        //    {
                
        //    };
        //    dt.Columns.Add("poster");
        //    dt.Columns.Add("data");
        //    dt.Columns.Add("buttons_date");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56");
        //    datagrid_list.RowBackground = null;
        //    datagrid_list.IsReadOnly = true;
        //    //datagrid_list.HeadersVisibility = DataGridHeadersVisibility.None;
        //    //datagrid_list.Foreground = Brushes.LightSlateGray;
        //    datagrid_list.BorderThickness = new Thickness(0, 0, 0, 0);
            
        //    datagrid_list.ItemsSource = dt.DefaultView;
        //    //datagrid_list.ColumnWidth = '*';
        //    datagrid_list.Columns[0].Width = 80;
        //    datagrid_list.Columns[2].Width = 80;
        //}

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add();
            dt.Columns.Add();
            dt.Columns.Add();
            dt.Columns.Add();
            
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            dt.Rows.Add(new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill }, "34", "56", "78");
            RowDefinition rd;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                rd = new RowDefinition() { Height= new GridLength(130)};
                grid_list.RowDefinitions.Add(rd);
                Button btf = new Button()
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0,0,0,0),
                    Content = new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill },
                    Tag = 12
                };
                btf.Click += button_favorite_Click;
                Button btw = new Button()
                {
                    Name = "button_watched" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Content = new Image() { Source = noWatchedImg, Stretch = Stretch.Fill },
                    Tag = 12
                };
                btw.Click += button_watched_Click;
                TextBlock tb = new TextBlock()
                {
                    Name = "textBlock_middle_content" + i,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Text = "Люди в черном: Интернэшнл (2019)"+Environment.NewLine + "Men in Black International"+
                    Environment.NewLine+ "США, реж. Ф. Гэри Грей 16+"+
                    Environment.NewLine+ "фантастика, боевик, комедия ...",
                    //Margin = new Thickness(5,5,5,5),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = 12
                };
                Image img = new Image()
                {
                    Source = posterImg,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = 12
                };
                TextBlock date = new TextBlock()
                {
                    Name = "textBlock_date" + i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Text = "26" +
                    Environment.NewLine + "ИЮЛЯ" +
                    Environment.NewLine + "2019",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0,40,0,0),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = 12
                };
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, i);
                grid_list.Children.Add(img);

                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, i);
                grid_list.Children.Add(tb);

                Grid.SetColumn(btf, 2);
                Grid.SetRow(btf, i);
                grid_list.Children.Add(btf);

                Grid.SetColumn(btw, 2);
                Grid.SetRow(btw, i);
                grid_list.Children.Add(btw);

                Grid.SetColumn(date, 2);
                Grid.SetRow(date, i);
                grid_list.Children.Add(date);
            }
            
        }
        
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.ToString() == "1")
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg };
                (sender as Button).Tag = "0";
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg };
                (sender as Button).Tag = "1";
            }
        }

        private void button_watched_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.ToString() == "1")
            {
                (sender as Button).Content = new Image() { Source = noWatchedImg };
                (sender as Button).Tag = "0";
            }
            else
            {
                (sender as Button).Content = new Image() { Source = WatchedImg };
                (sender as Button).Tag = "1";
            }
        }

        private void grid_list_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void testmouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                //string str = sender.;
                //Grid _grid = sender as Grid;
                //int _row = (int)_grid.GetValue(Grid.RowProperty);
                //int _column = (int)_grid.GetValue(Grid.ColumnProperty);
                //Title = string.Format("Кликнул по колонке {0}, строке {1}", _column, _row);
                //Title = Convert.ToInt32( (sender as Control).Tag).ToString(); 
            }
        }

        private void button_favorite_Click_1(object sender, RoutedEventArgs e)
        {
            stack_list.PageDown();
        }

        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType() == typeof(TextBlock))
            {
                Title = (e.Source as TextBlock).Tag.ToString();
            }
            else
            if (e.Source.GetType() == typeof(Image))
            {
                Title = (e.Source as Image).Tag.ToString();
            }
        }
    }
}
