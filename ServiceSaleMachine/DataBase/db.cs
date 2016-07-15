using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace ServiceSaleMachine
{
    public class db
    {
        MySqlConnectionStringBuilder mysqlCSB;
        MySqlConnection con;

        public db()
        {
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = "localhost";
            mysqlCSB.Database = "servterminal";
            mysqlCSB.UserID = "root";
            mysqlCSB.Password = "vzljot";
        }

        public bool CreateDB()
        {
            con = new MySqlConnection();
            con.ConnectionString = mysqlCSB.ConnectionString;
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `servterminal`;", con);
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool Connect()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            con = new MySqlConnection();
            con.ConnectionString = mysqlCSB.ConnectionString;

            try
            {
                con.Open();

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }
        public bool CreateTables()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "CREATE TABLE IF NOT EXISTS `devices` ( `id` int(11) NOT NULL,  `name` varchar(255) NOT NULL," +
                  "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";
            MySqlCommand cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            // создадим таблицу с логом работы
            query = "CREATE TABLE  IF NOT EXISTS `logwork` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                           "`iddev` int(255) NOT NULL,  `timework` int(11) NOT NULL, `idserv` int(255) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB AUTO_INCREMENT = 5 DEFAULT CHARSET = cp866;" +
                           "SET FOREIGN_KEY_CHECKS = 1; ";
            cmd.CommandText = query;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            // создадим таблицу обслуживания устройств
            query = "CREATE TABLE IF NOT EXISTS `refreshdevices` ( `id` int(11) NOT NULL AUTO_INCREMENT, `idserv` int(11) NOT NULL, `iddev` int(11) NOT NULL," + 
                           "`iduser` varchar(255) NOT NULL," + 
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            // создадим таблицу инкассации
            query = "CREATE TABLE IF NOT EXISTS `encashment` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11) NOT NULL , `amount` int(11) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            // создадим таблицу пользователей IF  NOT EXISTS
            query = "Show tables from servterminal like 'users'";
            cmd = new MySqlCommand(query, con);
            bool needCreate = true;
            try
            {
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    needCreate = false;
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            if (needCreate)
            {
                query = "CREATE TABLE `users` ( `id` int(11) NOT NULL AUTO_INCREMENT, `login` varchar(255) NOT NULL, `password` varchar(255) NOT NULL, `role` int(11) NOT NULL, " +
                               "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                               "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

                cmd = new MySqlCommand(query, con);
                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    return false;
                }

                query = "insert into users (login, password, role, datetime) values ('admin', '12345', 1, '" +
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                cmd = new MySqlCommand(query, con);
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    return false;
                }
            }

            // создадим таблицу платежей
            query = "CREATE TABLE IF NOT EXISTS `payments` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11), `amount` int(11) NOT NULL," +
                               "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                               "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            // создадим таблицу банкнот
            query = "CREATE TABLE IF NOT EXISTS `banknotes` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            return true;
        }

        public int GetCountBankNote()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select count(id) from banknotes";

            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string str = dr[0].ToString();
                        dr.Close();

                        return int.Parse(str);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return 0;
        }

        public UserInfo GetUserByName(string User, string Password)
        {
            string queryString = "select id, login, password, role from users where (login = '" + User + "') and (password='"+Password+"')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            UserInfo ui = null;

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        ui = new UserInfo();
                        ui.Id = (int)dr[0];
                        ui.Login = (string)dr[1];
                        ui.Role = (UserRole)dr[3];
                        dr.Close();

                        return ui;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return ui;
        }

        public DataTable GetDevices()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from devices";

            //using (MySqlConnection con = new MySqlConnection())
            {
                MySqlCommand com = new MySqlCommand(queryString, con);

                try
                {
                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }

                        dr.Close();
                    }
                }

                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    
                }
            }
            return dt;
            
        }

        public DataTable GetLogWork()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from logwork";

            //using (MySqlConnection con = new MySqlConnection())
            {
                MySqlCommand com = new MySqlCommand(queryString, con);

                try
                {
                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }

                        dr.Close();
                    }
                }

                catch (Exception ex)
                {
                    Debug.Print(ex.Message);

                }
            }
            return dt;

        }

        public bool Encashment(int iduser, int Amount)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO encashment (iduser, amount, datetime) VALUES (" + iduser.ToString() + "," + Amount.ToString() + "," +
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            Debug.Print(query);
            MySqlCommand cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool WriteWorkTime(int serv, int idDevice, int timeWork)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO logwork (idserv, iddev, timework, datetime) VALUES (" + serv.ToString() + "," + idDevice.ToString() + ","+
                 timeWork.ToString () + ", '"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +"')";
            Debug.Print(query);
            MySqlCommand cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool InsertMoney(int userid, int sum)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string query = "INSERT INTO payments (userid, amount, datetime) VALUES (" + userid.ToString() + "," + sum.ToString() + ","
                  + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            Debug.Print(query);
            MySqlCommand cmd = new MySqlCommand(query, con);
            try
            {
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public int GetCountMoney(DateTime dt) 
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select sum(amount) as dt from payments where " +
                "(datetime >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string str = dr[0].ToString();
                        return int.Parse(str);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return 0;
        }

        public DateTime GetLastEncashment() //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select max(datetime) as dt from encashment";

            DateTime dt;

            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string str = dr[0].ToString();

                        return (DateTime)dr[0];
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }

            return new DateTime(2016, 07, 01);
        }

        public int GetWorkTime(int Serv, int Dev, DateTime dt) //сколько времени (секунд) девайс отработал с даты Х
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select sum(timework) as dt from logwork where "+
                 "(idserv = " + Serv.ToString() + ") and (iddev = " + Dev.ToString() + ") and (datetime >= '" + 
                    dt.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string str = dr[0].ToString();
                        return int.Parse(str);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return 0;
        }

        public DateTime GetLastRefreshTime(int Serv, int Dev) //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return new DateTime(2016, 07, 01);

            string queryString = "select max(datetime) as dt from refreshdevices where " +
                 "(idserv = " + Serv.ToString() + ") and (iddev = " + Dev.ToString() + ")";

            DateTime dt;

            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string str = dr[0].ToString();

                        return (DateTime)dr[0];
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }

            return new DateTime(2016,07,01);
        }
    }
}
