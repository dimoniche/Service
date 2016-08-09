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
            //-------------------------------------------------------
            query = "CREATE TABLE IF NOT EXISTS `usermoney` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11) NOT NULL , `amount` int(11) NOT NULL," +
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
            query = "CREATE TABLE IF NOT EXISTS `payments` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11),"+
                "`amount` int(11) NOT NULL," +
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
            // таблица для чеков
               query = "CREATE TABLE IF NOT EXISTS `checks` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                              "`dt_create` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                              "`dt_fixed` datetime ON UPDATE CURRENT_TIMESTAMP, " +
                              "`checkstr` varchar(255) NOT NULL, "+
                              "`iduser` int(11)," +
                              "active boolean DEFAULT 0, "+
                              "`amount` int(11) NOT NULL," +
                              "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";
            cmd = new MySqlCommand(query, con);
            try
            {
                Debug.Print(query);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получаем данные статистики
        /// </summary>
        /// <returns></returns>
        public MoneyStatistic GetMoneyStatistic()
        {
            MoneyStatistic statistic = new MoneyStatistic();

            return statistic;
        }

        /// <summary>
        /// Запоминаем данные статистики
        /// </summary>
        /// <param name="statistic"></param>
        public void SetMoneyStatistic(MoneyStatistic statistic)
        {

        }

        /// <summary>
        /// Получаем данные статистики
        /// </summary>
        /// <returns></returns>
        public DeviceStatistic GetDevStatistic()
        {
            DeviceStatistic statistic = new DeviceStatistic();

            return statistic;
        }

        /// <summary>
        /// Запоминаем данные статистики
        /// </summary>
        /// <param name="statistic"></param>
        public void SetDevStatistic(DeviceStatistic statistic)
        {

        }

        /// <summary>
        /// получить количество банктот
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// получить запись о юзере по его логину и паролю
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
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
        /// <summary>
        /// получит ьинфу о всех устройствах
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// прочитать логи из БД
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// получить инфу о всех юзерах
        /// </summary>
        /// <returns></returns>
        public DataTable GetUsers()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from users";

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
        /// <summary>
        /// получить список счетов активных
        /// </summary>
        /// <returns></returns>
        public DataTable GetAmount()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from usermoney";

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

        /// <summary>
        /// получить инфу о инкассациях
        /// </summary>
        /// <param name="iduser"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// получить инфу о сумме денег у конкретного юзера, типа сколько на счету
        /// записей может быть несколько
        /// </summary>
        /// <param name="iduser"></param>
        /// <returns></returns>
        public int GetUserMoney(int iduser)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select sum(amount) as sa from usermoney where iduser='" + iduser.ToString() + "'";

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

        /// <summary>
        /// записать время работы устройства X от услуги Y
        /// </summary>
        /// <param name="serv"></param>
        /// <param name="idDevice"></param>
        /// <param name="timeWork"></param>
        /// <returns></returns>
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
        /// <summary>
        /// внести плату от юзера ... 
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public bool InsertMoney(int userid, int sum)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO payments (iduser, amount, datetime) VALUES (" + userid.ToString() + "," + sum.ToString() + ","
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
        /// <summary>
        /// внести деньги на счёт юзеру
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public bool AddToAmount(int userid, int sum)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO usermoney (iduser, amount, datetime) VALUES (" + userid.ToString() + "," + sum.ToString() + ",'"
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
        /// <summary>
        /// добавить пользователя
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="Psw"></param>
        /// <returns></returns>
        public bool InsertUser(string Login, string Psw)
        {
            string query = "insert into users (login, password, role, datetime) values ('"+Login+"', '"+Psw+"', 2, '" +
              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

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
        /// <summary>
        /// добавление в таблицу check записи 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public bool AddToCheck(int userid, int sum, string check)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO checks (iduser, amount, checkstr, dt_create) VALUES ("
                + userid.ToString() + "," + sum.ToString() + ",'" + check +"','" +
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
        /// <summary>
        /// погасить чек
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool FixedCheck(int id)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "UPDATE checks SET " +
                "active = 1, " +
                "dt_fixed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                "where id=" + id.ToString();

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
        /// <summary>
        /// удалить чек.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool deleteCheck(int id)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "delete from checks where id="+id.ToString();

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

        public CheckInfo GetCheckByStr(string check)
        {
            string queryString = "select id, dt_start, dt_fixed from checks where (checkstr='" + check + "')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            CheckInfo ch = null;

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        ch = new CheckInfo();
                        ch.Id = (int)dr[0];
                        ch.dt_start = (DateTime)dr[1];
                        ch.dt_fixed = (DateTime)dr[2];
                        ch.active = (Boolean)dr[3];
                        ch.IdUser = (int)dr[4];
                        ch.checkstr = (string)dr[5];
                        dr.Close();

                        return ch;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return ch;
        }
        /// <summary>
        /// все чеки
        /// </summary>
        /// <returns></returns>
        public DataTable GetChecks()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from checks";

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
        /// <summary>
        /// проверить существование юзера
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool ExistsUser(string User, string Password)
        {//хотя подойдет функция GetUserByName
            string queryString = "select id from users where (login = '" + User + "') and (password='" + Password + "')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            UserInfo ui = null;

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);

            }
            return false;
        }
        /// <summary>
        /// узнать сколько денег в аппарате с даты ХХХ (предполагается с даты инкассации, которую выяснять ранее и передадут сюда)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
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
        /// <summary>
        /// инфа о последней инкассации
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastEncashment() //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return new DateTime(2016, 07, 01);

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
        /// <summary>
        /// сколько времени (секунд) девайс отработал с даты Х
        /// </summary>
        /// <param name="Serv"></param>
        /// <param name="Dev"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetWorkTime(int Serv, int Dev, DateTime dt) 
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
        /// <summary>
        /// дата последнего сервисного обслуживания устройства Х от услуги У
        /// </summary>
        /// <param name="Serv"></param>
        /// <param name="Dev"></param>
        /// <returns></returns>
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

        public DataTable GetPaymentFromUser(int iduser)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return null;

            DataTable dt = new DataTable();
            string queryString = "select * from payment where iduser='" + iduser.ToString() + "'";

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
    }
}
