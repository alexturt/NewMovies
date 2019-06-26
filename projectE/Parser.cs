using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectE
{
    class Parser
    {
        string Html = @"https://www.kinopoisk.ru/top/lists/222/";
        public void UpdateList()
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
            AddFilm(ListFilm.Distinct().ToList());
        }

        private void AddFilm(List<string> ListFilm)
        {
            string[,] films = new string[25, 10];
            for (int i = 0; i < ListFilm.Count; i++)
            {
                string Html = @"https://www.kinopoisk.ru" + ListFilm[i];
                CQ film = CQ.CreateFromUrl(Html);
                films[i, 0] = Html; //ссылка
                string[] idFilm = Html.Split('/').ToArray();
                films[i, 1] = film.Find("#headerFilm > h1 > span").Text().Trim(); //Название
                films[i, 2] = film.Find("#infoTable > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > a").Text(); //Год
                films[i, 3] = film.Find("#infoTable > table > tbody > tr:nth-child(11) > td:nth-child(2) > span > a").Text(); //Жанр
                films[i, 4] = film.Find("#syn > tbody > tr:nth-child(1) > td > table > tbody > tr:nth-child(1) > td > span > div").Text(); //Описание
                films[i, 5] = FindTrailer(films[i, 1], films[i, 2]); //Трейлер
                foreach (IDomObject obj in film.Find("body > div.discovery-trailers-overlay > div > iframe"))
                {
                    films[i, 5] = obj.GetAttribute("src");
                }
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
            }
        }

        private string FindTrailer(string name, string year)
        {
            //https://www.youtube.com/results?search_query=%D0%B1%D0%BE%D0%BB%D1%8C+%D0%B8+%D1%81%D0%BB%D0%B0%D0%B2%D0%B0+%D1%82%D1%80%D0%B5%D0%B9%D0%BB%D0%B5%D1%80
            string[] masName = name.Split(' ').ToArray();
            string fullName = "";
            string linkFilm = "";
            for (int i=0; i<masName.Count(); i++)
            {
                fullName += masName[i] + "+";
            }
            fullName += year;
            string link = @"https://www.youtube.com/results?search_query=" + fullName;
            CQ film = CQ.CreateFromUrl(link);
            return  linkFilm = film.Find("#video-title").First().Attr("href");
        }
    }
}
