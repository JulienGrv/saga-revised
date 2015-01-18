using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.PrimaryTypes;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {



        //Third generation methods character
        bool LoadCharacterEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            MySqlDataReader reader = null;
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_12, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                reader = command.ExecuteReader();

                while (reader.Read())
                {


                    WorldCoordinate position = new WorldCoordinate();
                    position.coords = new Point(reader.GetFloat(13), reader.GetFloat(14), reader.GetFloat(15));
                    position.map = reader.GetByte(7);

                    WorldCoordinate savePosition = new WorldCoordinate();
                    savePosition.coords = new Point(reader.GetFloat(16), reader.GetFloat(17), reader.GetFloat(18));
                    savePosition.map = reader.GetByte(19);

                    character.CharacterName = reader.GetString(1);
                    reader.GetBytes(2, 0, character.CharacterFace, 0, 11);
                    character.CharacterExperience = reader.GetUInt32(4);
                    character.JobExperience = reader.GetUInt32(5);
                    character.Job = reader.GetByte(6);
                    character.HP = reader.GetUInt16(9);
                    character.SP = reader.GetUInt16(10);
                    character.LP = reader.GetByte(11);
                    character.Oxygen = reader.GetByte(12);
                    character.Position = position;
                    character.SavePosition = savePosition;                   
                    character.Strength = reader.GetByte(20);
                    character.Dexterity = reader.GetByte(21);
                    character.Intellect = reader.GetByte(22);
                    character.Concentration = reader.GetByte(23);
                    character.Luck = reader.GetByte(24);
                    character.Remaining = reader.GetByte(25);
                    character.Zeny = reader.GetUInt32(26);

                    return true;
                }

                return false;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("database", e.Message);
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
            }
        }
        bool SaveCharacterEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_13, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                command.Parameters.AddWithValue("Cexp", character.CharacterExperience);
                command.Parameters.AddWithValue("Jexp", character.JobExperience);
                command.Parameters.AddWithValue("Job", character.Job);
                command.Parameters.AddWithValue("Map", character.Position.map);
                command.Parameters.AddWithValue("HP", character.HP);
                command.Parameters.AddWithValue("SP", character.SP);
                command.Parameters.AddWithValue("LP", character.LP);
                command.Parameters.AddWithValue("LC", character.Oxygen);
                command.Parameters.AddWithValue("Posx", character.Position.x);
                command.Parameters.AddWithValue("Posy", character.Position.y);
                command.Parameters.AddWithValue("Posz", character.Position.z);
                command.Parameters.AddWithValue("Savex", character.SavePosition.x);
                command.Parameters.AddWithValue("Savey", character.SavePosition.y);
                command.Parameters.AddWithValue("Savez", character.SavePosition.z);
                command.Parameters.AddWithValue("Savemap", character.SavePosition.map);
                command.Parameters.AddWithValue("Str", character.Strength);
                command.Parameters.AddWithValue("Dex", character.Dexterity);
                command.Parameters.AddWithValue("Int", character.Intellect);
                command.Parameters.AddWithValue("Con", character.Concentration);
                command.Parameters.AddWithValue("Luc", character.Luck);
                command.Parameters.AddWithValue("Pending", character.Remaining);
                command.Parameters.AddWithValue("Rufi", character.Zeny);
                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsCharacterEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_74, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);             
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertCharacterEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();
            MySqlCommand command;

            try
            {                
                //LOAD ALL SKILL INFORMATION



                if (character.CharacterId > 0)
                {
                    command = new MySqlCommand(_query_84, connection);
                    command.Parameters.AddWithValue("CharId", character.CharacterId);
                }
                else
                {
                    command = new MySqlCommand(_query_85, connection);
                }

                command.Parameters.AddWithValue("Cexp", character.CharacterExperience);
                command.Parameters.AddWithValue("Jexp", character.JobExperience);
                command.Parameters.AddWithValue("Job", character.Job);
                command.Parameters.AddWithValue("Map", character.Position.map);
                command.Parameters.AddWithValue("HP", character.HP);
                command.Parameters.AddWithValue("SP", character.SP);
                command.Parameters.AddWithValue("LP", character.LP);
                command.Parameters.AddWithValue("LC", character.Oxygen);
                command.Parameters.AddWithValue("Posx", character.Position.x);
                command.Parameters.AddWithValue("Posy", character.Position.y);
                command.Parameters.AddWithValue("Posz", character.Position.z);
                command.Parameters.AddWithValue("Savex", character.SavePosition.x);
                command.Parameters.AddWithValue("Savey", character.SavePosition.y);
                command.Parameters.AddWithValue("Savez", character.SavePosition.z);
                command.Parameters.AddWithValue("Savemap", character.SavePosition.map);
                command.Parameters.AddWithValue("Str", character.Strength);
                command.Parameters.AddWithValue("Dex", character.Dexterity);
                command.Parameters.AddWithValue("Int", character.Intellect);
                command.Parameters.AddWithValue("Con", character.Concentration);
                command.Parameters.AddWithValue("Luc", character.Luck);
                command.Parameters.AddWithValue("Pending", character.Remaining);
                command.Parameters.AddWithValue("Rufi", character.Zeny);
                command.Parameters.AddWithValue("CharName", character.CharacterName);
                command.Parameters.AddWithValue("UppercasedCharName", character.CharacterName);
                command.Parameters.AddWithValue("CharFace", character.CharacterFace);
                command.Parameters.AddWithValue("UserId", dbq.OwnerId);


                if (character.CharacterId > 0)
                {
                    return command.ExecuteNonQuery() > 0;
                }
                else
                {
                    bool result = command.ExecuteNonQuery() > 0;
                    character.CharacterId = (uint)command.LastInsertedId;
                    return result && character.CharacterId > 0;
                }                                          
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods addition
        bool LoadAdditionsEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            MySqlCommand command = new MySqlCommand(_query_03, connection);
            IDataAdditionCollection additionCollection = dbq.createAdditionCollection();
            command.Parameters.AddWithValue("CharId", additionCollection.CharacterId);
            MySqlDataReader reader = null;

            try
            {

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1) == true) return true;
                    byte[] buffer = new byte[8];
                    reader.GetBytes(1, 0, buffer, 0, 4);
                    uint count = BitConverter.ToUInt32(buffer, 0);

                    for (int i = 0; i < count; i++)
                    {
                        reader.GetBytes(1, 4 + (8 * i), buffer, 0, 8);
                        uint addition = BitConverter.ToUInt32(buffer, 0);
                        uint duration = BitConverter.ToUInt32(buffer, 4);
                        additionCollection.Create(addition, duration);
                    }


                    return true;
                }

                __dbtracelog.WriteError("Database", "player addition-data of player with id {0} is missing", additionCollection.CharacterId);
                return continueOnError;
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
        }
        bool SaveAdditionsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataAdditionCollection additionCollection = dbq.createAdditionCollection();
            byte[] buffer = new byte[2044];
            int items = 0;

            try
            {

                MySqlCommand command = new MySqlCommand(_query_04, connection);
                command.Connection = connection;

                int offset = 4;
                foreach (AdditionState current in additionCollection.Additions)
                {
                    uint duration = (current.CanExpire) ? current.Lifetime : 0;
                    uint addition = current.Addition;

                    Array.Copy(BitConverter.GetBytes(addition), 0, buffer, offset + 0, 4);
                    Array.Copy(BitConverter.GetBytes(duration), 0, buffer, offset + 4, 4);
                    items++;
                    offset += 8;
                }
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);
                command.Parameters.AddWithValue("CharId", additionCollection.CharacterId);
                command.Parameters.AddWithValue("Additions", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }

        }
        bool ExistsAdditionsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_75, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertAdditionsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataAdditionCollection additionCollection = dbq.createAdditionCollection();
            MySqlCommand command = new MySqlCommand(_query_05, connection);            
            byte[] buffer = new byte[2044];
            int items = 0;


            try
            {

                int offset = 4;
                foreach (AdditionState current in additionCollection.Additions)
                {
                    uint duration = (current.CanExpire) ? current.Lifetime : 0;
                    uint addition = current.Addition;

                    Array.Copy(BitConverter.GetBytes(addition), 0, buffer, offset + 0, 4);
                    Array.Copy(BitConverter.GetBytes(duration), 0, buffer, offset + 4, 4);
                    items++;
                    offset += 8;
                }
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);

                command.Parameters.AddWithValue("CharId", additionCollection.CharacterId);
                command.Parameters.AddWithValue("Additions", buffer);
                return command.ExecuteNonQuery() > 0;

            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation method equipment
        bool LoadEquipmentEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataEquipmentCollection equipmentCollection = dbq.createEquipmentCollection();
            MySqlCommand command = new MySqlCommand(_query_14, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", equipmentCollection.CharacterId);

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {
                    byte[] buffer = new byte[68];
                    int offset = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        Rag2Item item;
                        reader.GetBytes(0, offset, buffer, 0, 68);
                        if (Rag2Item.Deserialize(out item, buffer, 0))
                        {
                            item.active = buffer[67];
                            equipmentCollection.Equipment[i] = item;
                        }

                        offset += 68;
                    }

                    return true;
                }

                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        bool SaveEquipmentEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataEquipmentCollection equipmentCollection = dbq.createEquipmentCollection();

            int offset = 0;
            byte[] buffer = new byte[1088];
            for (int i = 0; i < 16; i++)
            {
                Rag2Item item = equipmentCollection.Equipment[i];
                if (item != null)
                {
                    Rag2Item.Serialize(item, buffer, offset);
                    buffer[offset + 67] = item.active;
                }

                offset += 68;
            }

            try
            {
                MySqlCommand command = new MySqlCommand(_query_15, connection);
                command.Parameters.AddWithValue("CharId", equipmentCollection.CharacterId);
                command.Parameters.AddWithValue("Equipment", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsEquipmentEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_76, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertEquipmentEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataEquipmentCollection equipmentCollection = dbq.createEquipmentCollection();

            int offset = 0;
            byte[] buffer = new byte[1088];
            for (int i = 0; i < 16; i++)
            {
                Rag2Item item = equipmentCollection.Equipment[i];
                if (item != null)
                {
                    Rag2Item.Serialize(item, buffer, offset);
                    buffer[offset + 67] = item.active;
                }

                offset += 68;
            }

            try
            {
                MySqlCommand command = new MySqlCommand(_query_16, connection);
                command.Parameters.AddWithValue("CharId", equipmentCollection.CharacterId);
                command.Parameters.AddWithValue("Equipment", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods inventory
        bool LoadInventoryEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataSortableItemCollection sortableItemCollection = dbq.createInventoryCollection();
            MySqlCommand command = new MySqlCommand(_query_29, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    sortableItemCollection.Collection.Capacity = reader.GetByte(0);
                    if (reader.IsDBNull(1)) continue;

                    byte[] buffer2 = new byte[4];
                    reader.GetBytes(1, 0, buffer2, 0, 4);
                    uint count = BitConverter.ToUInt32(buffer2, 0);


                    int offset = 4;
                    for (int i = 0; i < count; i++)
                    {
                        //BUFFER
                        byte[] buffer = new byte[67];
                        reader.GetBytes(1, offset, buffer, 0, 67);

                        Rag2Item item;
                        if (Rag2Item.Deserialize(out item, buffer, 0))
                            sortableItemCollection.Collection.Add(item);
                        offset += 67;
                    }

                    return true;
                }

                __dbtracelog.WriteError("Database", "player inventory-data of player with id {0} is missing", sortableItemCollection.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

        }
        bool SaveInventoryEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataSortableItemCollection sortableItemCollection = dbq.createInventoryCollection();
            MySqlCommand command = new MySqlCommand(_query_30, connection);
            byte[] buffer = new byte[4];
            int items = 0;

            try
            {
                //SERIALIZE ALL ITEMS
                foreach (Rag2Item item in sortableItemCollection.Collection)
                {
                    if (item == null) continue;
                    int offset = buffer.Length;
                    Array.Resize<byte>(ref buffer, offset + 67);
                    Rag2Item.Serialize(item, buffer, offset);
                    items++;
                }

                //WRITE THE NUMBER OF ITEMS
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);
                command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);
                command.Parameters.AddWithValue("ContainerMaxStorage", sortableItemCollection.Collection.Capacity);
                command.Parameters.AddWithValue("Container", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsInventoryEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_77, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertInventoryEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataSortableItemCollection sortableItemCollection = dbq.createStorageCollection();
            MySqlCommand command = new MySqlCommand(_query_31, connection);
            byte[] buffer = new byte[4];
            int items = 0;

            try
            {
                //SERIALIZE ALL ITEMS
                foreach (Rag2Item item in sortableItemCollection.Collection)
                {
                    if (item == null) continue;
                    int offset = buffer.Length;
                    Array.Resize<byte>(ref buffer, offset + 67);
                    Rag2Item.Serialize(item, buffer, offset);
                    items++;
                }

                //WRITE THE NUMBER OF ITEMS
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);
                command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);
                command.Parameters.AddWithValue("MaxStorage", sortableItemCollection.Collection.Capacity);
                command.Parameters.AddWithValue("Container", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods jobs
        bool LoadJobinformationEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataJobinformationCollection jobInfo = dbq.createJobCollection();
            MySqlCommand command = new MySqlCommand(_query_32, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", jobInfo.CharacterId);

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                        reader.GetBytes(0, 0, jobInfo.Joblevels, 0, 15);
                    else
                        for (int i = 0; i < 15; i++)
                            jobInfo.Joblevels[i] = 1;

                    return true;
                }

                __dbtracelog.WriteError("Database", "player job-data of player with id {0} is missing", jobInfo.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        bool SaveJobinformationEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataJobinformationCollection jobInfo = dbq.createJobCollection();
            MySqlCommand command = new MySqlCommand(_query_33, connection);

            try
            {
                command.Parameters.AddWithValue("CharId", jobInfo.CharacterId);
                command.Parameters.AddWithValue("JobInformation", jobInfo.Joblevels);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsJobinformationEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_78, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertJobinformationEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataJobinformationCollection jobInfo = dbq.createJobCollection();
            MySqlCommand command = new MySqlCommand(_query_34, connection);

            try
            {
                command.Parameters.AddWithValue("CharId", jobInfo.CharacterId);
                command.Parameters.AddWithValue("JobInformation", jobInfo.Joblevels);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods storage
        bool LoadStorageEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataSortableItemCollection sortableItemCollection = dbq.createStorageCollection();
            MySqlCommand command = new MySqlCommand("SELECT `ContainerMaxStorage`,`Container` FROM `list_storage` WHERE `CharId`=?CharId", connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    sortableItemCollection.Collection.Capacity = reader.GetByte(0);
                    if (reader.IsDBNull(1)) continue;

                    byte[] buffer2 = new byte[4];
                    reader.GetBytes(1, 0, buffer2, 0, 4);
                    uint count = BitConverter.ToUInt32(buffer2, 0);

                    int offset = 4;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] buffer = new byte[67];
                        reader.GetBytes(1, offset, buffer, 0, 67);

                        Rag2Item item;
                        if (Rag2Item.Deserialize(out item, buffer, 0))
                            sortableItemCollection.Collection.Add(item);

                        offset += 67;
                    }

                    return true;
                }

                __dbtracelog.WriteError("Database", "player storage-data of player with id {0} is missing", sortableItemCollection.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        bool SaveStorageEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataSortableItemCollection sortableItemCollection = dbq.createStorageCollection();
            MySqlCommand command = new MySqlCommand("UPDATE `list_storage` SET `ContainerMaxStorage`=?ContainerMaxStorage, `Container`=?Container WHERE `CharId`=?CharId", connection);
            byte[] buffer = new byte[4];
            int items = 0;

            try
            {
                //SERIALIZE ALL ITEMS
                foreach (Rag2Item item in sortableItemCollection.Collection)
                {
                    if (item == null) continue;
                    int offset = buffer.Length;
                    Array.Resize<byte>(ref buffer, offset + 67);
                    Rag2Item.Serialize(item, buffer, offset);
                    items++;
                }

                //WRITE THE NUMBER OF ITEMS
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);
                command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);
                command.Parameters.AddWithValue("ContainerMaxStorage", sortableItemCollection.Collection.Capacity);
                command.Parameters.AddWithValue("Container", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsStorageEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_81, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertStorageEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataSortableItemCollection sortableItemCollection = dbq.createStorageCollection();
            MySqlCommand command = new MySqlCommand(_query_86, connection);
            byte[] buffer = new byte[4];
            int items = 0;

            try
            {
                //SERIALIZE ALL ITEMS
                foreach (Rag2Item item in sortableItemCollection.Collection)
                {
                    if (item == null) continue;
                    int offset = buffer.Length;
                    Array.Resize<byte>(ref buffer, offset + 67);
                    Rag2Item.Serialize(item, buffer, offset);
                    items++;
                }

                //WRITE THE NUMBER OF ITEMS
                Array.Copy(BitConverter.GetBytes(items), 0, buffer, 0, 4);
                command.Parameters.AddWithValue("CharId", sortableItemCollection.CharacterId);
                command.Parameters.AddWithValue("ContainerMaxStorage", sortableItemCollection.Collection.Capacity);
                command.Parameters.AddWithValue("Container", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods weapon
        public bool LoadWeaponsEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataWeaponCollection weapons = dbq.createWeaponCollection();
            MySqlCommand command = new MySqlCommand(_query_69, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", weapons.CharacterId);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(0)) continue;
                    byte[] buffer2 = new byte[375];
                    reader.GetBytes(1, 0, buffer2, 0, 375);
                    weapons.UnlockedWeaponSlots = reader.GetByte(2);
                    weapons.PrimaryWeaponIndex = reader.GetByte(3);
                    weapons.SeconairyWeaponIndex = reader.GetByte(4);
                    weapons.ActiveWeaponIndex = reader.GetByte(5);

                    for (int i = 0; i < 5; i++)
                    {
                        Weapon weapon = Weapon.Deserialize(buffer2, i * 75);
                        weapons[i] = weapon;
                    }

                    return true;
                }

                __dbtracelog.WriteError("Database", "player weapon-data of player with id {0} is missing", weapons.CharacterId);
                return continueOnError;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        public bool SaveWeaponsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataWeaponCollection weapons = dbq.createWeaponCollection();
            MySqlCommand command = new MySqlCommand(_query_70, connection);

            try
            {
                //SERIALIZE ALL ITEMS
                byte[] buffer = new byte[375];
                for (int i = 0; i < weapons.UnlockedWeaponSlots; i++)
                {
                    Weapon current = weapons[i];
                    if (current == null) continue;
                    Weapon.Serialize(current, buffer, i * 75);
                }

                //WRITE THE NUMBER OF ITEMS
                command.Parameters.AddWithValue("CharId", weapons.CharacterId);
                command.Parameters.AddWithValue("Weaponary", buffer);
                command.Parameters.AddWithValue("UnlockedWeaponCount", weapons.UnlockedWeaponSlots);
                command.Parameters.AddWithValue("PIndex", weapons.PrimaryWeaponIndex);
                command.Parameters.AddWithValue("SIndex", weapons.SeconairyWeaponIndex);
                command.Parameters.AddWithValue("AIndex", weapons.ActiveWeaponIndex);

                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return false;
            }
        }
        public bool InsertWeaponsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataWeaponCollection weapons = dbq.createWeaponCollection();
            MySqlCommand command = new MySqlCommand(_query_68, connection);

            try
            {
                //SERIALIZE ALL ITEMS
                byte[] buffer = new byte[375];
                for (int i = 0; i < weapons.UnlockedWeaponSlots; i++)
                {
                    Weapon current = weapons[i];
                    if (current == null) continue;
                    Weapon.Serialize(current, buffer, i * 75);
                }

                //WRITE THE NUMBER OF ITEMS
                command.Parameters.AddWithValue("CharId", weapons.CharacterId);
                command.Parameters.AddWithValue("Weaponary", buffer);
                command.Parameters.AddWithValue("UnlockedWeaponCount", weapons.UnlockedWeaponSlots);
                command.Parameters.AddWithValue("PrimairyWeapon", weapons.PrimaryWeaponIndex);
                command.Parameters.AddWithValue("SecondaryWeapon", weapons.SeconairyWeaponIndex);
                command.Parameters.AddWithValue("ActiveWeaponIndex", weapons.ActiveWeaponIndex);

                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return false;
            }
        }
        bool ExistsWeaponsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_82, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods zone information
        bool LoadZoneEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDataZoneInformationCollection zoneInformation = dbq.createDataZoneCollection();
            MySqlCommand command = new MySqlCommand(_query_71, connection);
            MySqlDataReader reader = null;
            command.Parameters.AddWithValue("CharId", zoneInformation.CharacterId);

            try
            {
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                        reader.GetBytes(0, 0, zoneInformation.ZoneInformation, 0, 255);

                    return true;
                }

                __dbtracelog.WriteError("Database", "player zone-data of player with id {0} is missing", zoneInformation.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                Trace.Write(e.ToString(), "database");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        bool SaveZoneEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataZoneInformationCollection zoneInformation = dbq.createDataZoneCollection();
            MySqlCommand command = new MySqlCommand(_query_72, connection);

            try
            {
                command.Parameters.AddWithValue("CharId", zoneInformation.CharacterId);
                command.Parameters.AddWithValue("ZoneState", zoneInformation.ZoneInformation);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                Trace.Write(e.ToString(), "database");
                return false;
            }
        }
        bool InsertZoneEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //HELPER VARIABLES
            IDataZoneInformationCollection zoneInformation = dbq.createDataZoneCollection();
            MySqlCommand command = new MySqlCommand(_query_73, connection);

            try
            {
                command.Parameters.AddWithValue("CharId", zoneInformation.CharacterId);
                command.Parameters.AddWithValue("ZoneState", zoneInformation.ZoneInformation);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                Trace.Write(e.ToString(), "database");
                return false;
            }
        }
        bool ExistsZoneEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_83, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation methods quest information
        bool SaveQuestEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //Save to the stream
            IDatabaseQuestStream collection = dbq.createDatabaseQuestStream();
            MySqlCommand command;
            try
            {
                byte[] value = collection.questCollection;
                if (value != null)
                {
                    command = new MySqlCommand(_query_57, connection);
                    command.Parameters.AddWithValue("CharId", collection.CharacterId);
                    command.Parameters.AddWithValue("State", value);
                    return command.ExecuteNonQuery() > 0;
                }
                else
                {
                    command = new MySqlCommand(_query_58, connection);
                    command.Parameters.AddWithValue("CharId", collection.CharacterId);
                    return command.ExecuteNonQuery() > 0;

                }
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool LoadQuestEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            IDatabaseQuestStream collection = dbq.createDatabaseQuestStream();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = _query_59;
            command.Parameters.AddWithValue("CharId", collection.CharacterId);
            MySqlDataReader mreader = null;

            try
            {
                mreader = command.ExecuteReader(); // argument CommandBehavior.SingleRow removed (Darkin)
                while (mreader.Read())
                {
                    if (!mreader.IsDBNull(0))
                    {
                        collection.questCollection = (byte[])mreader.GetValue(0);
                        return true;
                    }

                    return true;
                }

                __dbtracelog.WriteError("Database", "player quest-data of player with id {0} is missing", collection.CharacterId);
                return continueOnError;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (mreader != null) mreader.Close();
            }
        }
        bool InsertQuestEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            //Save to the stream
            IDatabaseQuestStream collection = dbq.createDatabaseQuestStream();
            MySqlCommand command;
            try
            {
                byte[] value = collection.questCollection;
                if (value != null)
                {
                    command = new MySqlCommand(_query_87, connection);
                    command.Parameters.AddWithValue("CharId", collection.CharacterId);
                    command.Parameters.AddWithValue("State", value);
                    return command.ExecuteNonQuery() > 0;
                }
                else
                {
                    command = new MySqlCommand(_query_56, connection);
                    command.Parameters.AddWithValue("CharId", collection.CharacterId);
                    return command.ExecuteNonQuery() > 0;

                }
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsQuestEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_79, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation method special skill information
        bool LoadSkillsEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            MySqlDataReader reader = null;
            IDataSkillCollection collection = dbq.createSkillCollection();

            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_60, connection);
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Skill skill = new Skill();
                    skill.Id = reader.GetUInt32(0);
                    skill.Experience = reader.GetUInt32(1);
                    if (Singleton.SpellManager.TryGetSpell(skill.Id, out skill.info) && skill.info.requiredJobs[collection.Job - 1] == 1 )
                        collection.Skills.Add(skill);

                        return true;
                }

                __dbtracelog.WriteError("Database", "player skill-data of player with id {0} is missing", collection.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
            }
        }
        bool LoadSpecialSkillsEx(MySqlConnection connection, IInfoProvider2 dbq, bool continueOnError)
        {
            MySqlDataReader reader = null;
            IDataSpecialSkillCollection collection = dbq.createDatabaseSpecialSkillCollection();
            
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_67, connection);
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

                while (reader.Read())
                {
                    byte[] buffer = new byte[64];
                    if (reader.IsDBNull(0) == false) reader.GetBytes(0, 0, buffer, 0, 64);
                    for (int i = 0; i < 16; i++)
                    {
                        uint skillid = BitConverter.ToUInt32(buffer, i * 4);
                        Skill skill = new Skill();
                        skill.Id = skillid;
                        if (Singleton.SpellManager.spells.TryGetValue(skillid, out skill.info))
                            collection.specialSkillCollection[i] = skill;
                    }

                    return true;
                }

                __dbtracelog.WriteError("Database", "player specialskill-data of player with id {0} is missing", collection.CharacterId);
                return continueOnError;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
            }
        }
        bool SaveSpecialSkillsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataSpecialSkillCollection collection = dbq.createDatabaseSpecialSkillCollection();
            try
            {
                //LOAD ALL SKILL INFORMATION
                byte[] buffer = new byte[64];
                for (int i = 0; i < 16; i++)
                {
                    Skill skill = collection.specialSkillCollection[i];
                    if (skill != null)
                        Array.Copy(BitConverter.GetBytes(skill.info.skillid), 0, buffer, i * 4, 4);
                }

                MySqlCommand command = new MySqlCommand(_query_66, connection);
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                command.Parameters.AddWithValue("Skills", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool InsertSpecialSkillsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataSpecialSkillCollection collection = dbq.createDatabaseSpecialSkillCollection();
            try
            {
                //LOAD ALL SKILL INFORMATION
                byte[] buffer = new byte[64];
                for (int i = 0; i < 16; i++)
                {
                    Skill skill = collection.specialSkillCollection[i];
                    if (skill != null)
                        Array.Copy(BitConverter.GetBytes(skill.info.skillid), 0, buffer, i * 4, 4);
                }

                MySqlCommand command = new MySqlCommand(_query_88, connection);
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                command.Parameters.AddWithValue("Skills", buffer);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }
        bool ExistsSpecialSkillsEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDataCharacter character = dbq.createDataCharacter();

            try
            {
                MySqlCommand command = new MySqlCommand(_query_80, connection);
                command.Parameters.AddWithValue("CharId", character.CharacterId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (MySqlException e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }

        //Third generation method black & friendlist information
        bool LoadFriendlistEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDatabaseFriendList collection = dbq.createDatabaseFriendList();
            MySqlDataReader reader = null;
            MySqlCommand command = new MySqlCommand(_query_23, connection);

            try
            {                
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                collection.friends.Clear();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    collection.friends.Add(reader.GetString(0));
                }

                return true;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if( reader != null ) reader.Close();
            }
        }
        bool LoadBlacklistEx(MySqlConnection connection, IInfoProvider2 dbq)
        {
            IDatabaseBlacklist collection = dbq.createDatabaseBlacklist();
            MySqlCommand command = new MySqlCommand(_query_24, connection);            
            MySqlDataReader reader = null;

            try
            {
                command.Parameters.AddWithValue("CharId", collection.CharacterId);
                collection.blacklist.Clear();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    collection.blacklist.Add(
                        new KeyValuePair<string, byte>
                        (
                            reader.GetString(0),    //ADD CHARACTER NAME
                            reader.GetByte(1)       //ADD REASON                 
                        )
                    );
                }

                return true;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }


        #region Old

        /// <summary>
        /// Deletes a character account per given character id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCharacterById(MySqlConnection connection, uint id)
        {
            MySqlCommand command = new MySqlCommand("DELETE FROM `characters` WHERE `CharId`=?CharId LIMIT 1", connection);
            command.Parameters.AddWithValue("CharId", id);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return false;
            }
        }

        /// <summary>
        /// Finds a list of characters per given playerid
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function is internally used by our login server.
        /// </remarks>
        public IEnumerable<CharInfo> FindCharacters(MySqlConnection connection, uint PlayerId)
        {
            MySqlCommand command = new MySqlCommand();
            MySqlDataReader reader = null;
            command.Connection = connection;
            command.CommandText = "SELECT `CharId`,`CharName`,`Cexp`,`Job`,`Map` FROM `characters` WHERE `UserId`=?PlayerId;";
            command.Parameters.AddWithValue("PlayerId", PlayerId);
            List<CharInfo> list = new List<CharInfo>();

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CharInfo info = new CharInfo();
                    info.charId = reader.GetUInt32(0);
                    info.name = reader.GetString(1);
                    info.cexp = reader.GetUInt32(2);
                    info.job = reader.GetByte(3);
                    info.map = reader.GetByte(4);
                    list.Add(info);
                }

                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Finds the character details of a character per given characterid
        /// </summary>
        /// <param name="CharId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool FindCharacterDetails(MySqlConnection connection, uint CharId, out CharDetails details)
        {
            MySqlCommand command = new MySqlCommand();
            MySqlDataReader reader = null;
            command.Connection = connection;
            command.CommandText = "SELECT `CharFace`,`Jexp`,`Map` FROM `characters` WHERE CharId=?CharId LIMIT 1;";
            command.Parameters.AddWithValue("CharId", CharId);

            bool result = false;
            details = new CharDetails();

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = true;
                    details.FaceDetails = new byte[11];
                    reader.GetBytes(0, 0, details.FaceDetails, 0, 11);
                    details.Jexp = reader.GetUInt32(1);
                    details.Map = reader.GetByte(2);
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine(e.Message, "Database");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return result;
        }
    
        /// <summary>
        /// Verifies if a given charactername already exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VerifyNameExists(string name)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(String.Format("SELECT COUNT(*) FROM `characters` WHERE `UppercasedCharName`='{0}' LIMIT 1", name.ToUpperInvariant()), connection);
            MySqlDataReader reader = null;
            bool res = false;

            try
            {
                reader = command.ExecuteReader(); // argument CommandBehavior.SingleRow removed (Darkin)
                while (reader.Read())
                {
                    res = reader.GetInt32(0) > 0;
                }
            }
            finally
            {
                ConnectionPool.Release(connection);
                if (reader != null) reader.Close();
            }

            return res;
        }


        public bool GetCharacterId(string name, out uint charId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_89, connection);
            MySqlDataReader reader = null;
            string uname = name.ToUpperInvariant();

            try
            {
                command.Parameters.AddWithValue("UppercasedCharName",uname);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    charId = reader.GetUInt32(0);
                    return true;
                }

                charId = 0;
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        #endregion


    }
}
