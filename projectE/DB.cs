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
            string urlinfo, string urlwatch, bool favorite, bool watched, string dateAdd)
        {
            if (conn == null)
                connect();
            byte[] bytes = null;
            try
            {
                bytes = wb.DownloadData(poster);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                name = name.Trim();
                cmd = new SQLiteCommand(conn);
                cmd.CommandText = @"INSERT INTO movies (name,year,date,country,genres,agerating,description,poster,URLtrailer,URLinfo,URLwatch,favorite,watched,dateAdd) " +
                    "SELECT @name,@year,@date,@country,LOWER(@genres),@agerating,@description,@poster,@URLtrailer,@URLinfo,@URLwatch,@favorite,@watched,@dateAdd " +
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
                cmd.Parameters.Add("@dateAdd", DbType.Date).Value = dateAdd;
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Добавили в базу " + name);
                }
                catch
                {
                    Console.WriteLine("Мы не сохранили " + name);
                }
            }
            catch
            {
                Console.WriteLine("Мы не сохранили " + name);
            }
        }
        // 




        // Settings get/set methods -->
        // (НЕ УДАЛЯЙТЕ)

        private bool showRestricted = true;

        public DataTable GetSettings()
        {
            if (conn == null)
                connect();
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
            if (setting == "age" || setting == "1") updateAgeRestriction(check);
            if (import)
            {
                ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE \"index\"=" + setting);
            }
            else
            {
                ExecuteQuery("UPDATE settings SET checked=" + check + " WHERE setting='" + setting + "'");
            }
            
        }
        
        public void updateAgeRestriction(bool _showRestricted)
        {
            showRestricted = _showRestricted;
        }

        // (НЕ УДАЛЯЙТЕ)
        // <-- Settings get/set methods





        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetRecommends()
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE agerating<>'18+' ORDER BY RANDOM() LIMIT 28", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies ORDER BY RANDOM()LIMIT 28", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        
        //выгрузка всех фильмов, сортировка по дате(сначала свежие)
        public DataTable GetMovies(int limit, int offset)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE NOT agerating='18+' ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
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
        //фильтрация по имени и жанру
        public DataTable GetMoviesByFilter(string name, string genre, string age, int year)
        {
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (year == 0)
                if (!showRestricted)
                {
                    dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE lower(genres) like lower('%" + genre + "%') AND lower(name) like lower('%" + name + "%') AND agerating like '%" + age + "' AND agerating<>'18+' ORDER BY date DESC limit 50", conn);
                }
                else
                {
                    dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE lower(genres) like lower('%" + genre + "%') AND lower(name) like lower('%" + name + "%') AND agerating like '%" + age + "' ORDER BY date DESC limit 50", conn);
                }
            else
                if (!showRestricted)
                {
                    dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE lower(genres) like lower('%" + genre + "%') AND lower(name) like lower('%" + name + "%') AND agerating like '%" + age + "' AND year=" + year + " AND agerating<>'18+' ORDER BY date DESC limit 50", conn);
                }
                else
                {
                    dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE lower(genres) like lower('%" + genre + "%') AND lower(name) like lower('%" + name + "%') AND agerating like '%" + age + "' AND year=" + year + " ORDER BY date DESC limit 50", conn);
                }
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
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
                dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies where agerating<>'18+'", conn);
            else
                dataAdapter = new SQLiteDataAdapter("SELECT COUNT(*) FROM movies", conn);
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
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE favorite=true AND agerating<>'18+' ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE favorite=true ORDER BY date DESC LIMIT " + limit + " OFFSET " + offset, conn);
            }
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
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE favorite=true AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE favorite=true ORDER BY date DESC ", conn);
            }
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
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date='" + date + "' AND agerating<>'18+' ", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date='" + date + "' ", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка фильмов за неделю
        public DataTable GetMoviesWeek()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' AND date<'" + today + "' AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' AND date<'" + today + "' ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка кол-ва фильмов за неделю
        public int GetMoviesWeekCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка кол-ва фильмов ДОБАВЛЕННЫХ В БД за неделю
        public int GetAddedMoviesWeekCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE dateAdd>'" + date + "' AND dateAdd<='" + today + "' AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE dateAdd>'" + date + "' AND dateAdd<='" + today + "' ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        //выгрузка фильмов за месяц
        public DataTable GetMoviesMonth()
        {
            DateTime dateTime = DateTime.Today.AddDays(-31);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' AND date<='" + today + "' AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT * FROM movies WHERE date>'" + date + "' AND date<='" + today + "' ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return dt;
        }
        //выгрузка кол-ва фильмов за месяц
        public int GetMoviesMonthCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-31);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
        }
        public int GetFavoritesMoviesWeekCount()
        {
            DateTime dateTime = DateTime.Today.AddDays(-7);
            string date = dateTime.Year.ToString() + '-' + (dateTime.Month < 10 ? "0" : "") + dateTime.Month.ToString() + '-' + (dateTime.Day < 10 ? "0" : "") + dateTime.Day.ToString() + " 00:00:00";
            string today = DateTime.Today.Year.ToString() + '-' + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString() + '-' + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString() + " 00:00:00";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter;
            if (!showRestricted)
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' AND favorite=true AND agerating<>'18+' ORDER BY date DESC", conn);
            }
            else
            {
                dataAdapter = new SQLiteDataAdapter("SELECT count(*) FROM movies WHERE date>'" + date + "' AND date<='" + today + "' AND favorite=true ORDER BY date DESC", conn);
            }
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            return int.Parse(dt.Rows[0][0].ToString());
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
        //устанавливаем/снимаем просмотренное
        public void SetPassword(string pass)
        {
            if (conn == null)
                connect();
            ExecuteQuery("UPDATE credentials SET password='" + pass + "' WHERE email='1'");
        }
        public string GetPassword()
        {
            string pass = "";
            if (conn == null)
                connect();
            DataTable dt = new DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("select password from credentials where email='1'", conn);
            dataAdapter.Fill(dt);
            dataAdapter.Dispose();
            if (dt.Rows.Count == 0)
            {
                createPassword();
                pass = "";
            }
            else
                pass = dt.Rows[0][0].ToString();
            return pass;
        }
        public void createPassword()
        {
            if (conn == null)
                connect();
            ExecuteQuery("insert into credentials values ('1','')");
            close();
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

//взято с http://www.cyberforum.ru/ado-net/thread1708878.html
namespace ASC.Data.SQLite
{

    /// <summary>
    /// Класс переопределяет функцию Lower() в SQLite, т.к. встроенная функция некорректно работает с символами > 128
    /// </summary>
    [SQLiteFunction(Name = "lower", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class LowerFunction : SQLiteFunction
    {

        /// <summary>
        /// Вызов скалярной функции Lower().
        /// </summary>
        /// <param name="args">Параметры функции</param>
        /// <returns>Строка в нижнем регистре</returns>
        public override object Invoke(object[] args)
        {
            try
            {
                if (args.Length == 0 || args[0] == null) return null;
                return ((string)args[0]).ToLower();
            }
            catch { return null; }
        }
    }

    /// <summary>
    /// Класс переопределяет функцию Upper() в SQLite, т.к. встроенная функция некорректно работает с символами > 128
    /// </summary>
    [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class UpperFunction : SQLiteFunction
    {

        /// <summary>
        /// Вызов скалярной функции Upper().
        /// </summary>
        /// <param name="args">Параметры функции</param>
        /// <returns>Строка в верхнем регистре</returns>
        public override object Invoke(object[] args)
        {
            if (args.Length == 0 || args[0] == null) return null;
            return ((string)args[0]).ToUpper();
        }
    }
}
