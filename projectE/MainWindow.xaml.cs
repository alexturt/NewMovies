using System;
using System.Data;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Diagnostics;

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
            CheckSettings();
            columns_count = 3;
            limit = columns_count * 8;
            offset = 0;
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Все", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Рекомендовано", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Избранное", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за сегодня", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за неделю", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за месяц", Foreground = Brushes.LightGray, Background = Brushes.Transparent });
            combobox_top_choose.SelectedIndex = 0;
        }
        DB db = new DB();
        DataTable dt_movies = new DataTable();
        int columns_count;
        int limit;
        int offset;
        int allmoviesCount;
        static BitmapImage noFavoriteImg = new BitmapImage(new Uri("/Resources/пустаязвезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage FavoriteImg = new BitmapImage(new Uri("/Resources/звезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage noWatchedImg = new BitmapImage(new Uri("/Resources/nowatched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage WatchedImg = new BitmapImage(new Uri("/Resources/watched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage posterNONE = new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

        //нажатие на кнопку открыть/закрыть правую панель
        private void button_panel_Click(object sender, RoutedEventArgs e)
        {
            if (grid.ColumnDefinitions[2].IsEnabled)
                closePanel();
            else
                openPanel();
        }
        //открытие правой панели
        void openPanel()
        {//тут все работает ок
            if (grid.ColumnDefinitions[2].IsEnabled)
                return;
            if (Width > 800)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
            }
            else
            {
                grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
                Width += grid.ColumnDefinitions[1].ActualWidth;
            }
            //recommends_load();
        }
        //закрытие правой панели
        void closePanel()
        {//тут все работает ок
            if (!grid.ColumnDefinitions[2].IsEnabled)
                return;
            double size = grid.ColumnDefinitions[2].ActualWidth;
            grid.ColumnDefinitions[2].IsEnabled = false;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            Width -= size;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSettings();
            offset = 0;
            update_movies("Все", limit, offset);
            show_movies(grid_list, button_sctoll_top);
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_content, button_sctoll_top);


        }
        //тут одни костыли
        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить номер строки
            //bool favorite = Convert.ToBoolean(dt_movies.Rows[index]["favorite"]);
            //изменяем в бд
            if (((Image)(sender as Button).Content).Source == FavoriteImg)
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                //dt_movies.Rows[index]["favorite"] = false;
                db.SetFavorite(index, false);
                //if (combobox_top_choose.SelectedIndex == 2)
                //    offset--;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                //dt_movies.Rows[index]["favorite"] = true;
                db.SetFavorite(index, true);
            }
        }
        //нажатие на кнопку добавить/удалить из просмотренного
        //private void button_watched_Click(object sender, RoutedEventArgs e)
        //{
        //    int index = Convert.ToInt32((sender as Button).Tag);//получить номер строки
        //    bool watched = Convert.ToBoolean(dt_movies.Rows[index]["watched"]);
        //    db.SetWatched(Convert.ToInt32(dt_movies.Rows[index]["id"]), !watched);//изменяем в бд
        //    if (watched)
        //    {
        //        (sender as Button).Content = new Image() { Source = noWatchedImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
        //        dt_movies.Rows[index]["watched"] = false;
        //    }
        //    else
        //    {
        //        (sender as Button).Content = new Image() { Source = WatchedImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
        //        dt_movies.Rows[index]["watched"] = true;
        //    }
        //}

        //определение id фильма по нажатию на элементы в центральном гриде и открытие правой вкладке и инфой о фильме
        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == null)
                return;
            if (e.Source.GetType() == typeof(TextBlock))
            {
                try
                {
                    Title.Text = (e.Source as TextBlock).Tag.ToString();
                    grid_content.MouseLeftButtonUp -= grid_list_MouseLeftButtonUp_1;
                    //GC.Collect();
                    dt_movies = db.GetMovie(Convert.ToInt32((e.Source as TextBlock).Tag));
                    content_load(0);//показать инфу о фильме в правой вкладке
                }
                catch { }
            }
            else
            if (e.Source.GetType() == typeof(Image))
            {
                Title.Text = (e.Source as Image).Tag.ToString();
                grid_content.MouseLeftButtonUp -= grid_list_MouseLeftButtonUp_1;
                //GC.Collect();
                dt_movies = db.GetMovie(Convert.ToInt32((e.Source as Image).Tag));
                content_load(0);//показать инфу о фильме в правой вкладке
            }
            e.Handled = true;//это чтобы родительские элементы ничего не натворили
        }
        //Подгрузка контента справа
        private void content_load(int index)
        {
            textBox_content_headet.Text = "Подробное описание";
            scroll_viewer_content.ScrollToTop();
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();
            grid_content.ColumnDefinitions.Clear();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());

            Image img = new Image()
            {
                Name = "image_right_content",
                Source = dt_movies.Rows[index]["poster"].GetType() == typeof(DBNull) ? posterNONE : LoadImage((byte[])dt_movies.Rows[index]["poster"]),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = index
            };

            TextBlock tb = new TextBlock();
            tb.Inlines.Add(createURL(dt_movies.Rows[index]["URLinfo"].ToString(), dt_movies.Rows[index]["name"].ToString(), 22, Brushes.AliceBlue));
            Run run = new Run()
            {
                FontSize = 16,
                Text = Environment.NewLine +
                    dt_movies.Rows[index]["year"].ToString() + "\r\n\r\n" +
                    dt_movies.Rows[index]["country"].ToString() + "\r\n\r\n" +
                    dt_movies.Rows[index]["genres"].ToString() + "\r\n\r\n" +
                    ConvertDate(dt_movies.Rows[index]["date"].ToString()) + "\r\n\r\n" +
                    dt_movies.Rows[index]["agerating"].ToString() + "\r\n\r\n",
                Foreground = Brushes.LightGray,
                Tag = index
            };
            tb.Inlines.Add(run);

            if (dt_movies.Rows[index]["URLtrailer"].ToString() != "")
                tb.Inlines.Add(createURL(dt_movies.Rows[index]["URLtrailer"].ToString(), "Трейлер\r\n", 14, Brushes.AliceBlue));
            if (dt_movies.Rows[index]["URLinfo"].ToString() != "")
                tb.Inlines.Add(createURL(dt_movies.Rows[index]["URLinfo"].ToString(), "Подробнее...\r\n", 14, Brushes.AliceBlue));
            if (dt_movies.Rows[index]["URLwatch"].ToString() != "")
                tb.Inlines.Add(createURL(dt_movies.Rows[index]["URLwatch"].ToString(), "Просмотр\r\n", 14, Brushes.AliceBlue));
            tb.TextWrapping = TextWrapping.WrapWithOverflow;
            tb.Padding = new Thickness(5, 5, 5, 5);
            TextBlock tb2 = new TextBlock()
            {
                Name = "textblock_right_description",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 14,
                Text = dt_movies.Rows[index]["description"].ToString(),
                //Margin = new Thickness(5,5,5,5),
                Foreground = Brushes.LightGray,
                Padding = new Thickness(5, 5, 5, 5),
                Tag = index
            };

            Grid.SetColumn(img, 0);
            Grid.SetRow(img, 0);
            grid_content.Children.Add(img);

            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 0);
            grid_content.Children.Add(tb);

            Grid.SetColumn(tb2, 0);
            Grid.SetColumnSpan(tb2, 2);
            Grid.SetRow(tb2, 1);
            grid_content.Children.Add(tb2);
            Title.Text = grid_content.RowDefinitions.Count.ToString();
        }
        //для вкладки подробнее создание гиперссылок
        private Hyperlink createURL(string url, string name, int size, Brush brush)
        {
            Uri uri = null;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            Hyperlink link = new Hyperlink()
            {
                NavigateUri = uri,
                Foreground = brush,
                FontSize = size
            };
            link.Inlines.Add(name);
            link.RequestNavigate += Hyperlink_RequestNavigate;
            return link;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.ToString());
            }
            catch { }
        }
        //







        //Костыль 
        public void update_movies(string movies, int limit, int offset)
        {
            switch (movies)
            {
                case "Все":
                    if (!IsChecked[1])//Проверка на возраст /18+ под запретом
                    {
                        dt_movies = db.GetMovies(limit, offset, true);
                        allmoviesCount = db.GetMoviesCount();
                        break;
                    }
                    else
                    {
                        dt_movies = db.GetMovies(limit, offset);
                        allmoviesCount = db.GetMoviesCount();
                        break;
                    }
                case "Рекомендовано":
                    dt_movies = db.GetRecommends();
                    //allmoviesCount = db.GetMoviesCount();
                    textBox_content_headet.Text = "Рекомендовано";
                    grid_content.MouseLeftButtonUp += grid_list_MouseLeftButtonUp_1;
                    break;
                case "Новинки за сегодня":
                    dt_movies = db.GetMoviesToday();
                    allmoviesCount = db.GetMoviesTodayCount();
                    break;
                case "Новинки за неделю":
                    dt_movies = db.GetMoviesWeek();
                    allmoviesCount = db.GetMoviesWeekCount();
                    break;
                case "Новинки за месяц":
                    dt_movies = db.GetMoviesMonth();
                    allmoviesCount = db.GetMoviesMonthCount();
                    break;
                case "Избранное":
                    dt_movies = db.GetFavorites(limit, offset);
                    allmoviesCount = db.GetFavoritesCount();
                    break;
            }
            GC.Collect();
            GC.Collect();
        }
        /// <summary>
        /// выгрузка фильмов из БД и вывод их в _grid
        /// </summary>
        /// <param name="_grid">куда выводить фильмы</param>
        /// <param name="_scrollViewer">скролл в котором находится грид</param>
        /// <param name="_button_scroll">кнопка скролла вверх</param>
        /// <param name="_dt">команда бд или таблица с фильмами</param>
        private void show_movies(Grid _grid, Button _button_scroll)
        {
            _button_scroll.IsEnabled = false;//выключение кнопки "вверх"
            _button_scroll.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            _grid.Children.Clear();//очистить элементы из центрального грида
            _grid.RowDefinitions.Clear();//удалить все строки из центрального грида
            _grid.ColumnDefinitions.Clear();
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Grid grid_row = null;
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (i % columns_count == 0)
                {
                    _grid.RowDefinitions.Add(new RowDefinition());
                    grid_row = new Grid();
                    Grid.SetColumn(grid_row, 0);
                    Grid.SetRow(grid_row, i / columns_count);
                    for (int j = 0; j < columns_count; j++)
                    {
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                    }
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    _grid.Children.Add(grid_row);
                }
                create_and_add_elements(grid_row, i);
            }
            if (dt_movies.Rows.Count == 0)
            {
                _grid.RowDefinitions.Add(new RowDefinition());
                TextBlock tb = new TextBlock()
                {
                    Text = ((TextBlock)combobox_top_choose.SelectedItem).Text + " нет",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18,
                    Foreground = Brushes.LightGray
                };
                Grid.SetRow(tb, 0);
                _grid.Children.Add(tb);
            }
        }

        private void create_and_add_elements(Grid _grid_row, int i)
        {
            Button btf = new Button()//кнопка добавления/удаления из избранного
            {
                Name = "button_favorite" + i,
                Height = 40,
                Width = 40,
                //Background = null,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0, 0, 0, 0),
                //присвоение соответсвтующей картинки
                Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) },
                Tag = dt_movies.Rows[i]["id"]//index(не id)
            };
            btf.Click += button_favorite_Click;//привязывание события клика
            //Button btw = new Button()//кнопка добавления/удаления из просмотренного
            //{
            //    Name = "button_watched" + i,
            //    Height = 40,
            //    Width = 40,
            //    Background = null,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    BorderThickness = new Thickness(0, 0, 0, 0),
            //    //присвоение соответсвтующей картинки
            //    Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["watched"]) == false ? noWatchedImg : WatchedImg, Stretch = Stretch.Fill, Margin = new Thickness(5) },
            //    Tag = i//id фильма
            //};
            //btw.Click += button_watched_Click;//привязывание события клика
            TextBlock tb = new TextBlock()//текст справа от постера
            {
                Name = "textBlock_middle_content" + i,
                TextWrapping = TextWrapping.WrapWithOverflow,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Text = dt_movies.Rows[i]["name"].ToString(),
                Foreground = Brushes.LightGray,
                FontSize = 14,
                Padding = new Thickness(5, 5, 5, 25),
                Tag = dt_movies.Rows[i]["id"]//index (не id)
            };
            Image img = new Image()//постер
            {
                Source = dt_movies.Rows[i]["poster"].GetType() == typeof(DBNull) ? posterNONE : LoadImage((byte[])dt_movies.Rows[i]["poster"]),//картинка из массива по id
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = dt_movies.Rows[i]["id"]//index (не id)
            };
            //расстановка и добавление элементов в грид

            Grid.SetColumn(img, i % columns_count * 2);
            Grid.SetColumnSpan(img, 2);
            Grid.SetRow(img, 0);
            _grid_row.Children.Add(img);

            Grid.SetColumn(tb, i % columns_count * 2);
            Grid.SetColumnSpan(tb, 2);
            Grid.SetRow(tb, 1);
            _grid_row.Children.Add(tb);

            //Grid.SetColumn(btf, i % columns_count * 2 + 1);
            //Grid.SetRow(btf, 1);
            //_grid_row.Children.Add(btf);

            //кнопка на постере //в методах сверху убрать лишние колонки
            Grid.SetColumn(btf, i % columns_count * 2 + 1);
            Grid.SetRow(btf, 0);
            _grid_row.Children.Add(btf);
        }
        //для отладки и прочего
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)//если нажали пробел
            {
            }
        }
        bool isRun;
        private void Upd()
        {
            isRun = true;
            Parser parser = new Parser();
            parser.UpdateList();
            isRun = false;
        }
        //нажали кнопку домой
        private void button_home_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(Upd);
            if (!isRun)
            {
                thread.Start();
            }
            combobox_top_choose.SelectedIndex = 0;
            offset = 0;
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_content, button_sctoll_top);

            update_movies("Все", limit, offset);
            show_movies(grid_list, button_sctoll_top);
            openPanel();
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            scroll_viewer_content.ScrollToTop();
            //thread.Abort();
            GC.Collect();
        }
        //нажали кнопку избранное (меню)
        private void button_favorite_list_Click_1(object sender, RoutedEventArgs e)
        {
            offset = 0;
            combobox_top_choose.SelectedIndex = 2;
            update_movies("Избранное", limit, offset);//показывает избранные фильмы
            show_movies(grid_list, button_sctoll_top);
            scroll_viewer_center.ScrollToTop();//проскролить вверх
        }
        //нажали кнопку закрыть окно
        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            Process.GetCurrentProcess().Kill();
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
                if (!grid.ColumnDefinitions[2].IsEnabled)
                    openPanel();
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
        //при изменении размера окна
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Width < 800 && grid.ColumnDefinitions[2].IsEnabled)//если ширина окна меньше 700
                closePanel(); // закрываем правую панель
            if (WindowState == WindowState.Maximized)
                BorderThickness = new Thickness(7);
            else
                BorderThickness = new Thickness(0);
        }
        //нажали кнопку "вверх"
        private void button_sctoll_top_Click(object sender, RoutedEventArgs e)
        {
            offset = 0;
            string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
            update_movies(str, limit, offset);
            show_movies(grid_list, button_sctoll_top);
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            button_sctoll_top.IsEnabled = false;//выключить кнопку
            button_sctoll_top.Visibility = Visibility.Hidden;//спрятать кнопку
        }
        //покрутили колесико в центральной вкладке
        private void stack_list_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (stack_list.Height > scroll_viewer_content.Height)
                if (e.Delta < 0)//если покрутили вниз
                {
                    button_sctoll_top.IsEnabled = true;//включить кнопку "вверх"
                    button_sctoll_top.Visibility = Visibility.Visible;//показать кнопку "вверх"
                }
        }
        //покрутили колесико в правой вкладке
        private void stack_content_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (stack_content.Height > scroll_viewer_content.Height)
                if (e.Delta < 0)//если покрутили вниз
                {
                    button_content_sctoll_top.IsEnabled = true;//включить кнопку "вверх"
                    button_content_sctoll_top.Visibility = Visibility.Visible;//показать кнопку "вверх"
                }
        }
        //нажали кнопку просмотренное (меню)
        private void button_watched_Click_1(object sender, RoutedEventArgs e)
        {
            //watched_load();//показывает просмотренные фильмы
            //scroll_viewer_center.ScrollToTop();//проскролить вверх
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {//удаление удаленных записей из файла БД
            db.Vacuum();
        }
        //из массива байт в картинку
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        //конвертирует дату из "гггг.мм.дд 00:00:00" в "20 июня 2019"
        private string ConvertDate(string str)
        {
            if (str == "01.01.0001 0:00:00")
                return "Вышел";
            if (str != null && str != "")
            {
                string[] temp = str.Remove(10).Split('.');
                switch (temp[1])
                {
                    case "01":
                        temp[1] = "Января";
                        break;
                    case "02":
                        temp[1] = "Февраля";
                        break;
                    case "03":
                        temp[1] = "Марта";
                        break;
                    case "04":
                        temp[1] = "Апреля";
                        break;
                    case "05":
                        temp[1] = "Мая";
                        break;
                    case "06":
                        temp[1] = "Июня";
                        break;
                    case "07":
                        temp[1] = "Июля";
                        break;
                    case "08":
                        temp[1] = "Августа";
                        break;
                    case "09":
                        temp[1] = "Сентября";
                        break;
                    case "10":
                        temp[1] = "Октября";
                        break;
                    case "11":
                        temp[1] = "Ноября";
                        break;
                    case "12":
                        temp[1] = "Декабря";
                        break;
                }
                str = temp[0] + " " + temp[1] + " " + temp[2];
            }
            else
                str = "Скоро";
            return str;
        }
        //костыль для прокрутки (сохраняет высоту предыдущего scroll_view)
        double kostil = 0;
        private void scroll_viewer_center_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == scroll_viewer_center.ScrollableHeight
                && combobox_top_choose.SelectedIndex != -1 && offset + limit < allmoviesCount
                && dt_movies.Rows.Count > 0)
            {
                kostil = scroll_viewer_center.ScrollableHeight;
                offset += limit;
                string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                update_movies(str, limit, offset);
                show_movies(grid_list, button_sctoll_top);
                scroll_viewer_center.ScrollToVerticalOffset(10);
            }
            if (e.VerticalOffset == 0 && combobox_top_choose.SelectedIndex != -1)
            {
                if (offset >= limit && dt_movies.Rows.Count > 0)
                {
                    offset -= limit;
                    string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                    update_movies(str, limit, offset);
                    show_movies(grid_list, button_sctoll_top);
                    scroll_viewer_center.ScrollToVerticalOffset(kostil - 10);
                }
                button_sctoll_top.IsEnabled = false;
                button_sctoll_top.Visibility = Visibility.Hidden;
            }
            else
            {
                button_sctoll_top.IsEnabled = true;
                button_sctoll_top.Visibility = Visibility.Visible;
            }

        }

        private void scroll_viewer_content_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == 0)
            {
                button_content_sctoll_top.IsEnabled = false;
                button_content_sctoll_top.Visibility = Visibility.Hidden;
            }
            else
            {
                button_content_sctoll_top.IsEnabled = true;
                button_content_sctoll_top.Visibility = Visibility.Visible;
            }
        }

        private void button_content_sctoll_top_Click(object sender, RoutedEventArgs e)
        {
            scroll_viewer_content.ScrollToTop();//проскролить вверх
            button_content_sctoll_top.IsEnabled = false;//выключить кнопку
            button_content_sctoll_top.Visibility = Visibility.Hidden;//спрятать кнопку
        }

        private void combobox_top_choose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((TextBlock)e.AddedItems[0]).Text)
            {
                case "Все":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за сегодня":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за неделю":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за месяц":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Рекомендовано":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_content, button_content_sctoll_top);
                    scroll_viewer_content.ScrollToTop();
                    break;
                case "Избранное":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_content_sctoll_top);
                    scroll_viewer_center.ScrollToTop();
                    break;
            }
        }
        //






        // Блок методов для меню настроек -->

        const int settings_amount = 8;
        bool[] IsChecked = new bool[settings_amount];

        //Settings 
        // 0 - notify; 1 - age; 2 - netflix_com; 3 - ivi_ru;
        // 4 - lostfilm_tv; 5 - kinokrad_co; 6 - filmzor_net; 7 - hdkinozor_ru;

        public void CheckSettings()
        {
            DataTable dt = db.GetSettings();
            for (int i = 0; i < settings_amount; i++)
            {
                IsChecked[i] = Convert.ToBoolean(dt.Rows[i].ItemArray[0].ToString());
            }
        }

        private void settings_load()//Подгрузка настроек
        {
            textBox_content_headet.Text = "Настройки";
            CheckSettings();
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();
            grid_content.ColumnDefinitions.Clear();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            //grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            //grid_content.RowDefinitions.Add(new RowDefinition());
            //grid_content.ShowGridLines = true;

            var foreColor = Brushes.WhiteSmoke;
            var backColor = Brushes.Cornsilk;
            var fontSize = 14;

            Button export_my_butt = new Button()//Экспорт
            {
                Name = "button_in_settings",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 30,
                FontSize = fontSize,
                Width = 160,
                Background = Brushes.Black,
                Foreground = foreColor,
                Content = "Экспорт настроек",
                ClickMode = ClickMode.Press,
                //Padding = new Thickness(50, 50, 50, 50)
            };
            Button import_my_butt = new Button()//Импорт
            {
                Name = "button_in_settings",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 30,
                FontSize = fontSize,
                Width = 160,
                Background = Brushes.Black,
                Foreground = foreColor,
                Content = "Импорт настроек",
                ClickMode = ClickMode.Press,
                //Padding = new Thickness(50, 50, 50, 50)
            };
            export_my_butt.Click += export_my_butt_Click;
            import_my_butt.Click += import_my_butt_Click;

            CheckBox notify = new CheckBox()//Уведомления
            {
                Name = "checkbox_in_settings_notify",
                IsThreeState = false,
                IsChecked = IsChecked[0],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "Включить уведомления?",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            CheckBox age = new CheckBox()//18+ защита
            {
                Name = "checkbox_in_settings_age",
                IsThreeState = false,
                IsChecked = IsChecked[1],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "Показывать 18+?",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            notify.Checked += notify_Checked;
            notify.Unchecked += notify_Unchecked;
            age.Checked += age_Checked;
            age.Unchecked += age_Unchecked;
            //notify.IsChecked = true;
            ComboBox Sources = new ComboBox()
            {
                Name = "combobox_in_settings",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 40,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            //------------//------------//
            Label lbl = new Label()//Лейбл для выбора источников
            {
                Name = "label_parser",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = fontSize,
                Content = "Выбор источников:",
                //Margin = new Thickness(5,5,5,5),
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            //------------
            CheckBox netflix_com = new CheckBox()//NetFlix
            {
                Name = "checkbox_in_settings_netflix_com",
                IsThreeState = false,
                IsChecked = IsChecked[2],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "netflix.com",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            CheckBox ivi_ru = new CheckBox()//Ivi
            {
                Name = "checkbox_in_settings_ivi_ru",
                IsThreeState = false,
                IsChecked = IsChecked[3],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "ivi.ru",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            CheckBox lostfilm_tv = new CheckBox()//Lostfilm
            {
                Name = "checkbox_in_settings_lostfilm_tv",
                IsThreeState = false,
                IsChecked = IsChecked[4],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "lostfilm.tv",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            //------------
            CheckBox kinokrad_co = new CheckBox()//Kinokrad
            {
                Name = "checkbox_in_settings_kinokrad_co",
                IsThreeState = false,
                IsChecked = IsChecked[5],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "kinokrad.co",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            CheckBox filmzor_net = new CheckBox()//Filmzor
            {
                Name = "checkbox_in_settings_filmzor_net",
                IsThreeState = false,
                IsChecked = IsChecked[6],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "filmzor.net",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            CheckBox hdkinozor_ru = new CheckBox()//HDKinozor
            {
                Name = "checkbox_in_settings_hdkinozor_ru",
                IsThreeState = false,
                IsChecked = IsChecked[7],
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "hdkinozor.ru",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
            //------------
            netflix_com.Checked += netflix_com_Checked;
            netflix_com.Unchecked += netflix_com_Unchecked;
            ivi_ru.Checked += ivi_ru_Checked;
            ivi_ru.Unchecked += ivi_ru_Unchecked;
            lostfilm_tv.Checked += lostfilm_tv_Checked;
            lostfilm_tv.Unchecked += lostfilm_tv_Unchecked;
            kinokrad_co.Checked += kinokrad_co_Checked;
            kinokrad_co.Unchecked += kinokrad_co_Unchecked;
            filmzor_net.Checked += filmzor_net_Checked;
            filmzor_net.Unchecked += filmzor_net_Unchecked;
            hdkinozor_ru.Checked += hdkinozor_ru_Checked;
            hdkinozor_ru.Unchecked += hdkinozor_ru_Unchecked;
            //------------//------------//


            Grid.SetColumn(notify, 0);
            Grid.SetRow(notify, 0);
            Grid.SetColumnSpan(notify, 2);
            grid_content.Children.Add(notify);

            Grid.SetColumn(age, 0);
            Grid.SetRow(age, 1);
            Grid.SetColumnSpan(age, 2);
            grid_content.Children.Add(age);

            Grid.SetColumn(import_my_butt, 3);
            Grid.SetRow(import_my_butt, 0);
            grid_content.Children.Add(import_my_butt);

            Grid.SetColumn(export_my_butt, 3);
            Grid.SetRow(export_my_butt, 1);
            grid_content.Children.Add(export_my_butt);

            Grid.SetColumn(Sources, 0);
            Grid.SetRow(Sources, 2);
            grid_content.Children.Add(Sources);

            Grid.SetColumn(lbl, 0);
            Grid.SetRow(lbl, 4);
            grid_content.Children.Add(lbl);

            Grid.SetColumn(netflix_com, 0);
            Grid.SetRow(netflix_com, 5);
            grid_content.Children.Add(netflix_com);

            Grid.SetColumn(ivi_ru, 1);
            Grid.SetRow(ivi_ru, 5);
            grid_content.Children.Add(ivi_ru);

            Grid.SetColumn(lostfilm_tv, 2);
            Grid.SetRow(lostfilm_tv, 5);
            grid_content.Children.Add(lostfilm_tv);

            Grid.SetColumn(kinokrad_co, 0);
            Grid.SetRow(kinokrad_co, 6);
            grid_content.Children.Add(kinokrad_co);

            Grid.SetColumn(filmzor_net, 1);
            Grid.SetRow(filmzor_net, 6);
            grid_content.Children.Add(filmzor_net);

            Grid.SetColumn(hdkinozor_ru, 2);
            Grid.SetRow(hdkinozor_ru, 6);
            grid_content.Children.Add(hdkinozor_ru);
        }

        //Import
        void import_my_butt_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = new string[16];
            string filename = "";//Полный адрес файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);//Начальная директория
            openFileDialog.Filter = "Only NewMovies settings file (*.nmsettings)|*.nmsettings";//Фильтр по расширению файла

            if (openFileDialog.ShowDialog() == true)//Выбор файла *.settings
            {
                filename = openFileDialog.FileName;
            }
            else//Если файл не был выбран
            {
                return;//Выход из обработчика события
            }
            using (StreamReader sr = new StreamReader(filename))
            {
                lines = sr.ReadLine().Split('=', ';');
            }
            int temp = 0;
            for (int j = 1; j < lines.Length; j += 2)
            {
                IsChecked[temp] = Convert.ToBoolean(lines[j]);
                db.SetSettings(temp.ToString(), IsChecked[temp], true);
                temp++;
            }
            settings_load();
            MessageBox.Show("Импорт настроек прошёл успешно!");
        }

        //Export
        void export_my_butt_Click(object sender, RoutedEventArgs e)
        {
            string line = "";
            string path = "";

            for (int i = 0; i < IsChecked.Length; i++)
            {
                line += i + "=" + IsChecked[i] + ";";
            }

            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);//Начальная директория
            folderBrowserDialog.Description = "Выберите, куда сохранить файл с настройками:";
            folderBrowserDialog.ShowNewFolderButton = false;

            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            path = folderBrowserDialog.SelectedPath + "\\NewMovies_v" + System.Windows.Forms.Application.ProductVersion.ToString() + "_export_file.nmsettings";

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(line);
            }
            MessageBox.Show("Для восстановления настроек выберите для импорта этот файл: " + path);
        }

        //Notification (0)
        void notify_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[0] = true;
            db.SetSettings("notify", IsChecked[0]);
            settings_load();
        }

        void notify_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[0] = false;
            db.SetSettings("notify", IsChecked[0]);
            settings_load();
        }

        //Age verification (1)
        void age_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[1] = true;
            db.SetSettings("age", IsChecked[1]);
            settings_load();
        }

        void age_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[1] = false;
            db.SetSettings("age", IsChecked[1]);
            settings_load();
        }

        //Netflix (2)
        void netflix_com_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[2] = true;
            db.SetSettings("netflix_com", IsChecked[2]);
            settings_load();
        }

        void netflix_com_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[2] = false;
            db.SetSettings("netflix_com", IsChecked[2]);
            settings_load();
        }

        //Ivi (3)
        void ivi_ru_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[3] = true;
            db.SetSettings("ivi_ru", IsChecked[3]);
            settings_load();
        }

        void ivi_ru_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[3] = false;
            db.SetSettings("ivi_ru", IsChecked[3]);
            settings_load();
        }

        //Lostfilm (4)
        void lostfilm_tv_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[4] = true;
            db.SetSettings("lostfilm_tv", IsChecked[4]);
            settings_load();
        }

        void lostfilm_tv_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[4] = false;
            db.SetSettings("lostfilm_tv", IsChecked[4]);
            settings_load();
        }

        //Kinokrad (5)
        void kinokrad_co_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[5] = true;
            db.SetSettings("kinokrad_co", IsChecked[5]);
            settings_load();
        }

        void kinokrad_co_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[5] = false;
            db.SetSettings("kinokrad_co", IsChecked[5]);
            settings_load();
        }

        //Filmzor (6)
        void filmzor_net_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[6] = true;
            db.SetSettings("filmzor_net", IsChecked[6]);
            settings_load();
        }

        void filmzor_net_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[6] = false;
            db.SetSettings("filmzor_net", IsChecked[6]);
            settings_load();
        }

        //Hdkinozor (7)
        void hdkinozor_ru_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked[7] = true;
            settings_load();
        }

        void hdkinozor_ru_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked[7] = true;
            settings_load();
        }

        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            settings_load();
            ShowNotification();
        }

        // <-- Блок методов для меню настроек




        // Блок методов для уведомлений -->

        public void ShowNotification(int time = 10000, string header = "Notification", string text = "All is Okay!")
        {
            if (IsChecked[0])
            {
                System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
                notifyIcon.Visible = true;
                using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico")).Stream)
                {
                    notifyIcon.Icon = new System.Drawing.Icon(iconStream);
                }
                notifyIcon.ShowBalloonTip(time, header, text, notifyIcon.BalloonTipIcon);
            }
        }
        
        // <-- Блок методов для уведомлений
    }
}
