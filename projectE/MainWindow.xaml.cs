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
            columns_count = 2;
            columns_count_recommends = 4;
            limit = columns_count * 8;
            //limit = 0;
            offset = 0;
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Все", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Рекомендовано", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Избранное", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за сегодня", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за неделю", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за месяц", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.SelectedIndex = 0;
            //DataTable dt =  db.GetMovieF();
            { }
        }
        DB db = new DB();
        DataTable dt_movies = new DataTable();
        int columns_count;
        int columns_count_recommends;
        int limit;
        int offset;
        int allmoviesCount;
        double scroll_viewer_right_last_height;
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
                grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
            }
            else
            {
                grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
                Width += grid.ColumnDefinitions[1].ActualWidth*1.5;
            }
        }
        void openPanel_withoutSizeChange()
        {//тут все работает ок
            if (grid.ColumnDefinitions[2].IsEnabled)
                return;
            if (Width > 800)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
            }
            else
            {
                grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
                //Width += grid.ColumnDefinitions[1].ActualWidth * 2;
            }
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
            show_movies(grid_list, button_sctoll_top, columns_count);
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_recommends, button_sctoll_top, columns_count_recommends);
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
                    Title.Text = (e.Source as TextBlock).Tag.ToString();
                    grid_content.MouseLeftButtonUp -= grid_list_MouseLeftButtonUp_1;
                    //GC.Collect();
                    dt_movies = db.GetMovie(Convert.ToInt32((e.Source as TextBlock).Tag));
                    content_load(0);//показать инфу о фильме в правой вкладке
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
            grid_recommends.Visibility = Visibility.Collapsed;
            grid_content.Visibility = Visibility.Visible;
            button_panel_close.Visibility = Visibility.Visible;
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();
            grid_content.ColumnDefinitions.Clear();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth=300});
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());

            Image img = new Image()
            {
                Name = "image_right_content",
                Source = dt_movies.Rows[index]["poster"].GetType() == typeof(DBNull) ? posterNONE : LoadImage((byte[])dt_movies.Rows[index]["poster"]),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = index,
                MaxWidth = 300
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
                    dt_movies = db.GetMovies(limit, offset);
                    allmoviesCount = db.GetMoviesCount();
                    break;
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
                    allmoviesCount = db.GetMoviesWeekCount();
                    break;
            }
            GC.Collect();
        }
        /// <summary>
        /// выгрузка фильмов из БД и вывод их в _grid
        /// </summary>
        /// <param name="_grid">куда выводить фильмы</param>
        /// <param name="_scrollViewer">скролл в котором находится грид</param>
        /// <param name="_button_scroll">кнопка скролла вверх</param>
        /// <param name="_dt">команда бд или таблица с фильмами</param>
        private void show_movies(Grid _grid, Button _button_scroll, int _columns_count)
        {
            _button_scroll.IsEnabled = false;//выключение кнопки "вверх"
            _button_scroll.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            _grid.Visibility = Visibility.Visible;
            _grid.Children.Clear();//очистить элементы из центрального грида
            _grid.RowDefinitions.Clear();//удалить все строки из центрального грида
            _grid.ColumnDefinitions.Clear();
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Grid grid_row = null;
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (i % _columns_count == 0)
                {
                    _grid.RowDefinitions.Add(new RowDefinition());
                    grid_row = new Grid();
                    Grid.SetColumn(grid_row, 0);
                    Grid.SetRow(grid_row, i / _columns_count);
                    for (int j = 0; j < _columns_count; j++)
                    {
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                    }
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    _grid.Children.Add(grid_row);
                }
                create_and_add_elements(grid_row, i, _columns_count);
            }
            string str = "Пусто";
            switch (((TextBlock)combobox_top_choose.SelectedItem).Text)
            {
                case "Все":
                    str = "Фильмов в базе данных нет ☹";
                    break;
                case "Рекомендовано":
                    str = "Рекомендованных фильмов нет ☹";
                    break;
                case "Избранное":
                    str = "Список избранного пуст ☹";
                    break;
                case "Новинки за сегодня":
                    str = "Новинок за сегодня нет ☹";
                    break;
                case "Новинки за неделю":
                    str = "Новинок за неделю нет ☹";
                    break;
                case "Новинки за месяц":
                    str = "Новинок за месяц нет ☹";
                    break;
            }
            if (dt_movies.Rows.Count == 0)
            {
                _grid.RowDefinitions.Add(new RowDefinition());
                TextBlock tb = new TextBlock()
                {
                    Text = str,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18,
                    Foreground = Brushes.LightGray,
                    IsEnabled = false
                };
                Grid.SetRow(tb, 0);
                _grid.Children.Add(tb);
            }
        }

        private void create_and_add_elements(Grid _grid_row, int i, int _columns_count)
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

            Grid.SetColumn(img, i % _columns_count * 2);
            Grid.SetColumnSpan(img, 2);
            Grid.SetRow(img, 0);
            _grid_row.Children.Add(img);

            Grid.SetColumn(tb, i % _columns_count * 2);
            Grid.SetColumnSpan(tb, 2);
            Grid.SetRow(tb, 1);
            _grid_row.Children.Add(tb);

            //Grid.SetColumn(btf, i % columns_count * 2 + 1);
            //Grid.SetRow(btf, 1);
            //_grid_row.Children.Add(btf);

            //кнопка на постере //в методах сверху убрать лишние колонки
            Grid.SetColumn(btf, i % _columns_count * 2 + 1);
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
            if (combobox_top_choose.SelectedIndex == 0 && textBox_content_headet.Text == "Рекомендовано")
                return;
            combobox_top_choose.SelectedIndex = 0;
            offset = 0;
            button_panel_close.Visibility = Visibility.Collapsed;
            grid_content.Visibility = Visibility.Collapsed;
            grid_recommends.Visibility = Visibility.Visible;
            textBox_content_headet.Text = "Рекомендовано";
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_recommends, button_sctoll_top, columns_count_recommends);

            update_movies("Все", limit, offset);
            show_movies(grid_list, button_sctoll_top, columns_count);
            openPanel();
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            scroll_viewer_content.ScrollToTop();
            //thread.Abort();
            GC.Collect();
        }
        //нажали кнопку избранное (меню)
        private void button_favorite_list_Click_1(object sender, RoutedEventArgs e)
        {
            if (combobox_top_choose.SelectedIndex == 2)
                return;
            offset = 0;
            combobox_top_choose.SelectedIndex = 2;
            update_movies("Избранное", limit, offset);//показывает избранные фильмы
            show_movies(grid_list, button_sctoll_top, columns_count);
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
            if (Width < 645 && grid.ColumnDefinitions[2].IsEnabled)//если ширина окна меньше 700
                closePanel(); // закрываем правую панель
            if (Width > 644 && !grid.ColumnDefinitions[2].IsEnabled)
                openPanel_withoutSizeChange();
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
            show_movies(grid_list, button_sctoll_top, columns_count);
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            button_sctoll_top.IsEnabled = false;//выключить кнопку
            button_sctoll_top.Visibility = Visibility.Hidden;//спрятать кнопку
            //lists = 1;
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
        int lists = 1;
        private void scroll_viewer_center_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Title.Text = lists.ToString() + " " + offset.ToString();
            if (e.VerticalOffset == scroll_viewer_center.ScrollableHeight
                && combobox_top_choose.SelectedIndex != -1 && offset + limit < allmoviesCount
                && dt_movies.Rows.Count > 0)
            {
                lists = offset / limit + 1;
                kostil = scroll_viewer_center.ScrollableHeight;
                offset += limit;
                string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                update_movies(str, limit, offset - columns_count* columns_count*lists);
                show_movies(grid_list, button_sctoll_top, columns_count);
                scroll_viewer_center.ScrollToVerticalOffset(10);
                //lists++;
            }
            if (e.VerticalOffset == 0 && combobox_top_choose.SelectedIndex != -1)
            {
                if (offset >= limit && dt_movies.Rows.Count > 0)
                {
                    offset -= limit;
                    lists = offset / limit;
                    string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                    update_movies(str, limit, offset - columns_count * columns_count * lists);
                    show_movies(grid_list, button_sctoll_top, columns_count);
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
            if (grid_recommends.Visibility == Visibility.Visible)
                scroll_viewer_right_last_height = e.VerticalOffset;
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
                    show_movies(grid_list, button_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за сегодня":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за неделю":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Новинки за месяц":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToTop();
                    break;
                case "Рекомендовано":
                    button_panel_close.Visibility = Visibility.Collapsed;
                    grid_content.Visibility = Visibility.Collapsed;
                    grid_recommends.Visibility = Visibility.Visible;
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_recommends, button_content_sctoll_top, columns_count_recommends);
                    scroll_viewer_content.ScrollToTop();
                    break;
                case "Избранное":
                    update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                    show_movies(grid_list, button_content_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToTop();
                    break;
            }

            
        }
        //






        // Блок методов для меню настроек -->

        const int settings_amount = 8;
        bool[] IsChecked = new bool[settings_amount];

        static Brush foreColorEnabled = Brushes.Green;
        static Brush foreColor = Brushes.WhiteSmoke;
        static Brush foreColorDisabled = Brushes.Red;
        static Brush backColorNULL = null;
        static Brush backColor = Brushes.WhiteSmoke;
        static Brush backColorButt = Brushes.Black;
        static Thickness borderThickness = new Thickness(0, 0, 0, 0);
        static int fontSize = 14;

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
            db.updateAgeRestriction(IsChecked[1]);
            dt.Dispose();
        }

        //Sources
        void Sources_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var temp = (sender as ComboBox).SelectedIndex;
            if (temp == 0)
            {
                settings_load();
            }
            else
            {
                db.SetSettings((temp + 1).ToString(), !IsChecked[temp + 1], true);
                settings_load();
            }
        }

        private void settings_load()//Подгрузка настроек
        {
            textBox_content_headet.Text = "Настройки";
            CheckSettings();
            GC.Collect();
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();
            grid_content.ColumnDefinitions.Clear();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());
            grid_content.RowDefinitions.Add(new RowDefinition());

            Button export_my_butt = new Button()//Экспорт
            {
                Name = "button_in_settings",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 30,
                FontSize = fontSize,
                Width = 160,
                Background = backColorButt,
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
                Background = backColorButt,
                Foreground = foreColor,
                Content = "Импорт настроек",
                ClickMode = ClickMode.Press,
                //Padding = new Thickness(50, 50, 50, 50)
            };
            Button kick_my_butt = new Button()//Сброс
            {
                Name = "button_in_settings",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 30,
                FontSize = fontSize,
                Width = 100,
                Background = backColorButt,
                Foreground = foreColor,
                Content = "Сброс",
                ClickMode = ClickMode.Press,
                //Padding = new Thickness(50, 50, 50, 50)
            };
            export_my_butt.Click += export_my_butt_Click;
            import_my_butt.Click += import_my_butt_Click;
            kick_my_butt.Click += kick_my_butt_Click;

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
            
            //Лейбл для выбора источников
            Label lbl = new Label()
            {
                Name = "label_in_settings",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = fontSize,
                Content = "Выбор источников:",
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };

            //Sources
            ComboBox Sources = new ComboBox()
            {
                Name = "combobox_in_settings",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 40,
                Width = 130,
                Cursor = Cursors.Cross,
                Text = "Выбор источников:",
                Background = backColorNULL,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5),
                MaxDropDownHeight = 1000
            };
            Sources.SelectionChanged += Sources_SelectionChanged;

            //Блок костылей
            TextBlock hdkinozor_ru = new TextBlock()//HDKinozor
            {
                Name = "checkbox_in_settings_hdkinozor_ru",
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Text = "hdkinozor.ru",
                //VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColorNULL,
                Foreground = foreColorEnabled,
                Padding = new Thickness(5, 5, 5, 5),
                //BorderThickness = borderThickness,
                IsEnabled = false
            };
            Sources.Items.Add(hdkinozor_ru);

            if (IsChecked[2])
            {
                TextBlock netflix_com = new TextBlock()//NetFlix
                {
                    Name = "checkbox_in_settings_netflix_com",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "netflix.com",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorEnabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(netflix_com);
            }
            else
            {
                TextBlock netflix_com = new TextBlock()//NetFlix
                {
                    Name = "checkbox_in_settings_netflix_com",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "netflix.com",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorDisabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(netflix_com);
            }
            if (IsChecked[3])
            {
                TextBlock ivi_ru = new TextBlock()//Ivi
                {
                    Name = "checkbox_in_settings_ivi_ru",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "ivi.ru",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorEnabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(ivi_ru);
            }
            else
            {
                TextBlock ivi_ru = new TextBlock()//Ivi
                {
                    Name = "checkbox_in_settings_ivi_ru",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "ivi.ru",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorDisabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(ivi_ru);
            }
            if (IsChecked[4])
            {
                TextBlock lostfilm_tv = new TextBlock()//Lostfilm
                {
                    Name = "checkbox_in_settings_lostfilm_tv",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "lostfilm.tv",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorEnabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(lostfilm_tv);
            }
            else
            {
                TextBlock lostfilm_tv = new TextBlock()//Lostfilm
                {
                    Name = "checkbox_in_settings_lostfilm_tv",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "lostfilm.tv",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorDisabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(lostfilm_tv);
            }
            //------------
            if (IsChecked[5])
            {
                TextBlock kinokrad_co = new TextBlock()//Kinokrad
                {
                    Name = "checkbox_in_settings_kinokrad_co",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = TextBox.,
                    Text = "kinokrad.co",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorEnabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(kinokrad_co);
            }
            else
            {
                TextBlock kinokrad_co = new TextBlock()//Kinokrad
                {
                    Name = "checkbox_in_settings_kinokrad_co",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = TextBox.,
                    Text = "kinokrad.co",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorDisabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(kinokrad_co);
            }
            if (IsChecked[6])
            {
                TextBlock filmzor_net = new TextBlock()//Filmzor
                {
                    Name = "checkbox_in_settings_filmzor_net",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "filmzor.net",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorEnabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(filmzor_net);
            }
            else
            {
                TextBlock filmzor_net = new TextBlock()//Filmzor
                {
                    Name = "checkbox_in_settings_filmzor_net",
                    Height = 40,
                    FontSize = fontSize,
                    //Width = 100,
                    Text = "filmzor.net",
                    //VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = backColorNULL,
                    Foreground = foreColorDisabled,
                    Padding = new Thickness(5, 5, 5, 5),
                    //BorderThickness = borderThickness,
                };
                Sources.Items.Add(filmzor_net);
            }

            //------------//------------//
            
            Grid.SetColumn(notify, 0);
            Grid.SetRow(notify, 2);
            Grid.SetColumnSpan(notify, 3);
            grid_content.Children.Add(notify);

            Grid.SetColumn(age, 0);
            Grid.SetRow(age, 3);
            Grid.SetColumnSpan(age, 3);
            grid_content.Children.Add(age);

            Grid.SetColumn(import_my_butt, 0);
            Grid.SetRow(import_my_butt, 0);
            Grid.SetColumnSpan(import_my_butt, 3);
            grid_content.Children.Add(import_my_butt);

            Grid.SetColumn(export_my_butt, 0);
            Grid.SetRow(export_my_butt, 1);
            Grid.SetColumnSpan(export_my_butt, 3);
            grid_content.Children.Add(export_my_butt);

            Grid.SetColumn(lbl, 0);
            Grid.SetRow(lbl, 4);
            Grid.SetColumnSpan(lbl, 3);
            grid_content.Children.Add(lbl);

            Grid.SetColumn(Sources, 0);
            Grid.SetRow(Sources, 5);
            grid_content.Children.Add(Sources);

            Grid.SetColumn(kick_my_butt, 1);
            Grid.SetRow(kick_my_butt, 5);
            Grid.SetColumnSpan(kick_my_butt, 2);
            grid_content.Children.Add(kick_my_butt);

            db.updateAgeRestriction(IsChecked[1]);
        }

        //Import
        void import_my_butt_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = new string[16];
            string filename = "";//Полный адрес файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);//Начальная директория
            openFileDialog.Filter = "Only NewMovies settings file (*.nmsettings)|*.nmsettings";//Фильтр по расширению файла

            if (openFileDialog.ShowDialog() == true)//Выбор файла *.nmsettings
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

        //Reset
        void kick_my_butt_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 2; i < settings_amount-1; i++)
            {
                IsChecked[i] = false;
                db.SetSettings(i.ToString(), IsChecked[i], true);
            }
            settings_load();
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
            db.SetSettings("hdkinozor_ru", true);
        }

        void hdkinozor_ru_Unchecked(object sender, RoutedEventArgs e)
        {
            db.SetSettings("hdkinozor_ru", true);
        }

        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_content_headet.Text == "Настройки")
                return;
            button_panel_close.Visibility = Visibility.Visible;
            grid_content.Visibility = Visibility.Visible;
            grid_recommends.Visibility = Visibility.Collapsed;
            settings_load();
            //ShowNotification();
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
        //content close
        private void button_panel_close_Click(object sender, RoutedEventArgs e)
        {
            button_panel_close.Visibility = Visibility.Collapsed;
            grid_content.Visibility = Visibility.Collapsed;
            grid_recommends.Visibility = Visibility.Visible;
            textBox_content_headet.Text = "Рекомендовано";
            scroll_viewer_content.ScrollToVerticalOffset(scroll_viewer_right_last_height);
        }

        private void button_filtering_close_Click(object sender, RoutedEventArgs e)
        {
            grid.RowDefinitions[2].IsEnabled = false;
            grid.RowDefinitions[2].Height = new GridLength(0);
            button_filtering_open.Visibility = Visibility.Visible;
        }

        private void button_filtering_open_Click(object sender, RoutedEventArgs e)
        {
            grid.RowDefinitions[2].IsEnabled = true;
            grid.RowDefinitions[2].Height = new GridLength(1,GridUnitType.Auto);
            button_filtering_open.Visibility = Visibility.Collapsed;
            textbox_filtering.Text = "";
        }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string name = textbox_filtering.Text.ToLower();
            string genre = ((TextBlock)combobox_filter.SelectedItem).Text;
            if (combobox_filter.SelectedIndex == 0)
                genre = "";
            else
                genre = genre.Substring(0, 5);

            dt_movies = db.GetMoviesByFilter(name, genre);
            allmoviesCount = dt_movies.Rows.Count;
            show_movies(grid_list, button_sctoll_top, columns_count);
            sw.Stop();
            //textbox_filtering.Text = sw.ElapsedMilliseconds.ToString();
        }

        // <-- Блок методов для уведомлений
    }
}
