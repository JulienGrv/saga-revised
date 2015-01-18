using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Saga.Data;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        public bool Connect(ConnectionInfo info)
        {
            __dbtracelog.WriteInformation("database", "Populating database pool with connections");

            //REFERENCE OUR CONNECTION INFO
            this.info = info;

            //GENERATE A EXCEPTION QEUEE
            List<MySqlException> Exceptions = new List<MySqlException>();
            bool success = false;

            //CREATE 5 CONNECTIONS
            for (int i = 0; i < info.pooledconnections; i++)
            {
                try
                {
                    __dbtracelog.WriteLine("database", "Creating database connection: {0}", i);
                    MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder();
                    cb.UserID = info.username;
                    cb.Password = info.password;
                    cb.Port = info.port;
                    cb.Server = info.host;
                    cb.Database = info.database;

                    MySqlConnection conn = new MySqlConnection(cb.ConnectionString);
                    conn.Open();
                    System.Threading.Timer myTimer = new System.Threading.Timer(callback, conn, 300000, 300000);
                    ConnectionPool.Release(conn);
                    success = true;
                }
                catch (Exception e)
                {
                    __dbtracelog.WriteError("Database", e.Message);
                    //Exceptions.Add(e);
                }
            }

            return success;
        }

        private void callback(object obj)
        {
            //PREVENTS ASYNC CALLBACKS TO HAPPEN WHILE IT PINGS
            lock (obj)
            {
                MySqlConnection conn = (MySqlConnection)obj;
                if (!conn.Ping())
                {
                    Console.WriteLine("Idle connection reopened");
                    conn.Open();
                }
            }
        }



    }
}
