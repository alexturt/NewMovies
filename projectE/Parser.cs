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
            UpdateListKinopoisk();
            UpdateListNetflix();
        }

        static string LoadPage(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                if (receiveStream != null)
                {
                    StreamReader readStream;
                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    result = readStream.ReadToEnd();
                    readStream.Close();
                }
                response.Close();
            }
            return result;
        }

        int CountUpdate = 3;

        private bool TimerUpd(TimeSpan dt)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (((now.Subtract(dt).Hours * 60 + (now.Subtract(dt).Minutes)) > 15) || (CountUpdate == 0))
            {
                return false;
            }
            else
            {
                if (((now.Subtract(dt).Hours * 60 + (now.Subtract(dt).Minutes)) > 15))
                {
                    CountUpdate = 3;
                }
                return true;
            }

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
                if (films[i, 1].Contains(name) && films[i, 2].Equals(year))
                {
                    flag = true; //да, это дубль
                }
                else
                {
                    flag = false; //еще нет такого фильма
                }
            }
            return flag;
        }

        public void UpdateListKinopoisk()
        {
            string Html = @"https://www.kinopoisk.ru/top/lists/222/";
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
                    //            film.Remove();
                }
                catch
                { }
            }
            AddFilmKinopoisk(ListFilm.Distinct().ToList());
        }

        private void AddFilmKinopoisk(List<string> ListFilm)
        {
            if (TimerUpd(DateTime.Now.TimeOfDay))
            {
                CountUpdate--;

                for (int i = 0; i < ListFilm.Count; i++)
                {
                    Task.Delay(13000).Start();
                    string Html = @"https://www.kinopoisk.ru" + ListFilm[i];
                    WebClient client = new WebClient();
                    client.DownloadFile(Html, "film.txt");
                    CQ film = CQ.CreateFromFile("film.txt");
                    if ((film.Find("#headerFilm > h1 > span").Text() != null))
                    {
                        films[i, 0] = Html; //ссылка
                        string[] idFilm = Html.Split('/').ToArray();
                        films[i, 1] = film.Find("#headerFilm > h1 > span").Text().Trim(); //Название
                        films[i, 2] = film.Find("#infoTable > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > a").Text(); //Год
                        films[i, 3] = film.Find("#infoTable > table > tbody > tr:nth-child(11) > td:nth-child(2) > span > a").Text(); //Жанр
                        films[i, 4] = film.Find("#syn > tbody > tr:nth-child(1) > td > table > tbody > tr:nth-child(1) > td > span > div").Text(); //Описание
                        films[i, 5] = FindTrailer(films[i, 1], films[i, 2]); //Трейлер

                        if (film.Find("#infoTable > table > tbody > tr.ratePopup > td:nth-child(2) > span").Text().Contains("зрителям, достигшим 18 лет"))
                        {
                            films[i, 6] = "18+"; //Возр.огр
                        }
                        else if (film.Find("#infoTable > table > tbody > tr.ratePopup > td:nth-child(2) > span").Text().Contains("зрителям, достигшим 6 лет"))
                        {
                            films[i, 6] = "6+"; //Возр.огр
                        }
                        films[i, 7] = film.Find("#photoBlock > div.film-img-box.feature_film_img_box.feature_film_img_box_" + idFilm[4] + " > a.popupBigImage > img").Attr("src"); //Картинка
                        films[i, 8] = film.Find("#div_rus_prem_td2 > div > span > a:nth-child(1)").Text(); //Дата выхода
                        films[i, 9] = film.Find("#infoTable > table > tbody > tr:nth-child(2) > td:nth-child(2) > div > a").Text(); //Страна
                        films[i, 10] = film.Find("#infoTable > table > tbody > tr:nth-child(2) > td:nth-child(2) > div > a").Text(); //Сам фильм
                        countElem++;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка загрузки кинопоиска, попробуйте позже.");
                    }
                }
            }
        }

        public void UpdateListNetflix()
        {
            string Html = @"http://netflix.kinoyou.com/god/2019/";
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
            if (TimerUpd(DateTime.Now.TimeOfDay))
            {
                for (int i = 0; i < ListFilm.Count; i++)
                {
                    Task.Delay(13000).Start();
                    string Html = ListFilm[i];
                    WebClient client = new WebClient();
                    client.DownloadFile(Html, "film.txt");
                    CQ film = CQ.CreateFromFile("film.txt");
                    string name = film.Find("#dle-content > div > h2").Text().Replace("смотреть онлайн", "");
                    string year = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(3) > div > div.finfo-text > a").Text();
                    if ((name != null) || (DoubleFilm(name, year)))
                    {
                        films[i+countElem, 0] = Html; //ссылка
                        films[countElem, 1] = name; //Название
                        films[countElem, 2] = year; //Год
                        films[countElem, 3] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(5) > div > div.finfo-text > a").Text(); //Жанр
                        films[countElem, 4] = film.Find("#dle-content > div > div.fstory-content.margin-b40.block-p > h4").Text(); //Описание
                        films[countElem, 5] = FindTrailer(name, year); //Трейлер
                        films[countElem, 6] = "?"; //Возр.огр
                        films[countElem, 7] = film.Find("#fstory-film > div > div.col-sm-4.col-xs-12.fstory-poster-in > div.fstory-poster > img").Attr("src"); //Картинка
                        films[countElem, 8] = ""; //Дата выхода
                        films[countElem, 9] = film.Find("#fstory-film > div > div.col-sm-8.col-xs-12 > div:nth-child(4) > div > div.finfo-text").Text(); //Страна
                        films[countElem, 10] = FindLinkFilm(name, year); //Сам фильм
                        countElem++;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка загрузки кинопоиска, попробуйте позже.");
                    }
                }
            }
        }
    }
}
