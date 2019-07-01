using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace projectE
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            if (!InstanceCheck())
            {
                MessageBox.Show("Программа уже запущена");
                base.Shutdown();
                // нажаловаться пользователю и завершить процесс
            }
        }

        // держим в переменной, чтобы сохранить владение им до конца пробега программы
        static Mutex InstanceCheckMutex;
        static bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new Mutex(true, "summer_event_2019_E", out isNew);
            return isNew;
        }
    }
}
