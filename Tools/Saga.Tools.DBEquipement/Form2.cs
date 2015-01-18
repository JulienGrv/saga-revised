using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Xml;
using System.Globalization;

namespace Saga.Tools.DBEquipement
{
    public partial class Form2 : Form
    {

        MySqlConnection conn;
        public Form2()
        {
            InitializeComponent();

            MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder();
            cb.UserID = "root";
            cb.Password = "root";
            cb.Port = 3306;
            cb.Server = "localhost";
            cb.Database = "saga";

            conn = new MySqlConnection(cb.ConnectionString);
            conn.Open();

            System.Threading.Timer myTimer = new System.Threading.Timer(callback, conn, 300000, 300000);
        }

        private void callback(object obj)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Ping();
            }
            else
            {
                conn.Open();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SAVE FILE
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*.xml|*.xml";
            
            //LOAD FILE
            if (dialog.ShowDialog() == DialogResult.OK)
            using( FileStream fs = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write))
            using (XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode))
            {

                writer.Indentation = 1;
                writer.IndentChar = '\t';
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement("Equipment");

                MySqlCommand command = new MySqlCommand("SELECT * FROM characters WHERE `CharId`=?CharId", conn);
                command.Parameters.AddWithValue("CharId", uint.Parse(textBox1.Text, NumberFormatInfo.InvariantInfo));

                MySqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {
                    byte[] buffer = new byte[5];

                    writer.WriteElementString("CharName", reader.GetString(1));                    
                    writer.WriteStartElement("CharFace");                    
                        reader.GetBytes(2, 0, buffer, 0, 5);
                        for (int i = 0; i < buffer.Length; i++) 
                        writer.WriteElementString("details", buffer[i].ToString());
                    writer.WriteEndElement();


                    writer.WriteElementString("UserId", reader.GetUInt32(3).ToString());
                    writer.WriteElementString("Cexp", reader.GetUInt32(4).ToString());
                    writer.WriteElementString("Jexp", reader.GetUInt32(5).ToString());
                    writer.WriteElementString("Job", reader.GetUInt32(6).ToString());
                    writer.WriteElementString("Map", reader.GetUInt32(7).ToString());
                }

                reader.Close();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SAVE FILE
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*.xml|*.xml";
            
            //LOAD FILE
            if (dialog.ShowDialog() == DialogResult.OK)
            using( FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
            using (XmlTextReader reader = new XmlTextReader(fs))
            {


                    reader.ReadStartElement();


                    uint cexp = 0;
                    uint jexp = 0;                    
                    uint userid = 0;
                    string charname = "";                   
                    byte job = 0;
                    byte map = 0;
                    byte[] buffer = new byte[5];

                    charname = reader.ReadElementString();                    
                    if( reader.IsStartElement("CharFace"))
                    {
                        XmlReader innerreader = reader.ReadSubtree();
                        int _pos = 0;
                        while( innerreader.ReadToFollowing("details") )
                        {
                            byte.TryParse(innerreader.ReadElementString(), out buffer[_pos]);
                            _pos++;
                        }
                        reader.ReadEndElement();
                    }

                    
                    uint.TryParse(reader.ReadElementString(), out userid);
                    uint.TryParse(reader.ReadElementString(), out cexp);
                    uint.TryParse(reader.ReadElementString(), out jexp);
                    byte.TryParse(reader.ReadElementString(), out job);
                    byte.TryParse(reader.ReadElementString(), out map);





                reader.ReadEndElement();

                MySqlCommand command = new MySqlCommand("INSERT INTO characters (`CharId`,`CharName`,`CharFace`,`UserId`,`Cexp`,`Jexp`,`Job`,`Map`) VALUES (?CharId, ?CharName, ?CharFace, ?UserId, ?Cexp, ?Jexp, ?Job, ?Map) ON duplicate KEY UPDATE `CharName`=?CharName,`CharFace`=?CharFace,`UserId`=?UserId,`Cexp`=?Cexp,`Jexp`=?Jexp,`Job`=?Job,`Map`=?Map", conn);
                command.Parameters.AddWithValue("CharId", uint.Parse(textBox1.Text, NumberFormatInfo.InvariantInfo));
                command.Parameters.AddWithValue("CharName", charname);
                command.Parameters.AddWithValue("CharFace", buffer);
                command.Parameters.AddWithValue("UserId", userid);
                command.Parameters.AddWithValue("Cexp", cexp);
                command.Parameters.AddWithValue("Jexp", jexp);
                command.Parameters.AddWithValue("Job", job);
                command.Parameters.AddWithValue("Map", map);
                command.ExecuteNonQuery();

            }
        }
    }
}