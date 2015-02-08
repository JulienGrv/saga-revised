using Saga.Shared.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Saga
{
    public static class ErrorLog
    {
        public static bool CreateLog(string filename, IEnumerable<Exception> Exceptions)
        {
            try
            {
                if (File.Exists("errorlog.xsl") == false)
                    using (FileStream fs = new FileStream("errorlog.xsl", FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(Resource.errorlog);
                    }

                using (XsltXmlWriter wr = new XsltXmlWriter(filename, Encoding.Default, "errorlog.xsl"))
                {
                    wr.Formatting = Formatting.Indented;
                    wr.Indentation = 1;
                    wr.IndentChar = '\t';

                    wr.WriteStartDocument();
                    wr.WriteStartElement("page");
                    wr.WriteElementString("title", "Error log");
                    wr.WriteStartElement("errorlog");
                    foreach (Exception x in Exceptions)
                    {
                        wr.WriteStartElement("error");
                        wr.WriteElementString("name", x.Message);
                        wr.WriteElementString("example", x.ToString());
                        wr.WriteEndElement();
                    }
                    wr.WriteEndElement();
                    wr.WriteEndElement();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}