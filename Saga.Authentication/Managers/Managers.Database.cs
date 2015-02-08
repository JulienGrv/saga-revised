using Saga.Authentication.Structures;
using Saga.Authentication.Utils.Definitions.Misc;
using Saga.Configuration;
using Saga.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace Saga.Managers
{
    public class Database : ManagerBase2
    {
        #region Ctor/Dtor

        public Database()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        internal IDatabase InternalDatabaseProvider;

        private ConnectionInfo info;

        #endregion Internal Members

        #region Protected Methods

        protected override void QuerySettings()
        {
            info = new ConnectionInfo();
            try
            {
                //CONTRUCT CONNECTION INFO
                DatabaseSettings section = (DatabaseSettings)ConfigurationManager.GetSection("Saga.Manager.Database");
                info.host = section.Host;
                info.password = section.Password;
                info.username = section.Username;
                info.database = section.Database;
                info.port = section.Port;
                info.pooledconnections = section.PooledConnections;

                //DISCOVER NEW TYPES
                object temp;
                string type = section.DbType;

                if (type != null)
                    if (type.Length > 0)
                        if (CoreService.TryFindType(type, out temp))
                            if (temp is IDatabase)
                                InternalDatabaseProvider = temp as IDatabase;
                            else
                            {
                                WriteError("DatabaseManager", "No manager founds");
                            }
                        //InternalDatabaseProvider = new MysqlBackend();
                        else
                        {
                            WriteError("DatabaseManager", "No manager found, missing .dll files");
                        }
                    //InternalDatabaseProvider = new MysqlBackend();
                    else
                    {
                        WriteError("DatabaseManager", "No manager founds");
                    }
                //InternalDatabaseProvider = new MysqlBackend();
                else
                {
                    WriteError("DatabaseManager", "Cannot initialize manager");
                }
                //InternalDatabaseProvider = new MysqlBackend();
            }
            catch (Exception)
            {
                WriteError("DatabaseManager", "Cannot initialize manager");
            }
        }

        protected override void Load()
        {
            try
            {
                //ADD THE DATABASE PROVIDER
                if (!InternalDatabaseProvider.Connect(info))
                {
                    WriteError("DatabaseManager", "Server failed to connect to the database server");
                }
                else if (!InternalDatabaseProvider.CheckServerVersion())
                {
                    WriteError("DatabaseManager", "Database version is incorrect");
                }
                else if (!InternalDatabaseProvider.CheckDatabaseFields())
                {
                    WriteError("DatabaseManager", "Database is missing tables or required table fields");
                }
            }
            catch (Exception e)
            {
                HostContext.AddUnhandeldException(e);
            }
        }

        #endregion Protected Methods

        #region Wrapped Methods

        public bool AddWorld(string name, string password, out int world)
        {
            return InternalDatabaseProvider.AddWorld(name, password, out world);
        }

        public bool RemoveWorld(string name)
        {
            return InternalDatabaseProvider.RemoveWorld(name);
        }

        public bool RenameWorld(string name, string newname)
        {
            return InternalDatabaseProvider.RenameWorld(name, newname);
        }

        public List<WorldInfo> GetWorldInformation()
        {
            return InternalDatabaseProvider.GetWorldInformation();
        }

        public bool Login(string username, out LoginResult result)
        {
            return InternalDatabaseProvider.Login(username, out result);
        }

        public bool GetCharacterWorldCount(uint userid, out Dictionary<byte, byte> CharPerWorld)
        {
            return InternalDatabaseProvider.GetCharacterWorldCount(userid, out CharPerWorld);
        }

        public bool UpdateCharacterOnWorld(uint userid, uint worldid)
        {
            return InternalDatabaseProvider.UpdateCharacterOnWorld(userid, worldid);
        }

        public bool RemoveCharacterOnWorld(uint userid, uint worldid)
        {
            return InternalDatabaseProvider.RemoveCharacterOnWorld(userid, worldid);
        }

        public List<AclEntry> ListAclEntries()
        {
            return InternalDatabaseProvider.ListAclEntries();
        }

        public List<AclEntry> FindMatchingAclEntries(string ip)
        {
            return InternalDatabaseProvider.FindAclEntries(ip);
        }

        public bool UpdateLoginEntry(uint AccountId, IPAddress address)
        {
            return InternalDatabaseProvider.UpdateLoginEntry(AccountId, address);
        }

        public bool CreateUserEntry(string username, string password, byte gender, int gmlevel)
        {
            return InternalDatabaseProvider.CreateLoginEntry(username, password, gender, gmlevel);
        }

        public bool UpdateSession(uint AccountId, uint session)
        {
            return InternalDatabaseProvider.UpdateSession(AccountId, session);
        }

        public bool UpdateLastplayerWorld(uint AccountId, byte WorldId)
        {
            return InternalDatabaseProvider.UpdateLastplayedWorld(AccountId, WorldId);
        }

        public bool ReleaseSessionId(uint sessionid)
        {
            return InternalDatabaseProvider.ReleaseSessionId(sessionid);
        }

        public int ClearWorldSessions(byte world)
        {
            return InternalDatabaseProvider.ClearSession(world);
        }

        public bool IsIpAllowed(IPAddress address)
        {
            if (InternalDatabaseProvider.IsDenied(address))
            {
                return InternalDatabaseProvider.IsAllowed(address);
            }

            return true;
        }

        public bool AddEntry(string ip, string mask, byte operation)
        {
            return InternalDatabaseProvider.AddAclEntry(ip, mask, operation);
        }

        public bool RemoveEntry(string ip, string mask, byte operation)
        {
            return InternalDatabaseProvider.RemoveAclEntry(ip, mask, operation);
        }

        public IPAddress GetAdressOfUser(uint userid)
        {
            return InternalDatabaseProvider.GetIpAddress(userid);
        }

        public long GetPlayedTimeOfUser(uint userid)
        {
            return InternalDatabaseProvider.GetGameTime(userid);
        }

        public List<string> GetAllCharactersOnline()
        {
            return InternalDatabaseProvider.GetAllCharactersOnline();
        }

        #endregion Wrapped Methods
    }
}

namespace Saga.Data
{
    public sealed class ConnectionInfo
    {
        public string host;
        public string password;
        public string username;
        public string database;
        public uint port;
        public int pooledconnections;
    }
}