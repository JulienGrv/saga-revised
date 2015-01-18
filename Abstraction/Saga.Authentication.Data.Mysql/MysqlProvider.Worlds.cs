using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Saga.Authentication.Structures;
using System.Data;
using System.Net;

namespace Saga.Authentication.Data.Mysql
{
    partial class MysqlProvider
    {


        bool AddWorld(string worldname, string password, out int worldid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_09, connection);
            command.Parameters.AddWithValue("Name", worldname);
            command.Parameters.AddWithValue("Proof", password);

            MySqlCommand command2 = new MySqlCommand(_query_10, connection);
            command2.Parameters.AddWithValue("Name", worldname);


            try
            {
                int unique = Convert.ToInt32(command2.ExecuteScalar());
                if (unique == 0)
                {
                    int succeed = command.ExecuteNonQuery();
                    worldid = (int)command.LastInsertedId;
                    return succeed > 0;
                }
                else
                {
                    worldid = -1;
                    return false;
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }


        bool RemoveWorld(string worldname)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_11, connection);
            command.Parameters.AddWithValue("Name", worldname);

            try
            {
                int succeed = command.ExecuteNonQuery();
                return succeed > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        bool RenameWorld(string worldname, string newworldname)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_12, connection);
            command.Parameters.AddWithValue("OldName", worldname);
            command.Parameters.AddWithValue("NewName", newworldname);

            MySqlCommand command2 = new MySqlCommand(_query_13, connection);
            command2.Parameters.AddWithValue("Name", newworldname);


            try
            {

                int unique = Convert.ToInt32(command2.ExecuteScalar());
                if (unique == 0)
                {

                    int succeed = command.ExecuteNonQuery();
                    return succeed > 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        List<AclEntry> FindAclEntries(string ip)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_14, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("IP", ip);

            try
            {
                List<AclEntry> list = new List<AclEntry>();
                reader = command.ExecuteReader();

                uint i = 0;
                while (reader.Read())
                {
                    AclEntry info = new AclEntry();
                    info.RuleId = ++i;
                    info.IP = reader.GetString(1);
                    info.Mask = reader.GetString(2);
                    info.Operation = reader.GetByte(3);
                    list.Add(info);
                }

                reader.Close();

                return list;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        bool RemoveAclEntry(string ip, string mask, byte operation)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_15, connection);
            command.Parameters.AddWithValue("Op", operation);
            command.Parameters.AddWithValue("FilterIp", ip);
            command.Parameters.AddWithValue("Mask", mask);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }      

        bool AddAclEntry(string ip, string mask, byte operation)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_16, connection);
            MySqlCommand command2 = new MySqlCommand(_query_17, connection);
            command.Parameters.AddWithValue("Op", operation);
            command.Parameters.AddWithValue("FilterIp", ip);
            command.Parameters.AddWithValue("Mask", mask);
            command2.Parameters.AddWithValue("Op", operation);
            command2.Parameters.AddWithValue("FilterIp", ip);
            command2.Parameters.AddWithValue("Mask", mask);

            try
            {
                if (Convert.ToUInt32(command.ExecuteScalar()) > 0)
                {
                    return true;
                }
                else
                {
                    return command2.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
            
        }


        bool IsAllowed(System.Net.IPAddress adress)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_18, connection);
            command.Parameters.AddWithValue("IP", adress.ToString());

            try
            {
                return  Convert.ToUInt32(command.ExecuteScalar()) > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        bool IsDenied(System.Net.IPAddress adress)
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_19, connection);
            command.Parameters.AddWithValue("IP", adress.ToString());
            
            try
            {
                return Convert.ToUInt32(command.ExecuteScalar()) > 0;              
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
            
        }


        List<AclEntry> ListAclEntries()
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_20, connection);
            MySqlDataReader reader = null;
           
            try
            {
                List<AclEntry> list = new List<AclEntry>();
                reader = command.ExecuteReader();

                uint i = 0;
                while (reader.Read())
                {
                    AclEntry info = new AclEntry();
                    info.RuleId = ++i;
                    info.IP = reader.GetString(1);
                    info.Mask = reader.GetString(2);
                    info.Operation = reader.GetByte(3);
                    list.Add(info);
                }

                reader.Close();

                return list;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
            
        }


        List<Saga.Authentication.Structures.WorldInfo> GetWorldInformation()
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_21, connection);
            MySqlDataReader reader = null;

            try
            {
                List<WorldInfo> list = new List<WorldInfo>();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    WorldInfo info = new WorldInfo();
                    info.WorldId = reader.GetByte(0);
                    info.Worldname = reader.GetString(1);
                    info.Worldproof = reader.GetString(2);
                    list.Add(info);
                }

                reader.Close();

                return list;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);               
            }
        }

        bool UpdateLastplayedWorld(uint AccountId, byte world)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_22, connection);
            command.Parameters.AddWithValue("UserId", AccountId);
            command.Parameters.AddWithValue("LastPlayedWorld", world);

            try
            {
                return command.ExecuteNonQuery() > 0;   
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return false;
        }

        bool UpdateSession(uint AccountId, uint sessionid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_23, connection);
            command.Parameters.AddWithValue("UserId", AccountId);
            command.Parameters.AddWithValue("ActiveSession", sessionid);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return false;
        }

        public bool ReleaseSessionId(uint sessionid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_24, connection);
            command.Parameters.AddWithValue("ActiveSession", sessionid);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return false;
        }


        int ClearSession(byte worldid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_25, connection);
            command.Parameters.AddWithValue("LastPlayedWorld", worldid);

            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return 0;

        }

        long GetGameTime(uint accountid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_26, connection);
            command.Parameters.AddWithValue("UserId", accountid);

            try
            {
                return Convert.ToInt64(command.ExecuteScalar());
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        IPAddress GetIpAddress(uint accountid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_27, connection);
            command.Parameters.AddWithValue("UserId", accountid);

            try
            {
                return IPAddress.Parse(Encoding.ASCII.GetString(command.ExecuteScalar() as byte[]));
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }
    }
}

