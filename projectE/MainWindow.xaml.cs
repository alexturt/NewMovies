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
        BitmapImage poster0Img = new BitmapImage(new Uri("/Resources/posterTEST.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster1Img = new BitmapImage(new Uri("/Resources/poster1.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster2Img = new BitmapImage(new Uri("/Resources/poster2.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster3Img = new BitmapImage(new Uri("/Resources/poster3.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster4Img = new BitmapImage(new Uri("/Resources/poster4.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster5Img = new BitmapImage(new Uri("/Resources/poster5.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster6Img = new BitmapImage(new Uri("/Resources/poster6.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage poster7Img = new BitmapImage(new Uri("/Resources/poster7.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        BitmapImage[] imgmass;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //пример добавление фильма в базу
            //db.connect();
            //db.AddMovie("123", 123, "2019-10-12", "123", "123", "123", "123", "https://st.kp.yandex.net/images/film_iphone/iphone360_1125423.jpg", "asd", "asd", "asd", false, false);
            //db.close();
            //тут тестовые данные
            imgmass = new BitmapImage[8]
                {
                    poster0Img,
                    poster1Img,
                    poster2Img,
                    poster3Img,
                    poster4Img,
                    poster5Img,
                    poster6Img,
                    poster7Img
                };

            dt.Columns.Add("name");
            dt.Columns.Add("nameEN");
            dt.Columns.Add("year");
            dt.Columns.Add("country");
            dt.Columns.Add("director");
            dt.Columns.Add("genre");
            dt.Columns.Add("date");
            dt.Columns.Add("agelimit");
            dt.Columns.Add("description");
            dt.Columns.Add("watched");
            dt.Columns.Add("favorite");
            {
                dt.Rows.Add(
                    "Люди в черном: Интернэшнл",
                    "Men in Black International",
                    "2019",
                    "США",
                    "Ф. Гэри Грей",
                    "фантастика, боевик, комедия, приключения",
                    "4 сентября 2019",
                    "16+",
                    "Люди в черном, тайная организация на страже покоя и безопасности Земли, уже не раз защищали нас от нападения отбросов Вселенной. На этот раз самая большая опасность для мирового сообщества, которой агентам предстоит противостоять — шпион в их рядах.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Детские игры",
                    "Child's Play",
                    "2019",
                    "Франция, Канада, США",
                    "Ларс Клевберг",
                    "ужасы",
                    "20 июня 2019",
                    "18+",
                    "Мать-одиночка Карен дарит своему сыну Энди куклу, о которой мечтают все дети. Однако, вскоре становится ясно, что Энди достается больше, чем просто игрушка…",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Дылда",
                    "",
                    "2019",
                    "Россия",
                    "Кантемир Балагов",
                    "драма",
                    "20 июня 2019",
                    "16+",
                    "История двух молодых женщин-фронтовичек, которые возвращаются в послевоенный Ленинград и пытаются обрести новую мирную жизнь, когда и вокруг, и главное, внутри них — руины.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "История игрушек 4",
                    "Toy Story 4",
                    "2019",
                    "США",
                    "Джош Кули",
                    "мультфильм, фэнтези, комедия, приключения, семейный",
                    "20 июня 2019",
                    "6+",
                    "Астронавт Базз, ковбой Вуди, собака Спиралька, тиранозавр Рекс и вся команда игрушек снова вместе, и они как никогда готовы к приключениям. С тех пор, как Энди поступил в колледж, игрушки поселились в доме своей новой хозяйки Бонни. Первый день Бонни в школе положит начало череде событий, в которой найдется место и путешествиям с погонями, и знакомству с новыми игрушками, и встречам с некоторыми из старых друзей, и даже романтической истории.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Люди Икс: Тёмный Феникс",
                    "Dark Phoenix",
                    "2019",
                    "США",
                    "Саймон Кинберг",
                    "фантастика, боевик, приключения",
                    "6 июня 2019",
                    "16+",
                    "История Джин Грей, которая разворачивается в тот момент, когда героиня превращается в культового Тёмного Феникса. Во время опасной для жизни спасательной миссии в космосе девушка оказывается поражена таинственной космической силой, которая превращает её в одного из самых могущественных мутантов. В борьбе с этой всё более изменчивой мощью и со своими собственными демонами Джин выходит из-под контроля: раскалывает семью Людей Икс и угрожает уничтожить всю нашу планету.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Тайная жизнь домашних животных 2",
                    "The Secret Life of Pets 2",
                    "2019",
                    "США, Франция, Япония",
                    "Крис Рено, Джонатан дель Валь",
                    "мультфильм, комедия, приключения, семейный",
                    "25 июля 2019",
                    "6+",
                    "Макс уезжает с хозяйкой, мужем и ребёнком в гости на ранчо. Тем временем, в городе у Снежка и Гиджет свои приключения.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Ма",
                    "Ma",
                    "2019",
                    "США",
                    "Тейт Тейлор",
                    "ужасы, триллер",
                    "13 июня 2019",
                    "18+",
                    "Все знают, что у МА самые крутые вечеринки в городе. Здесь всегда громкая музыка, и алкоголь льется рекой. В этом доме нет никаких ограничений, кроме одного: никогда не заходи в ее комнату. Добро пожаловать к МА.",
                    false,
                    false
                    );
                dt.Rows.Add(
                    "Аладдин",
                    "Aladdin",
                    "2019",
                    "США",
                    "Гай Ричи",
                    "мюзикл, фэнтези, мелодрама, комедия, приключения, семейный",
                    "30 августа 2019",
                    "6+",
                    "Молодой воришка по имени Аладдин хочет стать принцем, чтобы жениться на принцессе Жасмин. Тем временем визирь Аграбы Джафар намеревается захватить власть над Аграбой, а для этого он стремится заполучить волшебную лампу, хранящуюся в пещере чудес, доступ к которой разрешен лишь тому, кого называют «алмаз неограненный», и этим человеком является не кто иной, как сам Аладдин.",
                    false,
                    false
                    );
            }

            list_load();
        }
        //нажатие на кнопку добавить/удалить из избранного
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить id фильма
            bool favorite = Convert.ToBoolean(dt.Rows[index]["favorite"]);
            if (favorite)
            {
                (sender as Button).Content = new Image() { Source = noFavoriteImg };
                dt.Rows[index]["favorite"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = FavoriteImg };
                dt.Rows[index]["favorite"] = true;
            }
        }
        //нажатие на кнопку добавить/удалить из просмотренного
        private void button_watched_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);//получить id фильма
            bool watched = Convert.ToBoolean(dt.Rows[index]["watched"]);
            if (watched)
            {
                (sender as Button).Content = new Image() { Source = noWatchedImg };
                dt.Rows[index]["watched"] = false;
            }
            else
            {
                (sender as Button).Content = new Image() { Source = WatchedImg };
                dt.Rows[index]["watched"] = true;
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
            grid_content.RowDefinitions.Add(rd);
            Image img = new Image()
            {
                Name = "image_right_content",
                Source = imgmass[index],
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
                Text = dt.Rows[index]["name"].ToString() + Environment.NewLine + dt.Rows[index]["nameEN"] + Environment.NewLine +
                    "Год\t" + dt.Rows[index]["year"].ToString() + Environment.NewLine +
                    "Страна\t" + dt.Rows[index]["country"].ToString() + Environment.NewLine +
                    "Режиссер\t" + dt.Rows[index]["director"].ToString() + Environment.NewLine +
                    "Жанр\t" + dt.Rows[index]["genre"].ToString() + Environment.NewLine +
                    "Дата\t" + dt.Rows[index]["date"].ToString() + Environment.NewLine +
                    "Возраст\t" + dt.Rows[index]["agelimit"].ToString(),
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
                Text = dt.Rows[index]["description"].ToString(),
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

        private void settings_load()//Подгрузка настроек
        {
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
            //grid_content.RowDefinitions.Clear();
            //grid_content.ColumnDefinitions.Clear();
            ColumnDefinition cd = new ColumnDefinition();
            RowDefinition rd3 = new RowDefinition();
            RowDefinition rd4 = new RowDefinition();
            RowDefinition rd5 = new RowDefinition();
            RowDefinition rd6 = new RowDefinition();
            RowDefinition rd7 = new RowDefinition();
            RowDefinition rd8 = new RowDefinition();
            grid_content.ColumnDefinitions.Add(cd);
            grid_content.RowDefinitions.Add(rd3);
            grid_content.RowDefinitions.Add(rd4);
            grid_content.RowDefinitions.Add(rd5);
            grid_content.RowDefinitions.Add(rd6);
            grid_content.RowDefinitions.Add(rd7);
            grid_content.RowDefinitions.Add(rd8);
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
                Content = "NetFlix",
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
            CheckBox megogo_net = new CheckBox()//Megogo
            {
                Name = "checkbox_in_settings_megogo_net",
                IsThreeState = false,
                IsChecked = true,
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "megogo.net",
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
            CheckBox otherSource = new CheckBox()//Add other source
            {
                Name = "checkbox_in_settings_other",
                IsThreeState = false,
                IsChecked = false,
                Height = 40,
                FontSize = fontSize,
                //Width = 100,
                Content = "Другой источник...",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = backColor,
                Foreground = foreColor,
                Padding = new Thickness(5, 5, 5, 5)
            };
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

            Grid.SetColumn(megogo_net, 2);
            Grid.SetRow(megogo_net, 5);
            grid_content.Children.Add(megogo_net);

            Grid.SetColumn(kinokrad_co, 0);
            Grid.SetRow(kinokrad_co, 6);
            grid_content.Children.Add(kinokrad_co);

            Grid.SetColumn(filmzor_net, 1);
            Grid.SetRow(filmzor_net, 6);
            grid_content.Children.Add(filmzor_net);

            Grid.SetColumn(hdkinozor_ru, 2);
            Grid.SetRow(hdkinozor_ru, 6);
            grid_content.Children.Add(hdkinozor_ru);

            Grid.SetColumn(lostfilm_tv, 0);
            Grid.SetRow(lostfilm_tv, 7);
            grid_content.Children.Add(lostfilm_tv);

            Grid.SetColumn(otherSource, 1);
            Grid.SetRow(otherSource, 7);
            Grid.SetColumnSpan(otherSource, 2);
            grid_content.Children.Add(otherSource);


        }

        void save_my_butt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not ready");
        }
        //показать все фильмы
        private void list_load()
        {
            button_sctoll_top.IsEnabled = false;//выключение кнопки "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрыть кнопку "вверх"
            grid_list.Children.Clear();//очистить элементы из центрального грида
            grid_list.RowDefinitions.Clear();//удалить все строки из центрального грида
            RowDefinition rd;//строка
            for (int i = 0; i < dt.Rows.Count; i++)//цикл по всем фильмам
            {
                rd = new RowDefinition() { Height = new GridLength(1,GridUnitType.Star) };//создание строки с высотой "1*"
                grid_list.RowDefinitions.Add(rd);//добавление строки в грид
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
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill },
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
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["watched"]) == false ? noWatchedImg : WatchedImg, Stretch = Stretch.Fill },
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
                    Text = dt.Rows[i]["name"].ToString() + " " + dt.Rows[i]["year"].ToString() + Environment.NewLine + dt.Rows[i]["nameEN"] + Environment.NewLine +

                    dt.Rows[i]["country"].ToString() + " " + dt.Rows[i]["director"].ToString() + Environment.NewLine +
                    dt.Rows[i]["genre"].ToString() + Environment.NewLine,
                    //Margin = new Thickness(5,5,5,5),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = i//id фильма
                };
                Image img = new Image()//постер
                {
                    Source = imgmass[i],//картинка из массива по id
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
                    Text = dt.Rows[i]["date"].ToString().Replace(" ", Environment.NewLine),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 0),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 45, 5, 5),
                    Tag = i//id фильма
                };
                //расстановка и добавление элементов в грид
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, i);
                grid_list.Children.Add(img);

                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, i);
                grid_list.Children.Add(tb);

                Grid.SetColumn(date, 2);
                Grid.SetRow(date, i);
                grid_list.Children.Add(date);
                //btf и btw последние чтобы были поверх текста с датой
                Grid.SetColumn(btf, 2);
                Grid.SetRow(btf, i);
                grid_list.Children.Add(btf);

                Grid.SetColumn(btw, 2);
                Grid.SetRow(btw, i);
                grid_list.Children.Add(btw);

                
            }
        }
        //показывает список избранного
        private void favorite_load()
        {
            button_sctoll_top.IsEnabled = false;//выключаем кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрываем кнопку "вверх"
            grid_list.Children.Clear();//очищаем центральную панель от элементов
            grid_list.RowDefinitions.Clear();//удаляем все строки в центральном гриде
            RowDefinition rd;//строка
            for (int i = 0; i < dt.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt.Rows[i]["favorite"]) == false)//если фильм отмечен избранным
                    continue;//то переходим на следующую итерацию
                rd = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };//создание строки с высотой "1*"
                grid_list.RowDefinitions.Add(rd);//добавление строки в грид
                Button btf = new Button()//кнопка добавления/уделаения из избранного
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    //не надо проверять в избраном или нет т.к. показываем список избранного
                    Content = new Image() { Source = FavoriteImg, Stretch = Stretch.Fill },
                    Tag = i//id фильма
                };
                btf.Click += button_favorite_Click;//привязывание собития клика
                Button btw = new Button()//кнопка добавления/удаления из просмотренноо
                {
                    Name = "button_watched" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    //проверяем просмотренный или нет и присваиваем соответствующую картинку
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["watched"]) == false ? noWatchedImg:WatchedImg, Stretch = Stretch.Fill },
                    Tag = i//id фильма
                };
                btw.Click += button_watched_Click;//привязывание собития клика
                TextBlock tb = new TextBlock()//текст с инфой о фильма справа от постера
                {
                    Name = "textBlock_middle_content" + i,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    /*Text = "Люди в черном: Интернэшнл (2019)"+Environment.NewLine + "Men in Black International"+
                    Environment.NewLine+ "США, реж. Ф. Гэри Грей 16+"+
                    Environment.NewLine+ "фантастика, боевик, комедия ...",*/
                    Text = dt.Rows[i]["name"].ToString() + " " + dt.Rows[i]["year"].ToString() + Environment.NewLine + dt.Rows[i]["nameEN"] + Environment.NewLine +

                    dt.Rows[i]["country"].ToString() + " " + dt.Rows[i]["director"].ToString() + Environment.NewLine +
                    dt.Rows[i]["genre"].ToString() + Environment.NewLine,
                    //Margin = new Thickness(5,5,5,5),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = i//id фильма
                };
                Image img = new Image()//постер
                {
                    Source = imgmass[i],//картинка из массива по id
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
                    Text = dt.Rows[i]["date"].ToString().Replace(" ", Environment.NewLine),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = i//id фильма
                };

                int index = grid_list.RowDefinitions.Count-1;//индекс последней строки
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
        }
        //показывает просмотренные
        private void watched_load()
        {
            button_sctoll_top.IsEnabled = false;//выключаем кнопку "вверх"
            button_sctoll_top.Visibility = Visibility.Hidden;//скрываем кнопку "вверх"
            grid_list.Children.Clear();//очищаем элементы в центральной панели
            grid_list.RowDefinitions.Clear();//удаляем все строки в центральной панели
            RowDefinition rd;//новая строка
            for (int i = 0; i < dt.Rows.Count; i++)//цикл по всем фильмам
            {
                if (Convert.ToBoolean(dt.Rows[i]["watched"]) == false)//если фильм не просмотренный
                    continue;//то переходим на следующую итерацию
                rd = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }; //создание новой строки с высотой "1*"
                grid_list.RowDefinitions.Add(rd);//добавляем эту строку в центральную сетку
                Button btf = new Button()//создание кнопки "добавить/удалить из избранное"
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Content = new Image()
                    {//выбор картинки для кнопки в зависимости от добавлено ли в избранное или нет
                        Source = Convert.ToBoolean(dt.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill
                    },
                    Tag = i//id фильма
                };
                btf.Click += button_favorite_Click;//привязывание события клика
                Button btw = new Button()//создание кнопки "добавить/удалить из просмотренного"
                {
                    Name = "button_watched" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    //здесь не надо проверять просмотреный фильм или нет потому-что показываем просмотренное
                    Content = new Image() { Source = WatchedImg, Stretch = Stretch.Fill },
                    Tag = i//id фильма
                };
                btw.Click += button_watched_Click;//привязывание собития клика
                TextBlock tb = new TextBlock()//текст с инфой о фильме в центре справа от постера
                {
                    Name = "textBlock_middle_content" + i,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    /*Text = "Люди в черном: Интернэшнл (2019)"+Environment.NewLine + "Men in Black International"+
                    Environment.NewLine+ "США, реж. Ф. Гэри Грей 16+"+
                    Environment.NewLine+ "фантастика, боевик, комедия ...",*/
                    Text = dt.Rows[i]["name"].ToString() + " " + dt.Rows[i]["year"].ToString() + Environment.NewLine + dt.Rows[i]["nameEN"] + Environment.NewLine +

                    dt.Rows[i]["country"].ToString() + " " + dt.Rows[i]["director"].ToString() + Environment.NewLine +
                    dt.Rows[i]["genre"].ToString() + Environment.NewLine,
                    //Margin = new Thickness(5,5,5,5),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = i//id фильма
                };
                Image img = new Image()//создание постера
                {
                    Source = imgmass[i],//загружаем картинку из массива по id
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = i//id фильма
                };
                TextBlock date = new TextBlock()//текст с датой выхода фильма под кнопкам "добавить в избранное/просмотренное"
                {
                    Name = "textBlock_date" + i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Text = dt.Rows[i]["date"].ToString().Replace(" ", Environment.NewLine),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0),
                    Foreground = Brushes.Gray,
                    Padding = new Thickness(5, 5, 5, 5),
                    Tag = i
                };

                int index = grid_list.RowDefinitions.Count - 1;//индекс последней строки в гриде
                //расстановка и добавление элементов в грид
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, index);
                grid_list.Children.Add(img);

                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, index);
                grid_list.Children.Add(tb);

                Grid.SetColumn(btf, 2);
                Grid.SetRow(btf, index);
                grid_list.Children.Add(btf);
                // btw и btf последние чтобы были поверх текста с датой
                Grid.SetColumn(btw, 2);
                Grid.SetRow(btw, index);
                grid_list.Children.Add(btw);

                Grid.SetColumn(date, 2);
                Grid.SetRow(date, index);
                grid_list.Children.Add(date);
            }
        }
        //для отладки и прочего
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)//если нажали пробел
            {
                //stack_content.Visibility = Visibility.Hidden;
                scroll_viewer_center.ScrollToTop();
            }
        }
        //нажали кнопку домой
        private void button_home_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
        //    parser.UpdateList();
            list_load();//показывает все фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
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
            if (Width < 700 && grid.ColumnDefinitions[2].IsEnabled)//если ширина окна меньше 700
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
        //Settings butt clicked
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            settings_load();
        }
        //нажали кнопку просмотренное (меню)
        private void button_watched_Click_1(object sender, RoutedEventArgs e)
        {
            watched_load();//показывает просмотренные фильмы
            scroll_viewer_center.ScrollToTop();//проскролить вверх
        }
    }
}
