using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace WindowsFormsApplication1
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

        public bool Connect()
        {
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

            query = "CREATE TABLE  IF NOT EXISTS `logwork1` ( `id` int(11) NOT NULL AUTO_INCREMENT," +
                           "`iddev` int(255) NOT NULL,  `timework` int(11) NOT NULL," +
                           "`datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP," +
                           "PRIMARY KEY(`id`)) ENGINE = InnoDB AUTO_INCREMENT = 5 DEFAULT CHARSET = cp866;" +
                           "SET FOREIGN_KEY_CHECKS = 1; ";
            cmd.CommandText = query;
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
                    }
                   

                }

                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    
                }
            }
            return dt;
            
        }
        public bool WriteWorkTime(int idDevice, int timeWork)
        {
            
            string query = "INSERT INTO logwork (iddev, timework, datetime) VALUES ("+ idDevice.ToString() + ","+
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

    }
}
