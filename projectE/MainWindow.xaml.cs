using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

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

        private void Window_Initialized(object sender, EventArgs e)
        {
            //загрузка всех картинок из файлов, необходимо для установщика
            string path = Environment.CurrentDirectory.ToString();
            hide_image.Source = new BitmapImage(new Uri(path + @"\Resources\hide.png"));
            max_image.Source = new BitmapImage(new Uri(path + @"\Resources\window.png"));
            exit_image.Source = new BitmapImage(new Uri(path + @"\Resources\exit.png"));
            home_image.Source = new BitmapImage(new Uri(path + @"\Resources\домик2.png"));
            fav_image.Source = new BitmapImage(new Uri(path + @"\Resources\звезда2.png"));
            notify_image.Source = new BitmapImage(new Uri(path + @"\Resources\notify.png"));
            sett_image.Source = new BitmapImage(new Uri(path + @"\Resources\шестерня2.png"));
            filo_image.Source = new BitmapImage(new Uri(path + @"\Resources\лупастрелка.png"));
            panel_image.Source = new BitmapImage(new Uri(path + @"\Resources\панель2.png"));
            seacrh_image.Source = new BitmapImage(new Uri(path + @"\Resources\лупа2.png"));
            filc_image.Source = new BitmapImage(new Uri(path + @"\Resources\back_to_recomends.png"));
            scroll_image.Source = new BitmapImage(new Uri(path + @"\Resources\up.png"));
            content_scroll_image.Source = new BitmapImage(new Uri(path + @"\Resources\up.png"));
            panel_close_image.Source = new BitmapImage(new Uri(path + @"\Resources\back_to_recomends.png"));

            noFavoriteImg = new BitmapImage(new Uri(path + @"\Resources\пустаязвезда2.png"));
            FavoriteImg = new BitmapImage(new Uri(path + @"\Resources\звезда2.png"));
            notifyImg = new BitmapImage(new Uri(path + @"\Resources\notify.png"));
            redNotifyImg = new BitmapImage(new Uri(path + @"\Resources\rednotify.png"));

            text_brush = Brushes.White;
            background_brush = Brushes.Red;

            //Title.Foreground = text_brush;
            //textBlock_content_header.Foreground = text_brush;

            UpdateSettings();
            
            columns_count = 2;
            columns_count_recommends = 4;
            limit = columns_count * 8;
            offset = 0;
            //если их прописывать в xaml выдает ошибку
            combobox_top_choose.SelectedIndex = 0;
            combobox_search_year = Functions.update_combobox_years(combobox_search_year, text_brush);
        }

        public void ShowBox(string v)
        {
            MessageBox.Show(v, "Ошибка", MessageBoxButton.OK);
        }

        DB db = new DB();
        DataTable movies_table = new DataTable();
        int columns_count;
        int columns_count_recommends;
        int limit;
        int offset;
        int moviesCount;
        double scroll_viewer_right_last_height;
        private BitmapImage noFavoriteImg;
        private BitmapImage FavoriteImg;
        private BitmapImage notifyImg;
        private BitmapImage redNotifyImg;
        private System.Drawing.Icon icon_image = new System.Drawing.Icon(Environment.CurrentDirectory.ToString() + @"\Resources\icon.ico");
        private System.Drawing.Icon icon_Red_image = new System.Drawing.Icon(Environment.CurrentDirectory.ToString() + @"\Resources\iconRed.ico");
        private Brush text_brush;
        private Brush background_brush;
        //нажатие на кнопку открыть/закрыть правую панель
        private void button_panel_Click(object sender, RoutedEventArgs e)
        {
            if (grid.ColumnDefinitions[2].IsEnabled)
                closeRightPanel();
            else
                openRightPanel();
        }
        //открытие правой панели
        void openRightPanel()
        {
            if (grid.ColumnDefinitions[2].IsEnabled)
                return;
            grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
            grid.ColumnDefinitions[2].IsEnabled = true;
            if (Width <= 800)//800 минимальная ширина окна когда пользоваться удобно
                Width += grid.ColumnDefinitions[1].ActualWidth * 1.5;
        }
        //открытие правой панели без изменения размера окна
        void openRightPanel_withoutSizeChange()
        {
            if (grid.ColumnDefinitions[2].IsEnabled)
                return;
            grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
            grid.ColumnDefinitions[2].IsEnabled = true;
        }
        //закрытие правой панели
        void closeRightPanel()
        {
            if (!grid.ColumnDefinitions[2].IsEnabled)
                return;
            double size = grid.ColumnDefinitions[2].ActualWidth;
            grid.ColumnDefinitions[2].IsEnabled = false;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            Width -= size;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createNotifyIcon();
            UpdateSettings();
            offset = 0;
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_recommends, button_sctoll_top, columns_count_recommends);
            ShowNotification();

            thread = new Thread(Upd);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }


        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получаем id фильма
            //изменяем в бд
            if (db.IsFavorite(index) == true)
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
        //определение id фильма по нажатию на элементы в центральном гриде и открытие правой вкладке и инфой о фильме
        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == null)
                return;
            if (e.Source.GetType() == typeof(TextBlock))
                right_content_load(db.GetMovie(Convert.ToInt32((e.Source as TextBlock).Tag)).Rows[0]);//показать инфу о фильме в правой вкладке
            else
            if (e.Source.GetType() == typeof(Image))
                right_content_load(db.GetMovie(Convert.ToInt32((e.Source as Image).Tag)).Rows[0]);//показать инфу о фильме в правой вкладке
            e.Handled = true;//чтобы родительские элементы ничего не натворили
        }
        //вывод информации в правой панели
        private void right_content_load(DataRow _dataRow)
        {
            textBlock_content_header.Text = "Подробное описание";
            scroll_viewer_right_content.ScrollToTop();
            openRightPanel();
            grid_recommends.Visibility = Visibility.Collapsed;
            grid_settings.Visibility = Visibility.Collapsed;
            grid_right_content.Visibility = Visibility.Visible;
            button_panel_close.Visibility = Visibility.Visible;
            grid_right_content.Children.Clear();
            grid_right_content.RowDefinitions.Clear();
            grid_right_content.ColumnDefinitions.Clear();
            grid_right_content.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 300 });
            grid_right_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_right_content.RowDefinitions.Add(new RowDefinition());
            grid_right_content.RowDefinitions.Add(new RowDefinition());

            Image img = new Image()
            {
                Source = Functions.LoadImage(_dataRow["poster"]),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Uniform,
                MaxWidth = 300
            };
            Grid.SetColumn(img, 0);
            Grid.SetRow(img, 0);
            grid_right_content.Children.Add(img);

            TextBlock tb = new TextBlock();
            tb.Inlines.Add(Functions.createURL(_dataRow["URLinfo"].ToString(), _dataRow["name"].ToString(), 22));
            Run run = new Run()
            {
                FontSize = 16,
                Text = Environment.NewLine +
                    _dataRow["year"].ToString() + "\r\n\r\n" +
                    _dataRow["country"].ToString() + "\r\n\r\n" +
                    _dataRow["genres"].ToString() + "\r\n\r\n" +
                    Functions.ConvertDate(_dataRow["date"].ToString()) + "\r\n\r\n" +
                    _dataRow["agerating"].ToString() + "\r\n\r\n",
            };
            tb.Inlines.Add(run);
            tb.SetResourceReference(TextBlock.ForegroundProperty, "textBrush");
            if (_dataRow["URLtrailer"].ToString() != "")
                tb.Inlines.Add(Functions.createURL(_dataRow["URLtrailer"].ToString(), "Трейлер\r\n", 14));
            if (_dataRow["URLwatch"].ToString() != "")
                tb.Inlines.Add(Functions.createURL(_dataRow["URLwatch"].ToString(), "Просмотр\r\n", 14));
            tb.TextWrapping = TextWrapping.WrapWithOverflow;
            tb.Padding = new Thickness(5, 5, 5, 5);
            TextBlock tb2 = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 14,
                Text = _dataRow["description"].ToString(),
                Foreground = text_brush,
                Padding = new Thickness(5, 5, 5, 5),
            };
            tb2.SetResourceReference(TextBlock.ForegroundProperty, "textBrush");
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 0);
            grid_right_content.Children.Add(tb);

            Grid.SetColumn(tb2, 0);
            Grid.SetColumnSpan(tb2, 2);
            Grid.SetRow(tb2, 1);
            grid_right_content.Children.Add(tb2);
        }

        //получение фильмов из бд
        private void update_movies(string movies, int limit, int offset)
        {
            switch (movies)
            {
                case "Все":
                    movies_table = db.GetMovies(limit, offset);
                    moviesCount = db.GetMoviesCount();
                    break;
                case "Рекомендовано":
                    movies_table = db.GetRecommends();
                    textBlock_content_header.Text = "Рекомендовано";
                    break;
                case "Новинки за сегодня":
                    movies_table = db.GetMoviesToday();
                    moviesCount = movies_table.Rows.Count;
                    break;
                case "Новинки за неделю":
                    movies_table = db.GetMoviesWeek();
                    moviesCount = db.GetMoviesWeekCount();
                    break;
                case "Новинки за месяц":
                    movies_table = db.GetMoviesMonth();
                    moviesCount = movies_table.Rows.Count;
                    break;
                case "Избранное":
                    movies_table = db.GetFavorites(limit, offset);
                    moviesCount = movies_table.Rows.Count;
                    break;
            }
        }
        /// <summary>
        /// выгрузка фильмов из БД и вывод их в _grid
        /// </summary>
        /// <param name="_grid">куда выводить фильмы</param>
        /// <param name="_button_scroll">кнопка скролла вверх</param>
        /// <param name="_dt">таблица с фильмами</param>
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
            for (int i = 0; i < movies_table.Rows.Count; i++)//цикл по всем фильмам
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
                create_and_add_elements(grid_row, i, _columns_count, movies_table.Rows[i]);
            }
            if (movies_table.Rows.Count == 0)
            {
                string str = "Пусто";
                if (!searchPanelIsOpen())
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
                    str = "Результатов поиска \"" + textbox_search.Text + "\", жанры: "
                        + (combobox_search_genres.SelectedItem as TextBlock).Text + ", возраст: " + (combobox_search_age.SelectedItem as TextBlock).Text + ", год: " + (combobox_search_year.SelectedItem as TextBlock).Text + " нет";

                _grid.RowDefinitions.Add(new RowDefinition());
                TextBlock tb = new TextBlock()
                {
                    Text = str,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18,
                    Foreground = text_brush,
                    IsEnabled = false,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    TextAlignment = TextAlignment.Center
                };
                Grid.SetRow(tb, 0);
                _grid.Children.Add(tb);
            }
        }

        private void create_and_add_elements(Grid _grid_row, int i, int _columns_count, DataRow _dataRow)
        {

            Button btf = new Button()//кнопка добавления/удаления из избранного
            {
                Name = "button_favorite" + i,
                Height = 40,
                Width = 40,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0, 0, 0, 0),
                //Style = (Style)FindResource("ButtonStyle1"),
                //присвоение соответсвтующей картинки
                Content = new Image() { Source = Convert.ToBoolean(_dataRow["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) },
                Tag = _dataRow["id"]//index
            };
            btf.Click += button_favorite_Click;//привязывание события клика
            TextBlock tb = new TextBlock()//текст справа от постера
            {
                Name = "textBlock_middle_content" + i,
                TextWrapping = TextWrapping.WrapWithOverflow,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Text = movies_table.Rows[i]["name"].ToString(),
                //Foreground = text_brush,
                FontSize = 14,
                Padding = new Thickness(5, 5, 5, 25),
                Tag = _dataRow["id"],//index
                //Foreground = Resources.MergedDictionaries.Where(p => p["textBrush"] == Resources["textBrush"])
                //Foreground.SetValue(dp,ob)
            };
            tb.SetResourceReference(TextBlock.ForegroundProperty, "textBrush");
            Image img = new Image()//постер
            {
                Source = Functions.LoadImage(movies_table.Rows[i]["poster"]),//картинка из массива по id
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = _dataRow["id"]//index
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

            Grid.SetColumn(btf, i % _columns_count * 2 + 1);
            Grid.SetRow(btf, 0);
            _grid_row.Children.Add(btf);

        }

        bool isRun;
        private void Upd()
        {
            isRun = true;
            Parser parser = new Parser();
            parser.UpdateList();
            isRun = false;
        }
        Thread thread;
        //нажали кнопку домой
        private void button_home_Click(object sender, RoutedEventArgs e)
        {
            if (!isRun)
            {
                thread = new Thread(Upd);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            if (combobox_top_choose.SelectedIndex == 0 && textBlock_content_header.Text == "Рекомендовано"
                && !button_sctoll_top.IsEnabled && !button_content_sctoll_top.IsEnabled && !searchPanelIsOpen())
                return;
            searchPanelClose();
            combobox_top_choose.SelectedIndex = 0;
            offset = 0;
            button_panel_close.Visibility = Visibility.Collapsed;
            grid_settings.Visibility = Visibility.Collapsed;
            grid_right_content.Visibility = Visibility.Collapsed;
            grid_recommends.Visibility = Visibility.Visible;
            textBlock_content_header.Text = "Рекомендовано";
            update_movies("Рекомендовано", limit, offset);
            show_movies(grid_recommends, button_sctoll_top, columns_count_recommends);

            update_movies("Все", limit, offset);
            show_movies(grid_list, button_sctoll_top, columns_count);
            openRightPanel();
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            scroll_viewer_right_content.ScrollToTop();
        }
        //нажали кнопку избранное (меню)
        private void button_menu_favorite_Click(object sender, RoutedEventArgs e)
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
                    openRightPanel();
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
            if (Width < 641 && grid.ColumnDefinitions[2].IsEnabled)//если ширина окна меньше 641, минимальная "удобная" ширина
                closeRightPanel(); // закрываем правую панель
            if (Width > 640 && !grid.ColumnDefinitions[2].IsEnabled)
                openRightPanel_withoutSizeChange();
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
            button_scroll_hide();
        }
        //покрутили колесико в центральной вкладке
        private void stack_list_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (stack_list.Height > scroll_viewer_right_content.Height)
                if (e.Delta < 0)//если покрутили вниз
                    button_scroll_show();
        }
        //покрутили колесико в правой вкладке
        private void stack_content_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (stack_content.Height > scroll_viewer_right_content.Height)
                if (e.Delta < 0)//если покрутили вниз
                    button_content_scroll_show();
        }
        //удаление удаленных записей из файла БД
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Vacuum();
        }

        //показать/скрыть кнопку скрола вверх
        private void button_content_scroll_show()
        {
            button_content_sctoll_top.IsEnabled = true;//включить кнопку "вверх"
            button_content_sctoll_top.Visibility = Visibility.Visible;//показать кнопку "вверх"
        }
        private void button_content_scroll_hide()
        {
            button_content_sctoll_top.IsEnabled = false;
            button_content_sctoll_top.Visibility = Visibility.Hidden;
        }
        private void button_scroll_show()
        {
            button_sctoll_top.IsEnabled = true;//включить кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Visible;//показать кнопку "вверх"
        }
        private void button_scroll_hide()
        {
            button_sctoll_top.IsEnabled = false;
            button_sctoll_top.Visibility = Visibility.Hidden;
        }

        //охраняет высоту предыдущего "листа" scroll_viewera
        private double previous_scroll_height = 0;
        private void scroll_viewer_center_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!searchPanelIsOpen())
            {
                //если проскролили до конца вниз и еще есть "фильмы" в списке
                if (e.VerticalOffset == scroll_viewer_center.ScrollableHeight
                    && offset + limit < moviesCount
                    && moviesCount > 0)
                {
                    int lists = offset / limit + 1;
                    previous_scroll_height = scroll_viewer_center.ScrollableHeight;
                    offset += limit;
                    string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                    update_movies(str, limit, offset - columns_count * columns_count * lists);
                    show_movies(grid_list, button_sctoll_top, columns_count);
                    scroll_viewer_center.ScrollToVerticalOffset(10);
                }
                if (e.VerticalOffset == 0)
                {//если проскролили вверх
                    if (offset >= limit && moviesCount > 0)
                    {
                        offset -= limit;
                        int lists = offset / limit;
                        string str = ((TextBlock)combobox_top_choose.SelectedValue).Text;
                        update_movies(str, limit, offset - (columns_count * columns_count * lists));
                        show_movies(grid_list, button_sctoll_top, columns_count);
                        scroll_viewer_center.ScrollToVerticalOffset(previous_scroll_height - 10);
                    }
                    button_scroll_hide();
                }
                else
                    button_scroll_show();
            }
            else
            {
                //если проскролили до конца вниз и еще есть "фильмы" в списке
                if (e.VerticalOffset == scroll_viewer_center.ScrollableHeight
                    && offset + limit < moviesCount
                    && moviesCount > 0)
                {
                    int lists = offset / limit + 1;
                    previous_scroll_height = scroll_viewer_center.ScrollableHeight;
                    offset += limit;
                    search(limit, offset - columns_count * columns_count * lists);
                    scroll_viewer_center.ScrollToVerticalOffset(10);
                }
                if (e.VerticalOffset == 0)
                {//если проскролили вверх
                    if (offset >= limit && moviesCount > 0)
                    {
                        offset -= limit;
                        int lists = offset / limit;
                        search(limit, offset - columns_count * columns_count * lists);
                        scroll_viewer_center.ScrollToVerticalOffset(previous_scroll_height - 10);
                    }
                    button_scroll_hide();
                }
                else
                    button_scroll_show();
            }
        }
        //сохраняет предыдущую высоту и показывает/скрывать кнопку скрола вверх 
        private void scroll_viewer_content_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (grid_recommends.Visibility == Visibility.Visible)
                scroll_viewer_right_last_height = e.VerticalOffset;
            if (e.VerticalOffset == 0)
                button_content_scroll_hide();
            else
                button_content_scroll_show();
        }

        private void button_content_sctoll_top_Click(object sender, RoutedEventArgs e)
        {
            scroll_viewer_right_content.ScrollToTop();//проскролить вверх
            button_content_scroll_hide();
        }

        private void combobox_top_choose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            offset = 0;
            searchPanelClose();
            if (((TextBlock)e.AddedItems[0]).Text == "Рекомендовано")
            {
                button_panel_close.Visibility = Visibility.Collapsed;
                grid_settings.Visibility = Visibility.Collapsed;
                grid_right_content.Visibility = Visibility.Collapsed;
                grid_recommends.Visibility = Visibility.Visible;
                update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                show_movies(grid_recommends, button_content_sctoll_top, columns_count_recommends);
                scroll_viewer_right_content.ScrollToTop();
            }
            else
            {
                update_movies(((TextBlock)e.AddedItems[0]).Text, limit, offset);
                show_movies(grid_list, button_sctoll_top, columns_count);
                scroll_viewer_center.ScrollToTop();
            }
        }
        // Блок методов для меню настроек -->

        const int settings_amount = 8;
        bool[] IsChecked = new bool[settings_amount];
        const string settings_header = "Настройки";

        static Brush backColor = Brushes.WhiteSmoke;
        static Brush backColorButt = new SolidColorBrush(Color.FromRgb((byte)40, (byte)57, (byte)73));//#283949
        static Thickness borderThickness = new Thickness(0, 0, 0, 0);
        static int fontSize = 14;

        //Settings 
        // 0 - notify; 1 - age; 2 - netflix_com; 3 - ivi_ru;
        // 4 - lostfilm_tv; 5 - kinokrad_co; 6 - filmzor_net; 7 - hdkinozor_ru;
        //Обновление настроек
        public void UpdateSettings()
        {
            DataTable dt = db.GetSettings();
            for (int i = 0; i < settings_amount; i++)
            {
                IsChecked[i] = Convert.ToBoolean(dt.Rows[i].ItemArray[0].ToString());
            }
            db.updateAgeRating(IsChecked[1]);
            combobox_search_age = Functions.update_combobox_age(combobox_search_age, text_brush, IsChecked[1]);
            dt.Dispose();
        }

        //Подгрузка настроек
        private void LoadSettings()
        {
            UpdateSettings();

            textBlock_content_header.Text = settings_header;
            //textBlock_content_header.FontSize = fontSize + 2;
            /*
            label_other.Foreground = text_brush;
            label_sources.Foreground = text_brush;

            button_save.Foreground = text_brush;
            button_save.Background = backColorButt;
            button_cancel.Foreground = text_brush;
            button_cancel.Background = backColorButt;
            button_defaults.Foreground = text_brush;
            button_defaults.Background = backColorButt;

            button_export.Foreground = text_brush;
            button_export.Background = backColorButt;
            button_import.Foreground = text_brush;
            button_import.Background = backColorButt;

            button_age.Foreground = text_brush;
            

            checkbox_notify.Foreground = text_brush;*/
            checkbox_notify.IsChecked = IsChecked[0];
            if (db.GetPassword() == "")
            {
                db.updateAgeRating(false);
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
            //checkbox_age.Foreground = text_brush;
            checkbox_age.IsChecked = IsChecked[1];
            //checkbox_netflix_com.Foreground = text_brush;
            checkbox_netflix_com.IsChecked = IsChecked[2];
            //checkbox_ivi_ru.Foreground = text_brush;
            checkbox_ivi_ru.IsChecked = IsChecked[3];
            //checkbox_lostfilm_tv.Foreground = text_brush;
            checkbox_lostfilm_tv.IsChecked = IsChecked[4];
            //checkbox_kinokrad_co.Foreground = text_brush;
            checkbox_kinokrad_co.IsChecked = IsChecked[5];
            //checkbox_filmzor_net.Foreground = text_brush;
            checkbox_filmzor_net.IsChecked = IsChecked[6];
            //checkbox_hdkinozor_ru.Foreground = text_brush;
            checkbox_hdkinozor_ru.IsChecked = IsChecked[7];

            grid_recommends.Visibility = Visibility.Collapsed;
            grid_right_content.Visibility = Visibility.Collapsed;
            button_panel_close.Visibility = Visibility.Visible;
            grid_settings.Visibility = Visibility.Visible;

            db.updateAgeRating(IsChecked[1]);

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
            lines[3] = "false";//при иморте чтобы всегда не показывало 18+
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

        //About developers
        private void Button_about_Click(object sender, RoutedEventArgs e)
        {
            string about = "Александр Туртыгин - Управление командой и меню настроек; \nЕкатерина Бондаренко - Парсинг и БД; \nИгорь Чайкин - Дизайн приложения; \nРустам Романов - Главный QA проекта.";
            string version = "\n\nВерсия приложения: " + System.Windows.Forms.Application.ProductVersion.ToString();
            MessageBox.Show(about + version, "Над программой работали:");
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
                IsChecked[1] = false;
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
            if (textBlock_content_header.Text == settings_header)
                return;
            LoadSettings();
        }

        // <-- Блок методов для меню настроек

        // Блок методов для уведомлений -->
        System.Windows.Forms.NotifyIcon notifyIcon;
        private void ShowNotification()
        {
            if (IsChecked[0])//Can show notification?
            {
                int countMoviesOfWeek = db.GetMoviesWeekCount();
                int countFavoritesOfWeek = db.GetFavoritesMoviesWeekCount();
                string str = "";
                if (countFavoritesOfWeek > 0)
                    str = " и " + countFavoritesOfWeek + " фильма из списка избранного";
                if (countMoviesOfWeek != 0)
                    notifyIcon.ShowBalloonTip(15000, "Новые фильмы!", "Вышло " + countMoviesOfWeek + " новых фильмов за неделю" + str, notifyIcon.BalloonTipIcon);

            }
            set_icon_red();
            notify_image.Source = redNotifyImg;
        }
        //создает и помещает в трей иконку
        //ПКМ вызвает меню
        //даблклик открывает окно
        //balloontipclicked вызывается если нажали на уведомление
        private void createNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Visible = true;
            set_icon_normal();
            notifyIcon.DoubleClick += (s, e) =>
            {
                notify_image.Source = notifyImg;
                set_icon_normal();
                ShowInTaskbar = true;
                show_new_movies();
            };

            notifyIcon.BalloonTipClicked += (s, e) =>
            {
                notify_image.Source = notifyImg;
                set_icon_normal();
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

        private void set_icon_normal()
        {
            notifyIcon.Icon = icon_image;
        }
        private void set_icon_red()
        {
            notifyIcon.Icon = icon_Red_image;
        }

        //
        private void show_new_movies()
        {
            notify_image.Source = notifyImg;
            offset = 0;
            combobox_top_choose.SelectedIndex = 4;//новинки за неделю
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
            grid_settings.Visibility = Visibility.Collapsed;
            grid_right_content.Visibility = Visibility.Collapsed;
            grid_recommends.Visibility = Visibility.Visible;
            textBlock_content_header.Text = "Рекомендовано";
            scroll_viewer_right_content.ScrollToVerticalOffset(scroll_viewer_right_last_height);
        }

        private void button_search_panel_close_Click(object sender, RoutedEventArgs e)
        {
            searchPanelClose();
            offset = 0;
            update_movies("Все", limit, offset);
            show_movies(grid_list, button_sctoll_top, columns_count);
            scroll_viewer_center.ScrollToTop();
        }

        private void button_search_panel_open_Click(object sender, RoutedEventArgs e)
        {
            searchPanelOpen();
        }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            offset = 0;
            search(limit, offset);
            scroll_viewer_center.ScrollToTop();
        }

        private void search(int _limit, int _offset)
        {
            string name = textbox_search.Text.ToLower();
            string genre = ((TextBlock)combobox_search_genres.SelectedItem).Text;
            string age = ((TextBlock)combobox_search_age.SelectedItem).Text;
            int year;
            if (combobox_search_genres.SelectedIndex == 0)
                genre = "";
            else
                genre = genre.Substring(0, genre.Length - 2);
            if (combobox_search_age.SelectedIndex == 0)
                age = "";
            if (combobox_search_year.SelectedIndex == 0)
                year = 0;
            else
                year = int.Parse(((TextBlock)combobox_search_year.SelectedItem).Text);
            movies_table = db.GetMoviesByFilter(name, genre, age, year, _limit, _offset);
            moviesCount = db.GetMoviesByFilterCount(name, genre, age, year);
            show_movies(grid_list, button_sctoll_top, columns_count);
        }
        //нажатие на кнопку уведомлений
        private void button_notify_Click(object sender, RoutedEventArgs e)
        {
            notify_image.Source = notifyImg;
            show_new_movies();
            set_icon_normal();
        }

        private void textbox_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_search.Focus();
                offset = 0;
                search(limit, offset);
                scroll_viewer_center.ScrollToTop();
            }
        }

        private bool searchPanelIsOpen()
        {
            return grid.RowDefinitions[2].IsEnabled;
        }
        //закрыть панель поиска
        private void searchPanelClose()
        {
            if (grid.RowDefinitions[2].IsEnabled == false)
                return;
            grid.RowDefinitions[2].IsEnabled = false;
            grid.RowDefinitions[2].Height = new GridLength(0);
            button_search_panel_open.Visibility = Visibility.Visible;
        }
        //открыть панель поиска
        private void searchPanelOpen()
        {
            if (grid.RowDefinitions[2].IsEnabled == true)
                return;
            combobox_top_choose.SelectedIndex = 0;
            grid.RowDefinitions[2].IsEnabled = true;
            grid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);
            button_search_panel_open.Visibility = Visibility.Collapsed;
            textbox_search.Text = "";
            textbox_search.Focus();
        }

        private void button_age_Click(object sender, RoutedEventArgs e)
        {
            if (db.GetPassword() == "")
            {
                string str = "";
                Random rnd = new Random();
                str += rnd.Next(9).ToString();
                str += rnd.Next(9).ToString();
                str += rnd.Next(9).ToString();
                IsChecked[1] = false;
                checkbox_age.IsChecked = false;
                db.SetPassword(str);
                MessageBox.Show("Пароль: " + str, "Запомни", MessageBoxButton.OK);
                db.updateAgeRating(true);
                LoadSettings();
            }
            else
                if (!IsChecked[1])
                if (Functions.InputBox(this, "Выключение ограничения 18+", "Введите пароль") == db.GetPassword())
                {
                    checkbox_age.IsChecked = true;
                    IsChecked[1] = true;
                }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Resources["scrollBrush"] = Brushes.Black;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color color = e.NewValue.Value;
            Resources["backBrush"] = new SolidColorBrush(color);
            color.R += 17;
            color.G += 24;
            color.B += 31;
            Resources["backBrushLight"] = new SolidColorBrush(color);
            color.R -= 28;
            color.G -= 35;
            color.B -= 40;
            Resources["backBrushDark"] = new SolidColorBrush(color);
            color.R -= 5;
            color.G -= 5;
            color.B -= 5;
            Resources["borderBrush"] = new SolidColorBrush(color);
        }

        private void ClrPcker_text_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color color = e.NewValue.Value;
            Resources["textBrush"] = new SolidColorBrush(color);
        }
    }


}
