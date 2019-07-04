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
using System.Collections.Generic;


namespace projectE
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //MessageBox.Show(Environment.CurrentDirectory.ToString());
            InitializeComponent();

            hide_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\hide.png"));
            max_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\window.png"));
            exit_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\exit.png"));
            home_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\домик2.png"));
            fav_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\звезда2.png"));
            notify_image.Source = notifyImg;
            sett_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\шестерня2.png"));
            filo_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\лупастрелка.png"));
            panel_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\панель2.png"));
            seacrh_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\лупа2.png"));
            filc_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\back_to_recomends.png"));
            scroll_image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\up.png"));
            content_scroll_image.Source = scroll_image.Source;
            panel_close_image.Source = filc_image.Source;

            UpdateSettings();
            columns_count = 2;
            columns_count_recommends = 4;
            limit = columns_count * 8;
            offset = 0;
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Все", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Рекомендовано", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Избранное", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за сегодня", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за неделю", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.Items.Add(new TextBlock() { Text = "Новинки за месяц", Foreground = Brushes.LightGray, Background = Brushes.Transparent, FontSize = 14, Padding = new Thickness(5, 0, 0, 5) });
            combobox_top_choose.SelectedIndex = 0;
            update_combobox_years();
            { }
        }

        int limitWind;
        [STAThread]
        public void ShowBox(string v)
        {
            thread = new Thread(Upd);
            thread.Abort();
            MessageBox.Show(v, "Ошибка", MessageBoxButton.OK);
            isRun = false;
        }

        DB db = new DB();
        DataTable dt_movies = new DataTable();
        int columns_count;
        int columns_count_recommends;
        int limit;
        int offset;
        int allmoviesCount;
        double scroll_viewer_right_last_height;
        static BitmapImage noFavoriteImg = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString()+@"\пустаязвезда2.png"));
        static BitmapImage FavoriteImg = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\звезда2.png"));
        //static BitmapImage noWatchedImg = new BitmapImage(new Uri("/Resources/nowatched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        //static BitmapImage WatchedImg = new BitmapImage(new Uri("/Resources/watched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage posterNONE = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\poster_none.png"));
        static BitmapImage notifyImg = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\notify.png"));
        static BitmapImage redNotifyImg = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\rednotify.png"));

        List<object> tags = new List<object>();

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
                Width += grid.ColumnDefinitions[1].ActualWidth * 1.5;
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
        string password;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createNotifyIcon();
            UpdateSettings();
            offset = 0;
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_recommends, button_sctoll_top, columns_count_recommends);
            ///////////////
            int countWeek = db.GetMoviesWeekCount();
            int countFavoritesWeek = db.GetFavoritesMoviesWeekCount();
            string str = "";
            if (countFavoritesWeek > 0)
                str = " и " + countFavoritesWeek + " фильма из списка избранного";
            if (countWeek != 0)
                ShowNotification(1500000, "Новые фильмы!", "Вышло " + countWeek + " новых фильмов за неделю" + str);

            ///////////////
            //ShowNotification(1500000, "Новые фильмы!", "Вышло 6 новых фильмов за сегодня и 2 фильма из списка избранного!");
        }

        private void update_combobox_age()
        {
            combobox_filter_age.Items.Clear();
            combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "Все" });
            combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "0+" });
            combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "6+" });
            combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "12+" });
            combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "16+" });
            if (IsChecked[1])
                combobox_filter_age.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "18+" });
            combobox_filter_age.SelectedIndex = 0;
        }

        private void update_combobox_years()
        {
            combobox_filter_year.Items.Clear();
            combobox_filter_year.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = "Все" });
            combobox_filter_year.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = DateTime.Now.Year.ToString() });
            combobox_filter_year.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = (DateTime.Now.Year - 1).ToString() });
            combobox_filter_year.Items.Add(new TextBlock() { Background = null, Foreground = Brushes.LightGray, FontSize = 14, Padding = new Thickness(5, 0, 0, 5), Text = (DateTime.Now.Year - 2).ToString() });
         
            combobox_filter_year.SelectedIndex = 0;
        }
        //тут одни костыли
        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить номер строки
            //изменяем в бд
            if (((Image)(sender as Button).Content).Source == FavoriteImg)
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                db.SetFavorite(index, false);
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                db.SetFavorite(index, true);
            }
        }
        int tag_index;
        //определение id фильма по нажатию на элементы в центральном гриде и открытие правой вкладке и инфой о фильме
        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == null)
                return;
            if (e.Source.GetType() == typeof(TextBlock))
            {
                tag_index = Convert.ToInt32((e.Source as TextBlock).Tag);
                grid_right.MouseLeftButtonUp -= grid_list_MouseLeftButtonUp_1;
                GC.Collect();
                dt_movies = db.GetMovie(Convert.ToInt32((e.Source as TextBlock).Tag));
                content_load(0);//показать инфу о фильме в правой вкладке
            }
            else
            if (e.Source.GetType() == typeof(Image))
            {
                tag_index = Convert.ToInt32((e.Source as Image).Tag);
                grid_right.MouseLeftButtonUp -= grid_list_MouseLeftButtonUp_1;
                GC.Collect();
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
            grid_content.Visibility = Visibility.Collapsed;
            grid_right.Visibility = Visibility.Visible;
            button_panel_close.Visibility = Visibility.Visible;
            grid_right.Children.Clear();
            grid_right.RowDefinitions.Clear();
            grid_right.ColumnDefinitions.Clear();
            grid_right.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 300 });
            grid_right.ColumnDefinitions.Add(new ColumnDefinition());
            grid_right.RowDefinitions.Add(new RowDefinition());
            grid_right.RowDefinitions.Add(new RowDefinition());

            
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
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, 0);
                grid_right.Children.Add(img);
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
                Foreground = Brushes.LightGray,
                Padding = new Thickness(5, 5, 5, 5),
                Tag = index
            };


            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 0);
            grid_right.Children.Add(tb);

            Grid.SetColumn(tb2, 0);
            Grid.SetColumnSpan(tb2, 2);
            Grid.SetRow(tb2, 1);
            grid_right.Children.Add(tb2);
        }
        //для вкладки подробнее создание гиперссылок
        public Hyperlink createURL(string url, string name, int size, Brush brush)
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

        public void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
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
                    textBox_content_headet.Text = "Рекомендовано";
                    grid_right.MouseLeftButtonUp += grid_list_MouseLeftButtonUp_1;
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
            if (!filterIsOpen())
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
            else
                str = "Результатов поиска \"" + textbox_filtering.Text + "\", жанры: " + (combobox_filter_genres.SelectedItem as TextBlock).Text + ", возраст: " + (combobox_filter_age.SelectedItem as TextBlock).Text + ", год: " + (combobox_filter_year.SelectedItem as TextBlock).Text + " нет";
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
                    IsEnabled = false,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    TextAlignment = TextAlignment.Center
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
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Style = (Style)FindResource("ButtonStyle1"),
                    //присвоение соответсвтующей картинки
                    Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) },
                    Tag = dt_movies.Rows[i]["id"]//index
                };

                btf.Click += button_favorite_Click;//привязывание события клика
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
                    Tag = dt_movies.Rows[i]["id"]//index
                };

                Image img = new Image()//постер
                {
                    Source = dt_movies.Rows[i]["poster"].GetType() == typeof(DBNull) ? posterNONE : LoadImage((byte[])dt_movies.Rows[i]["poster"]),//картинка из массива по id
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = dt_movies.Rows[i]["id"]//index
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

                //кнопка на постере //в методах сверху убрать лишние колонки
                Grid.SetColumn(btf, i % _columns_count * 2 + 1);
                Grid.SetRow(btf, 0);
                _grid_row.Children.Add(btf);
                tags.Add(dt_movies.Rows[i]["id"]);
            
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
            GC.Collect();
        }
        Thread thread;
        //нажали кнопку домой
        private void button_home_Click(object sender, RoutedEventArgs e)
        {
            thread = new Thread(Upd);
            if (!isRun)
            {
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

            }
            if (combobox_top_choose.SelectedIndex == 0 && textBox_content_headet.Text == "Рекомендовано"
                && !button_sctoll_top.IsEnabled && !button_content_sctoll_top.IsEnabled && !filterIsOpen())
                return;
            filterClose();
            grid.RowDefinitions[2].IsEnabled = false;
            grid.RowDefinitions[2].Height = new GridLength(0);
            button_filtering_open.Visibility = Visibility.Visible;
            combobox_top_choose.SelectedIndex = 0;
            offset = 0;
            button_panel_close.Visibility = Visibility.Collapsed;
            grid_content.Visibility = Visibility.Collapsed;
            grid_right.Visibility = Visibility.Collapsed;
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
            /*try
            {
                thread.Abort();
            } catch { }*/
            GC.Collect();
            //Process.GetCurrentProcess().Kill();
            ShowInTaskbar = false;
            WindowState = WindowState.Minimized;
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
            //ShowInTaskbar = false;
            WindowState = WindowState.Minimized;
            //Hide();
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
        }
        //удаление удаленных записей из файла БД
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Vacuum();

        }
        //из массива байт в картинку
        public static BitmapImage LoadImage(byte[] imageData)
        {
            try
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
            catch
            {
                return new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            }
        }
        //конвертирует дату из "гггг.мм.дд 00:00:00" в "20 июня 2019"
        public string ConvertDate(string str)
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
            if (e.VerticalOffset == scroll_viewer_center.ScrollableHeight
                && combobox_top_choose.SelectedIndex != -1 && offset + limit < allmoviesCount
                && dt_movies.Rows.Count > 0 && !filterIsOpen())
            {
                lists = offset / limit + 1;
                kostil = scroll_viewer_center.ScrollableHeight;
                offset += limit;
                string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                update_movies(str, limit, offset - columns_count * columns_count * lists);
                show_movies(grid_list, button_sctoll_top, columns_count);
                scroll_viewer_center.ScrollToVerticalOffset(10);
            }
            if (e.VerticalOffset == 0 && combobox_top_choose.SelectedIndex != -1)
            {
                if (offset >= limit && dt_movies.Rows.Count > 0 && !filterIsOpen())
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
            offset = 0;
            if (combobox_top_choose.SelectedIndex > 0)
            {
                filterClose();
            }
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
                    grid_right.Visibility = Visibility.Collapsed;
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
        const string settings_header = "Настройки";

        static Brush foreColorEnabled = Brushes.Green;
        static Brush foreColor = Brushes.WhiteSmoke;
        static Brush foreColorDisabled = Brushes.Red;
        static Brush backColor = Brushes.WhiteSmoke;
        static Brush backColorButt = new SolidColorBrush(Color.FromRgb((byte) 40, (byte) 57, (byte)73));//#283949
        static Thickness borderThickness = new Thickness(0, 0, 0, 0);
        static int fontSize = 14;

        //Settings 
        // 0 - notify; 1 - age; 2 - netflix_com; 3 - ivi_ru;
        // 4 - lostfilm_tv; 5 - kinokrad_co; 6 - filmzor_net; 7 - hdkinozor_ru;

        //Обновление настроек
        public void UpdateSettings()
        {
            password = db.GetPassword();
            DataTable dt = db.GetSettings();
            for (int i = 0; i < settings_amount; i++)
            {
                IsChecked[i] = Convert.ToBoolean(dt.Rows[i].ItemArray[0].ToString());
            }
            db.updateAgeRestriction(IsChecked[1]);
            update_combobox_age();
            dt.Dispose();
        }

        //Подгрузка настроек
        private void LoadSettings()
        {
            UpdateSettings();

            textBox_content_headet.Text = settings_header;
            textBox_content_headet.FontSize = fontSize + 2;

            label_other.Foreground = foreColor;
            label_sources.Foreground = foreColor;

            button_save.Foreground = foreColor;
            button_save.Background = backColorButt;
            button_cancel.Foreground = foreColor;
            button_cancel.Background = backColorButt;
            button_defaults.Foreground = foreColor;
            button_defaults.Background = backColorButt;

            button_export.Foreground = foreColor;
            button_export.Background = backColorButt;
            button_import.Foreground = foreColor;
            button_import.Background = backColorButt;
            
            checkbox_notify.Foreground = foreColor;
            checkbox_notify.IsChecked = IsChecked[0];
            if (password == "")
            {
                db.updateAgeRestriction(false);
                IsChecked[1] = false;
                checkbox_age.IsEnabled = false;
                button_age.Visibility = Visibility.Visible;
                button_age.Content = "Получить пароль";                    
            }
            else
            {
                if (!IsChecked[1])
                {
                    checkbox_age.IsEnabled = false;
                    button_age.Visibility = Visibility.Visible;
                    button_age.Content = "Ввести пароль";
                }
                else
                {
                    checkbox_age.IsEnabled = true;
                    button_age.Visibility = Visibility.Collapsed;
                }
            }
            checkbox_age.Foreground = foreColor;
            checkbox_age.IsChecked = IsChecked[1];
            checkbox_netflix_com.Foreground = foreColor;
            checkbox_netflix_com.IsChecked = IsChecked[2];
            checkbox_ivi_ru.Foreground = foreColor;
            checkbox_ivi_ru.IsChecked = IsChecked[3];
            checkbox_lostfilm_tv.Foreground = foreColor;
            checkbox_lostfilm_tv.IsChecked = IsChecked[4];
            checkbox_kinokrad_co.Foreground = foreColor;
            checkbox_kinokrad_co.IsChecked = IsChecked[5];
            checkbox_filmzor_net.Foreground = foreColor;
            checkbox_filmzor_net.IsChecked = IsChecked[6];
            checkbox_hdkinozor_ru.Foreground = foreColor;
            checkbox_hdkinozor_ru.IsChecked = IsChecked[7];
            
            grid_recommends.Visibility = Visibility.Collapsed;
            grid_right.Visibility = Visibility.Collapsed;
            button_panel_close.Visibility = Visibility.Visible;
            grid_content.Visibility = Visibility.Visible;
            
            db.updateAgeRestriction(IsChecked[1]);

            GC.Collect();
        }

        //Export
        private void Button_export_Click(object sender, RoutedEventArgs e)
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

        //Import
        private void Button_import_Click(object sender, RoutedEventArgs e)
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

            LoadSettings();
            MessageBox.Show("Импорт настроек прошёл успешно!");
        }

        //Save
        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            IsChecked[0] = (bool)checkbox_notify.IsChecked;
            IsChecked[1] = (bool)checkbox_age.IsChecked;
            IsChecked[2] = (bool)checkbox_netflix_com.IsChecked;
            IsChecked[3] = (bool)checkbox_ivi_ru.IsChecked;
            IsChecked[4] = (bool)checkbox_lostfilm_tv.IsChecked;
            IsChecked[5] = (bool)checkbox_kinokrad_co.IsChecked;
            IsChecked[6] = (bool)checkbox_filmzor_net.IsChecked;
            //IsChecked[7] = (bool)checkbox_hdkinozor_ru.IsChecked;
            for (int i = 0; i < settings_amount - 1; i++) 
            {
                db.SetSettings(i.ToString(), IsChecked[i], true);
            }
            LoadSettings();
        }

        //Cancel
        private void Button_cancel_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        //Defaults
        private void Button_defaults_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < settings_amount; i++)
            {
                if (i == 0 || i == 1 || i == 7)
                {
                    IsChecked[i] = true;
                }
                else
                {
                    IsChecked[i] = false;
                }
                db.SetSettings(i.ToString(), IsChecked[i], true);
            }
            LoadSettings();
        }

        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            //пример использования инпутбокса
            //string value = "";
            //InputBox("Изменение ограничения 18+", "Введите пароль",ref value);
            //Title.Text = value;
            //конец примера
            if (textBox_content_headet.Text == settings_header)
                return;
            LoadSettings();
        }
        
        // <-- Блок методов для меню настроек








        // Блок методов для уведомлений -->
        System.Windows.Forms.NotifyIcon notifyIcon;
        public void ShowNotification(int time = 10000, string header = "Notification", string text = "This is a base notification!")
        {
            if (IsChecked[0])//Can show notification?
            {
                notifyIcon.ShowBalloonTip(time, header, text, notifyIcon.BalloonTipIcon);
            }
            iconRed();
            button_notify.Content = new Image() { Source = redNotifyImg, Stretch = Stretch.Fill, Margin = new Thickness(6) };
        }
        //создает и помещает в трей иконку
        //ПКМ вызвает меню
        //даблклик открывает окно
        //balloontipclicked вызывается если нажали на уведомление
        public void createNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Visible = true;
            iconNormal();
            notifyIcon.DoubleClick += (s, e) => 
            {
                button_notify.Content = new Image() { Source = notifyImg, Stretch = Stretch.Fill, Margin = new Thickness(6) };
                iconNormal();
                ShowInTaskbar = true;
                show_new_movies();
            };
            
            notifyIcon.BalloonTipClicked += (s, e) => 
            {
                button_notify.Content = new Image() { Source = notifyImg, Stretch = Stretch.Fill, Margin = new Thickness(6) };
                iconNormal();
                show_new_movies();
            };
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem("Открыть", (s, e) =>
            {
                ShowInTaskbar = true;
                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;
                Activate();
            });
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new System.Windows.Forms.MenuItem("Выход", (s, e) => 
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                try
                {
                    thread.Abort();
                }
                catch { }
                Process.GetCurrentProcess().Kill();
            });
            contextMenu.MenuItems.Add(menuItem);
            notifyIcon.ContextMenu = contextMenu;
        }
        // <-- Блок методов для уведомлений


        private void iconNormal()
        {
            //using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico")).Stream)
            {
                notifyIcon.Icon = new System.Drawing.Icon(Environment.CurrentDirectory.ToString() + @"\icon.ico");
            }
        }
        private void iconRed()
        {
            //using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/iconRed.ico")).Stream)
            {
                notifyIcon.Icon = new System.Drawing.Icon(Environment.CurrentDirectory.ToString() + @"\iconRed.ico");
            }
        }




        //
        private void show_new_movies()
        {
            button_notify.Content = new Image() { Source = notifyImg, Stretch = Stretch.Fill, Margin = new Thickness(6) };
            offset = 0;
            combobox_top_choose.SelectedIndex = 4;
            update_movies("Новинки за неделю", limit, offset);
            show_movies(grid_list, button_sctoll_top, columns_count);
            scroll_viewer_center.ScrollToTop();
            ShowInTaskbar = true;
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            Activate();
        }


        //content close
        private void button_panel_close_Click(object sender, RoutedEventArgs e)
        {
            button_panel_close.Visibility = Visibility.Collapsed;
            grid_content.Visibility = Visibility.Collapsed;
            grid_right.Visibility = Visibility.Collapsed;
            grid_recommends.Visibility = Visibility.Visible;
            textBox_content_headet.Text = "Рекомендовано";
            scroll_viewer_content.ScrollToVerticalOffset(scroll_viewer_right_last_height);
        }

        private void button_filtering_close_Click(object sender, RoutedEventArgs e)
        {
            filterClose();
        }

        private void button_filtering_open_Click(object sender, RoutedEventArgs e)
        {
            filterOpen();
        }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            search();
        }
        private void search()
        {
            string name = textbox_filtering.Text.ToLower();
            string genre = ((TextBlock)combobox_filter_genres.SelectedItem).Text;
            string age = ((TextBlock)combobox_filter_age.SelectedItem).Text;
            int year;
            if (combobox_filter_genres.SelectedIndex == 0)
                genre = "";
            else
                genre = genre.Substring(0, genre.Length - 2);
            if (combobox_filter_age.SelectedIndex == 0)
                age = "";
            if (combobox_filter_year.SelectedIndex == 0)
                year = 0;
            else
                year = int.Parse(((TextBlock)combobox_filter_year.SelectedItem).Text);
            dt_movies = db.GetMoviesByFilter(name, genre, age, year);
            allmoviesCount = dt_movies.Rows.Count;
            show_movies(grid_list, button_sctoll_top, columns_count);
            scroll_viewer_center.ScrollToTop();
        }
        private void button_notify_Click(object sender, RoutedEventArgs e)
        {
            button_notify.Content = new Image() { Source = notifyImg, Stretch = Stretch.Fill, Margin = new Thickness(6) };
            show_new_movies();
            iconNormal();
            //notify_load();
            //добавить отображение текстов уведомлений в правой вкладке
        }

        //private void notify_load()
        //{
        //    textBox_content_headet.Text = "Уведомления";
        //    scroll_viewer_content.ScrollToTop();
        //    if (!grid.ColumnDefinitions[2].IsEnabled)
        //        openPanel();
        //    grid_recommends.Visibility = Visibility.Collapsed;
        //    grid_content.Visibility = Visibility.Collapsed;
        //    grid_right.Visibility = Visibility.Visible;
        //    button_panel_close.Visibility = Visibility.Visible;
        //    grid_right.Children.Clear();
        //    grid_right.RowDefinitions.Clear();
        //    grid_right.ColumnDefinitions.Clear();
        //    grid_right.ColumnDefinitions.Add(new ColumnDefinition());
        //    grid_right.RowDefinitions.Add(new RowDefinition());
        //    grid_right.RowDefinitions.Add(new RowDefinition());
        //    TextBlock tb = new TextBlock()
        //    {
        //        Text = "Тута что-то будет связанное с уведомлениями",
        //        TextWrapping = TextWrapping.WrapWithOverflow,
        //        FontSize = 22,
        //        Background = Brushes.Transparent,
        //        Foreground = Brushes.LightGray,
        //        Padding = new Thickness(30)
        //    };
        //    Grid.SetColumn(tb, 0);
        //    Grid.SetRow(tb, 0);
        //    grid_right.Children.Add(tb);
        //}

        private void textbox_filtering_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_search.Focus();
                search();
            }
        }

        public bool filterIsOpen()
        {
            return grid.RowDefinitions[2].IsEnabled;
        }

        private void filterClose()
        {
            grid.RowDefinitions[2].IsEnabled = false;
            grid.RowDefinitions[2].Height = new GridLength(0);
            button_filtering_open.Visibility = Visibility.Visible;
        }

        private void filterOpen()
        {
            combobox_top_choose.SelectedIndex = 0;
            grid.RowDefinitions[2].IsEnabled = true;
            grid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);
            button_filtering_open.Visibility = Visibility.Collapsed;
            textbox_filtering.Text = "";
            textbox_filtering.Focus();
        }
        
        public static bool InputBox(string title, string promptText, ref string value)
        {
            Window window = new Window();
            TextBlock label = new TextBlock();
            PasswordBox textBox = new PasswordBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            window.Loaded += (s, e) => { textBox.Focus(); };
            window.Title = title;
            label.Text = promptText;
            textBox.Password = value;
            textBox.PasswordChar = '*';
            textBox.MaxLength = 4;
            buttonOk.Content = "OK";
            buttonCancel.Content = "Cancel";
            buttonOk.IsDefault = true;
            buttonOk.Click += (s, e) => { window.Close(); };
            buttonCancel.IsCancel = true;
            
            label.HorizontalAlignment = HorizontalAlignment.Stretch;
            label.VerticalAlignment = VerticalAlignment.Stretch;
            label.TextAlignment = TextAlignment.Center;
            label.FontSize = 24;
            label.Margin = new Thickness(5);
            textBox.Margin = new Thickness(5);
            textBox.Width = 80;
            textBox.VerticalAlignment = VerticalAlignment.Stretch;
            textBox.HorizontalAlignment = HorizontalAlignment.Center;
            //textBox.alig = TextAlignment.Center;
            textBox.FontSize = 26;
            //textBox.pa
            buttonOk.VerticalAlignment = VerticalAlignment.Stretch;
            buttonOk.HorizontalAlignment = HorizontalAlignment.Stretch;
            buttonOk.Margin = new Thickness(5);
            buttonCancel.VerticalAlignment = VerticalAlignment.Stretch;
            buttonCancel.HorizontalAlignment = HorizontalAlignment.Stretch;
            buttonCancel.Margin = new Thickness(5);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Background = Brushes.LightGray;

            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);
            Grid.SetColumnSpan(label, 2);

            Grid.SetColumn(textBox, 0);
            Grid.SetRow(textBox, 1);
            Grid.SetColumnSpan(textBox, 2);

            Grid.SetColumn(buttonOk, 0);
            Grid.SetRow(buttonOk, 2);

            Grid.SetColumn(buttonCancel, 1);
            Grid.SetRow(buttonCancel, 2);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonOk);
            grid.Children.Add(buttonCancel);
            
            window.Height = 200;
            window.Width = 250;
            window.Content = grid;
            window.ResizeMode = ResizeMode.NoResize;
            //window.AllowsTransparency = true;
            //window.BorderBrush = Brushes.Black;
            //window.BorderThickness = new Thickness(10);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.ShowDialog();
            value = textBox.Password;
            return true;
        }

        private void button_age_Click(object sender, RoutedEventArgs e)
        {
            if (password == "")
            {
                string str = "";
                Random rnd = new Random();
                str += rnd.Next(9).ToString();
                str += rnd.Next(9).ToString();
                str += rnd.Next(9).ToString();
                str += rnd.Next(9).ToString();
                IsChecked[1] = false;
                checkbox_age.IsChecked = false;
                db.SetPassword(str);
                ShowBox("Пароль: " + str);
                db.updateAgeRestriction(true);
                LoadSettings();
            }
            else
                if(!IsChecked[1])
                {
                    string value = "";
                    InputBox("Выключение ограничения 18+", "Введите пароль",ref value);
                    if (value == db.GetPassword())
                    {
                        checkbox_age.IsChecked = true;
                        IsChecked[1] = true;
                        
                        //LoadSettings();
                    }
                }
        }
    }


}
