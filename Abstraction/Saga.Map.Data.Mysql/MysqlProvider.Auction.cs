using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Saga.Map.Definitions.Misc;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.PrimaryTypes;
using System.Diagnostics;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        public bool DeleteRegisteredAuctionItem(MySqlConnection connection, uint id)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = _query_06;
            command.Connection = connection;
            command.Parameters.AddWithValue("AuctionId", id);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Find a comment by a given player id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindCommentByPlayerId(MySqlConnection connection, uint id)
        {
            StringBuilder mysqlcommand = new StringBuilder("SELECT Comment FROM auction_comment WHERE");
            mysqlcommand.AppendFormat(" CharId='{0}'", id);
            mysqlcommand.AppendFormat(" LIMIT 1;");

            MySqlCommand command = new MySqlCommand(mysqlcommand.ToString(), connection);
            MySqlDataReader reader = command.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }

                return string.Empty;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return string.Empty;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Find comment of a given item id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindCommentById(MySqlConnection connection, uint id)
        {
            StringBuilder mysqlcommand = new StringBuilder("SELECT auction_comment.Comment FROM auction LEFT OUTER JOIN auction_comment ON auction.CharId = auction_comment.CharId WHERE ");
            mysqlcommand.AppendFormat(" AuctionId='{0}'", id);
            mysqlcommand.AppendFormat(" LIMIT 1;");

            MySqlCommand command = new MySqlCommand(mysqlcommand.ToString(), connection);
            MySqlDataReader reader = command.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }

                return string.Empty;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return string.Empty;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public int GetOwnerItemCount(Character target)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_07, connection);
            command.Parameters.AddWithValue("CharName", target.Name);
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
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }

            return count;
        }

        public bool GetItemByAuctionId(MySqlConnection connection, uint id, out AuctionArgument item)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = _query_08;
            command.Connection = connection;
            command.Parameters.AddWithValue("AuctionId", id);
            MySqlDataReader reader = null;
            item = null;

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    item = new AuctionArgument();
                    item.name = reader.GetString(4);
                    item.zeny = reader.GetUInt32(7);

                    byte[] buffer = new byte[67];
                    reader.GetBytes(8, 0, buffer, 0, 67);
                    Rag2Item.Deserialize(out item.item, buffer, 0);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return false;
        }
        
        /// <summary>
        /// Search market
        /// </summary>
        /// <param name="Argument"></param>
        /// <returns></returns>
        public IEnumerable<MarketItemArgument> SearchMarket(MarketSearchArgument Argument)
        {
            StringBuilder mysqlcommand = new StringBuilder("SELECT * FROM `auction` WHERE ");
            mysqlcommand.AppendFormat("Categorie={0} ", Argument.item_cat);
            if (Argument.min_clvl > 0) mysqlcommand.AppendFormat("AND ReqClvl >= {0} ", Argument.min_clvl);
            if (Argument.max_clvl > 0) mysqlcommand.AppendFormat("AND ReqClvl <= {0} ", Argument.max_clvl);
            if (Argument.searchType > 0) mysqlcommand.AppendFormat("AND `{0}` LIKE '%{1}%' ", (Argument.searchType == 2) ? "CharName" : "ItemName", Argument.searchstring);
            if (Argument.item_class > 0) mysqlcommand.AppendFormat("AND Grade = {0} ", Argument.max_clvl);
            switch (Argument.SortBy)
            {
                case 0: mysqlcommand.AppendFormat("ORDER BY ItemName ASC ", Argument.max_clvl); break;
                case 1: mysqlcommand.AppendFormat("ORDER BY ItemName DESC ", Argument.max_clvl); break;
                case 2: mysqlcommand.AppendFormat("ORDER BY CharName ASC ", Argument.max_clvl); break;
                case 3: mysqlcommand.AppendFormat("ORDER BY CharName DESC ", Argument.max_clvl); break;
                case 4: mysqlcommand.AppendFormat("ORDER BY Price ASC ", Argument.max_clvl); break;
                case 5: mysqlcommand.AppendFormat("ORDER BY Price DESC ", Argument.max_clvl); break;
                case 6: mysqlcommand.AppendFormat("ORDER BY ReqClvl ASC ", Argument.max_clvl); break;
                case 7: mysqlcommand.AppendFormat("ORDER BY ReqClvl DESC ", Argument.max_clvl); break;
                default: mysqlcommand.AppendFormat("ORDER BY CharName ASC ", Argument.max_clvl); break;
            }
            mysqlcommand.AppendFormat("LIMIT {0}, 36", Argument.index * 36);


            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(mysqlcommand.ToString(), connection);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    byte[] buffer = new byte[67];
                    MarketItemArgument item = new MarketItemArgument();                    
                                       
                    item.id = reader.GetUInt32(0);
                    item.sender = reader.GetString(4);
                    item.price = reader.GetUInt32(7);
                    item.expires = reader.GetMySqlDateTime(9).GetDateTime();
                    reader.GetBytes(8, 0, buffer, 0, 67);
                    Rag2Item.Deserialize(out item.item, buffer, 0);
                    
                    yield return item;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        /// <summary>
        /// Search for owner market items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MarketItemArgument> SearchMarketForOwner(Character target)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_09, connection);
            command.Parameters.AddWithValue("?CharId", target.ModelId);
            MySqlDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    byte[] buffer = new byte[67];
                    MarketItemArgument item = new MarketItemArgument();     
                    item.id = reader.GetUInt32(0);
                    item.sender = reader.GetString(4);
                    item.price = reader.GetUInt32(7);
                    item.expires = reader.GetMySqlDateTime(9).GetDateTime();
                    reader.GetBytes(8, 0, buffer, 0, 67);
                    Rag2Item.Deserialize(out item.item, buffer, 0);
                    yield return item;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        /// <summary>
        /// Registers a new market item
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public uint RegisterNewMarketItem(Character target, MarketItemArgument arg)
        {
            MySqlConnection connection = ConnectionPool.Request();
            uint result = 0;

            try
            {
                byte[] buffer = new byte[67];
                Rag2Item.Serialize(arg.item, buffer, 0);

                MySqlCommand command = new MySqlCommand(_query_10, connection);
                command.Parameters.AddWithValue("Categorie", arg.item.info.categorie);
                command.Parameters.AddWithValue("Grade", 0);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                command.Parameters.AddWithValue("CharName", arg.sender);
                command.Parameters.AddWithValue("ItemName", arg.item.info.name);
                command.Parameters.AddWithValue("ReqClvl", arg.item.clvl);
                command.Parameters.AddWithValue("Price", arg.price);
                command.Parameters.AddWithValue("ItemContent", buffer);
                command.Parameters.AddWithValue("Expires", arg.expires);

                command.ExecuteNonQuery();
                result = Convert.ToUInt32(command.LastInsertedId);
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return result;
        }

        /// <summary>
        /// Deregister a new market item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint UnregisterMarketItem(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                MySqlCommand command = new MySqlCommand(_query_11, connection);
                command.Parameters.AddWithValue("AuctionId", id);
                return (uint)command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return 0;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        /// <summary>
        /// Update the current comment of a given characterid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public uint UpdateCommentByPlayerId(uint id, string message)
        {
            StringBuilder mysqlcommand = new StringBuilder("INSERT INTO auction_comment VALUES ");
            mysqlcommand.AppendFormat("('{0}',", id);
            mysqlcommand.AppendFormat("'{0}')", message);
            mysqlcommand.AppendFormat("ON duplicate KEY UPDATE ");
            mysqlcommand.AppendFormat("Comment='{0}'", message);

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(mysqlcommand.ToString(), connection);
            uint result = 0;

            try
            {
                result = Convert.ToUInt32(command.ExecuteNonQuery());
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return result;
        }        

    }
}
