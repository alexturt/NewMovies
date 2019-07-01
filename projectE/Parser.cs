using CsQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace projectE
{
    class Parser
    {
        DB db = new DB();

        string[,] films = new string[500, 11];
        int countElem = 0;

        const int settings_amount = 8;
        bool[] IsChecked = new bool[settings_amount];

        public void CheckSettings()
        {
            DataTable dt = db.GetSettings();
            for (int i = 0; i < settings_amount; i++)
            {
                IsChecked[i] = Convert.ToBoolean(dt.Rows[i].ItemArray[0].ToString());
            }
        }

        public bool stop = false;
        public CQ IsConnectInternet(string Html)
        {
            if (!stop)
            {
                try
                {
                    CQ film = CQ.CreateFromUrl(Html);
                    return film;
                }
                catch
                {
                    MainWindow mw = new MainWindow();
                    mw.ShowBox("Интернет соединение отсутствует или очень медленное. Загрузка новых данных не удалась");
                    stop = true;
                    return null;
                }
            }
            return null;
        }

        public void UpdateList()
        {
            TimeSpan FullTime = TimeSpan.Zero;
            TimeSpan Start;
            TimeSpan Stop;

            CheckSettings();

            if (IsChecked[7])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим ХДКинозор. Начало: " + Start);
                UpdateListHDKinozor(@"https://hdkinozor.ru/top2019.html");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2019-t2");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[3])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Иви. Начало: " + Start);
                UpdateListIvi(@"https://www.ivi.ru/movies/2019/page3");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[5])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Кинокрад. Начало: " + Start);
                UpdateListKinokrad(@"https://kino50.top/filmy-online/browse/1/all/all/2019?sort_by=field_weight_value");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[2])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Нетфликс. Начало: " + Start);
                UpdateListNetflix(@"http://netflix.kinoyou.com/god/2019/");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[4])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Лостфильм. Начало: " + Start);
                UpdateListLostfilm(@"https://www.lostfilm.tv/series/?type=search&s=3&t=1");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            Console.WriteLine("Было выгружено : " + countElem + ". Всего потрачено: " + FullTime);

            addBd();

            Console.WriteLine("Добавление в БД закончено.");
        }

        private string FindTrailer(string name, string year)
        {
            string[] masName = name.Split(' ').ToArray();
            string fullName = "";
            for (int i = 0; i < masName.Count(); i++)
            {
                fullName += masName[i] + "+";
            }
            fullName += year + "+трейлер";
            string link = @"https://www.youtube.com/results?search_query=" + fullName;
            return link;
        }

        private string FindLinkFilm(string name, string year)
        {
            string[] masName = name.Split(' ').ToArray();
            string fullName = "";
            for (int i = 0; i < masName.Count(); i++)
            {
                fullName += masName[i] + "+";
            }
            fullName += year + "+фильм";
            string link = @"https://www.youtube.com/results?search_query=" + fullName;
            return link;
        }

        private string FindData(string name, string year)
        {
            string[] data;
            string datat = "";
            string[] masName = name.ToLower().Split(' ');
            string nameP = "";
            for (int i = 0; i < masName.Count(); i++)
            {
                nameP += masName[i] + "+";
            }
            string Html = @"https://www.megacritic.ru/poisk-filmov/search-results?order=rdate&criteria=5&scope=title_introtext&query=any&keywords=" + nameP;// + year + "%20дата%20выхода";
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 11; i++)
                {
                    foreach (IDomObject obj in film.Find("#jr-pagenav-ajax > div.jrTableGrid.jrDataList.jrResults > div:nth-child(" + i + ") > div.jr-listing-outer.jrCol.jrTableColumnMain > div.jrContentTitle"))
                    {

                        if (obj.FirstChild.FirstChild.NodeValue.ToLower().Equals(name.ToLower().Trim()))
                        {
                            data = obj.NextSibling.NextSibling.FirstElementChild.FirstElementChild.LastChild.NodeValue.Split(' ');
                            data = data[0].Split('.');
                            datat = data[2] + "-" + data[1] + "-" + data[0];

                        }
                    }
                }
            }
            if (datat == "")
            {
                Html = @"https://www.kinoafisha.info/search/?q=" + nameP;
                film = IsConnectInternet(Html);
                if (film != null)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        foreach (IDomObject obj in film.Find("#main-left > div.layer-empty > div.grid.grid-search > div > div.grid_cell10 > div > article:nth-child(" + i + ") > div > div.films_right > a"))
                        {
                            if (obj.FirstChild.FirstChild.FirstChild.NodeValue.Trim().Equals(name.Trim()))
                            {
                                string a = @"https:" + obj.GetAttribute("href");
                                film = IsConnectInternet(a);
                                if (film != null)
                                {
                                    //#main-left > div:nth-child(7) > div > div > div > div.grid_row > div.grid_cell9 > div.movieInfoV2_info > div:nth-child(4) > span.movieInfoV2_infoData
                                    foreach (IDomObject obj1 in film.Find(".movieInfoV2_infoData"))
                                    {
                                        try
                                        {
                                            string[] masDat = obj1.FirstChild.NodeValue.Trim().Split(' ');
                                            if ((masDat.Count() == 3))
                                            {
                                                datat = masDat[0] + " " + masDat[1] + " " + masDat[2];
                                                return datat;
                                            }
                                            else
                                            {
                                                datat = "0+";
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }

                if (datat == "")
                {
                    datat = "0001-01-01";
                }
            }
            return datat;
        }

        private string FindAgeRating(string name, string year)
        {
            string[] age;
            string ageR = "";
            string[] masName = name.ToLower().Split(' ');
            string nameP = "";
            for (int i = 0; i < masName.Count(); i++)
            {
                nameP += masName[i] + "+";
            }
            int countStr = 1;
            string Html = @"https://www.megacritic.ru/poisk-filmov/search-results?page=1&order=rdate&criteria=5&scope=title_introtext&query=any&keywords=" + nameP;
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                foreach (IDomObject obj in film.Find("#jr-pagenav-ajax > div.jr-pagenav.jrTableGrid.jrPagination.jrPaginationTop > div.jrCol4.jrPagenavPages > a"))
                {
                    try
                    {
                        countStr = Convert.ToInt32(obj.FirstChild.NodeValue);
                    }
                    catch { countStr = 1; }
                }
                int j = 2;
                do
                {
                    for (int i = 1; i < 11; i++)
                    {
                        foreach (IDomObject obj in film.Find("#jr-pagenav-ajax > div.jrTableGrid.jrDataList.jrResults > div:nth-child(" + i + ") > div.jr-listing-outer.jrCol.jrTableColumnMain > div.jrContentTitle"))
                        {

                            if (obj.FirstChild.FirstChild.NodeValue.ToLower().Equals(name.ToLower().Trim()))
                            {
                                age = obj.NextSibling.NextSibling.FirstElementChild.FirstElementChild.LastChild.NodeValue.Split(' ');
                                ageR = age[1].Replace("г.", "").Replace("(", "").Replace(")", "").Trim();
                                if (ageR.Length > 3)
                                {
                                    ageR = ageR.Remove(3);
                                }
                                return ageR;
                            }
                        }
                    }
                    if (countStr != 1)
                    {
                        Html = @"https://www.megacritic.ru/poisk-filmov/search-results?page=" + j + "&order=rdate&criteria=5&scope=title_introtext&query=any&keywords=" + nameP;
                        film = IsConnectInternet(Html);
                        if (film != null)
                        {
                            j++;
                        }

                    }
                } while (j < countStr);
            }
            if (ageR == "")
            {
                Html = @"https://www.kinoafisha.info/search/?q=" + nameP;
                film = IsConnectInternet(Html);
                if (film != null)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        foreach (IDomObject obj in film.Find("#main-left > div.layer-empty > div.grid.grid-search > div > div.grid_cell10 > div > article:nth-child(" + i + ") > div > div.films_right > a"))
                        {
                            if (obj.FirstChild.FirstChild.FirstChild.NodeValue.Trim().Equals(name.Trim()))
                            {
                                string a = @"https:" + obj.GetAttribute("href");
                                film = IsConnectInternet(a);
                                if (film != null)
                                {
                                    //#main-left > div:nth-child(7) > div > div > div > div.grid_row > div.grid_cell9 > div.movieInfoV2_info > div:nth-child(4) > span.movieInfoV2_infoData
                                    foreach (IDomObject obj1 in film.Find(".movieInfoV2_infoData"))
                                    {
                                        try
                                        {
                                            ageR = obj1.FirstChild.NodeValue.Trim();
                                            if ((ageR.Length == 3) || (ageR.Length == 2))
                                            {
                                                return ageR;
                                            }
                                            else
                                            {
                                                ageR = "0+";
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (ageR == "")
            {
                ageR = "0+";
            }
            return ageR;
        }

        private bool DoubleFilm(string name, string year)
        {
            bool flag = false;
            for (int i = 0; i < countElem + 1; i++)
            {
                if (films[i, 0] != null)
                {
                    if (films[i, 1].Contains(name) && films[i, 2].Equals(year))
                    {
                        flag = true; //да, это дубль
                    }
                    else
                    {
                        flag = false; //еще нет такого фильма
                    }
                }
                else { return flag; }
            }
            return flag;
        }

        private void UpdateListNetflix(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 13; i++)
                {
                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(" + i + ") > div > div.short-content > h4 > a"))
                    {
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                AddFilmNetflix(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmNetflix(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait(); ;
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string name = film.Find("#dle-content > div > h2").Text().Replace("смотреть онлайн", "");
                    string year = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(3) > div > div.finfo-text > a").Text();
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name.Trim(); //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a"))
                            {
                                films[countElem, 3] += obj.FirstChild.NodeValue + " ";
                            }
                            films[countElem, 4] = film.Find("#dle-content > div > div.fstory-content.margin-b40.block-p > h4").Text(); //Описание
                            films[countElem, 5] = FindTrailer(name, year); //Трейлер
                            films[countElem, 6] = FindAgeRating(name, year); //Возр.огр
                            films[countElem, 7] = film.Find("#fstory-film > div > div.col-sm-4.col-xs-12.fstory-poster-in > div.fstory-poster > img").Attr("src"); //Картинка
                            films[countElem, 8] = ReData(FindData(name, year)); //Дата выхода
                                                                                //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text"))
                            {
                                films[countElem, 9] += obj.FirstChild.NodeValue + " ";
                            }
                            films[countElem, 10] = Html; //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        private void UpdateListFilmzor(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 13; i++)
                {
                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(" + i + ") > div.kino-title > a"))
                    {
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                AddFilmFilmzor(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmFilmzor(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait();
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string name = film.Find("#kino-page > div.kino-title-full > h1").Text().Replace("(фильм 2019) смотреть онлайн", "").Replace("(видео)", "").Trim();
                    string year = film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(1) > span.value").Text();
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name.Trim(); //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(2) > span.value")) //Жанр
                            {
                                films[countElem, 3] += obj.FirstChild.LastChild.NodeValue + " ";
                            }
                            foreach (IDomObject obj in film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix")) //Жанр
                            {
                                films[countElem, 4] += obj.LastChild.NodeValue + " ";
                            }
                            //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                            films[countElem, 5] = FindTrailer(name, year); //Трейлер
                            films[countElem, 6] = FindAgeRating(name, year); ; //Возр.огр
                            films[countElem, 7] = @"https://filmzor.net" + film.Find("#kino-page > div.fcols.clearfix > div > img").Attr("src"); //Картинка
                            films[countElem, 8] = ReData(film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(7) > span.value").Text()); //Дата выхода
                            if ((films[countElem, 8] == "") || (films[countElem, 8] == "-"))
                            {
                                films[countElem, 8] = ReData(FindData(name, year));
                            }
                            //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(3) > span.value")) //Страна
                            {
                                films[countElem, 9] += obj.FirstChild.NodeValue + " ";
                            }
                            films[countElem, 10] = Html; //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        public void UpdateListIvi(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 12; i++)
                {
                    foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > div > div > section > div > ul > li:nth-child(" + i + ") > a"))
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                }
                AddFilmIvi(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmIvi(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait();
                string Html = @"https://www.ivi.ru" + ListFilm[i] + "/description";
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string name = film.Find("body > div.page-wrapper > div > section > div.title-block > h1").Text().Replace("Фильм ", "").Trim();
                    string year = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > a:nth-child(1)").Text();
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name.Trim(); //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span:nth-child(3) > a")) //Жанр
                            {
                                films[countElem, 3] += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                            films[countElem, 4] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.description").Text().Trim();
                            //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                            films[countElem, 5] = Html.Replace("/description", "") + "/trailers#play"; //Трейлер
                            films[countElem, 6] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span.separate > span:nth-child(3)").Text(); //Возр.огр
                            films[countElem, 7] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > aside > figure > a > img").Attr("src"); //Картинка
                            films[countElem, 8] = ReData(film.Find("body > div.page-wrapper > div > div > section:nth-child(3) > aside > dl > dd").Text()); //Дата выхода
                            if (films[countElem, 8] == "")
                            {
                                films[countElem, 8] = ReData(FindData(name, year));
                            }
                            //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > a:nth-child(2)")) //Страна
                            {
                                films[countElem, 9] += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                            films[countElem, 10] = FindLinkFilm(name, year); //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        private void UpdateListKinokrad(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 25; i++)
                {
                    foreach (IDomObject obj in film.Find("#block-ovg-content > div > div > div.movie-grid.clearfix.browse.fullwidth.view-content > div:nth-child(" + i + ") > a"))
                    {
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                AddFilmKinokrad(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmKinokrad(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait();
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string[] nameFr = film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > h1").Text().Split('(').ToArray();

                    string name = nameFr[0];
                    nameFr = nameFr[1].Split(')');
                    string year = nameFr[0];
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name; //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > table > tbody > tr.field-genres > td")) //Жанр
                            {
                                films[countElem, 3] += obj.FirstChild.FirstChild.FirstChild.NodeValue.Trim() + " ";
                            }
                            films[countElem, 4] = film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.description.mdl-cell.mdl-cell--12-col > div > p").Text().Trim(); //Описание
                                                                                                                                                                                                                             //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                            films[countElem, 5] = Html; //Трейлер
                            films[countElem, 6] = FindAgeRating(name, year); ; //Возр.огр
                            films[countElem, 7] = @"https://kino50.top/" + film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.cover.mdl-cell.mdl-cell--4-col-desktop.mdl-cell--3-col-tablet.mdl-cell--3-col-phone > div > img").Attr("src"); //Картинка
                            films[countElem, 8] = ReData(FindData(name, year)); //Дата выхода
                                                                                //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > table > tbody > tr.field-country > td > span > a")) //Страна
                            {
                                films[countElem, 9] += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                            films[countElem, 10] = FindLinkFilm(name, year); //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        private void UpdateListHDKinozor(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 300; i++)
                {
                    foreach (IDomObject obj in film.Find("#dle-content > div > div > ul > li:nth-child(" + i + ") > a"))
                    {
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                AddFilmHDKinozor(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmHDKinozor(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait();
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string[] nameFr = film.Find("#dle-content > h1").Text().Split('(').ToArray();
                    string name = nameFr[0];
                    string year = "0000";
                    try
                    {
                        nameFr = nameFr[1].Split(')');
                        year = nameFr[0];
                    }
                    catch
                    {
                        //#dle-content > div:nth-child(5) > dl > span.dnina > span > a
                        year = film.Find("#dle-content > div:nth-child(5) > dl > span.dnina > span > a").Text();
                    }
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name; //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(12) > span > span")) //Жанр
                            { //#dle-content > div:nth-child(5) > dl > dd:nth-child(8) > span > span:nth-child(1) > a > span
                              //#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > span:nth-child(1) > a > span
                                films[countElem, 3] += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                            }
                            if (films[countElem, 3] == null)
                            {
                                try
                                {
                                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(8) > span > span")) //Жанр
                                    { //1
                                        films[countElem, 3] += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                                    }
                                }
                                catch
                                {
                                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(10) > span > span")) //Жанр
                                    { //1
                                        films[countElem, 3] += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                                    }
                                }
                                if (films[countElem, 3] == null)
                                {
                                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > span:nth-child(1) > a")) //Жанр
                                    { //1
                                        films[countElem, 3] += obj.FirstChild.NodeValue.Trim() + " ";
                                    }
                                }
                            }
                            foreach (IDomObject obj in film.Find("#dle-content > div.kikos > div")) //Описание
                            {
                                films[countElem, 4] = obj.FirstChild.NextElementSibling.PreviousSibling.NodeValue.Trim() + " ";
                            }
                            //       films[countElem, 4] = film.Find("#dle-content > div.kikos > div > span.masha_index.masha_index44").Text().Trim(); //Описание
                            //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                            films[countElem, 5] = FindTrailer(name, year); //Трейлер
                            films[countElem, 6] = FindAgeRating(name, year); ; //Возр.огр
                                                                               //       films[countElem, 7] = @"https://hdkinozor.ru" + film.Find("#dle-content > div:nth-child(5) > div > div.cmokka > img").Attr("src"); //Картинка
                            foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > div > div.cmokka > img")) //Картинка
                            {
                                films[countElem, 7] = @"https://hdkinozor.ru" + obj.GetAttribute("data-cfsrc");
                            }
                            films[countElem, 8] = ReData(FindData(name, year)); //Дата выхода
                                                                                //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(8) > span > a")) //Страна
                            {
                                films[countElem, 9] += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                            if (films[countElem, 9] == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > a")) //Страна
                                { //#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > a
                                    films[countElem, 9] += obj.FirstChild.NodeValue.Trim() + " ";
                                }
                                if (films[countElem, 9] == null)
                                { //#dle-content > div:nth-child(5) > dl > dd:nth-child(4) > span > a > span
                                    foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(4) > span > a")) //Страна
                                    { //#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > a
                                        films[countElem, 9] += obj.FirstChild.NodeValue.Trim() + " ";
                                    }
                                }
                            }
                            films[countElem, 10] = Html; //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        private void UpdateListLostfilm(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 35; i++)
                {
                    foreach (IDomObject obj in film.Find("#serials_list > div:nth-child(" + i + ") > a"))
                    {
                        if (obj.GetAttribute("href") != null)
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                AddFilmLostfilm(ListFilm.Distinct().ToList());
            }
        }

        private void AddFilmLostfilm(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(500).Wait();
                string Html = @"https://www.lostfilm.tv" + ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string name = film.Find("#left-pane > div:nth-child(1) > h1 > div.title-ru").Text().Trim();
                    string[] yearMas = film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(1)").Text().Split(' ').ToArray();
                    string year = yearMas[2];
                    if (name != "")
                    {
                        if (!DoubleFilm(name, year))
                        {
                            films[countElem, 0] = Html; //ссылка
                            films[countElem, 1] = name; //Название
                            films[countElem, 2] = year; //Год
                                                        //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                            foreach (IDomObject obj in film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.right-box > a:nth-child(1)")) //Жанр
                            {
                                //            if (!(obj.FirstChild.NodeValue.Trim()).Equals("По сериалу"))
                                //             {
                                films[countElem, 3] += obj.FirstChild.NodeValue.Trim() + " ";
                                //             }
                            }
                            foreach (IDomObject obj in film.Find("#left-pane > div.text-block.description > div.body > div")) //Описание
                            {
                                try
                                {
                                    films[countElem, 4] = obj.FirstChild.NextElementSibling.PreviousSibling.NodeValue.Trim() + "... (полный текст читай на сайте-источнике)";

                                }
                                catch
                                {
                                    films[countElem, 4] = "Прости, описание отсутствует. Читай на сайте-источнике :)";
                                }
                            }
                            //       films[countElem, 4] = film.Find("#dle-content > div.kikos > div > span.masha_index.masha_index44").Text().Trim(); //Описание
                            //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                            films[countElem, 5] = Html + "/video"; //Трейлер
                            films[countElem, 6] = FindAgeRating(name, year); ; //Возр.огр
                                                                               //       films[countElem, 7] = @"https://hdkinozor.ru" + film.Find("#dle-content > div:nth-child(5) > div > div.cmokka > img").Attr("src"); //Картинка

                            films[countElem, 7] = @"https://e-torrent.ru/uploads/posts/2016-11/1479684493_garo_svyaschennoe_plamya_2016.jpg"; //Картинка
                            films[countElem, 8] = ReData(film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(1)").Text()); //Дата выхода
                                                                                                                                                                //          films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                            foreach (IDomObject obj in film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(3)")) //Страна
                            {
                                films[countElem, 9] += obj.FirstChild.ParentNode.NextSibling.NodeValue.Replace("(", "").Replace(")", "").Trim() + " ";
                            }
                            films[countElem, 10] = Html; //Сам фильм
                            countElem++;
                        }
                    }
                }
            }
        }

        public void addBd()
        {
            DB db = new DB();
            for (int i = 0; i < countElem; i++)
            {
                db.AddMovie(films[i, 1],
                    Convert.ToInt32(films[i, 2]),
                    films[i, 8],
                    films[i, 9],
                    films[i, 3],
                    films[i, 6],
                    films[i, 4],
                    films[i, 7],
                    films[i, 5],
                    films[i, 0],
                    films[i, 10],
                    false,
                    false);
            }
        }

        private string ReData(string data)
        {
            // дд месяц год
            string[] dataMas = data.Split(' ');
            string mounth = "";
            if (dataMas.Count() == 1)
            {
                return data;
            }
            else
            {
                switch (dataMas[1].Trim().ToLower())
                {
                    case "января": mounth = "01"; break;
                    case "февраля": mounth = "02"; break;
                    case "марта": mounth = "03"; break;
                    case "апреля": mounth = "04"; break;
                    case "мая": mounth = "05"; break;
                    case "июня": mounth = "06"; break;
                    case "июля": mounth = "07"; break;
                    case "августа": mounth = "08"; break;
                    case "сентября": mounth = "09"; break;
                    case "октября": mounth = "10"; break;
                    case "ноября": mounth = "11"; break;
                    case "декабря": mounth = "12"; break;
                }
                if (dataMas[0].Trim().Length == 1)
                {
                    dataMas[0] = "0" + dataMas[0];
                }
            }
            return dataMas[2].Trim() + "-" + mounth + "-" + dataMas[0].Trim();
        }
    }
}//гггг-мм-дд
