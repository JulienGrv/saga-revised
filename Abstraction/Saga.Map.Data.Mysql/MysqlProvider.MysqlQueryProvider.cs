using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Data;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        class MysqlQueryProvider : Saga.Data.IQueryProvider
        {
            QueryParameterCollection param = new QueryParameterCollection();
            string cmdtxt = string.Empty;

            #region IQueryProvider Members

            public QueryParameterCollection Parameters
            {
                get { return param; }
            }

            public string CmdText
            {
                get
                {
                    return cmdtxt;
                }
                set
                {
                    if (value == null)
                        cmdtxt = string.Empty;
                    else
                        cmdtxt = value;
                }
            }

            #endregion

            #region IQueryProvider Members

            string Saga.Data.IQueryProvider.CmdText
            {
                get
                {
                    return cmdtxt;
                }
                set
                {
                    if (value == null)
                        cmdtxt = string.Empty;
                    else
                        cmdtxt = value;
                }
            }

            QueryParameterCollection Saga.Data.IQueryProvider.Parameters
            {
                get { return param; }
            }

            #endregion
        }


    }
}
