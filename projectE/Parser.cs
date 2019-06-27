using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlParserSharp;
using System.Net;
using System.IO;
using Leaf.xNet;

namespace projectE
{
    class Parser
    {
        string[,] films = new string[500, 11];
        int countElem = 0;

        public void UpdateList()
        {
            //      UpdateListKinopoisk(@"https://www.kinopoisk.ru/top/lists/222/");
            UpdateListNetflix(@"http://netflix.kinoyou.com/god/2019/");
            UpdateListFilmzor(@"https://filmzor.net/filter/fy2019-t2");
            UpdateListIvi(@"https://www.ivi.ru/movies/2019/page3");
            UpdateListKinokrad(@"https://kino50.top/filmy-online/browse/1/all/all/2019?sort_by=field_weight_value");
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

        private bool DoubleFilm(string name, string year)
        {
            bool flag = false;
            for (int i = 0; i < films.Length; i++)
            {
                try
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
                catch { return flag; }
            }
            return flag;
        }

        public void UpdateListKinopoisk(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = CQ.CreateFromUrl(Html);
            foreach (IDomObject obj in film.Find(".js-film-list-item a"))
            {
                try
                {
                    if (!(obj.GetAttribute("href").Contains("actor")))
                    {
                        if (obj.GetAttribute("href").Contains("film"))
                        {
                            ListFilm.Add(obj.GetAttribute("href"));
                        }
                    }
                }
                catch
                { }
            }
            AddFilmKinopoisk(ListFilm.Distinct().ToList());
        }

        private void AddFilmKinopoisk(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(2000).Wait();
                string Html = @"https://www.kinopoisk.ru" + ListFilm[i];
                WebClient client = new WebClient();
                client.DownloadFile(Html, "film.txt");
                CQ film = CQ.CreateFromFile("film.txt");
                string name = film.Find("#headerFilm > h1 > span").Text().TrimEnd();
                string year = film.Find("#infoTable > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > a").Text();
                if (name != "")
                {
                    if (DoubleFilm(name, year))
                    {
                        films[countElem, 0] = Html; //ссылка
                        string[] idFilm = Html.Split('/').ToArray();
                        films[countElem, 1] = name.Trim(); //Название
                        films[countElem, 2] = year; //Год
                                                    //       films[countElem, 3] = film.Find("#infoTable > table > tbody > tr:nth-child(11) > td:nth-child(2) > span > a").Text(); //Жанр
                        foreach (IDomObject obj in film.Find("#infoTable > table > tbody > tr:nth-child(11) > td:nth-child(2) > span > a"))
                        {
                            films[countElem, 3] += obj.FirstChild.NodeValue + " ";
                        }
                        films[countElem, 4] = film.Find("#syn > tbody > tr:nth-child(1) > td > table > tbody > tr:nth-child(1) > td > span > div").Text(); //Описание
                        films[countElem, 5] = FindTrailer(name, year); //Трейлер

                        if (film.Find("#infoTable > table > tbody > tr.ratePopup > td:nth-child(2) > span").Text().Contains("зрителям, достигшим 18 лет"))
                        {
                            films[countElem, 6] = "18+"; //Возр.огр
                        }
                        else if (film.Find("#infoTable > table > tbody > tr.ratePopup > td:nth-child(2) > span").Text().Contains("зрителям, достигшим 6 лет"))
                        {
                            films[countElem, 6] = "6+"; //Возр.огр
                        }
                        films[countElem, 7] = film.Find("#photoBlock > div.film-img-box.feature_film_img_box.feature_film_img_box_" + idFilm[4] + " > a.popupBigImage > img").Attr("src"); //Картинка
                        films[countElem, 8] = film.Find("#div_rus_prem_td2 > div > span > a:nth-child(1)").Text(); //Дата выхода
                                                                                                                   //          films[countElem, 9] = film.Find("#infoTable > table > tbody > tr:nth-child(2) > td:nth-child(2) > div > a").Text(); //Страна
                        foreach (IDomObject obj in film.Find("#infoTable > table > tbody > tr:nth-child(2) > td:nth-child(2) > div > a"))
                        {
                            films[countElem, 9] += obj.FirstChild.NodeValue + " ";
                        }
                        films[countElem, 10] = film.Find("#infoTable > table > tbody > tr:nth-child(2) > td:nth-child(2) > div > a").Text(); //Сам фильм
                        countElem++;
                    }
                }
            }
        }

        public void UpdateListNetflix(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = CQ.CreateFromUrl(Html);
            for (int i = 1; i < 13; i++)
            {
                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(" + i + ") > div > div.short-content > h4 > a"))
                {
                    try
                    {
                        ListFilm.Add(obj.GetAttribute("href"));
                    }
                    catch
                    { }
                }
            }
            AddFilmNetflix(ListFilm.Distinct().ToList());
        }

