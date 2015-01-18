using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        public Saga.Data.IQueryProvider GetQueryProvider()
        {
            return new MysqlQueryProvider();
        }

        public IDataReader ExecuteDataReader(Saga.Data.IQueryProvider query)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand();
            MySqlDataReader reader = null;

            try
            {

                command.CommandText = query.CmdText;
                command.Connection = connection;
                foreach (KeyValuePair<string, object> pair in query.Parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }

                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return null;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                if (reader != null && reader.IsClosed == false) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        public IDataReader ExecuteDataReader(Saga.Data.IQueryProvider query, CommandBehavior behavior)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand();
            MySqlDataReader reader = null;

            try
            {

                command.CommandText = query.CmdText;
                command.Connection = connection;
                foreach (KeyValuePair<string, object> pair in query.Parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }


                return command.ExecuteReader(behavior);
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return null;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                if (reader != null && reader.IsClosed == false) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        public int ExecuteNonQuery(Saga.Data.IQueryProvider query)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand();

            try
            {
                command.CommandText = query.CmdText;
                command.Connection = connection;
                foreach (KeyValuePair<string, object> pair in query.Parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }


                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
            }
        }

    }
}
