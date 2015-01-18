using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Saga.Authentication.Utils.Definitions.Misc;
using Saga.Data;

namespace Saga.Authentication.Data.Mysql
{
    partial class MysqlProvider
    {

        /// <summary>
        /// Create a new connection pool with the limit of 5 connections.
        /// </summary>
        public MysqlProvider()
        {
            ConnectionPool = new Pool<MySqlConnection>(5);
        }

        /// <summary>
        /// Connection info required to login
        /// </summary>
        private ConnectionInfo info;

        /// <summary>
        /// Connection pool
        /// </summary>
        private Pool<MySqlConnection> ConnectionPool;


    }
}
