using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Saga.Authentication.Data.Mysql
{
    partial class MysqlProvider
    {

        /// <summary>
        /// Checks the mysql server version
        /// </summary>
        /// <returns></returns>
        public bool CheckServerVersion()
        {
            MySqlConnection connection = ConnectionPool.Request();
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
                        return true;
                    }


                    return true;
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
            finally
            {
                ConnectionPool.Release( connection);
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
                    "list_acl",
                    "list_worldcharacters",
                    "list_worlds",
                    "login"
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
                        Trace.TraceError("Table mising: {0}", requiredTables[i]);
                        installed = false;
                    }
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;

            command = new MySqlCommand("SHOW FIELDS FROM list_acl;", connection);
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
                            if (name != "RuleId") Trace.TraceError("Table list_acl missing field RuleId");
                            break;
                        case 1:
                            if (name != "FilterIp") Trace.TraceError("Table list_acl missing field FilterIp");
                            break;
                        case 2:
                            if (name != "Mask") Trace.TraceError("Table list_acl missing field Mask");
                            break;
                        case 3:
                            if (name != "Operation") Trace.TraceError("Table list_acl missing field Operation");
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

            command = new MySqlCommand("SHOW FIELDS FROM list_worldcharacters;", connection);
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
                            if (name != "UserId") Trace.TraceError("Table list_worldcharacters missing field UserId");
                            break;
                        case 1:
                            if (name != "WorldId") Trace.TraceError("Table list_worldcharacters missing field WorldId");
                            break;
                        case 2:
                            if (name != "CharacterCount") Trace.TraceError("Table list_worldcharacters missing field CharacterCount");
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

            command = new MySqlCommand("SHOW FIELDS FROM list_worlds;", connection);
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
                            if (name != "Id") Trace.TraceError("Table list_worlds missing field Id");
                            break;
                        case 1:
                            if (name != "Name") Trace.TraceError("Table list_worlds missing field Name");
                            break;
                        case 2:
                            if (name != "Proof") Trace.TraceError("Table list_worlds missing field Proof");
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

            command = new MySqlCommand("SHOW FIELDS FROM login;", connection);
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
                            if (name != "UserId") Trace.TraceError("Table login missing field UserId");
                            break;
                        case 1:
                            if (name != "Username") Trace.TraceError("Table login missing field Username");
                            break;
                        case 2:
                            if (name != "Password") Trace.TraceError("Table login missing field Password");
                            break;
                        case 3:
                            if (name != "Gender") Trace.TraceError("Table login missing field Gender");
                            break;
                        case 4:
                            if (name != "LastUserIP") Trace.TraceError("Table login missing field LastUserIP");
                            break;
                        case 5:
                            if (name != "LastSession") Trace.TraceError("Table login missing field LastSession");
                            break;
                        case 6:
                            if (name != "ActiveSession") Trace.TraceError("Table login missing field ActiveSession");
                            break;
                        case 7:
                            if (name != "LastLoginDate") Trace.TraceError("Table login missing field LastLoginDate");
                            break;
                        case 8:
                            if (name != "IsActivated") Trace.TraceError("Table login missing field IsActivated");
                            break;
                        case 9:
                            if (name != "IsBanned") Trace.TraceError("Table login missing field IsBanned");
                            break;
                        case 10:
                            if (name != "HasAgreed") Trace.TraceError("Table login missing field HasAgreed");
                            break;
                        case 11:
                            if (name != "DateOfBirth") Trace.TraceError("Table login missing field DateOfBirth");
                            break;
                        case 12:
                            if (name != "DateOfRegistration") Trace.TraceError("Table login missing field DateOfRegistration");
                            break;
                        case 13:
                            if (name != "IsTestAccount") Trace.TraceError("Table login missing field IsTestAccount");
                            break;
                        case 14:
                            if (name != "LastPlayedWorld") Trace.TraceError("Table login missing field LastPlayedWorld");
                            break;
                        case 15:
                            if (name != "GmLevel") Trace.TraceError("Table login missing field GmLevel");
                            break;
                        case 16:
                            if (name != "GameTime") Trace.TraceError("Table login missing field GameTime");
                            break;
                    }

                    if (++i >= 16)
                        installed = true;
                    if (installed == true)
                        break;
                }
            }

            //Abaort if tables were not even installed correct
            if (installed == false)
                return false;


            return true;

 

        }

    }
}