        private void AddFilmNetflix(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(1000).Wait(); ;
                string Html = ListFilm[i];
                WebClient client = new WebClient();
                client.DownloadFile(Html, "film.txt");
                CQ film = CQ.CreateFromFile("film.txt");
                string name = film.Find("#dle-content > div > h2").Text().Replace("смотреть онлайн", "");
                string year = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(3) > div > div.finfo-text > a").Text();
                if (name != "")
                {
                    if (!DoubleFilm(name, year))
                    {
                        films[countElem, 0] = Html; //ссылка
                        films[countElem, 1] = name; //Название
                        films[countElem, 2] = year; //Год
                                                    //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                        foreach (IDomObject obj in film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a"))
                        {
                            films[countElem, 3] += obj.FirstChild.NodeValue + " ";
                        }
                        films[countElem, 4] = film.Find("#dle-content > div > div.fstory-content.margin-b40.block-p > h4").Text(); //Описание
                        films[countElem, 5] = FindTrailer(name, year); //Трейлер
                        films[countElem, 6] = "?"; //Возр.огр
                        films[countElem, 7] = film.Find("#fstory-film > div > div.col-sm-4.col-xs-12.fstory-poster-in > div.fstory-poster > img").Attr("src"); //Картинка
                        films[countElem, 8] = ""; //Дата выхода
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

        public void UpdateListFilmzor(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = CQ.CreateFromUrl(Html);
            for (int i = 1; i < 13; i++)
            {
                foreach (IDomObject obj in film.Find("#dle-content > div:nth-child(" + i + ") > div.kino-title > a"))
                {
                    try
                    {
                        ListFilm.Add(obj.GetAttribute("href"));
                    }
                    catch
                    { }
                }
            }
            AddFilmFilmzor(ListFilm.Distinct().ToList());
        }

        private void AddFilmFilmzor(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(1000).Wait();
                string Html = ListFilm[i];
                WebClient client = new WebClient();
                client.DownloadFile(Html, "film.txt");
                CQ film = CQ.CreateFromFile("film.txt");
                string name = film.Find("#kino-page > div.kino-title-full > h1").Text().Replace("(фильм 2019) смотреть онлайн", "").Replace("(видео)", "").Trim();
                string year = film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(1) > span.value").Text();
                if (name != "")
                {
                    if (!DoubleFilm(name, year))
                    {
                        films[countElem, 0] = Html; //ссылка
                        films[countElem, 1] = name; //Название
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
                        films[countElem, 6] = "?"; //Возр.огр
                        films[countElem, 7] = @"https://filmzor.net" + film.Find("#kino-page > div.fcols.clearfix > div > img").Attr("src"); //Картинка
                        films[countElem, 8] = film.Find("#kino-page > div.fcols.clearfix > ul > li:nth-child(7) > span.value").Text(); //Дата выхода
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

        public void UpdateListIvi(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = CQ.CreateFromUrl(Html);
            for (int i = 1; i < 12; i++)
            {
                foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > div > div > section > div > ul > li:nth-child(" + i + ") > a"))
                {
                    try
                    {
                        ListFilm.Add(obj.GetAttribute("href"));
                    }
                    catch
                    { }
                }
            }
            AddFilmIvi(ListFilm.Distinct().ToList());
        }

        private void AddFilmIvi(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(1000).Wait();
                string Html = @"https://www.ivi.ru" + ListFilm[i] + "/description";
                WebClient client = new WebClient();
                client.DownloadFile(Html, "film.txt");
                CQ film = CQ.CreateFromFile("film.txt");
                string name = film.Find("body > div.page-wrapper > div > section > div.title-block > h1").Text().Replace("Фильм ", "").Trim();
                string year = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > a:nth-child(1)").Text();
                if (name != "")
                {
                    if (!DoubleFilm(name, year))
                    {
                        films[countElem, 0] = Html; //ссылка
                        films[countElem, 1] = name; //Название
                        films[countElem, 2] = year; //Год
                                                    //        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                        foreach (IDomObject obj in film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span:nth-child(3) > a")) //Жанр
                        {
                            films[countElem, 3] += obj.FirstChild.NodeValue.Trim() + " ";
                        }
                        films[countElem, 4] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.description").Text().Trim();
                        //       films[countElem, 4] = film.Find("#kino-page > div.kino-inner-full.mb-rem1.clearfix > div.kino-text > div.kino-desc.full-text.clearfix > div").Text(); //Описание
                        films[countElem, 5] = Html.Replace("/description", "") + "/trailers#play"; //Трейлер
                        films[countElem, 6] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > article > div.top-wrapper > div.properties > span.separate > span:nth-child(3)").Text(); ; //Возр.огр
                        films[countElem, 7] = film.Find("body > div.page-wrapper > div > div > section:nth-child(1) > aside > figure > a > img").Attr("src"); //Картинка
                        films[countElem, 8] = ""; //Дата выхода
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

        public void UpdateListKinokrad(string Html)
        {
            List<string> ListFilm = new List<string>();
            CQ film = CQ.CreateFromUrl(Html);
            for (int i = 1; i < 25; i++)
            {
                foreach (IDomObject obj in film.Find("#block-ovg-content > div > div > div.movie-grid.clearfix.browse.fullwidth.view-content > div:nth-child(" + i + ") > a"))
                {
                    try
                    {
                        ListFilm.Add(obj.GetAttribute("href"));
                    }
                    catch
                    { }
                }
            }
            AddFilmKinokrad(ListFilm.Distinct().ToList());
        }

        private void AddFilmKinokrad(List<string> ListFilm)
        {
            for (int i = 0; i < ListFilm.Count; i++)
            {
                Task.Delay(1000).Wait();
                string Html = ListFilm[i];
                WebClient client = new WebClient();
                client.DownloadFile(Html, "film.txt");
                CQ film = CQ.CreateFromFile("film.txt");
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
                        films[countElem, 6] = "0+" ; //Возр.огр
                        films[countElem, 7] = @"https://kino50.top/" + film.Find("#block-ovg-content > div > section.info-section.section--center.mdl-grid.mdl-shadow--2dp > div.cover.mdl-cell.mdl-cell--4-col-desktop.mdl-cell--3-col-tablet.mdl-cell--3-col-phone > div > img").Attr("src"); //Картинка
                        films[countElem, 8] = ""; //Дата выхода
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
}
