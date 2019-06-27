using System;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.Windows.Controls;

namespace WpfApp2
{
    public class DB
    {

        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private string path = @"Data Source=C:\Users\xumuk\Source\Repos\test\WpfApp2\bin\Debug\newMovies.db;New=False;Version=3";

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
        // получение подразделения
        public DataTable GetDepartments()
        {
            //connect();
            string scmd = "Select poster from movies";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            //close();
            return dt;
        }
        // получение подразделения
        public DataSet GetPosts()
        {
            connect();
            string scmd = "Select * from posts";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            close();
            return ds;
        }
        // получение табелей
        public DataSet GetTabels(int id_dep, int month, int year)
        {
            AddTabelsNotExist(id_dep, month, year);
            connect();
            string scmd = "select tabels.id,staff.id,staff.name,staff.schedule,posts.name,data,month,year from tabels cross join staff, posts where staff.id_dep=" +
                id_dep + " and month=" + month + " and year=" + year + " and tabels.id_staff=staff.id and staff.id_post=posts.id";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            close();
            return ds;
        }
        // добавляет не существующие табели по сотрудникам и дате
        public void AddTabelsNotExist(int id_dep, int month, int year)
        {
            connect();
            string scmd = "select id from staff where id_dep=" + id_dep;
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataTable dt = new DataTable();
            DataTable count = new DataTable();
            dataAdapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                scmd = "select count(*) from tabels where id_staff="
                    + dt.Rows[i].ItemArray.First() + " and month=" + month + " and year=" + year;
                dataAdapter = new SQLiteDataAdapter(scmd, conn);
                count.Clear();
                dataAdapter.Fill(count);
                if (Convert.ToInt32(count.Rows[0].ItemArray.First()) == 0)
                {
                    ExecuteQuery("insert into tabels (id_staff,month,year) values (" +
                        Convert.ToInt32(dt.Rows[i].ItemArray.First()) + "," + month + "," + year + ")");
                }
            }
            close();
        }
        // получение сотрудников
        public DataSet GetStaff()
        {
            connect();
            string scmd = "select staff.id,staff.name,department.name,posts.name,schedule,contacts from staff cross join department, posts where staff.id_dep = department.id and id_post = posts.id";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            close();
            return ds;
        }
        // добавление подразделения
        public void AddImage(byte[] value)
        {
            //connect();
            //ExecuteQuery("insert into department (name) values ('" + name + "')");
            cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"INSERT INTO gth (image) VALUES (@image)";
            cmd.Parameters.Add("@image", DbType.Binary, 10000).Value = value;
            cmd.ExecuteNonQuery();
            //close();
        }
        // добавление подразделения
        public void AddPost(string name)
        {
            connect();
            ExecuteQuery("insert into posts (name) values ('" + name + "')");
            close();
        }
        // изменение подразделения
        public void ChDepartment(string name, int index)
        {
            connect();
            ExecuteQuery("update department set name='" + name + "' where id=" + index);
            close();
        }
        // изменение подразделения
        public void ChPost(string name, int index)
        {
            connect();
            ExecuteQuery("update posts set name='" + name + "' where id=" + index);
            close();
        }
        // получение id подразделения по id сотрудника
        public int GetIndexDep(int index)
        {
            connect();
            string scmd = "select id_dep from staff where id=" + index;
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            index = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0]);
            close();
            return index;
        }
        // получение id должности по id сотрудника
        public int GetIndexPost(int index)
        {
            connect();
            string scmd = "select id_post from staff where id=" + index;
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            index = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0]);
            close();
            return index;
        }
        // добавление сотрудника
        public void AddStaff(int index, string name, int index_post, string schedule, string contacts)
        {
            connect();
            ExecuteQuery("insert into staff (id_dep,name,id_post,schedule,contacts) values (" + index + ",'" + name + "'," + index_post + ",'" + schedule + "','" + contacts + "')");
            close();
        }
        // изменение сотрудника
        public void ChStaff(int index_staff, int index_dep, string name, int index_post, string schedule, string contacts)
        {
            connect();
            ExecuteQuery("update staff set id_dep=" + index_dep + ",name='" + name + "',id_post="+index_post+",schedule='" + schedule + "',contacts='" + contacts + "' where id=" + index_staff);
            close();
        }
        // изменение табелей
        public void ChTabels(int index, string data)
        {
            connect();
            ExecuteQuery("update tabels set data='" + data + "' where id=" + index);
            close();
        }
        // получение фио гл.бухгалтера
        public string GetFio()
        {
            connect();
            string scmd = "select staff.name as fio from staff left join posts where posts.name='главный бухгалтер' and id_post=posts.id";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(scmd, conn);
            DataTable ds = new DataTable();
            dataAdapter.Fill(ds);
            close();
            return ds.Rows[0][0].ToString();
        }
        public void delete(DataGrid dg)
        {
            if (dg.SelectedIndex == -1 || dg.SelectedIndex == dg.Items.Count - 1)
                return;
            connect();
            switch (dg.Tag.ToString())
            {
                case "department":
                    ExecuteQuery("delete from department where id=" + Convert.ToInt32(((DataRowView)dg.SelectedItem).Row[0]));
                    break;
                case "posts":
                    ExecuteQuery("delete from posts where id=" + Convert.ToInt32(((DataRowView)dg.SelectedItem).Row[0]));
                    break;
                case "staff":
                    ExecuteQuery("delete from staff where id=" + Convert.ToInt32(((DataRowView)dg.SelectedItem).Row[0]));
                    break;
                case "notes":
                    ExecuteQuery("delete from notes where id=" + Convert.ToInt32(((DataRowView)dg.SelectedItem).Row[0]));
                    break;
            }
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
        private void ExecuteQuery(string txtQuery,byte[] data)
        {
            connect();
            cmd = conn.CreateCommand();
            cmd.CommandText = txtQuery;
            cmd.Parameters.Add("@binary", DbType.Binary).Value = data;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public void close()
        {
            conn.Close();
        }
    }
}
