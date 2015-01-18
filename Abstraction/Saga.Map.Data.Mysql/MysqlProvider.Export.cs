using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {
        public List<uint> GetCharacters()
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_22, connection);
            MySqlDataReader reader = null;

            List<uint> characters = new List<uint>();
            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    characters.Add(reader.GetUInt32(0));
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }

            return characters;
        }
    }
}
