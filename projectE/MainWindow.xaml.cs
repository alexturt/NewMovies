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
using System.Threading;

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
            
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            grid.ColumnDefinitions[2].IsEnabled = false;
            Width = 450;
            stack_content.Visibility = Visibility.Hidden;
        }
        DB db = new DB();
        DataTable dt_movies = new DataTable();
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
            grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions[2].IsEnabled = true;
            Width += grid.ColumnDefinitions[1].ActualWidth;
            stack_content.Visibility = Visibility.Visible;
        }
        //закрытие правой панели
        void closePanel()
        {//тут все работает ок
            double size = grid.ColumnDefinitions[2].ActualWidth;
            grid_content.Children.Clear();
            grid.ColumnDefinitions[2].IsEnabled = false;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            Width -= size;
            stack_content.Visibility = Visibility.Hidden;
        }
        //тестовые данные
        DataTable dt = new DataTable();
        BitmapImage noFavoriteImg = new BitmapImage(new Uri("/Resources/пустаязвезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage FavoriteImg = new BitmapImage(new Uri("/Resources/звезда2.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage noWatchedImg = new BitmapImage(new Uri("/Resources/nowatched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage WatchedImg = new BitmapImage(new Uri("/Resources/watched.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetMoviesFromDB();
            //Image img = new Image() {  Source = (dt_movies.Rows[11]["poster"] as BitmapImage) };
            //grid_content.Children.Add(img);
            //poster0Img = LoadImage((byte[])dt_movies.Rows[11]["poster"]);
            //пример добавление фильма в базу
            //db.connect();
            //db.AddMovie("123", 123, "2019-10-12", "123", "123", "123", "123", "https://metanit.com/sharp/wpf/pics/3.6.png", "asd", "asd", "asd", false, false);
            //db.close();
            //тут тестовые данные
            list_load();
        }
        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить id фильма
            bool favorite = Convert.ToBoolean(dt_movies.Rows[index]["favorite"]);
            db.SetFavorite(Convert.ToInt32(dt_movies.Rows[index]["id"]),!favorite);
            if (favorite)
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg };
                dt_movies.Rows[index]["favorite"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg };
                dt_movies.Rows[index]["favorite"] = true;
            }
        }
        //нажатие на кнопку добавить/удалить из просмотренного
        private void button_watched_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить id фильма
            //тут добавить запрос на редактирования в БД
            bool watched = Convert.ToBoolean(dt_movies.Rows[index]["watched"]);
            if (watched)
            {
                (sender as Button).Content = new Image() { Source = noWatchedImg };
                dt_movies.Rows[index]["watched"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = WatchedImg };
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
        
        private void content_load(int index)//Подгрузка контента
        {
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();
            grid_content.ColumnDefinitions.Clear();
            RowDefinition rd = new RowDefinition();
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.ColumnDefinitions.Add(new ColumnDefinition());
            grid_content.RowDefinitions.Add(rd);
            grid_content.RowDefinitions.Add(new RowDefinition());
            Image img = new Image()
            {
                Name = "image_right_content",
                Source = dt_movies.Rows[index]["poster"].GetType() == typeof(DBNull) ? new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreImageCache
                } : LoadImage((byte[])dt_movies.Rows[index]["poster"]),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform
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
                Foreground = Brushes.Gray,
                Padding = new Thickness(5, 5, 5, 5)
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
                Foreground = Brushes.Gray,
                Padding = new Thickness(5, 5, 5, 5)
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





        //Блок методов для меню настроек -->
        
        private void settings_load()//Подгрузка настроек
        {
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

            Button save_my_butt = new Button()//Импорт и экспорт
            {
                Name = "button_in_settings",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 50,
                FontSize = fontSize,
                Width = 100,
                Background = Brushes.Black,
                Foreground = foreColor,
                Content = "Export/Import",
                ClickMode = ClickMode.Press,
                //Padding = new Thickness(50, 50, 50, 50)
            };
            save_my_butt.Click += save_my_butt_Click;

            CheckBox notify = new CheckBox()//Уведомления
            {
                Name = "checkbox_in_settings_notify",
                IsThreeState = false,
                IsChecked = true,
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
                IsChecked = false,
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
                IsChecked = true,
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
                IsChecked = true,
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
                IsChecked = true,
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
                IsChecked = true,
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
                IsChecked = true,
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
                IsChecked = true,
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

            Grid.SetColumn(save_my_butt, 3);
            Grid.SetRow(save_my_butt, 0);
            Grid.SetRowSpan(save_my_butt, 2);
            grid_content.Children.Add(save_my_butt);

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

        //Export or Import
        void save_my_butt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not ready");
        }
        
        //Notification
        void notify_Checked(object sender, RoutedEventArgs e)
        {

        }

        void notify_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Age verification
        void age_Checked(object sender, RoutedEventArgs e)
        {

        }

        void age_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        
        //Netflix
        void netflix_com_Checked(object sender, RoutedEventArgs e)
        {

        }

        void netflix_com_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Ivi
        void ivi_ru_Checked(object sender, RoutedEventArgs e)
        {

        }

        void ivi_ru_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Lostfilm
        void lostfilm_tv_Checked(object sender, RoutedEventArgs e)
        {

        }

        void lostfilm_tv_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Kinokrad
        void kinokrad_co_Checked(object sender, RoutedEventArgs e)
        {

        }

        void kinokrad_co_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Filmzor
        void filmzor_net_Checked(object sender, RoutedEventArgs e)
        {

        }

        void filmzor_net_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //Hdkinozor
        void hdkinozor_ru_Checked(object sender, RoutedEventArgs e)
        {

        }

        void hdkinozor_ru_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        
        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            settings_load();
        }

        //<-- Блок методов для меню настроек







        //показать все фильмы
        private void list_load()
        {
            button_sctoll_top.IsEnabled = false;//выключение кнопки "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            grid_list.Children.Clear();//очистить элементы из центрального грида
            grid_list.RowDefinitions.Clear();//удалить все строки из центрального грида
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                create_and_add_elements(i);
            }
        }
        //показывает список избранного
        private void favorite_load()
        {
            button_sctoll_top.IsEnabled = false;//выключаем кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрываем кнопку "вверх"
            grid_list.Children.Clear();//очищаем центральную панель от элементов
            grid_list.RowDefinitions.Clear();//удаляем все строки в центральном гриде
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false)//если фильм отмечен избранным
                    continue;//то переходим на следующую итерацию
                create_and_add_elements(i);
            }
        }
        //показывает просмотренные
        private void watched_load()
        {
            button_sctoll_top.IsEnabled = false;//выключаем кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрываем кнопку "вверх"
            grid_list.Children.Clear();//очищаем элементы в центральной панели
            grid_list.RowDefinitions.Clear();//удаляем все строки в центральной панели
            for (int i = 0; i < dt_movies.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt_movies.Rows[i]["watched"]) == false)//если фильм не просмотренный
                    continue;//то переходим на следующую итерацию
                create_and_add_elements(i);
            }
        }
        private void create_and_add_elements(int i)
        {
            grid_list.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });//добавление строки в грид
            Button btf = new Button()//кнопка добавления/удаления из избранного
            {
                Name = "button_favorite" + i,
                Height = 40,
                Width = 40,
                Background = null,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0, 0, 0, 0),
                //присвоение соответсвтующей картинки
                Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill },
                Tag = i//id фильма
            };
            btf.Click += button_favorite_Click;//привязывание события клика
            Button btw = new Button()//кнопка добавления/удаления из просмотренного
            {
                Name = "button_watched" + i,
                Height = 40,
                Width = 40,
                Background = null,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                BorderThickness = new Thickness(0, 0, 0, 0),
                //присвоение соответсвтующей картинки
                Content = new Image() { Source = Convert.ToBoolean(dt_movies.Rows[i]["watched"]) == false ? noWatchedImg : WatchedImg, Stretch = Stretch.Fill },
                Tag = i//id фильма
            };
            btw.Click += button_watched_Click;//привязывание события клика
            TextBlock tb = new TextBlock()//текст справа от постера
            {
                Name = "textBlock_middle_content" + i,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                /*Text = "Люди в черном: Интернэшнл (2019)"+Environment.NewLine + "Men in Black International"+
                Environment.NewLine+ "США, реж. Ф. Гэри Грей 16+"+
                Environment.NewLine+ "фантастика, боевик, комедия ...",*/
                Text = dt_movies.Rows[i]["name"].ToString() + " " + dt_movies.Rows[i]["year"].ToString() + Environment.NewLine +

                dt_movies.Rows[i]["country"].ToString() + Environment.NewLine +
                dt_movies.Rows[i]["genres"].ToString() + Environment.NewLine +
                dt_movies.Rows[i]["agerating"].ToString(),
                //Margin = new Thickness(5,5,5,5),
                Foreground = Brushes.Gray,
                Padding = new Thickness(5, 5, 5, 5),
                Tag = i//id фильма
            };
            Image img = new Image()//постер
            {
                Source = dt_movies.Rows[i]["poster"].GetType()==typeof(DBNull)? new BitmapImage(new Uri("/Resources/poster_none.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache }:LoadImage((byte[])dt_movies.Rows[i]["poster"]),//картинка из массива по id
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = Stretch.Uniform,
                Tag = i//id фильма
            };
            TextBlock date = new TextBlock()//текст с датой под кнопками добавить/удалить из избранного/просмотренного
            {
                Name = "textBlock_date" + i,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Text = ConvertDate(dt_movies.Rows[i]["date"].ToString()).Replace(" ", Environment.NewLine),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                Foreground = Brushes.Gray,
                Padding = new Thickness(5, 45, 5, 5),
                Tag = i//id фильма
            };
            int index = grid_list.RowDefinitions.Count - 1;//индекс последней строки в гриде
            //расстановка и добавление элементов в грид
            Grid.SetColumn(img, 0);
            Grid.SetRow(img, index);
            grid_list.Children.Add(img);

            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, index);
            grid_list.Children.Add(tb);

            Grid.SetColumn(date, 2);
            Grid.SetRow(date, index);
            grid_list.Children.Add(date);
            //btf и btw последние чтобы были поверх текста с датой
            Grid.SetColumn(btf, 2);
            Grid.SetRow(btf, index);
            grid_list.Children.Add(btf);

            Grid.SetColumn(btw, 2);
            Grid.SetRow(btw, index);
            grid_list.Children.Add(btw);
        }
        //для отладки и прочего
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)//если нажали пробел
            {
                //stack_content.Visibility = Visibility.Hidden;
                Title.Text = grid_list.RowDefinitions.Count.ToString();
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
            list_load();//показывает все фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
            thread.Abort();
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
                WindowState = WindowState.Normal;//то возвращаем к нормальному
            else
                WindowState = WindowState.Maximized;//иначе делаем во весь экран
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
            db.connect();
            dt_movies = db.GetMovies();
            db.close();
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
            if (str != null || str != "")
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
    }
}
