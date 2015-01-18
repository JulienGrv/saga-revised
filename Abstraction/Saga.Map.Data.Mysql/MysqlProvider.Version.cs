using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider 
    {
        public bool CheckServerVersion(MySqlConnection connection)
        {
            try
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"([0-9])\.([0-9])\.([0-9]).*?");
                System.Text.RegularExpressions.Match match = regex.Match(connection.ServerVersion);

                if (match.Success)
                {
                    int Major = int.Parse(match.Groups[1].Value);
                    int Minor = int.Parse(match.Groups[2].Value);
                    int Revision = int.Parse(match.Groups[3].Value);
                    if (Major < 5)
                    {
                        __dbtracelog.WriteError("Database", "You are not running a compatible mysql versions. Please use mysql 5.0 (depreciated), 5.1 or 6.0");
                        return false;
                    }
                    else if (Major == 5 && Minor < 1)
                    {
                        __dbtracelog.WriteWarning("Database", "You are running mysql 5.0 keep in mind that for best performance mysql 5.1 or 6.0 is better");
                        __canUseStoredProcedures = false;
                        return true;
                    }
                    else
                    {
                        __canUseStoredProcedures = true;
                        return true;
                    }
                }
                else
                {
                    __dbtracelog.WriteError("Database", "Could not match the mysql version string, version treated incorrect by default");
                    return false;
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        public bool CheckDatabaseFields(MySqlConnection connection)
        {
            MySqlCommand command;
            bool installed = true;

            command = new MySqlCommand("SHOW TABLES;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                string[] requiredTables = new string[] {
                    "auction",
                    "auction_comment",
                    "characters",
                    "list_additions",
                    "list_blacklist",
                    "list_equipment",
                    "list_eventparticipants",
                    "list_eventrewards",
                    "list_friends",
                    "list_inventory",
                    "list_joblinformation",
                    "list_learnedskills",
                    "list_maildata",
                    "list_questhistory",
                    "list_quests",
                    "list_queststates",
                    "list_specialskills",
                    "list_storage",
                    "list_weaponary",
                    "list_zoneinformation"
                };

                bool[] tablesFound = new bool[requiredTables.Length];

                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    for (int i = 0; i < requiredTables.Length; i++)
                    {
                        if (requiredTables[i] == name)
                            tablesFound[i] = true;
                    }
                }

                for (int i = 0; i < requiredTables.Length; i++)
                {
                    if (tablesFound[i] == false)
                    {
                        __dbtracelog.WriteError("Database", "Table mising: {0}", requiredTables[i]);
                        installed = false;
                    }
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM auction;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "AuctionId") __dbtracelog.WriteError("Database", "Table auction missing field AuctionId");
                            break;
                        case 1:
                            if (name != "Categorie") __dbtracelog.WriteError("Database", "Table auction missing field Categorie");
                            break;
                        case 2:
                            if (name != "Grade") __dbtracelog.WriteError("Database", "Table auction missing field Grade");
                            break;
                        case 3:
                            if (name != "CharId") __dbtracelog.WriteError("Database", "Table auction missing field CharId");
                            break;
                        case 4:
                            if (name != "CharName") __dbtracelog.WriteError("Database", "Table auction missing field CharName");
                            break;
                        case 5:
                            if (name != "ItemName") __dbtracelog.WriteError("Database", "Table auction missing field ItemName");
                            break;
                        case 6:
                            if (name != "ReqClvl") __dbtracelog.WriteError("Database", "Table auction missing field ReqClvl");
                            break;
                        case 7:
                            if (name != "Price") __dbtracelog.WriteError("Database", "Table auction missing field Price");
                            break;
                        case 8:
                            if (name != "ItemContent") __dbtracelog.WriteError("Database", "Table auction missing field ItemContent");
                            break;
                        case 9:
                            if (name != "Expires") __dbtracelog.WriteError("Database", "Table auction missing field Expires");
                            break;
                    }

                    if (++i >= 9)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM auction_comment;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database", "Table auction_comment missing field CharId");
                            break;
                        case 1:
                            if (name != "Comment") __dbtracelog.WriteError("Database", "Table auction_comment missing field Comment");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM characters;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database", "Table characters missing field CharId");
                            break;
                        case 1:
                            if (name != "CharName") __dbtracelog.WriteError("Database", "Table characters missing field CharName");
                            break;
                        case 2:
                            if (name != "CharFace") __dbtracelog.WriteError("Database","Table characters missing field CharFace");
                            break;
                        case 3:
                            if (name != "UserId") __dbtracelog.WriteError("Database","Table characters missing field UserId");
                            break;
                        case 4:
                            if (name != "Cexp") __dbtracelog.WriteError("Database","Table characters missing field Cexp");
                            break;
                        case 5:
                            if (name != "Jexp") __dbtracelog.WriteError("Database","Table characters missing field Jexp");
                            break;
                        case 6:
                            if (name != "Job") __dbtracelog.WriteError("Database","Table characters missing field Job");
                            break;
                        case 7:
                            if (name != "Map") __dbtracelog.WriteError("Database","Table characters missing field Map");
                            break;
                        case 8:
                            if (name != "Gender") __dbtracelog.WriteError("Database","Table characters missing field Gender");
                            break;
                        case 9:
                            if (name != "HP") __dbtracelog.WriteError("Database","Table characters missing field HP");
                            break;
                        case 10:
                            if (name != "SP") __dbtracelog.WriteError("Database","Table characters missing field SP");
                            break;
                        case 11:
                            if (name != "LP") __dbtracelog.WriteError("Database","Table characters missing field LP");
                            break;
                        case 12:
                            if (name != "LC") __dbtracelog.WriteError("Database","Table characters missing field LC");
                            break;
                        case 13:
                            if (name != "Position.x") __dbtracelog.WriteError("Database","Table characters missing field Position.x");
                            break;
                        case 14:
                            if (name != "Position.y") __dbtracelog.WriteError("Database","Table characters missing field Position.y");
                            break;
                        case 15:
                            if (name != "Position.z") __dbtracelog.WriteError("Database","Table characters missing field Position.z");
                            break;
                        case 16:
                            if (name != "Saveposition.x") __dbtracelog.WriteError("Database","Table characters missing field Saveposition.x");
                            break;
                        case 17:
                            if (name != "Saveposition.y") __dbtracelog.WriteError("Database","Table characters missing field Saveposition.y");
                            break;
                        case 18:
                            if (name != "Saveposition.z") __dbtracelog.WriteError("Database","Table characters missing field Saveposition.z");
                            break;
                        case 19:
                            if (name != "Saveposition.map") __dbtracelog.WriteError("Database","Table characters missing field Saveposition.map");
                            break;
                        case 20:
                            if (name != "Stats.Str") __dbtracelog.WriteError("Database","Table characters missing field Stats.Str");
                            break;
                        case 21:
                            if (name != "Stats.Dex") __dbtracelog.WriteError("Database","Table characters missing field Stats.Dex");
                            break;
                        case 22:
                            if (name != "Stats.Int") __dbtracelog.WriteError("Database","Table characters missing field Stats.Int");
                            break;
                        case 23:
                            if (name != "Stats.Con") __dbtracelog.WriteError("Database","Table characters missing field Stats.Con");
                            break;
                        case 24:
                            if (name != "Stats.Luc") __dbtracelog.WriteError("Database","Table characters missing field Stats.Luc");
                            break;
                        case 25:
                            if (name != "Stats.Pending") __dbtracelog.WriteError("Database","Table characters missing field Stats.Pending");
                            break;
                        case 26:
                            if (name != "Rufi") __dbtracelog.WriteError("Database","Table characters missing field Rufi");
                            break;
                        case 27:
                            if (name != "UppercasedCharName") __dbtracelog.WriteError("Database","Table characters missing field UppercasedCharName");
                            break;
                    }

                    if (++i >= 27)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_additions;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_additions missing field CharId");
                            break;
                        case 1:
                            if (name != "Additions") __dbtracelog.WriteError("Database","Table list_additions missing field Additions");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_blacklist;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_blacklist missing field CharId");
                            break;
                        case 1:
                            if (name != "FriendName") __dbtracelog.WriteError("Database","Table list_blacklist missing field FriendName");
                            break;
                        case 2:
                            if (name != "Reason") __dbtracelog.WriteError("Database","Table list_blacklist missing field Reason");
                            break;
                    }

                    if (++i >= 2)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_equipment;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_equipment missing field CharId");
                            break;
                        case 1:
                            if (name != "Equipement") __dbtracelog.WriteError("Database","Table list_equipment missing field Equipement");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_eventparticipants;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "EventId") __dbtracelog.WriteError("Database","Table list_eventparticipants missing field EventId");
                            break;
                        case 1:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_eventparticipants missing field CharId");
                            break;
                        case 2:
                            if (name != "CharName") __dbtracelog.WriteError("Database","Table list_eventparticipants missing field CharName");
                            break;
                    }

                    if (++i >= 2)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_eventrewards;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "RewardId") __dbtracelog.WriteError("Database","Table list_eventrewards missing field RewardId");
                            break;
                        case 1:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_eventrewards missing field CharId");
                            break;
                        case 2:
                            if (name != "ItemId") __dbtracelog.WriteError("Database","Table list_eventrewards missing field ItemId");
                            break;
                        case 3:
                            if (name != "ItemCount") __dbtracelog.WriteError("Database","Table list_eventrewards missing field ItemCount");
                            break;
                    }

                    if (++i >= 3)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_friends;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_friends missing field CharId");
                            break;
                        case 1:
                            if (name != "FriendName") __dbtracelog.WriteError("Database","Table list_friends missing field FriendName");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_inventory;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_inventory missing field CharId");
                            break;
                        case 1:
                            if (name != "ContainerMaxStorage") __dbtracelog.WriteError("Database","Table list_inventory missing field ContainerMaxStorage");
                            break;
                        case 2:
                            if (name != "Container") __dbtracelog.WriteError("Database","Table list_inventory missing field Container");
                            break;
                    }

                    if (++i >= 2)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_joblinformation;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_joblinformation missing field CharId");
                            break;
                        case 1:
                            if (name != "JobInformation") __dbtracelog.WriteError("Database","Table list_joblinformation missing field JobInformation");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_learnedskills;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_learnedskills missing field CharId");
                            break;
                        case 1:
                            if (name != "SkillId") __dbtracelog.WriteError("Database","Table list_learnedskills missing field SkillId");
                            break;
                        case 2:
                            if (name != "SkillExp") __dbtracelog.WriteError("Database","Table list_learnedskills missing field SkillExp");
                            break;
                        case 3:
                            if (name != "Job") __dbtracelog.WriteError("Database","Table list_learnedskills missing field Job");
                            break;
                    }

                    if (++i >= 3)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_maildata;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "MailId") __dbtracelog.WriteError("Database","Table list_maildata missing field MailId");
                            break;
                        case 1:
                            if (name != "Sender") __dbtracelog.WriteError("Database","Table list_maildata missing field Sender");
                            break;
                        case 2:
                            if (name != "Receiptent") __dbtracelog.WriteError("Database","Table list_maildata missing field Receiptent");
                            break;
                        case 3:
                            if (name != "Date") __dbtracelog.WriteError("Database","Table list_maildata missing field Date");
                            break;
                        case 4:
                            if (name != "IsRead") __dbtracelog.WriteError("Database","Table list_maildata missing field IsRead");
                            break;
                        case 5:
                            if (name != "IsChecked") __dbtracelog.WriteError("Database","Table list_maildata missing field IsChecked");
                            break;
                        case 6:
                            if (name != "Topic") __dbtracelog.WriteError("Database","Table list_maildata missing field Topic");
                            break;
                        case 7:
                            if (name != "Message") __dbtracelog.WriteError("Database","Table list_maildata missing field Message");
                            break;
                        case 8:
                            if (name != "Attachment") __dbtracelog.WriteError("Database","Table list_maildata missing field Attachment");
                            break;
                        case 9:
                            if (name != "Zeny") __dbtracelog.WriteError("Database","Table list_maildata missing field Zeny");
                            break;
                        case 10:
                            if (name != "DateRead") __dbtracelog.WriteError("Database","Table list_maildata missing field DateRead");
                            break;
                        case 11:
                            if (name != "IsOutbox") __dbtracelog.WriteError("Database","Table list_maildata missing field IsOutbox");
                            break;
                        case 12:
                            if (name != "IsInbox") __dbtracelog.WriteError("Database","Table list_maildata missing field IsInbox");
                            break;
                        case 13:
                            if (name != "IsPending") __dbtracelog.WriteError("Database","Table list_maildata missing field IsPending");
                            break;
                    }

                    if (++i >= 13)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_questhistory;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_questhistory missing field CharId");
                            break;
                        case 1:
                            if (name != "QuestId") __dbtracelog.WriteError("Database","Table list_questhistory missing field QuestId");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_quests;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "QuestId") __dbtracelog.WriteError("Database","Table list_quests missing field QuestId");
                            break;
                        case 1:
                            if (name != "Region") __dbtracelog.WriteError("Database","Table list_quests missing field Region");
                            break;
                        case 2:
                            if (name != "Req_Clvl") __dbtracelog.WriteError("Database","Table list_quests missing field Req_Clvl");
                            break;
                        case 3:
                            if (name != "Req_Jlvl") __dbtracelog.WriteError("Database","Table list_quests missing field Req_Jlvl");
                            break;
                        case 4:
                            if (name != "Req_Quest") __dbtracelog.WriteError("Database","Table list_quests missing field Req_Quest");
                            break;
                        case 5:
                            if (name != "QuestType") __dbtracelog.WriteError("Database","Table list_quests missing field QuestType");
                            break;
                        case 6:
                            if (name != "NPC") __dbtracelog.WriteError("Database","Table list_quests missing field NPC");
                            break;
                    }

                    if (++i >= 6)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_queststates;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharID") __dbtracelog.WriteError("Database","Table list_queststates missing field CharID");
                            break;
                        case 1:
                            if (name != "State") __dbtracelog.WriteError("Database","Table list_queststates missing field State");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_specialskills;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_specialskills missing field CharId");
                            break;
                        case 1:
                            if (name != "Skills") __dbtracelog.WriteError("Database","Table list_specialskills missing field Skills");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_storage;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_storage missing field CharId");
                            break;
                        case 1:
                            if (name != "ContainerMaxStorage") __dbtracelog.WriteError("Database","Table list_storage missing field ContainerMaxStorage");
                            break;
                        case 2:
                            if (name != "Container") __dbtracelog.WriteError("Database","Table list_storage missing field Container");
                            break;
                    }

                    if (++i >= 2)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_weaponary;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_weaponary missing field CharId");
                            break;
                        case 1:
                            if (name != "Weaponary") __dbtracelog.WriteError("Database","Table list_weaponary missing field Weaponary");
                            break;
                        case 2:
                            if (name != "UnlockedWeaponCount") __dbtracelog.WriteError("Database","Table list_weaponary missing field UnlockedWeaponCount");
                            break;
                        case 3:
                            if (name != "PrimairyWeapon") __dbtracelog.WriteError("Database","Table list_weaponary missing field PrimairyWeapon");
                            break;
                        case 4:
                            if (name != "SecondaryWeapoin") __dbtracelog.WriteError("Database","Table list_weaponary missing field SecondaryWeapoin");
                            break;
                        case 5:
                            if (name != "ActiveWeaponIndex") __dbtracelog.WriteError("Database","Table list_weaponary missing field ActiveWeaponIndex");
                            break;
                    }

                    if (++i >= 5)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_zoneinformation;", connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = false;
                int i = 0;
                while (reader.Read())
                {
                    string name = reader.GetString(0);

                    switch (i)
                    {
                        case 0:
                            if (name != "CharId") __dbtracelog.WriteError("Database","Table list_zoneinformation missing field CharId");
                            break;
                        case 1:
                            if (name != "ZoneState") __dbtracelog.WriteError("Database","Table list_zoneinformation missing field ZoneState");
                            break;
                    }

                    if (++i >= 1)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            if (__canUseStoredProcedures == false) 
                return true;

            command = new MySqlCommand("SHOW PROCEDURE STATUS WHERE `Db`=@Db", connection);
            command.Parameters.AddWithValue("Db", connection.Database);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                installed = true;

                string[] requiredTables = new string[] {
                    "list_availablepersonalrequests",
                    "list_availablequests"
                };

                bool[] tablesFound = new bool[requiredTables.Length];

                while (reader.Read())
                {
                    string name = reader.GetString(1);
                    for (int i = 0; i < requiredTables.Length; i++)
                    {
                        if (requiredTables[i] == name)
                            tablesFound[i] = true;
                    }
                }

                for (int i = 0; i < requiredTables.Length; i++)
                {
                    if (tablesFound[i] == false)
                    {
                        __dbtracelog.WriteError("Database","Stored procedure mising: {0}", requiredTables[i]);
                        installed = false;
                    }
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;
            

            return true;
        }

    }
}
