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
            //grid.ColumnDefinitions[2].Width = new GridLength(0);
            grid.ColumnDefinitions[2].IsEnabled = false;
            Width = 450;
            stack_content.Visibility = Visibility.Hidden;
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
            stack_content.Visibility = Visibility.Visible;
        }
        void closePanel()
        {
            double size = grid.ColumnDefinitions[2].ActualWidth;
            grid_content.Children.Clear();
            grid.ColumnDefinitions[2].IsEnabled = false;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
            Width -= size;
            
            
            stack_content.Visibility = Visibility.Hidden;
        }

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
        
        private void button_favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);
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

        private void button_watched_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32((sender as Button).Tag);
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
        
        private void grid_list_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == null)
                return;
            if (e.Source.GetType() == typeof(TextBlock))
            {
                Title.Text = (e.Source as TextBlock).Tag.ToString();
                content_load(Convert.ToInt32((e.Source as TextBlock).Tag));
            }
            else
            if (e.Source.GetType() == typeof(Image))
            {
                Title.Text = (e.Source as Image).Tag.ToString();
                content_load(Convert.ToInt32((e.Source as Image).Tag));
            }
            e.Handled = true;
        }
        
        private void content_load(int index)
        {
            if (!grid.ColumnDefinitions[2].IsEnabled)
                openPanel();
            grid_content.Children.Clear();
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
            /*Text = dt.Rows[i]["name"].ToString() + Environment.NewLine + dt.Rows[i]["nameEN"] + Environment.NewLine +
                    "Год\t" + dt.Rows[i]["year"].ToString() + Environment.NewLine +
                    "Страна\t" + dt.Rows[i]["country"].ToString() + Environment.NewLine +
                    "Режиссер\t" + dt.Rows[i]["director"].ToString() + Environment.NewLine +
                    "Жанр\t" + dt.Rows[i]["genre"].ToString() + Environment.NewLine +
                    "Дата\t" + dt.Rows[i]["date"].ToString() + Environment.NewLine +
                    "Возраст\t" + dt.Rows[i]["agelimit"].ToString(),*/
        }
        private void list_load()
        {
            grid_list.Children.Clear();
            grid_list.RowDefinitions.Clear();
            RowDefinition rd;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                rd = new RowDefinition() { Height = new GridLength(1,GridUnitType.Star) };
                grid_list.RowDefinitions.Add(rd);
                Button btf = new Button()
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill },
                    Tag = i
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
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["watched"]) == false ? noWatchedImg : WatchedImg, Stretch = Stretch.Fill },
                    Tag = i
                };
                btw.Click += button_watched_Click;
                TextBlock tb = new TextBlock()
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
                    Tag = i
                };
                Image img = new Image()
                {
                    Source = imgmass[i],
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = i
                };
                TextBlock date = new TextBlock()
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
        private void favotite_load()
        {
            grid_list.Children.Clear();
            grid_list.RowDefinitions.Clear();
            RowDefinition rd;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dt.Rows[i]["favorite"]) == false)
                    continue;
                rd = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                grid_list.RowDefinitions.Add(rd);
                Button btf = new Button()
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Content = new Image() { Source = FavoriteImg, Stretch = Stretch.Fill },
                    Tag = i
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
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["watched"]) == false ? noWatchedImg:WatchedImg, Stretch = Stretch.Fill },
                    Tag = i
                };
                btw.Click += button_watched_Click;
                TextBlock tb = new TextBlock()
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
                    Tag = i
                };
                Image img = new Image()
                {
                    Source = imgmass[i],
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = i
                };
                TextBlock date = new TextBlock()
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

                int index = grid_list.RowDefinitions.Count-1;

                Grid.SetColumn(img, 0);
                Grid.SetRow(img, index);
                grid_list.Children.Add(img);

                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, index);
                grid_list.Children.Add(tb);

                Grid.SetColumn(btf, 2);
                Grid.SetRow(btf, index);
                grid_list.Children.Add(btf);

                Grid.SetColumn(btw, 2);
                Grid.SetRow(btw, index);
                grid_list.Children.Add(btw);

                Grid.SetColumn(date, 2);
                Grid.SetRow(date, index);
                grid_list.Children.Add(date);
            }
        }
        private void watched_load()
        {
            grid_list.Children.Clear();
            grid_list.RowDefinitions.Clear();
            RowDefinition rd;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dt.Rows[i]["watched"]) == false)
                    continue;
                rd = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                grid_list.RowDefinitions.Add(rd);
                Button btf = new Button()
                {
                    Name = "button_favorite" + i,
                    Height = 40,
                    Width = 40,
                    Background = null,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Content = new Image() { Source = Convert.ToBoolean(dt.Rows[i]["favorite"]) == false ? noFavoriteImg : FavoriteImg, Stretch = Stretch.Fill },
                    Tag = i
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
                    Content = new Image() { Source = WatchedImg, Stretch = Stretch.Fill },
                    Tag = i
                };
                btw.Click += button_watched_Click;
                TextBlock tb = new TextBlock()
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
                    Tag = i
                };
                Image img = new Image()
                {
                    Source = imgmass[i],
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Uniform,
                    Tag = i
                };
                TextBlock date = new TextBlock()
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

                int index = grid_list.RowDefinitions.Count - 1;

                Grid.SetColumn(img, 0);
                Grid.SetRow(img, index);
                grid_list.Children.Add(img);

                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, index);
                grid_list.Children.Add(tb);

                Grid.SetColumn(btf, 2);
                Grid.SetRow(btf, index);
                grid_list.Children.Add(btf);

                Grid.SetColumn(btw, 2);
                Grid.SetRow(btw, index);
                grid_list.Children.Add(btw);

                Grid.SetColumn(date, 2);
                Grid.SetRow(date, index);
                grid_list.Children.Add(date);
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                //stack_content.Visibility = Visibility.Hidden;
                scroll_viewer_center.ScrollToTop();
            }
        }

        private void button_home_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
          //  parser.UpdateList();
            list_load();
            scroll_viewer_center.ScrollToTop();
        }

        private void button_favorite_Click_1(object sender, RoutedEventArgs e)
        {
            favotite_load();
            scroll_viewer_center.ScrollToTop();
        }

        private void button_watched_Click_1(object sender, RoutedEventArgs e)
        {
            watched_load();
            scroll_viewer_center.ScrollToTop();
        }

        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_maximazing_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void button_hide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        { 
            if (Width < 700 && grid.ColumnDefinitions[2].IsEnabled)
            {
                /*grid.ColumnDefinitions[2].Width = new GridLength(0);
                grid.ColumnDefinitions[2].IsEnabled = false;
                stack_content.Visibility = Visibility.Hidden;*/
                //Width = grid.ColumnDefinitions[1].ActualWidth + 40;
                closePanel();
            }
        }
    }
}
