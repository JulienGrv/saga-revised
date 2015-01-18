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
    public partial class Form4 : Form
    {

        MySqlConnection conn;
        public Form4()
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

                writer.WriteStartElement("Inventory");

                MySqlCommand command = new MySqlCommand("SELECT `ContainerMaxStorage`,`Container` FROM `inventory` WHERE `CharId`=?CharId", conn);
                command.Parameters.AddWithValue("CharId", uint.Parse(textBox1.Text, NumberFormatInfo.InvariantInfo));

                MySqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {

                    byte MaxInventoryStorage = reader.GetByte(0);
                    byte[] buffer2 = new byte[4];
                    reader.GetBytes(1, 0, buffer2, 0, 4);
                    uint count = BitConverter.ToUInt32(buffer2, 0);

                    byte[] buffer = new byte[67];
                    int offset = 4;
                    for (int i = 0; i < count; i++)
                    {
                        //READ IT CHUNKED
                        reader.GetBytes(1, offset, buffer, 0, 67);
                        writer.WriteStartElement("Item");

                        //WRITES THE ITEM ID
                        writer.WriteStartElement("ItemId");
                        writer.WriteValue(BitConverter.ToUInt32(buffer, 0));
                        writer.WriteEndElement();

                        //WRITES THE DURABILITY OF THE ITEM
                        writer.WriteStartElement("Durability");
                        writer.WriteValue(BitConverter.ToUInt16(buffer, 51));
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        offset += 67;
                    }
                }

                reader.Close();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SAVE FILE
            byte[] buffer = new byte[4];
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*.xml|*.xml";
            
            //LOAD FILE
            if (dialog.ShowDialog() == DialogResult.OK)
            using( FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
            using (XmlTextReader reader = new XmlTextReader(fs))
            {

                reader.ReadStartElement();

                int i = 0;
                while (reader.IsStartElement("Item"))
                {
                    int offset = buffer.Length;
                    Array.Resize<byte>(ref buffer, buffer.Length + 67);
                    reader.ReadStartElement();

                    Array.Copy(
                        BitConverter.GetBytes(uint.Parse(reader.ReadElementString())),
                        0,
                        buffer,
                        offset,
                        4);

                    Array.Copy(
                        BitConverter.GetBytes(ushort.Parse(reader.ReadElementString())),
                        0,
                        buffer,
                        offset + 51,
                        2);

                    reader.ReadEndElement();
                    offset += 67;
                    i++;
                }

                Array.Copy(
                    BitConverter.GetBytes(i),
                    0,
                    buffer,
                    0,
                    4);

                reader.ReadEndElement();


                MySqlCommand command = new MySqlCommand("INSERT INTO `inventory` (`CharId`,`Container`,`ContainerMaxStorage`) VALUES (?CharId,?Container, ?MaxInventory) ON duplicate KEY UPDATE `ContainerMaxStorage`=?MaxInventory AND `Container`=?Container;", conn);
                command.Parameters.AddWithValue("CharId", uint.Parse(textBox1.Text, NumberFormatInfo.InvariantInfo));
                command.Parameters.AddWithValue("Container", buffer);
                command.Parameters.AddWithValue("MaxInventory", 32);
                command.ExecuteNonQuery();

            }
        }
    }
}