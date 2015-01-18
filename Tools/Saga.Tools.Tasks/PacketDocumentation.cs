using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Saga.Tools.Tasks
{
    public class PacketDocumentation : Task
    {

        private string toolpath = string.Empty;
        private string title = string.Empty;
        private string outputfile = string.Empty;
        private string rootnode = "page";
        private ITaskItem[] source = new TaskItem[0];
        private ITaskItem[] rootnodeattributes = new TaskItem[0];
        private ITaskItem[] rootnodeattributesnames = new TaskItem[0];
        static List<PacketInformation> CMSG = new List<PacketInformation>();
        static List<PacketInformation> SMSG = new List<PacketInformation>();


        /// <summary>
        /// Get's or set's the toolpath of the documentation exe
        /// </summary>
        public string ToolPath
        {
            get
            {
                return toolpath;
            }
            set
            {
                toolpath = value;
            }
        }

        /// <summary>
        /// Output file
        /// </summary>
        [Required]
        public string OutputFile
        {
            get
            {
                return outputfile;
            }
            set
            {
                outputfile = value;
            }
        }

        [Required]
        public ITaskItem[] Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }


        public override bool Execute()
        {
            CMSG.Clear();
            SMSG.Clear();

            //Parse packet files
            ParseFiles();

            //Sorting debug
            CMSG.Sort();
            SMSG.Sort();

            GenerateOutput();

            // This is where the task would presumably do its work.
            return !Log.HasLoggedErrors;
        }

        private void ParseFiles()
        {
            try
            {
                foreach (ITaskItem item in Source)
                {
                    if (item.ItemSpec.Length > 0)
                    {
                        string filename = Path.GetFileName(item.ItemSpec);
                        if (filename.StartsWith("CMSG"))
                        {
                            try
                            {
                                PacketInformation current = PacketInformation.GenerateFromFile(item.ItemSpec);
                                if (current != null) CMSG.Add(current);
                            }
                            catch (Exception e)
                            {
                                Log.LogErrorFromException(e, false, true, item.ItemSpec);
                            }
                        }
                        else if (filename.StartsWith("SMSG"))
                        {
                            try
                            {
                                PacketInformation current = PacketInformation.GenerateFromFile(item.ItemSpec);
                                if (current != null) SMSG.Add(current);
                            }
                            catch (Exception e)
                            {
                                Log.LogErrorFromException(e, false, true, item.ItemSpec);
                            }
                        }
                        else
                        {
                            Log.LogWarning("Filename is not regonized: {0}", filename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, false, true, null);
            }
        }

        private void GenerateOutput()
        {
            try
            {
                using (FileStream stream = new FileStream(OutputFile, FileMode.Create, FileAccess.Write))
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Indentation = 1;
                    writer.IndentChar = '\t';
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();

                   
                    writer.WriteStartElement("Packets");
                    writer.WriteStartElement("ClientSide");
                    foreach (PacketInformation pair in CMSG)
                    {

                        writer.WriteStartElement("Packet");
                        writer.WriteElementString("Name", pair.name);
                        writer.WriteElementString("Identifier", string.Format("{0:X4}", pair.id));
                        writer.WriteElementString("Remarks", pair.notes);
                        if (pair.example.Length > 0)
                            writer.WriteElementString("Example", pair.example);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("ServerSide");
                    foreach (PacketInformation pair in SMSG)
                    {
                        writer.WriteStartElement("Packet");
                        writer.WriteElementString("Name", pair.name);
                        writer.WriteElementString("Identifier", string.Format("{0:X4}", pair.id));
                        writer.WriteElementString("Remarks", pair.notes);
                        if (pair.example.Length > 0)
                            writer.WriteElementString("Example", pair.example);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Log.LogError("Error creating output");
            }
        }

        #region Nested Classes

        public class PacketInformation : IComparable
        {
            public uint id = 0;
            public string notes = "";
            public string name = "";
            public string example = "";

            static uint i;
            public static PacketInformation GenerateFromFile(string filename)
            {
                PacketInformation c = new PacketInformation();
                c.id = ++i;
                c.notes = "None";
                c.name = Path.GetFileNameWithoutExtension(filename);

                Regex reg = new Regex("(C|SMSG_)([A-F0-9]*_)([a-zA-Z0-9]*)");
                c.name = reg.Replace(c.name, "$1$3");


                bool class_started = false;
                StringBuilder sb = new StringBuilder();

                {

                    sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sb.Append("<packets>");

                    using (StringWriter writer = new StringWriter(sb))
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        while (reader.Peek() > -1 && class_started == false)
                        {
                            string line = reader.ReadLine();
                            string line2 = line.TrimStart(' ', '\t');
                            if (line2.Length >= 3 && line2.Substring(0, 3) == "///")
                                writer.WriteLine(line2.Substring(3));

                        }
                    }
                    sb.Append("</packets>");
                }
                {


                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.None;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.None;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;

                    using (StringReader reader = new StringReader(sb.ToString()))
                    using (XmlReader xmlreader = XmlReader.Create(reader, settings))
                    {
                        string value = "";
                        while (xmlreader.Read())
                        {
                            switch (xmlreader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    value = null;
                                    break;
                                case XmlNodeType.Text:
                                    value = xmlreader.Value;
                                    break;
                                case XmlNodeType.EndElement:
                                    switch (xmlreader.Name.ToLowerInvariant())
                                    {
                                        case "remarks": c.notes = value; break;
                                        case "example": c.example = value.Trim(' ', '\t', '\n', '\r'); break;
                                        case "id": c.id = uint.Parse(value, System.Globalization.NumberStyles.HexNumber, null); break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                return c;
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                if (obj is PacketInformation)
                {
                    PacketInformation ab = obj as PacketInformation;
                    if (ab.id > this.id) return -1;
                }
                return 0;
            }

            #endregion
        }

        #endregion

    }
}