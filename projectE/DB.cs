﻿using System;
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
            //try
            //{
                cmd = new SQLiteCommand(conn);
                //cmd.CommandText = @"INSERT INTO movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched) VALUES (@name,@year,@date,@country,@genres,@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched)";
                cmd.CommandText = @"INSERT INTO movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched) " +
                    "SELECT @name,@year,@date,@country,@genres,@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched " +
                    "WHERE NOT EXISTS(SELECT 1 FROM movies WHERE name=@name AND year=@year)";
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
      //      }
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Ошибка добавления в БД" + ex.Message + ". Date: " + date + ". Site: " + urlinfo);
            //}
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
            dataAdapter.Dispose();
            return dt;
        }

        public void SetSettings(string setting, bool check, bool import = false)
        {
            if (conn == null)
                connect();
            if (import)
            {
                ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE \"index\"=" + setting);
            }
            else
            {
                ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE setting='" + setting + "'");
            }
            
        }
        // (НЕ УДАЛЯЙТЕ)
        // <-- Settings get/set methods


            
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetRecommends()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies ORDER BY date ASC LIMIT 25", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetMovies(int limit, int offset, bool restricted = false)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            if (restricted)
            {
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE NOT \"agerating\"=\"18+\" ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
                dataAdapter.Fill(dt);
                dataAdapter.Dispose();
                return dt;
            }
            else
            {
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
                dataAdapter.Fill(dt);
                dataAdapter.Dispose();
                return dt;
            }
        }
        //выгрузка фильма по id
        public DataTable GetMovie(int index)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE id=" + index, conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка кол-ва фильмов
        public int GetMoviesCount()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка кол-ва фильмов
        public int GetMoviesTodayCount()
        {
            string date = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies WHERE date='" + date + "' ", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка всго избранного, сортировка по дате(сначала свежие)
        public DataTable GetFavorites(int limit, int offset)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE favorite=true ORDER BY date DESC LIMIT " + limit+" OFFSET "+offset, conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка кол-ва избранного, сортировка по дате(сначала свежие)
        public int GetFavoritesCount()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies WHERE favorite=true ORDER BY date DESC", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка сегодняшних фильмов
        public DataTable GetMoviesToday()
        {
            string date = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date='" + date+"' ", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
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
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' ORDER BY date DESC", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
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
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies WHERE date>'" + date + "' AND favorite=false", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
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
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' ORDER BY date DESC", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
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
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies WHERE date>'" + date + "' AND favorite=false  ORDER BY date DESC", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка всго просмотренного, сортировка по дате(сначала свежие)
        public DataTable GetWatched()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE watched=true ORDER BY date DESC", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //устанавливаем/снимаем избранное
        public void SetFavorite(int index, bool favorite)
        {
            if (conn == null)
                connect();
            ExecuteQuery("UPDATE movies SET favorite=" + favorite + " WHERE id=" + index);
        }
        //устанавливаем/снимаем просмотренное
        public void SetWatched(int index, bool watched)
        {
            if (conn == null)
                connect();
            ExecuteQuery("UPDATE movies SET watched=" + watched + " WHERE id=" + index);
        }
        //для удаления удаленных записей из файла БД
        public void Vacuum()
        {
            if (conn == null)
                connect();
            ExecuteQuery("VACUUM;");
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
