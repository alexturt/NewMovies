using System;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.Windows.Controls;
using System.Net;

namespace projectE
{
    public class DB
    {

        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private string path = "Data Source="+ Environment.CurrentDirectory + "\\newMovies.db;New=False;Version=3";
        private WebClient wb = new WebClient();

        public void connect()
        {
            // создаём объект для подключения к БД
            conn = new SQLiteConnection(path);
            // устанавливаем соединение с БД
            conn.Open();
        }

        public void connect(string Path)
        {
            path = Path;
            // создаём объект для подключения к БД
            conn = new SQLiteConnection(path);
            // устанавливаем соединение с БД
            conn.Open();
        }
        
        
        // добавление подразделения
        public void AddMovie(string name, int year, string date,
            string country, string genres, string agerating,
            string description, string poster, string urltrailer,
            string urlinfo, string urlwatch, bool favorite, bool watched)
        {
            byte[] bytes = wb.DownloadData(poster);
            cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"INSERT INTO movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched) VALUES (@name,@year,@date,@country,@genres,@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched)";
            cmd.Parameters.Add("@name", DbType.String).Value = name;
            cmd.Parameters.Add("@year", DbType.Int32).Value = year;
            cmd.Parameters.Add("@date", DbType.Date).Value = date;
            cmd.Parameters.Add("@country", DbType.String).Value = country;
            cmd.Parameters.Add("@genres", DbType.String).Value = genres;
            cmd.Parameters.Add("@agerating", DbType.String).Value = agerating;
            cmd.Parameters.Add("@description", DbType.String).Value = description;
            cmd.Parameters.Add("@poster", DbType.Binary).Value = bytes;
            cmd.Parameters.Add("@URLtrailer", DbType.String).Value = urltrailer;
            cmd.Parameters.Add("@URLinfo", DbType.String).Value = urlinfo;
            cmd.Parameters.Add("@URLwatch", DbType.String).Value = urlwatch;
            cmd.Parameters.Add("@favorite", DbType.Boolean).Value = favorite;
            cmd.Parameters.Add("@watched", DbType.Boolean).Value = watched;
            cmd.ExecuteNonQuery();
        }
        
        private void ExecuteQuery(string txtQuery)
        {
            connect();
            cmd = conn.CreateCommand();
            cmd.CommandText = txtQuery;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public void close()
        {
            conn.Close();
        }
    }
}
