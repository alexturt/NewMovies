using Microsoft.VisualStudio.TestTools.UnitTesting;
using projectE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System.Net;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows;

namespace projectE.Tests
{
    [TestClass()]
    public class MainWindowTests
    {

        [TestMethod()]
        public void MainWindowTest()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void ShowBoxTest()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowBox("asd");
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void createURLTest()
        {
            MainWindow mainWindow = new MainWindow();
            Hyperlink hl = new Hyperlink();
            var v = mainWindow.createURL("as1123", "name1", 20, Brushes.Gray);
            Assert.IsInstanceOfType(v, typeof(Hyperlink));
        }

        [TestMethod()]
        public void update_moviesTest()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.update_movies("Все", 25, 0);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void LoadImageTest()
        {
            byte[] bytes = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var v = MainWindow.LoadImage(bytes);
            Assert.IsInstanceOfType(v, typeof(BitmapImage));
        }

        [TestMethod()]
        public void ConvertDateTest()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.ConvertDate("09.08.2019 0:00:00");
            }
            catch
            {
                Assert.Fail("ошибка конвертирования даты");
            }
        }

        [TestMethod()]
        public void ConvertDateTest2()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.ConvertDate("asd");
            }
            catch
            {
                Assert.Fail("ошибка конвертирования даты");
            }
        }

        [TestMethod()]
        public void createNotifyIconTest()
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.createNotifyIcon();
            }
            catch
            {
                Assert.Fail("ошибка создания иконки в трее");
            }
        }

        [TestMethod()]
        public void filterIsOpenTest()
        {
            MainWindow mainWindow = new MainWindow();
            var v = mainWindow.filterIsOpen();
            Assert.IsInstanceOfType(v,typeof(bool));
        }
    }
}