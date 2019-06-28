using System;
using System.Data;
using System.Data.SQLite;
using System.Net;

namespace projectE
{
    public class DB
    {

        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private string path = "Data Source="+ Environment.CurrentDirectory + "\\newMovies.db;New=False;Version=3";
        private WebClient wb = new WebClient();

        public DB()
        {
            connect();
        }

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
        
        
        // добавление фильма
        public void AddMovie(string name, int year, string date,
            string country, string genres, string agerating,
            string description, string poster, string urltrailer,
            string urlinfo, string urlwatch, bool favorite, bool watched)
        {
            byte[] bytes = null;
            try
            {
                bytes = wb.DownloadData(poster);
            }
            catch(Exception ex)
            {            }
            cmd = new SQLiteCommand(conn);
            //cmd.CommandText = @"INSERT INTO movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched) VALUES (@name,@year,@date,@country,@genres,@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched)";
            cmd.CommandText = @"insert into movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched) "+
                "select @name,@year,@date,@country,@genres,@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched "+
                "where not exists(SELECT 1 from movies where name=@name and year=@year)";
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
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetMovies()
        {
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка всго избранного, сортировка по дате(сначала свежие)
        public DataTable GetFavorites()
        {
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where favorite=true order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка всго просмотренного, сортировка по дате(сначала свежие)
        public DataTable GetWatched()
        {
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where watched=true order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //устанавливаем/снимаем избранное
        public void SetFavorite(int index, bool favorite)
        {
            ExecuteQuery("update movies set favorite=" + favorite + " where id=" + index);
        }
        //устанавливаем/снимаем просмотренное
        public void SetWatched(int index, bool watched)
        {
            ExecuteQuery("update movies set watched=" + watched + " where id=" + index);
        }
        //для удаления удаленных записей из файла БД
        public void Vacuum()
        {
            ExecuteQuery("vacuum;");
            close();
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
