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

            //grid.ColumnDefinitions[2].Width = new GridLength(0);
            //grid.ColumnDefinitions[2].IsEnabled = false;
            //Width = 450;
            //stack_content.Visibility = Visibility.Hidden;
            columns_count = 3;
        }
        DB db = new DB();
        DataTable dt_movies = new DataTable();
        int columns_count;
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
                stack_content.Visibility = Visibility.Visible;
            }
            else
            {
                grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions[2].IsEnabled = true;
                Width += grid.ColumnDefinitions[1].ActualWidth;
                stack_content.Visibility = Visibility.Visible;
            }
            recommends_load();
        }
        //закрытие правой панели
        void closePanel()
        {//тут все работает ок
            if (!grid.ColumnDefinitions[2].IsEnabled)
                return;
            double size = grid.ColumnDefinitions[2].ActualWidth;
            //grid_content.Children.Clear();
            grid.ColumnDefinitions[2].IsEnabled = false;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            Width -= size;
            stack_content.Visibility = Visibility.Hidden;
        }
        
        DataTable dt = new DataTable();
        static BitmapImage noFavoriteImg = new BitmapImage(new Uri("/Resources/пустаязвезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage FavoriteImg = new BitmapImage(new Uri("/Resources/звезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage noWatchedImg = new BitmapImage(new Uri("/Resources/nowatched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage WatchedImg = new BitmapImage(new Uri("/Resources/watched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        static BitmapImage posterNONE = new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

    private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Все", Foreground = Brushes.LightSlateGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Рекомендовано", Foreground = Brushes.LightSlateGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за сегодня", Foreground = Brushes.LightSlateGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за неделю", Foreground = Brushes.LightSlateGray, Background = Brushes.Transparent });
            combobox_top_choose.Items.Add(new TextBlock() { IsEnabled = false, Text = "Новинки за месяц", Foreground = Brushes.LightSlateGray, Background = Brushes.Transparent });
            combobox_top_choose.SelectedIndex = 0;
            GetMoviesFromDB();
            //Image img = new Image() {  Source = (dt_movies.Rows[11]["poster"] as BitmapImage) };
            //grid_content.Children.Add(img);
            //poster0Img = LoadImage((byte[])dt_movies.Rows[11]["poster"]);
            //пример добавление фильма в базу
            //db.connect();
            //db.AddMovie("123", 123, "2019-10-12", "123", "123", "123", "123", "https://metanit.com/sharp/wpf/pics/3.6.png", "asd", "asd", "asd", false, false);
            //db.close();
            //тут тестовые данные
            new_movies_load();
            recommends_load();
        }
        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить номер строки
            bool favorite = Convert.ToBoolean(dt_movies.Rows[index]["favorite"]);
            db.SetFavorite(Convert.ToInt32(dt_movies.Rows[index]["id"]),!favorite);//изменяем в бд
            if (favorite)
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                dt_movies.Rows[index]["favorite"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                dt_movies.Rows[index]["favorite"] = true;
            }
        }
        //нажатие на кнопку добавить/удалить из просмотренного
        private void button_watched_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить номер строки
            bool watched = Convert.ToBoolean(dt_movies.Rows[index]["watched"]);
            db.SetWatched(Convert.ToInt32(dt_movies.Rows[index]["id"]), !watched);//изменяем в бд
            if (watched)
            {
                (sender as Button).Content = new Image() { Source = noWatchedImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                dt_movies.Rows[index]["watched"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = WatchedImg, Stretch = Stretch.Fill, Margin = new Thickness(5) };
                dt_movies.Rows[index]["watched"] = true;
            }
        }

        //определение id фильма по нажатию на элементы в центральном гриде и открытие правой вкладке и инфой о фильме
        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == null)
                return;
            if (e.Source.GetType() == typeof(TextBlock))
            {
                Title.Text = (e.Source as TextBlock).Tag.ToString();
                content_load(Convert.ToInt32((e.Source as TextBlock).Tag));//показать инфу о фильме в правой вкладке
            }
            else
            if (e.Source.GetType() == typeof(Image))
            {
                Title.Text = (e.Source as Image).Tag.ToString();
                content_load(Convert.ToInt32((e.Source as Image).Tag));//показать инфу о фильме в правой вкладке
            }
            e.Handled = true;//это чтобы родительские элементы ничего не натворили
        }
        //Подгрузка контента справа
        private void content_load(int index)
        {
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

            TextBlock tb = new TextBlock()
            {
                Name = "textblock_right_content",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 16,
                Text = dt_movies.Rows[index]["name"].ToString() + Environment.NewLine + 
                    "Год\t" + dt_movies.Rows[index]["year"].ToString() + Environment.NewLine +
                    "Страна\t" + dt_movies.Rows[index]["country"].ToString() + Environment.NewLine +
                    "Жанр\t" + dt_movies.Rows[index]["genres"].ToString() + Environment.NewLine +
                    "Дата\t" + ConvertDate(dt_movies.Rows[index]["date"].ToString()) + Environment.NewLine +
                    "Возраст\t" + dt_movies.Rows[index]["agerating"].ToString(),//добавить ссылки (трейлер инфа просмотр)???
                //Margin = new Thickness(5,5,5,5),
                Foreground = Brushes.LightGray,
                Padding = new Thickness(5, 5, 5, 5),
                Tag = index
            };

            TextBlock tb2 = new TextBlock()
            {
                Name = "textblock_right_description",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 16,
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
            //Title.Text = grid_content.RowDefinitions.Count.ToString();
        }





        // Блок методов для меню настроек -->
        const int settings_amount = 8;
        bool[] IsChecked = new bool[settings_amount];//Settings 
        // 0 - notify; 1 - age; 2 - netflix_com; 3 - ivi_ru;
        // 4 - lostfilm_tv; 5 - kinokrad_co; 6 - filmzor_net; 7 - hdkinozor_ru;

        private void settings_load()//Подгрузка настроек
        {
            DataTable dt = db.GetSettings();
            for (int i = 0; i < settings_amount; i++)
            {
                IsChecked[i] = Convert.ToBoolean(dt.Rows[i].ItemArray[0].ToString());
            }
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
                Width = 100,
                Background = Brushes.Black,
                Foreground = foreColor,
                Content = "Экспорт",
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
                Width = 100,
                Background = Brushes.Black,
                Foreground = foreColor,
                Content = "Импорт",
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
                Content = "Включить детский режим?",
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

        //Export and Import
        void import_my_butt_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = new string[16];
            string filename = "";//Полный адрес файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);//Начальная директория
            openFileDialog.Filter = "Only settings file (*.settings)|*.settings";//Фильтр по расширению файла

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
                lines = sr.ReadLine().Split('=',';');
            }
            int temp = 0;
            for (int j = 1; j < lines.Length; j += 2)
            {
                IsChecked[temp] = Convert.ToBoolean(lines[j]);
                db.SetSettings(temp.ToString(), IsChecked[temp], true);
                temp++;
            }
            settings_load();
        }
        void export_my_butt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not ready");
            //MessageBox.Show(db.GetSettings());
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
            MessageBox.Show("Нельзя убрать основной источник!");
            IsChecked[7] = true;
            settings_load();
        }
        
        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            settings_load();
        }

        // <-- Блок методов для меню настроек







        //показать все фильмы
        private void new_movies_load()
        {
            button_sctoll_top.IsEnabled = false;//выключение кнопки "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            grid_list.Children.Clear();//очистить элементы из центрального грида
            grid_list.RowDefinitions.Clear();//удалить все строки из центрального грида
            grid_list.ColumnDefinitions.Clear();
            grid_list.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //db.connect();
            //db.GetMovies();
            //db.close();
            Grid grid_row = null;
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (i%columns_count == 0)
                {
                    grid_list.RowDefinitions.Add(new RowDefinition());
                    grid_row = new Grid();
                    Grid.SetColumn(grid_row, 0);
                    Grid.SetRow(grid_row, i/columns_count);
                    for (int j = 0; j < columns_count; j++)
                    {
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                    }
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid_list.Children.Add(grid_row);
                }
                create_and_add_elements(grid_row, i);
            }
        }
        //показать все фильмы
        private void recommends_load()
        {
            button_sctoll_top.IsEnabled = false;//выключение кнопки "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            grid_content.Children.Clear();//очистить элементы из центрального грида
            grid_content.RowDefinitions.Clear();//удалить все строки из центрального грида
            grid_content.ColumnDefinitions.Clear();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //db.GetMovies();
            Grid grid_row = null;
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (i % columns_count == 0)
                {
                    grid_content.RowDefinitions.Add(new RowDefinition());
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
                    grid_content.Children.Add(grid_row);
                }
                create_and_add_elements(grid_row, i);
            }
        }
        //показывает список избранного
        private void favorite_load()
        {
            button_sctoll_top.IsEnabled = false;//выключение кнопки "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            grid_list.Children.Clear();//очистить элементы из центрального грида
            grid_list.RowDefinitions.Clear();//удалить все строки из центрального грида
            grid_list.ColumnDefinitions.Clear();
            grid_list.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //db.GetMovies();
            Grid grid_row = null;
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false)//если фильм отмечен избранным
                    continue;//то переходим на следующую итерацию
                if (i % columns_count == 0)
                {
                    grid_content.RowDefinitions.Add(new RowDefinition());
                    grid_row = new Grid() { Name = "grid_row"+i};
                    Grid.SetColumn(grid_row, 0);
                    Grid.SetRow(grid_row, i / columns_count);
                    for (int j = 0; j < columns_count; j++)
                    {
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid_row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                    }
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
                    grid_row.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid_list.Children.Add(grid_row);
                }
                create_and_add_elements(grid_row, i);
            }
        }
        //показывает просмотренные
        private void watched_load()
        {
            button_sctoll_top.IsEnabled = false;//выключаем кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрываем кнопку "вверх"
            grid_list.Children.Clear();//очищаем элементы в центральной панели
            grid_list.RowDefinitions.Clear();//удаляем все строки в центральной панели
            db.GetWatched();
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt_movies.Rows[i]["watched"]) == false)//если фильм не просмотренный
                    continue;//то переходим на следующую итерацию
                create_and_add_elements(grid_list, i);
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
                Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill, Margin = new Thickness(5)},
                Tag = i//index(не id)
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
                Padding = new Thickness(5,5,5,25),
                Tag = i//index (не id)
            };
            Image img = new Image()//постер
            {
                Source = dt_movies.Rows[i]["poster"].GetType()==typeof(DBNull)? posterNONE :LoadImage((byte[])dt_movies.Rows[i]["poster"]),//картинка из массива по id
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = i//index (не id)
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
            if(e.Key == Key.Space)//если нажали пробел
            {
                //stack_content.Visibility = Visibility.Hidden;
                //Title.Text = grid_list.RowDefinitions.Count.ToString();
                this.Resources.Remove("textColor");
                SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Red);
                Resources.Add("textColor", solidColorBrush);
                Title.Foreground = Brushes.Red;
                //this.UpdateLayout();
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
            new_movies_load();//показывает все фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
       //     thread.Abort();
            GC.Collect();
        }
        //нажали кнопку избранное (меню)
        private void button_favorite_Click_1(object sender, RoutedEventArgs e)
        {
            favorite_load();//показывает избранные фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
        }
        //нажали кнопку закрыть окно
        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
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
            if (Width < 840 && grid.ColumnDefinitions[2].IsEnabled)//если ширина окна меньше 700
                closePanel(); // закрываем правую панель
        }
        //нажали кнопку "вверх"
        private void button_sctoll_top_Click(object sender, RoutedEventArgs e)
        {
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            button_sctoll_top.IsEnabled = false;//выключить кнопку
            button_sctoll_top.Visibility = Visibility.Hidden;//спрятать кнопку
        }
        //покрутили колесико в центральной вкладке
        private void stack_list_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)//если покрутили вниз
            {
                button_sctoll_top.IsEnabled = true;//включить кнопку "вверх"
                button_sctoll_top.Visibility = Visibility.Visible;//показать кнопку "вверх"
            }
        }
        
        //нажали кнопку просмотренное (меню)
        private void button_watched_Click_1(object sender, RoutedEventArgs e)
        {
            watched_load();//показывает просмотренные фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
        }
        //выгрузка всех фильмов из БД
        private void GetMoviesFromDB()
        {
            dt_movies = db.GetMovies();
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

        private void scroll_viewer_center_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == 0)
            {
                button_sctoll_top.IsEnabled = false;
                button_sctoll_top.Visibility = Visibility.Hidden;
            }
            else
            {
                button_sctoll_top.IsEnabled = true;
                button_sctoll_top.Visibility = Visibility.Visible;
            }
        }
    }
}
