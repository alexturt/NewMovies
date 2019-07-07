using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace projectE
{
    class Functions
    {
        private static BitmapImage posterNONE = new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + @"\Resources\poster_none.png"));

        //из массива байт в картинку
        public static BitmapImage LoadImage(object _imageData)
        {
            try
            {
                byte[] imageData = (byte[])_imageData;
                if (imageData == null || imageData.Length == 0)
                    return posterNONE;
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
                return posterNONE;
            }
        }

        public static Hyperlink createURL(string url, string name, int size, Brush brush)
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
            link.RequestNavigate += (s, e) => { try { Process.Start(e.Uri.ToString()); } catch { } };
            return link;
        }

        //обновление комбобокса годов, текущего + два прошлых года
        public static ComboBox update_combobox_years(ComboBox comboBox, Brush foregroung)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(new TextBlock() { Background = Brushes.Transparent, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "Все" });
            comboBox.Items.Add(new TextBlock() { Background = Brushes.Transparent, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = DateTime.Now.Year.ToString() });
            comboBox.Items.Add(new TextBlock() { Background = Brushes.Transparent, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = (DateTime.Now.Year - 1).ToString() });
            comboBox.Items.Add(new TextBlock() { Background = Brushes.Transparent, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = (DateTime.Now.Year - 2).ToString() });
            comboBox.SelectedIndex = 0;
            return comboBox;
        }
        //обновление комбобокса с возрастными рейтингами
        public static ComboBox update_combobox_age(ComboBox comboBox, Brush foregroung, bool ageRating)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "Все" });
            comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "0+" });
            comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "6+" });
            comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "12+" });
            comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "16+" });
            if (ageRating)
                comboBox.Items.Add(new TextBlock() { Background = null, Foreground = foregroung, FontSize = 14, Padding = new Thickness(5, 0, 0, 0), Text = "18+" });
            comboBox.SelectedIndex = 0;
            return comboBox;
        }
        //конвертирует дату из "гггг.мм.дд 00:00:00" в "20 июня 2019"
        public static string ConvertDate(string str)
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

        public static string InputBox(Window owner, string title, string promptText)
        {
            Window window = new Window();
            TextBlock label = new TextBlock();
            PasswordBox textBox = new PasswordBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            window.Loaded += (s, e) => { textBox.Focus(); };
            window.Title = title;
            window.Owner = owner;
            label.Text = promptText;

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
            textBox.FontSize = 26;

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
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.ShowDialog();
            return textBox.Password;
        }
    }
}
