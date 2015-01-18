using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using MySql.Data.MySqlClient;
using System.Data;
using Saga.PrimaryTypes;
using System.Diagnostics;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     Receiptent=?Receiptent
        /// AND 
        ///     IsInbox=1;
        /// </Sql>
        public IEnumerable<Mail> GetInboxMail(MySqlConnection connection, Character target)
        {
            MySqlCommand command = new MySqlCommand(_query_35, connection);
            MySqlCommand command2 = new MySqlCommand(_query_36, connection);
            command.Parameters.AddWithValue("Receiptent", target.Name);
            command2.Parameters.AddWithValue("Receiptent", target.Name);
            MySqlDataReader reader = null;
            List<Mail> list = new List<Mail>();


            try
            {
                command2.ExecuteNonQuery();
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    Mail mailsubject = new Mail();
                    mailsubject.MailId = reader.GetUInt32(0);
                    mailsubject.MailId = reader.GetUInt32(0);
                    mailsubject.Sender = reader.GetString(2);
                    mailsubject.Time = reader.GetDateTime(3);
                    mailsubject.IsRead = reader.GetByte(4);
                    mailsubject.Topic = reader.GetString(6);
                    mailsubject.Message = reader.GetString(7);
                    mailsubject.Zeny = (int)reader.GetUInt32(9);


                    if (reader.IsDBNull(8) == false)
                    {
                        byte[] buffer = new byte[67];
                        reader.GetBytes(8, 0, buffer, 0, 67);
                        Rag2Item.Deserialize(out mailsubject.Attachment, buffer, 0);
                    }
                    if (mailsubject.IsRead == 0)
                    {
                        mailsubject.Valid = (byte)(30 - Math.Min(30, (int)((DateTime.Now - mailsubject.Time).TotalDays)));
                    }
                    else
                    {
                        DateTime dateread = reader.GetDateTime(10);
                        mailsubject.Valid = (byte)(7 - Math.Min(7, (int)((DateTime.Now - dateread).TotalDays)));
                    }

                    list.Add(mailsubject);
                }

                return list;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return list;
            }
            finally
            {                
                if (reader != null) reader.Close();
            }
        }


        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     Sender=?Sender
        /// AND 
        ///     IsOutbox=1;
        /// </Sql>
        public IEnumerable<Mail> GetOutboxMail(Character target)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_37, connection);
            command.Parameters.AddWithValue("Sender", target.Name);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // 0 - MailId
                    // 1 - Sender
                    // 2 - Receiptent
                    // 3 - Date
                    // 4 - IsRead
                    // 5 - IsChecked
                    // 6 - Topic
                    // 7 - Message
                    // 8 - Attachment
                    // 9 - Zeny


                    Mail mailsubject = new Mail();
                    mailsubject.MailId = reader.GetUInt32(0);
                    mailsubject.Sender = reader.GetString(1);
                    mailsubject.Time = reader.GetDateTime(3);
                    mailsubject.IsRead = reader.GetByte(4);
                    mailsubject.Topic = reader.GetString(6);
                    mailsubject.Message = reader.GetString(7);
                    mailsubject.Zeny = (int)reader.GetUInt32(9);


                    if (reader.IsDBNull(8) == false)
                    {
                        byte[] buffer = new byte[67];
                        reader.GetBytes(8, 0, buffer, 0, 67);
                        Rag2Item.Deserialize(out mailsubject.Attachment, buffer, 0);
                    }
                    if (mailsubject.IsRead == 0)
                    {
                        mailsubject.Valid = (byte)(30 - Math.Min(30, (int)((DateTime.Now - mailsubject.Time).TotalDays)));
                    }
                    yield return mailsubject;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }


        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     Receiptent=?Receiptent
        /// AND 
        ///     IsInbox=1;
        /// </Sql>
        public int GetInboxMailCount(MySqlConnection connection, string name)
        {
            MySqlCommand command = new MySqlCommand(_query_38, connection);
            command.Parameters.AddWithValue("Receiptent", name);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    return (int)reader.GetUInt32(0);
                }

                return 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }



        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     Receiptent=?Receiptent
        /// AND 
        ///     IsInbox=1;
        /// AND
        ///     IsChecked=0;
        /// </Sql>
        public int GetInboxUncheckedCount(MySqlConnection connection, string name)
        {
            MySqlCommand command = new MySqlCommand(_query_39, connection);
            command.Parameters.AddWithValue("Receiptent", name);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    return (int)reader.GetUInt32(0);
                }

                return 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }


        /// <Sql>
        /// SELECT 
        ///     COUNT(*)
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     Receiptent=?Receiptent
        /// AND 
        ///     IsOutbox=1;
        /// </Sql>
        public int GetOutboxMailCount(Character target)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_40, connection);
            command.Parameters.AddWithValue("Sender", target.Name);
            MySqlDataReader reader = null;
            int count = 0;

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    count = (int)reader.GetUInt32(0);
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }

            return count;
        }


        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM
        ///     list_maildata
        /// WHERE
        ///     list_maildata.MailId=1;
        /// </Sql>
        public uint GetZenyAttachment(uint MailId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_41, connection);
            command.Parameters.AddWithValue("Id", MailId);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    return reader.GetUInt32(0);
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }

            return 0;
        }


        /// <Sql>
        /// UPDATE
        ///     list_maildata
        /// SET
        ///     Zeny=?Zeny 
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool UpdateZenyAttachment(uint MailId, uint Zeny)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_42, connection);
            command.Parameters.AddWithValue("Id", MailId);
            command.Parameters.AddWithValue("Zeny", Zeny);

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
                ConnectionPool.Release(connection);
            }
        }


        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM
        ///     list_maildata
        /// WHERE
        ///     list_maildata.MailId=1;
        /// </Sql>
        public Rag2Item GetItemAttachment(uint MailId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_43, connection);
            command.Parameters.AddWithValue("Id", MailId);
            MySqlDataReader reader = null;

            try
            {

                byte[] buffer = (byte[])command.ExecuteScalar();
                if (buffer != null)
                {
                    Rag2Item item;
                    if (Rag2Item.Deserialize(out item, buffer, 0))
                        return item;
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        /// <Sql>
        /// UPDATE
        ///     list_maildata
        /// SET
        ///     Attachment=?Attachment 
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool UpdateItemAttachment(uint MailId, Rag2Item Attachment)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_44, connection);
            command.Parameters.AddWithValue("Id", MailId);
            command.Parameters.AddWithValue("Attachment", Attachment);

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
                ConnectionPool.Release(connection);
            }
        }


        /// <Sql>
        /// INSERT INTO
        ///     list_maildata
        /// (
        ///     Sender,
        ///     Receiptent,
        ///     Date,
        ///     Topic,
        ///     Message,           
        ///     Attachment,
        ///     Zeny
        /// ) VALUES (
        ///     @Sender,
        ///     @Receiptent,
        ///     @Date,
        ///     @Topic,
        ///     @Attachment,
        ///     @Zeny
        ///  ) 
        /// </Sql>
        public bool InsertNewMailItem(Character target, MailItem item)
        {
            byte[] buffer = null;
            if (item.item != null)
            {
                buffer = new byte[67];
                Rag2Item.Serialize(item.item, buffer, 0);
            }

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_45, connection);
            command.Parameters.AddWithValue("Sender", (target != null) ? target.Name : string.Empty);
            command.Parameters.AddWithValue("Receiptent", item.Recieptent);
            command.Parameters.AddWithValue("Date", item.Timestamp);
            command.Parameters.AddWithValue("Topic", item.Topic);
            command.Parameters.AddWithValue("Message", item.Content);
            command.Parameters.AddWithValue("Zeny", item.Zeny);
            command.Parameters.AddWithValue("Attachment", buffer);            

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
                ConnectionPool.Release(connection);
            }
        }

        /// <Sql>
        /// UPDATE 
        ///     list_maildata 
        /// SET 
        ///     IsRead=1
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool MarkAsReadMailItem(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_46, connection);
            command.Parameters.AddWithValue("Id", id);

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
                ConnectionPool.Release(connection);
            }
        }


        /// <Sql>
        /// SELECT 
        ///     * 
        /// FROM 
        ///     list_maildata 
        /// WHERE 
        ///     InboxId=?Id 
        /// LIMIT 1;
        /// </Sql>
        public MailItem GetMailItemById(MySqlConnection connection, uint id)
        {
            MySqlCommand command = new MySqlCommand(_query_47, connection);
            command.Parameters.AddWithValue("Id", id);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader(); // argument CommandBehavior.SingleRow removed (Darkin)
                while (reader.Read())
                {
                    MailItem item = new MailItem();
                    item.Recieptent = reader.GetString(2);
                    item.Topic = reader.GetString(6);
                    item.Content = reader.GetString(7);
                    item.Timestamp = reader.GetDateTime(3);
                    item.Zeny = reader.GetUInt32(9);
                    if (reader.IsDBNull(8) == false)
                    {
                        byte[] buffer = new byte[67];
                        reader.GetBytes(8, 0, buffer, 0, 67);
                        Rag2Item.Deserialize(out item.item, buffer, 0);
                    }

                    //Singleton.Item.TryGetItemWithCount(27, 1, out item.item);
                    return item;
                }
                return null;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }



        /// <Sql>
        /// SELECT 
        ///     list_maildata.Receiptent,
        ///     COUNT(list_maildata.MailId) 
        /// FROM 
        ///     list_maildata
        /// WHERE 
        ///     IsPending=0 
        /// LIMIT 300;
        /// </Sql>
        public IEnumerable<KeyValuePair<string, uint>> GetPendingMails()
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_48, connection);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return new KeyValuePair<string, uint>
                    (
                        reader.GetString(0),
                        reader.GetUInt32(1)
                    );
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }


        /// <Sql>
        /// UPDATE
        ///     list_maildata
        /// SET
        ///     IsPending=0
        ///     IsInbox=1
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool ClearPendingMails(MySqlConnection connection, string Receiptent)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = _query_49;
            command.Connection = connection;
            command.Parameters.AddWithValue("Receiptent", Receiptent);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }


        /// <Sql>
        /// DELETE FROM
        ///     list_maildata
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool DeleteMails(MySqlConnection connection, uint id)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = _query_50;
            command.Connection = connection;
            command.Parameters.AddWithValue("Id", id);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }


        /// <Sql>
        /// UPDATE 
        ///     list_maildata
        /// SET 
        ///     IsOutbox=0
        /// WHERE 
        ///     MailId=?Id
        /// </Sql>
        public bool DeleteMailFromOutbox(MySqlConnection connection, uint id)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = _query_51;
            command.Parameters.AddWithValue("Id", id);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

    }
}
