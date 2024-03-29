﻿using CsQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace projectE
{
    public class Parser
    {
        DB db = new DB();
        string name, year, date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch;
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
            try
            {
                CQ film = CQ.CreateFromUrl(Html);
                return film;
            }
            catch
            {
                return null;
            }
        }

        public void UpdateList()
        {
            TimeSpan FullTime = TimeSpan.Zero;
            TimeSpan Start;
            TimeSpan Stop;

            CheckSettings();

            Console.WriteLine("---------------------------------------------------------------НАЧАЛО ПАРСИНГА 2019--------------------------------------");
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

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2019-t4");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[3])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Иви. Начало: " + Start);
                UpdateListIvi(@"https://www.ivi.ru/movies/2019");
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
            Console.WriteLine("---------------------------------------------------------------КОНЕЦ ПАРСИНГА 2019--------------------------------------");
            Console.WriteLine("---------------------------------------------------------------НАЧАЛО ПАРСИНГА 2018--------------------------------------");
            //---------------------------------------------------------------2018-------------------------
            if (IsChecked[7])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим ХДКинозор 2018. Начало: " + Start);
                UpdateListHDKinozor(@"https://hdkinozor.ru/top2018.html");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[2])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Нетфликс 2018. Начало: " + Start);
                UpdateListNetflix(@"http://netflix.kinoyou.com/god/2018/");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор 2018. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2018");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2018-t2");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2018-t4");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }
            if (IsChecked[3])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Иви. Начало: " + Start);
                UpdateListIvi(@"https://www.ivi.ru/movies/2018");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }
            Console.WriteLine("---------------------------------------------------------------КОНЕЦ ПАРСИНГА 2018--------------------------------------");
            Console.WriteLine("---------------------------------------------------------------НАЧАЛО ПАРСИНГА 2017--------------------------------------");
            //---------------------------------------------------------------2017-------------------------
            if (IsChecked[7])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим ХДКинозор 2017. Начало: " + Start);
                UpdateListHDKinozor(@"https://hdkinozor.ru/top2017.html");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }
            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2017-t2");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }

            if (IsChecked[6])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Фильмзор. Начало: " + Start);
                UpdateListFilmzor(@"https://filmzor.net/filter/fy2017-t4");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }
            if (IsChecked[3])
            {
                Start = DateTime.Now.TimeOfDay;
                Console.WriteLine("Парсим Иви. Начало: " + Start);
                UpdateListIvi(@"https://www.ivi.ru/movies/2017");
                Stop = DateTime.Now.TimeOfDay;
                FullTime += Stop.Subtract(Start);
                Console.WriteLine("Конец: " + Stop + ". Потрачено: " + Stop.Subtract(Start));
            }
            Console.WriteLine("---------------------------------------------------------------КОНЕЦ ПАРСИНГА 2017--------------------------------------");
            Console.WriteLine("Было выгружено : " + countElem + ". Всего потрачено: " + FullTime);
            
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
                                    foreach (IDomObject obj1 in film.Find(".movieInfoV2_infoData"))
                                    {
                                        try
                                        {
                                            string[] masDat = obj1.FirstChild.NodeValue.Trim().Split(' ');
                                            if ((masDat.Count() == 3))
                                            {
                                                datat = masDat[0] + " " + masDat[1] + " " + masDat[2];
                                            }
                                            else
                                            {
                                                datat = "";
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
            if (datat == "")
            {
                Html = @"https://www.ivi.ru/search/?q=" + nameP;
                film = IsConnectInternet(Html);
                if (film != null)
                {
                    for (int i = 1; i < 11; i++)
                    {
                        try
                        {
                            foreach (IDomObject obj in film.Find("#result-video > ul > li:nth-child(" + i + ") > a"))
                            {
                                if (obj.FirstChild.NextSibling.NextElementSibling.FirstChild.NextElementSibling.FirstChild.NodeValue.ToLower().Equals(name.ToLower().Trim()))
                                {
                                    film = IsConnectInternet(@"https://www.ivi.ru" + obj.GetAttribute("href").Replace("trailers#play", "") + "description");
                                    if (film != null)
                                    {
                                        datat = ReData(film.Find("body > div.page-wrapper > div > div > section:nth-child(3) > aside > dl > dd").Text());
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            data = datat.Split('-');
            if (data[0].Equals(year) && (TextIsDate(datat)))
            {
                return datat;
            }
            else
            {
                return "0001-01-01";
            }
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
                        else { break; }

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
                                    foreach (IDomObject obj1 in film.Find(".movieInfoV2_infoData"))
                                    {
                                        try
                                        {
                                            ageR = obj1.FirstChild.NodeValue.Trim();
                                            if ((ageR.Length == 3) || (ageR.Length == 2))
                                            {
                                                return ageR;
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
                Html = @"https://www.ivi.ru/search/?q=" + nameP;
                film = IsConnectInternet(Html);
                if (film != null)
                {
                    for (int i = 1; i < 11; i++)
                    {
                        try
                        {
                            foreach (IDomObject obj in film.Find("#result-video > ul > li:nth-child(" + i + ") > a"))
                            {
                                if (obj.FirstChild.NextSibling.NextElementSibling.FirstChild.NextElementSibling.FirstChild.NodeValue.ToLower().Equals(name.ToLower().Trim()))
                                {
                                    film = IsConnectInternet(@"https://www.ivi.ru" + obj.GetAttribute("href").Replace("trailers#play", "") + "description");
                                    if (film != null)
                                    {
                                        ageR = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span.separate > span").Text();
                                        if ((ageR.Length != 2) || (ageR.Length != 3))
                                        {
                                            ageR = film.Find("body > div.page-wrapper > div.content-main > section.cols-wrapper > article > div.properties > span.separate > span:nth-child(3)").Text();

                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            if ((ageR == "") || (ageR.Length != 2) || (ageR.Length != 3))
            {
                ageR = "0+";
            }
            return ageR;
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
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    name = film.Find("#dle-content > div > h2").Text().Replace("смотреть онлайн", "").Trim();
                    year = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(3) > div > div.finfo-text > a").Text();
                    if (name != "")
                    {

                        urlinfo = Html; //ссылка
                        genres = null;                //Жанр
                        foreach (IDomObject obj in film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a"))
                        {
                            genres += obj.FirstChild.NodeValue + " ";
                        }
                        description = null;
                        description = film.Find("#dle-content > div > div.fstory-content.margin-b40.block-p > h4").Text(); //Описание
                        urltrailer = FindTrailer(name, year); //Трейлер
                        agerating = FindAgeRating(name, year); //Возр.огр
                        poster = film.Find("#fstory-film > div > div.col-sm-4.col-xs-12.fstory-poster-in > div.fstory-poster > img").Attr("src"); //Картинка
                        date = ReData(FindData(name, year)); //Дата выхода
                        country = null;                              //Страна
                        foreach (IDomObject obj in film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text"))
                        {
                            country += obj.FirstChild.NodeValue + " ";
                        }
                        urlwatch = Html; //Сам фильм
                        countElem++;
                        //           }
                    }
                }
                db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
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

                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    name = film.Find("#kino-page > div.kino-title-full > h1").Text().Replace("(фильм 2019) смотреть онлайн", "").Replace("(фильм 2018) смотреть онлайн", "").Replace("(фильм 2017) смотреть онлайн", "").Replace("сериал смотреть онлайн", "").Replace("все серии", "").Replace("(видео)", "").Trim();
                    year = film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(1) > span.value").Text();
                    if (name != "")
                    {

                        urlinfo = Html; //ссылка
                        genres = null;          //Жанр
                        foreach (IDomObject obj in film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(2) > span.value")) //Жанр
                        {
                            if (genres == null)
                            {
                                try
                                {
                                    genres += obj.FirstChild.LastChild.NodeValue + " ";
                                }
                                catch { }
                            }
                            if (genres == null)
                            {
                                foreach (IDomObject obj1 in film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(3) > span.value > span"))
                                {
                                    try
                                    {
                                        genres += obj1.FirstChild.LastChild.NodeValue + " ";
                                    }
                                    catch
                                    {
                                        genres += obj1.FirstChild.NodeValue + " ";
                                    }
                                }
                            }
                        }
                        if (genres == null)
                        {

                        }
                        description = null;
                        foreach (IDomObject obj in film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix")) //Жанр
                        {
                            description += obj.LastChild.NodeValue + " ";
                        }
                        urltrailer = FindTrailer(name, year); //Трейлер
                        agerating = FindAgeRating(name, year); //Возр.огр
                        poster = @"https://filmzor.net" + film.Find("#kino-page > div.fcols.clearfix > div > img").Attr("src"); //Картинка
                        date = ReData(film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(7) > span.value").Text()); //Дата выхода
                        if ((date == "") || (date == "-") || !TextIsDate(date))
                        {
                            date = ReData(FindData(name, year));
                        }

                        country = null; //Страна
                        foreach (IDomObject obj in film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(3) > span.value")) //Страна
                        {
                            if (obj.FirstChild.NodeValue != null)
                            {
                                country += obj.FirstChild.NodeValue + " ";
                            }
                            else
                            {
                                country = film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(4) > span.value").First().Text();
                            }
                        }
                        urlwatch = Html; //Сам фильм
                        countElem++;
                    }
                    //}
                }
                db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
            }
        }

        static bool TextIsDate(string text)
        {
            var dateFormat = "yyyy-mm-dd";
            DateTime scheduleDate;
            if (DateTime.TryParseExact(text, dateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out scheduleDate))
            {
                return true;
            }
            return false;
        }

        public void UpdateListIvi(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = IsConnectInternet(Html);
            if (film != null)
            {
                for (int i = 1; i < 30; i++)
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
                string Html = @"https://www.ivi.ru" + ListFilm[i] + "/description";
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    name = film.Find("body > div.page-wrapper > div > section > div.title-block > h1").Text().Replace("Фильм ", "").Trim();
                    year = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > a:nth-child(1)").Text();
                    if (year == "")
                    {
                        year = film.Find("body > div.page-wrapper > div > section > div.properties > a:nth-child(1)").Text();
                    }

                    if (name != "")
                    {

                        urlinfo = Html; //ссылка
                        genres = null;            //Жанр
                        foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span:nth-child(3) > a")) //Жанр
                        {
                            genres += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        if (genres == null)
                        {
                            foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > section > div.properties > span:nth-child(3) > a")) //Жанр
                            {
                                genres += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                        }
                        description = null;
                        description = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.description").Text().Replace("Описание фильма", "").Trim();
                        //Описание
                        urltrailer = Html.Replace("/description", "") + "/trailers#play"; //Трейлер
                        agerating = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span.separate > span:nth-child(3)").Text(); //Возр.огр
                        if (agerating == "")
                        {
                            agerating = film.Find("body > div.page-wrapper > div > section > div.properties > span.separate > span:nth-child(3)").Text();
                        }
                        poster = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > aside > figure > a > img").Attr("src"); //Картинка
                        date = ReData(film.Find("body > div.page-wrapper > div > div > section:nth-child(3) > aside > dl > dd").Text()); //Дата выхода
                        if (date == "")
                        {
                            foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section.cols-wrapper.cols-wrapper_secondary > aside > dl > dd:nth-child(4)")) //Страна
                            {
                                date = obj.FirstChild.NodeValue.Trim() + " ";
                                break;
                            }
                            date = ReData(date);
                        }
                        if ((date == "") || (date == "-") || !TextIsDate(date))
                        {
                            date = ReData(FindData(name, year));
                        }
                        country = null; //Страна
                        foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > a:nth-child(2)")) //Страна
                        {
                            country += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        if (country == null)
                        {
                            country = film.Find("body > div.page-wrapper > div > section > div.properties > a:nth-child(2)").Text().Trim();
                        }
                        urlwatch = FindLinkFilm(name, year); //Сам фильм
                        countElem++;
                    }

                    db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
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
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string[] nameFr = film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > h1").Text().Split('(').ToArray();

                    name = nameFr[0];
                    nameFr = nameFr[1].Split(')');
                    year = nameFr[0];
                    if (name != "")
                    {

                        urlinfo = Html; //ссылка
                        genres = null;      //Жанр
                        foreach (IDomObject obj in film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > table > tbody > tr.field-genres > td")) //Жанр
                        {
                            genres += obj.FirstChild.FirstChild.FirstChild.NodeValue.Trim() + " ";
                        }
                        description = null;
                        description = film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.description.mdl-cell.mdl-cell--12-col > div > p").Text().Trim(); //Описание
                                                                              
                        urltrailer = Html; //Трейлер
                        agerating = FindAgeRating(name, year); ; //Возр.огр
                        poster = @"https://kino50.top/" + film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.cover.mdl-cell.mdl-cell--4-col-desktop.mdl-cell--3-col-tablet.mdl-cell--3-col-phone > div > img").Attr("src"); //Картинка
                        date = ReData(film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > table > tbody > tr.field-date > td > span > time").Text());
                        if ((date == "") || (date == "-") || !TextIsDate(date))
                        {
                            date = ReData(FindData(name, year)); //Дата выхода
                        }
                        country = null;                              //Страна
                        foreach (IDomObject obj in film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.info.mdl-cell.mdl-cell--8-col-desktop.mdl-cell--5-col-tablet.mdl-cell--4-col-phone > table > tbody > tr.field-country > td > span > a")) //Страна
                        {
                            country += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        urlwatch = FindLinkFilm(name, year); //Сам фильм
                        countElem++;
                    }
                    db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
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
                string Html = ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    string[] nameFr = film.Find("#dle-content > h1").Text().Split('(').ToArray();
                    name = nameFr[0];
                    year = "0000";
                    try
                    {
                        nameFr = nameFr[1].Split(')');
                        year = nameFr[0];
                    }
                    catch
                    {

                        year = film.Find("#dle-content > div:nth-child(5) > dl > span.dnina > span > a").Text();
                    }
                    if (name != "")
                    {

                        urlinfo = Html; //ссылка
                        genres = null;
                        foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(12) > span > span")) //Жанр
                        {
                            genres += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                        }
                        if (genres == null)
                        {
                            if (genres == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(8) > span > span")) //Жанр
                                {
                                    try
                                    {
                                        genres += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                                    }
                                    catch { }
                                }
                            }
                            if (genres == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(10) > span > span")) //Жанр
                                {
                                    genres += obj.FirstChild.FirstChild.NodeValue.Trim() + " ";
                                }
                            }
                            if (genres == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > span:nth-child(1) > a")) //Жанр
                                {
                                    genres += obj.FirstChild.NodeValue.Trim() + " ";
                                }
                            }
                            if (genres == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd")) //Жанр
                                { //1
                                    try
                                    {
                                        genres += obj.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.FirstElementChild.FirstElementChild.FirstElementChild.FirstChild.NodeValue.Trim() + " ";
                                        break;
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            if (genres == null)
                            { genres = "-"; }

                        }
                        description = null;
                        foreach (IDomObject obj in film.Find("#dle-content > div.kikos > div")) //Описание
                        {
                            try
                            {
                                description = obj.FirstChild.NextElementSibling.PreviousSibling.NodeValue.Trim() + " ";
                            }
                            catch
                            {
                                description = obj.FirstChild.NextSibling.NodeValue.Trim() + " ";
                            }
                        }
                        urltrailer = FindTrailer(name, year); //Трейлер
                        agerating = FindAgeRating(name, year); //Возр.огр
                        poster = null;                                    //Картинка
                        foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > div > div.cmokka > img")) //Картинка
                        {
                            poster = @"https://hdkinozor.ru" + obj.GetAttribute("src");
                        }
                        date = ReData(FindData(name, year)); //Дата выхода

                        country = null;                                    //Страна
                        foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(8) > span > a")) //Страна
                        {
                            country += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        if (country == null)
                        {
                            foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(6) > span > a")) //Страна
                            {
                                country += obj.FirstChild.NodeValue.Trim() + " ";
                            }
                            if (country == null)
                            {
                                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(5) > dl > dd:nth-child(4) > span > a")) //Страна
                                {
                                    country += obj.FirstChild.NodeValue.Trim() + " ";
                                }
                            }
                        }
                        urlwatch = Html; //Сам фильм
                        countElem++;
                    }
                }
                db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
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
                string Html = @"https://www.lostfilm.tv" + ListFilm[i];
                CQ film = IsConnectInternet(Html);
                if (film != null)
                {
                    name = film.Find("#left-pane > div:nth-child(1) > h1 > div.title-ru").Text().Trim();
                    string[] yearMas = film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(1)").Text().Split(' ').ToArray();
                    year = yearMas[2];
                    if (name != "")
                    {

                        genres = null;
                        urlinfo = Html; //ссылка
                                        //Жанр
                        foreach (IDomObject obj in film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.right-box > a:nth-child(1)")) //Жанр
                        {
                            genres += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        description = null;
                        foreach (IDomObject obj in film.Find("#left-pane > div.text-block.description > div.body > div")) //Описание
                        {
                            try
                            {
                                description = obj.FirstChild.NextElementSibling.PreviousSibling.NodeValue.Trim() + "... (полный текст читай на сайте-источнике)";

                            }
                            catch
                            {
                                description = "Прости, описание отсутствует. Читай на сайте-источнике :)";
                            }
                        }
                        urltrailer = Html + "/video"; //Трейлер
                        agerating = FindAgeRating(name, year); ; //Возр.огр
                        poster = @"https://e-torrent.ru/uploads/posts/2016-11/1479684493_garo_svyaschennoe_plamya_2016.jpg"; //Картинка
                        date = ReData(film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(1)").Text()); //Дата выхода
                        country = null;                                                                                                                    
                        foreach (IDomObject obj in film.Find("#left-pane > div:nth-child(4) > div.details-pane > div.left-box > a:nth-child(3)")) //Страна
                        {
                            country += obj.FirstChild.ParentNode.NextSibling.NodeValue.Replace("(", "").Replace(")", "").Trim() + " ";
                        }
                        urlwatch = Html; //Сам фильм
                        countElem++;
                    }
                }
                db.AddMovie(name, Convert.ToInt32(year), date, country, genres, agerating, description, poster, urltrailer, urlinfo, urlwatch, false, false, ReNowData(DateTime.Now));
            }
        }

        private string ReData(string data)
        {
            // дд месяц год
            string[] dataMas = data.Trim().Split(' ');
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
            try
            {
                return dataMas[2].Trim() + "-" + mounth + "-" + dataMas[0].Trim();
            }
            catch
            {
                return null;
            }
        }

        private string ReNowData(DateTime date)
        {
            string[] dataMas = date.ToString().Split(' ');
            dataMas = dataMas[0].Split('.');
            return dataMas[2] + "-" + dataMas[1] + "-" + dataMas[0];
        }
    }
}
