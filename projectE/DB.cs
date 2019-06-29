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
            if (conn == null)
                connect();
            byte[] bytes = null;
            try
            {
                bytes = wb.DownloadData(poster);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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
        // 



        // Settings get/set methods -->
        // (НЕ УДАЛЯЙТЕ)
        public DataTable GetSettings()
        {
            if (conn == null)
                connect();
            //System.Windows.MessageBox.Show("GetSettings()");
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT checked FROM settings", conn);
            dataAdapter.Fill(dt);
            return dt;
        }

        public void SetSettings(string setting, bool check, bool import = false)
        {
            if (conn == null)
                connect();
            if (import)
            {
                ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE \"index\"=" + setting);
                return;
            }
            ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE setting='" + setting + "'");
        }
        // (НЕ УДАЛЯЙТЕ)
        // <-- Settings get/set methods


            
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetRecommends()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies order by date asc limit 25", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetMovies(int limit, int offset)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies order by date desc limit "+ limit +" offset " + offset, conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка фильма по id
        public DataTable GetMovie(int index)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where id="+index, conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка кол-ва фильмов
        public int GetMoviesCount()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select count(*) from movies", conn);
            dataAdapter.Fill(dt);
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка кол-ва фильмов
        public int GetMoviesTodayCount()
        {
            string date = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select count(*) from movies where date='" + date + "' ", conn);
            dataAdapter.Fill(dt);
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка всго избранного, сортировка по дате(сначала свежие)
        public DataTable GetFavorites(int limit, int offset)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where favorite=true order by date desc limit " + limit+" offset "+offset, conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка кол-ва избранного, сортировка по дате(сначала свежие)
        public int GetFavoritesCount()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select count(*) from movies where favorite=true order by date desc", conn);
            dataAdapter.Fill(dt);
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка сегодняшних фильмов
        public DataTable GetMoviesToday()
        {
            string date = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where date='"+date+"' ", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка фильмов за неделю
        public DataTable GetMoviesWeek()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where date>'" + date + "'   order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка кол-ва фильмов за неделю
        public int GetMoviesWeekCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select count(*) from movies where date>'" + date + "' and favorite=false", conn);
            dataAdapter.Fill(dt);
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка фильмов за месяц
        public DataTable GetMoviesMonth()
        {
            DateTime dateTime = DateTime.Today.AddDays(-31);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where date>'" + date + "'   order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //выгрузка кол-ва фильмов за месяц
        public int GetMoviesMonthCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-31);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select count(*) from movies where date>'" + date + "' and favorite=false  order by date desc", conn);
            dataAdapter.Fill(dt);
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка всго просмотренного, сортировка по дате(сначала свежие)
        public DataTable GetWatched()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select * from movies where watched=true order by date desc", conn);
            dataAdapter.Fill(dt);
            return dt;
        }
        //устанавливаем/снимаем избранное
        public void SetFavorite(int index, bool favorite)
        {
            if (conn == null)
                connect();
            ExecuteQuery("update movies set favorite=" + favorite + " where id=" + index);
        }
        //устанавливаем/снимаем просмотренное
        public void SetWatched(int index, bool watched)
        {
            if (conn == null)
                connect();
            ExecuteQuery("update movies set watched=" + watched + " where id=" + index);
        }
        //для удаления удаленных записей из файла БД
        public void Vacuum()
        {
            if (conn == null)
                connect();
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
