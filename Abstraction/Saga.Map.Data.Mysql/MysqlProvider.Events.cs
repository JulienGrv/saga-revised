using MySql.Data.MySqlClient;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        bool GetPlayerId(string name, out uint charid)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_17, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharName", name);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    charid = reader.GetUInt32(0);
                    return true;
                }

                charid = 0;
                return false;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                charid = 0;
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
                if (reader != null)
                    reader.Close();
            }
        }

        public bool DeleteEventItemId(uint RewardId)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_18, connection);
            command.Parameters.AddWithValue("RewardId", RewardId);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
            }
        }

        public bool CreateEventItem(uint CharId, uint ItemId, byte Stack)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_19, connection);
            command.Parameters.AddWithValue("CharId", CharId);
            command.Parameters.AddWithValue("ItemId", ItemId);
            command.Parameters.AddWithValue("ItemCount", Stack);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
            }
        }

        public EventItem FindEventItemById(Character target, uint RewardId)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_20, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", target.ModelId);
            command.Parameters.AddWithValue("RewardId", RewardId);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EventItem b = new EventItem();
                    b.EventId = reader.GetUInt32(0);
                    b.ItemId = reader.GetUInt32(2);
                    b.ItemCount = reader.GetByte(3);
                    return b;
                }

                return new EventItem();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return new EventItem();
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
                if (reader != null)
                    reader.Close();
            }
        }

        public System.Collections.ObjectModel.Collection<EventItem> FindEventItemList(Character target)
        {
            //HELPER VARIABLES
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_21, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", target.ModelId);
            System.Collections.ObjectModel.Collection<EventItem> collection = new System.Collections.ObjectModel.Collection<EventItem>();

            try
            {
                reader =command.ExecuteReader();
                while (reader.Read())
                {
                    EventItem b = new EventItem();
                    b.EventId = reader.GetUInt32(0);
                    b.ItemId = reader.GetUInt32(2);
                    b.ItemCount = reader.GetByte(3);
                    collection.Add(b);
                }

                reader.Close();

                return collection;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return collection;
            }
            finally
            {
                //ALWAYS CLOSE THE CONNECTION AND REPOOL THE ITEMS
                ConnectionPool.Release(connection);
                if (reader != null)
                    reader.Close();
            }
        }

    }
}

