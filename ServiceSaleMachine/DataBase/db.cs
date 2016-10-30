﻿using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace ServiceSaleMachine
{
    public class db
    {
        MySqlConnectionStringBuilder mysqlCSB;
        MySqlConnection con;
        Log log;

        public MySqlConnection Connection { get { return con; } private set {;} }

        public db(Log log)
        {
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = Globals.DbConfiguration.Server;
            mysqlCSB.Database = Globals.DbConfiguration.Database;
            mysqlCSB.UserID = Globals.DbConfiguration.UserID;
            mysqlCSB.Password = Globals.DbConfiguration.Password;

            this.log = log;
        }

        public db()
        {
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = Globals.DbConfiguration.Server;
            mysqlCSB.Database = Globals.DbConfiguration.Database;
            mysqlCSB.UserID = Globals.DbConfiguration.UserID;
            mysqlCSB.Password = Globals.DbConfiguration.Password;

            this.log = null;
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
                con.StateChange += Con_StateChange;

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Что нужно сделать при закрытии приложения
        /// </summary>
        public void CloseForm()
        {
            con.StateChange -= Con_StateChange;
        }

        private void Con_StateChange(object sender, StateChangeEventArgs e)
        {
            if(e.CurrentState == ConnectionState.Closed || e.CurrentState == ConnectionState.Broken)
            {
                // конект закрылся - сделаем реконнект
                con.Close();
                con.StateChange -= Con_StateChange;
                con = null;

                Connect();
            }
        }

        public bool CreateTables()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "CREATE TABLE IF NOT EXISTS `devices` ( `id` int(11) NOT NULL,  `name` varchar(255) NOT NULL," +
                  "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // создадим таблицу с логом работы
            query = "CREATE TABLE  IF NOT EXISTS `logwork` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                           "`iddev` int(255) NOT NULL,  `timework` int(11) NOT NULL, `idserv` int(255) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB AUTO_INCREMENT = 5 DEFAULT CHARSET = cp866;" +
                           "SET FOREIGN_KEY_CHECKS = 1; ";

            ExecuteNonQuery(query);

            // создадим таблицу обслуживания устройств
            query = "CREATE TABLE IF NOT EXISTS `refreshdevices` ( `id` int(11) NOT NULL AUTO_INCREMENT, `idserv` int(11) NOT NULL, `iddev` int(11) NOT NULL," + 
                           "`iduser` varchar(255) NOT NULL," + 
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // создадим таблицу инкассации
            query = "CREATE TABLE IF NOT EXISTS `encashment` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11) NOT NULL , `amount` int(11) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // создадим таблицу пользователей IF  NOT EXISTS
            query = "Show tables from servterminal like 'users'";
            MySqlCommand cmd = new MySqlCommand(query, con);
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

            ExecuteNonQuery(query);

            // -------------- таблица системных значений
            query = "CREATE TABLE IF NOT EXISTS `systemvalues` (namevalue varchar(255) NOT NULL , " +
                    "ownvalue varchar(255) NOT NULL" +
                    ") ENGINE = InnoDB DEFAULT CHARSET = cp866; ";// SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            if (needCreate)
            {
                query = "CREATE TABLE `users` ( `id` int(11) NOT NULL AUTO_INCREMENT, `login` varchar(255) NOT NULL, `password` varchar(255) NOT NULL, `role` int(11) NOT NULL, " +
                               "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                               "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

                ExecuteNonQuery(query);

                query = "insert into users (login, password, role, datetime) values ('admin', '12345', 1, '" +
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                ExecuteNonQuery(query);
            }

            // создадим таблицу платежей
            query = "CREATE TABLE IF NOT EXISTS `payments` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11),"+
                "`amount` int(11) NOT NULL," +
                               "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                               "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // создадим таблицу банкнот
            query = "CREATE TABLE IF NOT EXISTS `banknotes` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // таблица для чеков
            query = "CREATE TABLE IF NOT EXISTS `checks` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                              "`dt_create` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                              "`dt_fixed` datetime ON UPDATE CURRENT_TIMESTAMP, " +
                              "`checkstr` varchar(255) NOT NULL, "+
                              "`iduser` int(11)," +
                              "`number` int(11)," +
                              "active boolean DEFAULT 0, " +
                              "`amount` int(11) NOT NULL," +
                              "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);

            // создадим таблицу оказанных услуг
            query = "CREATE TABLE IF NOT EXISTS `serviceset` ( `id` int(11) NOT NULL AUTO_INCREMENT, `iduser` int(11) NOT NULL , `amount` int(11) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = cp866;  SET FOREIGN_KEY_CHECKS = 1";

            ExecuteNonQuery(query);


            return true;
        }

        private string GetSystemValue(string name)
        {
            MySqlDataReader dr = Execute("select namevalue, ownvalue from systemvalues where namevalue='" + name+"'");
            if (dr != null)
            {
                string str = "";

                if (dr.HasRows)
                {
                    dr.Read();

                    try
                    {
                        if (dr.IsDBNull(0) != true)
                        {
                            str = dr[1].ToString();
                        }
                    }
                    catch
                    {
                    }
                }

                dr.Close();
                return str;
            }
            return "";
        }
        /// <summary>
        /// заполнение таблицы системных значений при старте
        /// </summary>
        public void FillSystemValues()
        {
           // String str = GetSystemValue("nextnumbercheck");
            MySqlDataReader dsv = Execute("select * from systemvalues where namevalue='nextnumbercheck'");
            if (dsv != null)
            {
                if (!dsv.HasRows)
                {
                    dsv.Close();//обязательно!
                    string query = "insert into systemvalues (namevalue, ownvalue) values ('nextnumbercheck', '0')";
                    ExecuteNonQuery(query);
                }
                else
                {
                    dsv.Close();//обязательно!
                }
            }

            dsv = Execute("select * from systemvalues where namevalue='nextnumberdeliverycheck'");
            if (dsv != null)
            {
                if (!dsv.HasRows)
                {
                    dsv.Close();//обязательно!
                    string query = "insert into systemvalues (namevalue, ownvalue) values ('nextnumberdeliverycheck', '0')";
                    ExecuteNonQuery(query);
                }
                else
                {
                    dsv.Close();//обязательно!
                }
            }
        }

        /// <summary>
        /// Получаем данные статистики
        /// </summary>
        /// <returns></returns>
        public MoneyStatistic GetMoneyStatistic()
        {
            MoneyStatistic statistic = new MoneyStatistic();

            DateTime dt = GetLastEncashment(); //дата последней инкассации

            statistic.AllMoneySumm = GlobalDb.GlobalBase.GetCountMoney(dt);             // платежи с даты последней инкассации
            statistic.ServiceMoneySumm = GlobalDb.GlobalBase.GetCountMoneyService(dt);  // сумма оказанных услуг
            statistic.AccountMoneySumm = GlobalDb.GlobalBase.GetSummFromAccount();
            statistic.CountBankNote = GlobalDb.GlobalBase.GetCountBankNote();
            statistic.BarCodeMoneySumm = GlobalDb.GlobalBase.GetSummFromBarCode(); 

            return statistic;
        }

        /// <summary>
        /// Сумма с указанной даты
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MoneyStatistic GetMoneyStatistic(DateTime dt)
        {
            MoneyStatistic statistic = new MoneyStatistic();

            statistic.AllMoneySumm = GlobalDb.GlobalBase.GetCountMoney(dt);             // платежи с даты последней инкассации
            statistic.ServiceMoneySumm = GlobalDb.GlobalBase.GetCountMoneyService(dt);  // сумма оказанных услуг
            statistic.AccountMoneySumm = GlobalDb.GlobalBase.GetSummFromAccount();
            statistic.CountBankNote = GlobalDb.GlobalBase.GetCountBankNote();
            statistic.BarCodeMoneySumm = GlobalDb.GlobalBase.GetSummFromBarCode();

            return statistic;
        }

        /// <summary>
        /// исполнить запрос и вернуть набор данных
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private MySqlDataReader Execute(string cmd)
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                con.Close();
                Connect();
            }

            MySqlCommand com = new MySqlCommand(cmd, con);

            try
            {
                MySqlDataReader dr = com.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return null;
            }
        }

        private bool ExecuteNonQuery(string query)
        {
            if(con.State == ConnectionState.Closed)
            {
                con.Close();
                Connect();
            }

            MySqlCommand cmd2 = new MySqlCommand(query, con);
            try
            {
                cmd2.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// извлечь из набора данных одно поле одной записи и вернуть int
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private int GetIntFromReq(MySqlDataReader dr )
        {
            string str = "0";
            if (dr != null)
            {
                if (dr.HasRows)
                {
                    dr.Read();

                    try
                    {
                        if (dr.IsDBNull(0) != true)
                        {
                            str = dr[0].ToString();
                        }
                    }
                    catch
                    {
                    }
                }

                dr.Close();
            }

            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime GetDTFromReq(MySqlDataReader dr)
        {
            DateTime dt = new DateTime(2016, 07, 01); ;
            if (dr != null)
            {
                if (dr.HasRows)
                {
                    dr.Read();

                    try
                    {
                        if (dr.IsDBNull(0) != true)
                        {
                            dt = (DateTime)dr[0];
                        }
                    }
                    catch
                    {
                    }
                }

                dr.Close();
            }
            return dt;
        }
        /// <summary>
        /// подсчитать деньги на незакрытых штрихкодах
        /// </summary>
        /// <returns></returns>
        public int GetSummFromBarCode()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            MySqlDataReader dr = Execute("select sum(amount) from checks where active = 0");

            return GetIntFromReq(dr);

        }


        /// <summary>
        /// Запоминаем данные статистики
        /// </summary>
        /// <param name="statistic"></param>
        public void SetMoneyStatistic(MoneyStatistic statistic)
        {

        }

        /// <summary>
        /// получить количество банктот
        /// </summary>
        /// <returns></returns>
        public int GetCountBankNote()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            MySqlDataReader dr = Execute("select count(id) from banknotes");

            return GetIntFromReq(dr);

        }
        /// <summary>
        /// очистка инфы о банктнотах (вызывать сразу после инкассации)
        /// </summary>
        /// <returns></returns>
        public bool ClearBankNotes()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string queryString = "delete from banknotes";

            MySqlCommand com = new MySqlCommand(queryString, con);

            return ExecuteNonQuery(queryString);
        }
        /// <summary>
        /// Внесение банкноты
        /// </summary>
        /// <returns></returns>
        public bool InsertBankNote()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO banknotes (datetime) VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            return ExecuteNonQuery(query);
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

            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                con.Close();
                Connect();
            }

            MySqlCommand com = new MySqlCommand(queryString, con);

            UserInfo ui = null;

            using (MySqlDataReader dr = com.ExecuteReader())
            {
                if (dr != null)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        try
                        {
                            ui = new UserInfo();
                            ui.Id = (int)dr[0];
                            ui.Login = (string)dr[1];
                            ui.Role = (UserRole)dr[3];
                        }
                        catch
                        {

                        }
                    }

                    dr.Close();
                    return ui;
                }
            }

            return ui;
        }

        private DataTable getDataTable(string query)
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                con.Close();
                Connect();
            }

            MySqlCommand com = new MySqlCommand(query, con);

            DataTable dt = new DataTable();

            using (MySqlDataReader dr = com.ExecuteReader())
            {
                if (dr != null)
                {
                    if (dr.HasRows)
                    {
                        dt.Load(dr);
                    }

                    dr.Close();
                }
            }

            return dt;            
        }

        /// <summary>
        /// получить инфу о всех устройствах
        /// </summary>
        /// <returns></returns>
        public DataTable GetDevices()
        {
            string queryString = "select * from devices";

            return getDataTable(queryString);

            
        }
        /// <summary>
        /// прочитать логи из БД
        /// </summary>
        /// <returns></returns>
        public DataTable GetLogWork()
        {
            string queryString = @"select * from logwork";
            return getDataTable(queryString);

        }

        /// <summary>
        /// получить инфу о всех юзерах
        /// </summary>
        /// <returns></returns>
        public DataTable GetUsers()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from users";
            return getDataTable(queryString);

        }

        /// <summary>
        /// получить список счетов активных
        /// </summary>
        /// <returns></returns>
        public DataTable GetAmount()
        {
            DataTable dt = new DataTable();
            string queryString = @"select * from usermoney";
            return getDataTable(queryString);

        }

        public DataTable GetAmount(int id)
        {
            DataTable dt = new DataTable();

            string queryString = @"select * from usermoney where iduser='" + id.ToString() + "'";
            return getDataTable(queryString);
        }

        /// <summary>
        /// записать инфу о инкассациях
        /// </summary>
        /// <param name="iduser"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public bool Encashment(int iduser, int amount)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO encashment (iduser, amount, datetime) VALUES (" + iduser.ToString() + "," + amount.ToString() + ",'" +
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            Debug.Print(query);
            return ExecuteNonQuery(query);

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

            MySqlDataReader dr = Execute("select sum(amount) as sa from usermoney where iduser='" + iduser.ToString() + "'");

            return GetIntFromReq(dr);

        }

        public int GetSummFromAccount()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            MySqlDataReader dr = Execute("select sum(amount) as sa from usermoney");

            return GetIntFromReq(dr);

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

            return ExecuteNonQuery(query);

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

            string query = "INSERT INTO payments (iduser, amount, datetime) VALUES (" + userid.ToString() + "," + sum.ToString() + ",'"
                  + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// Занесем оказанную услугу в базу
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public bool InsertService(int userid, int sum)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO serviceset (iduser, amount, datetime) VALUES (" + userid.ToString() + "," + sum.ToString() + ",'"
                  + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            return ExecuteNonQuery(query);
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

            return ExecuteNonQuery(query);

        }

        /// <summary>
        /// добавить пользователя
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="Psw"></param>
        /// <returns></returns>
        public bool InsertUser(string login, string Psw)
        {
            string query = "insert into users (login, password, role, datetime) values ('"+login+"', '"+Psw+"', 2, '" +
              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            return ExecuteNonQuery(query);

        }

        /// <summary>
        /// Получить номер фискального чека
        /// </summary>
        /// <returns></returns>
        public int GetCurrentNumberCheck() //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select namevalue, ownvalue from  systemvalues where " +
                 "namevalue= 'nextnumbercheck'";


            MySqlDataReader dr = Execute(queryString);
            if (dr != null)
            {
                int i = 0;

                if (dr.HasRows)
                {
                    dr.Read();
                    string s = "0";

                    try
                    {
                        if (dr.IsDBNull(0) != true)
                            s = (string)dr[1];

                        int.TryParse(s, out i);
                    }
                    catch
                    {
                        i = 0;
                    }
                }

                dr.Close();
                return i;
            }

            return 0;
        }

        /// <summary>
        /// записать значение в текущий номер фискальных чеков (увеличенный или обнулить по команде)
        /// </summary>
        /// <param name="CurrentN"></param>
        /// <returns></returns>
        public bool SetNumberCheck(int CurrentN)
        {
            string query = "update systemvalues set ownvalue=" + CurrentN.ToString() + " where namevalue = 'nextnumbercheck'";
            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// получить номер чека со сдачей
        /// </summary>
        /// <returns></returns>
        public int GetCurrentNumberDeliveryCheck() //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select namevalue, ownvalue from  systemvalues where " +
                 "namevalue= 'nextnumberdeliverycheck'";


            MySqlDataReader dr = Execute(queryString);
            if (dr != null)
            {
                int i = 0;

                if (dr.HasRows)
                {
                    dr.Read();
                    string s = "0";

                    try
                    {
                        if (dr.IsDBNull(0) != true)
                            s = (string)dr[1];

                        int.TryParse(s, out i);
                    }
                    catch
                    {
                        i = 0;
                    }
                }

                dr.Close();
                return i;
            }

            return 0;
        }

        /// <summary>
        /// записать значение в текущий номер чеков со сдачей (увеличенный или обнулить по команде)
        /// </summary>
        /// <param name="CurrentN"></param>
        /// <returns></returns>
        public bool SetNumberDeliveryCheck(int CurrentN)
        {
            string query = "update systemvalues set ownvalue=" + CurrentN.ToString() + " where namevalue = 'nextnumberdeliverycheck'";
            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// добавление в таблицу check записи 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sum"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool AddToCheck(int userid, int sum, string check)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            // получим текущий номер чека со сдачей
            int nc = GetCurrentNumberDeliveryCheck();

            string query = "INSERT INTO checks (iduser, amount, checkstr, number, dt_create) VALUES ("
                + userid.ToString() + "," + sum.ToString() + ",'" + check +"','" + nc.ToString() + "','"+
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            SetNumberDeliveryCheck(nc + 1);
            return ExecuteNonQuery(query);
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

            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// Фиксируем все чеки
        /// </summary>
        /// <returns></returns>
        public bool FixedAllCheck()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "UPDATE checks SET " +
                "active = 1, " +
                "dt_fixed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// удалить чек.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCheck(int id)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "delete from checks where id="+id.ToString();

            return ExecuteNonQuery(query);
        }

        public CheckInfo GetCheckByStr(string check)
        {
            int index = check.IndexOf('\n');

            if(index != -1)
                check = check.Remove(check.IndexOf('\n'), 1);

            index = check.IndexOf('\r');

            if (index != -1)
                check = check.Remove(check.IndexOf('\r'), 1);

            string queryString = "select id, dt_create, dt_fixed, active, iduser, checkstr," +
                  " number, amount from checks where (checkstr='" + check + "')";

            MySqlCommand com = new MySqlCommand(queryString, con);

            CheckInfo ch = null;

            MySqlDataReader dr = Execute(queryString);
            if (dr == null)
            {
                return null;
            }
            
            if (dr.HasRows)
            {
                dr.Read();
                ch = new CheckInfo();
                ch.Id = (int)dr[0];

                try
                {
                    if (dr[1] != null)
                        ch.dt_start = (DateTime)dr[1];
                }
                catch
                {
                    ch.dt_start = new DateTime();
                }

                try
                {
                    if (dr[2] != null)
                        ch.dt_fixed = (DateTime)dr[2];
                }
                catch
                {
                    ch.dt_fixed = new DateTime();
                }

                try
                {
                    ch.active = (Boolean)dr[3];
                    ch.IdUser = (int)dr[4];
                    ch.checkstr = (string)dr[5];
                    ch.Number = (int)dr[6];
                    ch.Amount = (int)dr[7];
                }
                catch
                {
                }
            }

            dr.Close();
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

            return getDataTable(queryString);

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

            MySqlDataReader dr = Execute(queryString);

            dr.Close();
            if (dr.HasRows)
            {
                return true;
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

            MySqlDataReader dr = Execute(queryString);

            return GetIntFromReq(dr);
        }

        /// <summary>
        /// узнать на какую сумму оказано услуг с даты ХХХ
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetCountMoneyService(DateTime dt)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select sum(amount) as dt from serviceset where " +
                "(datetime >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            MySqlDataReader dr = Execute(queryString);

            return GetIntFromReq(dr);
        }

        /// <summary>
        /// инфа о последней инкассации
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastEncashment() //
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return new DateTime(2016, 07, 01);

            string queryString = "select max(datetime) as dt from encashment";
            MySqlDataReader dr = Execute(queryString);
            return GetDTFromReq(dr);


        }

        /// <summary>
        /// сколько времени (минут) девайс отработал с даты Х
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

            MySqlDataReader dr = Execute(queryString);

            return GetIntFromReq(dr);

        }

        /// <summary>
        /// сколько времени (минут) отработали все девайсы со всех сервисов с даты Х
        /// </summary>
        /// <param name="Serv"></param>
        /// <param name="Dev"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetWorkTime(DateTime dt)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return 0;

            string queryString = "select sum(timework) as dt from logwork where " +
                 "(datetime >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            MySqlDataReader dr = Execute(queryString);

            return GetIntFromReq(dr);
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
            MySqlDataReader dr = Execute(queryString);
            return GetDTFromReq(dr);

        }

        /// <summary>
        /// дата последнего сервисного обслуживания 
        /// </summary>
        /// <param name="Serv"></param>
        /// <param name="Dev"></param>
        /// <returns></returns>
        public DateTime GetLastRefreshTime()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return new DateTime(2016, 07, 01);

            string queryString = "select max(datetime) as dt from refreshdevices";

            MySqlDataReader dr = Execute(queryString);
            return GetDTFromReq(dr);
        }

        /// <summary>
        /// обслужили устройство X от услуги Y
        /// </summary>
        /// <param name="serv"></param>
        /// <param name="idDevice"></param>
        /// <param name="timeWork"></param>
        /// <returns></returns>
        public bool WriteRefreshTime(int serv, int idDevice)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return false;

            string query = "INSERT INTO refreshdevices (idserv, iddev, iduser, datetime) VALUES (" + serv.ToString() + "," + idDevice.ToString() + "," + "0" + ",'" +
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            return ExecuteNonQuery(query);
        }

        /// <summary>
        /// Платежи от конкретного юзера
        /// </summary>
        /// <param name="iduser"></param>
        /// <returns></returns>
        public DataTable GetPaymentFromUser(int iduser)
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return null;

            string queryString = "select * from payments where iduser='" + iduser.ToString() + "'";

            return getDataTable(queryString);
        }

        /// <summary>
        /// Платежи всех пользователей
        /// </summary>
        /// <param name="iduser"></param>
        /// <returns></returns>
        public DataTable GetPaymentAllUser()
        {
            if (Globals.ClientConfiguration.Settings.offDataBase == 1) return null;

            string queryString = "select * from payments";

            return getDataTable(queryString);
        }
    }
}
