using Saga.Configuration;
using Saga.Data;
using Saga.Map;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.PrimaryTypes;
using Saga.Quests.Objectives;
using Saga.Structures;
using Saga.Structures.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Saga.Managers
{
    public class Database : ManagerBase2
    {
        #region Ctor/Dtor

        private FileStream writeStream;
        private FileStream readStream;

        public Database()
        {
            try
            {
                string filename = Server.SecurePath("~/restores/{0}.bak", Server.AssemblyName);
                string directory = Path.GetDirectoryName(filename);
                Directory.CreateDirectory(directory);

                writeStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (writeStream.Length == 0)
                {
                    writeStream.Write(new byte[4], 0, 4);
                    writeStream.Flush();
                }
                else
                {
                    writeStream.Seek(0, SeekOrigin.End);
                }
                readStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (Exception)
            {
                WriteError("DatabaseManager", "Failed to initialize restore-point system");
            }
        }

        ~Database()
        {
            if (writeStream != null)
                writeStream.Close();
            if (readStream != null)
                readStream.Close();
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
                string type = section.DBType;

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

        #region Public Methods

        public byte[] Serialize(Character character)
        {
            if (character == null) return null;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(gzip, character);
                    gzip.Flush();
                    gzip.Close();
                    return stream.GetBuffer();
                }
            }
            catch (SerializationException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool TransSave(Character character)
        {
            try
            {
                if (character != null)
                {
                    DBQProvider dbq = new DBQProvider(character);
                    return InternalDatabaseProvider.TransactionSave(dbq);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TransLoad(Character character)
        {
            try
            {
                if (character != null)
                {
                    DBQProvider dbq = new DBQProvider(character);
                    return InternalDatabaseProvider.TransactionLoad(dbq, false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TransLoad(Character character, bool continueOnError)
        {
            try
            {
                if (character != null)
                {
                    DBQProvider dbq = new DBQProvider(character);
                    return InternalDatabaseProvider.TransactionLoad(dbq, continueOnError);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TransRepair(Character character)
        {
            try
            {
                if (character != null)
                {
                    DBQProvider dbq = new DBQProvider(character);
                    return InternalDatabaseProvider.TransactionRepair(dbq);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TransactionInsert(Character character, uint owner)
        {
            try
            {
                if (character != null)
                {
                    DBQProvider dbq = new DBQProvider(character);
                    dbq.OwnerId = owner;
                    return InternalDatabaseProvider.TransactionInsert(dbq);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void PostLoad(Character character)
        {
            //Apply character configuration
            Singleton.CharacterConfiguration.ApplyConfiguration(character);

            //Update strength
            character._status.MaxPAttack += (ushort)(2 * character.stats.BASE.strength);
            character._status.MinPAttack += (ushort)(1 * character.stats.BASE.strength);
            character._status.MaxHP += (ushort)(10 * character.stats.BASE.strength);
            character._status.BasePHitrate += (ushort)(1 * character.stats.BASE.dexterity);
            character._status.BasePCritrate += (ushort)(1 * character.stats.BASE.dexterity);
            character._status.MaxMAttack += (ushort)(6 * character.stats.BASE.intelligence);
            character._status.MinMAttack += (ushort)(3 * character.stats.BASE.intelligence);
            character._status.BaseRHitrate += (ushort)(1 * character.stats.BASE.intelligence);
            character._status.MaxRAttack += (ushort)(4 * character.stats.BASE.concentration);
            character._status.MinRAttack += (ushort)(2 * character.stats.BASE.concentration);
            character._status.BasePHitrate += (ushort)(2 * character.stats.BASE.concentration);

            //Apply skill information
            for (int i = 0; i < 16; i++)
            {
                Skill skill = character.SpecialSkills[i];
                if (skill != null && skill.info != null)
                {
                    i += (skill.info.special - 1);
                    if (skill.info.skilltype == 2)
                        Singleton.Additions.ApplyAddition(skill.info.addition, character);
                }
            }

            //Apply weapon information
            int WeaponIndex = (character.weapons.ActiveWeaponIndex == 1) ? character.weapons.SeconairyWeaponIndex : character.weapons.PrimaryWeaponIndex;
            if (WeaponIndex < character.weapons.UnlockedWeaponSlots)
            {
                Weapon CurrentWeapon = character.weapons[WeaponIndex];

                if (CurrentWeapon != null)
                    if (character.FindRequiredRootSkill(CurrentWeapon.Info.weapon_skill))
                    {
                        //Default battle additions
                        character._status.MaxWMAttack += (ushort)CurrentWeapon.Info.max_magic_attack;
                        character._status.MinWMAttack += (ushort)CurrentWeapon.Info.min_magic_attack;
                        character._status.MaxWPAttack += (ushort)CurrentWeapon.Info.max_short_attack;
                        character._status.MinWPAttack += (ushort)CurrentWeapon.Info.min_short_attack;
                        character._status.MaxWRAttack += (ushort)CurrentWeapon.Info.max_range_attack;
                        character._status.MinWRAttack += (ushort)CurrentWeapon.Info.min_range_attack;

                        //Reapplies alterstone additions
                        for (int i = 0; i < 8; i++)
                        {
                            uint addition = CurrentWeapon.Slots[i];
                            if (addition > 0)
                            {
                                Singleton.Additions.ApplyAddition(addition, character);
                            }
                        }

                        CurrentWeapon._active = 1;
                    }
                    else
                    {
                        CurrentWeapon._active = 0;
                    }
            }

            Common.Internal.CharacterCheckCapacities(character);
        }

        #endregion Public Methods

        #region Backup & Restore utillities

        private void CopyTo(byte[] buffer, int index, bool value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 4);
        }

        private void CopyTo(byte[] buffer, int index, byte value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 1);
        }

        private void CopyTo(byte[] buffer, int index, sbyte value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 1);
        }

        private void CopyTo(byte[] buffer, int index, short value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 2);
        }

        private void CopyTo(byte[] buffer, int index, ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 2);
        }

        private void CopyTo(byte[] buffer, int index, int value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 4);
        }

        private void CopyTo(byte[] buffer, int index, uint value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 4);
        }

        private void CopyTo(byte[] buffer, int index, long value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 8);
        }

        private void CopyTo(byte[] buffer, int index, ulong value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 8);
        }

        private void CopyTo(byte[] buffer, int index, string name, int minSize)
        {
            minSize = Math.Min(minSize, name.Length);
            Encoding.Unicode.GetBytes(name, 0, minSize, buffer, 32);
        }

        private void CopyTo(byte[] buffer, int index, DateTime value)
        {
            Array.Copy(BitConverter.GetBytes(value.ToBinary()), 0, buffer, index, 8);
        }

        private bool ReadAsBool(byte[] buffer, int index)
        {
            return BitConverter.ToBoolean(buffer, index);
        }

        private byte ReadAsByte(byte[] buffer, int index)
        {
            return buffer[index];
        }

        private sbyte ReadAsSByte(byte[] buffer, int index)
        {
            return (sbyte)buffer[index];
        }

        private short ReadAsInt16(byte[] buffer, int index)
        {
            return BitConverter.ToInt16(buffer, index);
        }

        private ushort ReadAsUint16(byte[] buffer, int index)
        {
            return BitConverter.ToUInt16(buffer, index);
        }

        private int ReadAsInt32(byte[] buffer, int index)
        {
            return BitConverter.ToInt32(buffer, index);
        }

        private uint ReadAsUint32(byte[] buffer, int index)
        {
            return BitConverter.ToUInt32(buffer, index);
        }

        private long ReadAsInt64(byte[] buffer, int index)
        {
            return BitConverter.ToInt64(buffer, index);
        }

        private ulong ReadAsUint64(byte[] buffer, int index)
        {
            return BitConverter.ToUInt64(buffer, index);
        }

        private string ReadAsString(byte[] buffer, int index, int minSize)
        {
            return Encoding.Unicode.GetString(buffer, index, minSize);
        }

        private DateTime ReadAsDate(byte[] buffer, int index)
        {
            long date = BitConverter.ToInt64(buffer, index);
            return DateTime.FromBinary(date);
        }

        public bool GetCharacterId(string name, out uint charId)
        {
            return InternalDatabaseProvider.GetCharacterId(name, out charId);
        }

        public void Restore()
        {
            if (readStream == null) return;
            lock (readStream)
            {
                readStream.Seek(0, SeekOrigin.Begin);                         //Seek to start

                byte[] buffer = new byte[4];                                  //Container for header size
                readStream.Read(buffer, 0, 4);                                //Read header size
                uint numRecords = BitConverter.ToUInt32(buffer, 0);           //Get header count

                Console.WriteLine("Welcome to the manual restore utility.");
                Console.WriteLine("Restore-points detected: {0}", numRecords);
                if (numRecords == 0) return;

                Console.WriteLine("Press (s) to skip");
                Console.WriteLine("Press (r) to restore");

                byte[] headingBuffer = new byte[64];                          //buffer container
                while (numRecords > 0)
                {
                    numRecords--;
                    long posStart = readStream.Position;
                    readStream.Read(headingBuffer, 0, 64);                    //read data segment 1
                    uint size = ReadAsUint32(headingBuffer, 0);               //read size
                    DateTime date = ReadAsDate(headingBuffer, 4);             //read datetime
                    int countA = ReadAsInt32(headingBuffer, 16);              //read numberOfRestoreTries
                    int countB = ReadAsInt32(headingBuffer, 20);              //read succeeded
                    int countC = ReadAsInt32(headingBuffer, 24);              //read exceptions
                    string name = ReadAsString(headingBuffer, 32, 32);        //read name

                    Console.WriteLine("Segment found {0} {1} {2} {3} {4}", name, date, countA.ToString().PadLeft(5), countB.ToString().PadLeft(5), countC.ToString().PadLeft(5));

                Listen:
                    char key = Console.ReadKey(true).KeyChar;
                    if (key == 'r')
                    {
                        MemoryStream stream = null;
                        GZipStream gzip = null;
                        BinaryFormatter formatter = new BinaryFormatter();

                        try
                        {
                            byte[] value = new byte[size];
                            readStream.Read(value, 0, (int)size);
                            Character restoredCharacter = null;

                            stream = new MemoryStream(value);
                            gzip = new GZipStream(stream, CompressionMode.Decompress);

                            restoredCharacter = formatter.Deserialize(gzip) as Character;
                            gzip.Close();

                            Console.WriteLine("restoring character {0}...", restoredCharacter.Name);
                            if (!Singleton.Database.TransSave(restoredCharacter))
                            {
                                Console.WriteLine("restore failed");
                                WriteWarning("RestorePointCentre", "segement {0} {1} failed to save", name, date);

                                //Go back the size of data
                                readStream.Seek(posStart, SeekOrigin.Begin);
                                CopyTo(headingBuffer, 16, ++countA);
                                //Adjust header for failed
                                readStream.Write(headingBuffer, 0, 64);
                                readStream.Seek(size, SeekOrigin.Current);
                            }
                            else
                            {
                                //Go back the size of data
                                readStream.Seek(posStart, SeekOrigin.Begin);
                                CopyTo(headingBuffer, 20, ++countB);
                                //Adjust header for success
                                readStream.Write(headingBuffer, 0, 64);
                                readStream.Seek(size, SeekOrigin.Current);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to deserialize character information. You need to restore the restore points with the same version that created the restorepoint.");
                            WriteWarning("RestorePointCentre", "segement {0} {1} failed to deserialize reason: {2}", name, date, e.Message);

                            //Go back the size of data
                            readStream.Seek(posStart, SeekOrigin.Begin);
                            CopyTo(headingBuffer, 24, ++countC);
                            //Adjust header for success
                            readStream.Write(headingBuffer, 0, 64);
                            readStream.Seek(size, SeekOrigin.Current);
                        }
                        finally
                        {
                            if (stream != null) stream.Dispose();
                        }
                    }
                    else if (key == 's')
                    {
                        readStream.Seek(size, SeekOrigin.Current);                //skip rest of data
                        continue;
                    }
                    else goto Listen;
                }

                Console.WriteLine("Manual restore utility has finished");
            }
        }

        public void AutoRestore()
        {
            if (readStream == null) return;
            lock (readStream)
            {
                readStream.Seek(0, SeekOrigin.Begin);                         //Seek to start

                byte[] buffer = new byte[4];                                  //Container for header size
                readStream.Read(buffer, 0, 4);                                //Read header size
                uint numRecords = BitConverter.ToUInt32(buffer, 0);           //Get header count

                if (numRecords == 0) return;                                  //Stop processing if count is 0

                Console.WriteLine("Welcome to the automatic restore utility.");
                Console.WriteLine("Restore-points detected: {0}", numRecords);
                Console.WriteLine("The server will now automaticlly restore characters last-known configuration.");

                BooleanSwitch mswitch2 = new BooleanSwitch("SkipPreviousHandledRestorePoints", "Skips restore points that have been treated earlier", "1");

                byte[] headingBuffer = new byte[64];                          //buffer container
                while (numRecords > 0)
                {
                    numRecords--;
                    long posStart = readStream.Position;
                    readStream.Read(headingBuffer, 0, 64);                    //read data segment 1
                    uint size = ReadAsUint32(headingBuffer, 0);               //read size
                    DateTime date = ReadAsDate(headingBuffer, 4);             //read datetime
                    int countA = ReadAsInt32(headingBuffer, 16);              //read numberOfRestoreTries
                    int countB = ReadAsInt32(headingBuffer, 20);              //read succeeded
                    int countC = ReadAsInt32(headingBuffer, 24);              //read exceptions
                    string name = ReadAsString(headingBuffer, 32, 32);        //read name

                    if (countA > 10)
                    {
                        WriteInformation("RestorePointCentre", "Skipping segement {0} {1} reason: failed to be processed for 10x already", name, date);
                        readStream.Seek(size, SeekOrigin.Current);                //skip rest of data
                        continue;
                    }
                    if (countB > 0 && mswitch2.Enabled)
                    {
                        WriteInformation("RestorePointCentre", "Skipping segement {0} {1} reason: was restored earlier", name, date);
                        readStream.Seek(size, SeekOrigin.Current);                //skip rest of data
                        continue;
                    }
                    if (countC > 0)
                    {
                        WriteInformation("RestorePointCentre", "Skipping segement {0} {1} reason: had exceptions last time", name, date);
                        readStream.Seek(size, SeekOrigin.Current);                //skip rest of data
                        continue;
                    }

                    WriteInformation("RestorePointCentre", "Process segement {0} {1}", name, date);
                    MemoryStream stream = null;
                    GZipStream gzip = null;
                    BinaryFormatter formatter = new BinaryFormatter();

                    try
                    {
                        byte[] value = new byte[size];
                        readStream.Read(value, 0, (int)size);
                        Character restoredCharacter = null;

                        stream = new MemoryStream(value);
                        gzip = new GZipStream(stream, CompressionMode.Decompress);

                        restoredCharacter = formatter.Deserialize(gzip) as Character;
                        gzip.Close();

                        if (!Singleton.Database.TransSave(restoredCharacter))
                        {
                            WriteWarning("RestorePointCentre", "segement {0} {1} failed to save", name, date);

                            //Go back the size of data
                            readStream.Seek(posStart, SeekOrigin.Begin);
                            CopyTo(headingBuffer, 16, ++countA);
                            //Adjust header for failed
                            readStream.Write(headingBuffer, 0, 64);
                            readStream.Seek(size, SeekOrigin.Current);
                        }
                        else
                        {
                            WriteLine("RestorePointCentre", "segement {0} {1} was saved", name, date);

                            //Go back the size of data
                            readStream.Seek(posStart, SeekOrigin.Begin);
                            CopyTo(headingBuffer, 20, ++countB);
                            //Adjust header for success
                            readStream.Write(headingBuffer, 0, 64);
                            readStream.Seek(size, SeekOrigin.Current);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed for unknown reason.");
                        WriteWarning("RestorePointCentre", "segement {0} {1} failed to deserialize reason: {2}", name, date, e.Message);

                        //Go back the size of data
                        readStream.Seek(posStart, SeekOrigin.Begin);
                        CopyTo(headingBuffer, 24, ++countC);
                        //Adjust header for success
                        readStream.Write(headingBuffer, 0, 64);
                        readStream.Seek(size, SeekOrigin.Current);
                    }
                    finally
                    {
                        if (stream != null) stream.Dispose();
                    }
                }
            }

            BooleanSwitch mswitch = new BooleanSwitch("AutoMoveRestorePoints", "Moves restores points file on restart of the server", "1");
            if (mswitch.Enabled) reopenRestorePoints();
        }

        private void reopenRestorePoints()
        {
            try
            {
                string filename2 = Server.SecurePath("~/restores/{0:X2}.{1}.bak", DateTime.Now.ToBinary(), Server.AssemblyName);
                string filename = Server.SecurePath("~/restores/{0}.bak", Server.AssemblyName);
                readStream.Close();              //Close stream
                writeStream.Close();             //Close stream
                File.Copy(filename, filename2);  //Copy file
                WriteInformation("RestorePointCentre", "Moving current restore-point file to: {0}", filename2);

                writeStream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (writeStream.Length == 0)
                {
                    writeStream.Write(new byte[4], 0, 4);
                    writeStream.Flush();
                }
                else
                {
                    writeStream.Seek(0, SeekOrigin.End);
                }
                readStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (Exception)
            {
                WriteError("DatabaseManager", "Failed to reopen restore-point system");
            }
        }

        public bool CreateRestorePoint(Character character)
        {
            try
            {
                if (character == null || character.ModelId == 0) return false;
                byte[] bytes = Serialize(character);
                WriteBytes(character.Name, character.ModelId, bytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void WriteBytes(string name, uint uniqueid, byte[] value)
        {
            if (writeStream == null) return;
            lock (writeStream)
            {
                writeStream.Seek(0, SeekOrigin.End);                            //Seek to end

                //Write data segments
                byte[] headingBuffer = new byte[64];                            //buffer container
                CopyTo(headingBuffer, 0, (uint)value.LongLength);               //Offset 00 -  4 - Size of data
                CopyTo(headingBuffer, 4, DateTime.Now);                         //Offset  4 - 12 - Date time
                CopyTo(headingBuffer, 12, uniqueid);                            //Offset 12 - 16 - Size of data
                //Offset 16 - 32 - Resevered
                CopyTo(headingBuffer, 32, name, 16);                            //Offset 32 - 64 - Name
                writeStream.Write(headingBuffer, 0, 64);                          //Write unbuffered data to stream
                writeStream.Flush();                                            //Flush data
                writeStream.Write(value, 0, value.Length);                      //Write unbuffered data to stream
                writeStream.Flush();                                            //Flush data

                //Adjust heading segment
                writeStream.Seek(0, SeekOrigin.Begin);                          //Seek to start
                byte[] buffer = new byte[4];                                    //Container for header size
                writeStream.Read(buffer, 0, 4);                                 //Read header size
                writeStream.Seek(0, SeekOrigin.Begin);                          //Seek to start

                uint numRecords = BitConverter.ToUInt32(buffer, 0);             //Get header count
                numRecords++;                                                   //Increment number with 1
                buffer = BitConverter.GetBytes(numRecords);                     //Get new header count
                writeStream.Write(buffer, 0, 4);                                //Write new header
                writeStream.Flush();                                            //Write unbuffered data to stream
                writeStream.Seek(0, SeekOrigin.End);                            //Seek to end
            }
        }

        #endregion Backup & Restore utillities

        #region Private Methods

        /// <summary>
        /// Helper method that reappplies the weapon's effects.
        /// </summary>
        /// <remarks>
        /// Because this requires the skills to be loaded this
        /// method should be loaded after the character has finished
        /// loading.
        /// </remarks>
        /// <param name="character"></param>
        private void DoWeaponary(Character character)
        {
            int WeaponIndex = (character.weapons.ActiveWeaponIndex == 1) ? character.weapons.SeconairyWeaponIndex : character.weapons.PrimaryWeaponIndex;
            if (WeaponIndex < character.weapons.UnlockedWeaponSlots)
            {
                Weapon CurrentWeapon = character.weapons[WeaponIndex];
                if (character.FindRequiredRootSkill(CurrentWeapon.Info.weapon_skill))
                {
                    //Default battle additions
                    character._status.MaxWMAttack += (ushort)CurrentWeapon.Info.max_magic_attack;
                    character._status.MinWMAttack += (ushort)CurrentWeapon.Info.min_magic_attack;
                    character._status.MaxWPAttack += (ushort)CurrentWeapon.Info.max_short_attack;
                    character._status.MinWPAttack += (ushort)CurrentWeapon.Info.min_short_attack;
                    character._status.MaxWRAttack += (ushort)CurrentWeapon.Info.max_range_attack;
                    character._status.MinWRAttack += (ushort)CurrentWeapon.Info.min_range_attack;

                    //Reapplies alterstone additions
                    for (int i = 0; i < 8; i++)
                    {
                        uint addition = CurrentWeapon.Slots[i];
                        if (addition > 0)
                        {
                            Singleton.Additions.ApplyAddition(addition, character);
                        }
                    }

                    CurrentWeapon._active = 1;
                }
                else
                {
                    CurrentWeapon._active = 0;
                }
            }
        }

        #endregion Private Methods

        #region Wrapped Members

        public bool IsQuestComplete(Character character, uint QuestId)
        {
            return InternalDatabaseProvider.IsQuestComplete(character.ModelId, QuestId);
        }

        public bool MarkAsReadMailItem(uint id)
        {
            return InternalDatabaseProvider.MarkAsReadMailItem(id);
        }

        public void LoadSkills(Character character)
        {
            InternalDatabaseProvider.LoadSkills(character, character.ModelId);
        }

        public IEnumerable<uint> GetAllLearnedSkills(Character target)
        {
            return InternalDatabaseProvider.GetAllLearnedSkills(target);
        }

        public List<uint> GetJobSpeciaficSkills(Character target, byte job)
        {
            return InternalDatabaseProvider.GetJobSpeciaficSkills(target, job);
        }

        public bool InsertNewSkill(uint CharId, uint SkillId, byte job)
        {
            return InternalDatabaseProvider.InsertNewSkill(CharId, SkillId, job);
        }

        public bool InsertNewSkill(Character target, uint SkillId, uint Experience)
        {
            return InternalDatabaseProvider.InsertNewSkill(target.ModelId, SkillId, target.job, Experience);
        }

        public bool UpdateSkill(Character target, uint SkillId, uint Experience)
        {
            return InternalDatabaseProvider.UpdateSkill(target, SkillId, Experience);
        }

        public bool UpgradeSkill(Character target, uint OldSkillId, uint NewSkillId, uint Experience)
        {
            return InternalDatabaseProvider.UpgradeSkill(target, OldSkillId, NewSkillId, Experience);
        }

        #region Mails

        public bool DeleteMails(uint id)
        {
            return InternalDatabaseProvider.DeleteMails(id);
        }

        public IEnumerable<Mail> GetInboxMail(Character target)
        {
            return InternalDatabaseProvider.GetInboxMail(target);
        }

        public IEnumerable<Mail> GetOutboxMail(Character target)
        {
            return InternalDatabaseProvider.GetOutboxMail(target);
        }

        public bool InsertNewMailItem(Character target, MailItem item)
        {
            return InternalDatabaseProvider.InsertNewMailItem(target, item);
        }

        public int GetInboxUncheckedCount(string name)
        {
            return InternalDatabaseProvider.GetInboxUncheckedCount(name);
        }

        public MailItem GetMailItemById(uint id)
        {
            return InternalDatabaseProvider.GetMailItemById(id);
        }

        public uint GetZenyAttachment(uint id)
        {
            return InternalDatabaseProvider.GetZenyAttachment(id);
        }

        public bool UpdateItemAttachment(uint MailId, Rag2Item Attachment)
        {
            return InternalDatabaseProvider.UpdateItemAttachment(MailId, Attachment);
        }

        public Rag2Item GetItemAttachment(uint MailId)
        {
            return InternalDatabaseProvider.GetItemAttachment(MailId);
        }

        public bool UpdateZenyAttachment(uint Id, uint Zeny)
        {
            return InternalDatabaseProvider.UpdateZenyAttachment(Id, Zeny);
        }

        public IEnumerable<KeyValuePair<string, uint>> GetPendingMails()
        {
            return InternalDatabaseProvider.GetPendingMails();
        }

        /// <summary>
        /// Returns the number of emails found in the inbox of
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public int GetInboxMailCount(string name)
        {
            return InternalDatabaseProvider.GetInboxMailCount(name);
        }

        public bool GetItemByAuctionId(uint id, out AuctionArgument item)
        {
            return InternalDatabaseProvider.GetItemByAuctionId(id, out item);
        }

        public bool DeleteRegisteredAuctionItem(uint id)
        {
            return InternalDatabaseProvider.DeleteRegisteredAuctionItem(id);
        }

        public bool ClearPendingMails(string Receiptent)
        {
            return InternalDatabaseProvider.ClearPendingMails(Receiptent);
        }

        public bool DeleteMailFromOutbox(uint id)
        {
            return InternalDatabaseProvider.DeleteMailFromOutbox(id);
        }

        #endregion Mails

        #region Market

        /// <summary>
        /// Search market
        /// </summary>
        /// <param name="Argument"></param>
        /// <returns></returns>
        public IEnumerable<MarketItemArgument> SearchMarket(MarketSearchArgument Argument)
        {
            return InternalDatabaseProvider.SearchMarket(Argument);
        }

        /// <summary>
        /// Search for owner market items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MarketItemArgument> SearchMarketForOwner(Character target)
        {
            return InternalDatabaseProvider.SearchMarketForOwner(target);
        }

        /// <summary>
        /// Registers a new market item
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public uint RegisterNewMarketItem(Character target, MarketItemArgument arg)
        {
            return InternalDatabaseProvider.RegisterNewMarketItem(target, arg);
        }

        /// <summary>
        /// Deregister a new market item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint UnregisterMarketItem(uint id)
        {
            return InternalDatabaseProvider.UnregisterMarketItem(id);
        }

        /// <summary>
        /// Find a comment by a given player id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindCommentByPlayerId(uint id)
        {
            return InternalDatabaseProvider.FindCommentByPlayerId(id);
        }

        /// <summary>
        /// Find comment of a given item id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindCommentById(uint id)
        {
            return InternalDatabaseProvider.FindCommentById(id);
        }

        /// <summary>
        /// Update the current comment of a given characterid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public uint UpdateCommentByPlayerId(uint id, string message)
        {
            return InternalDatabaseProvider.UpdateCommentByPlayerId(id, message);
        }

        /// <summary>
        /// Returns the number of registered items by the character
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int GetOwnerItemCount(Character target)
        {
            return InternalDatabaseProvider.GetOwnerItemCount(target);
        }

        #endregion Market

        /// <summary>
        /// Finds a list of characters per given playerid
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function is internally used by our login server.
        /// </remarks>
        public IEnumerable<CharInfo> FindCharacters(uint PlayerId)
        {
            return InternalDatabaseProvider.FindCharacters(PlayerId);
        }

        /// <summary>
        /// Finds the character details of a character per given characterid
        /// </summary>
        /// <param name="CharId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool FindCharacterDetails(uint CharId, out CharDetails details)
        {
            return InternalDatabaseProvider.FindCharacterDetails(CharId, out details);
        }

        /// <summary>
        /// Verifies if a given charactername already exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VerifyNameExists(string name)
        {
            return InternalDatabaseProvider.VerifyNameExists(name);
        }

        /// <summary>
        /// Deletes a given character by their id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCharacterById(uint id)
        {
            return InternalDatabaseProvider.DeleteCharacterById(id);
        }

        public bool GiveItemReward(string name, uint itemid, byte count)
        {
            uint playerid = 0;
            if (InternalDatabaseProvider.GetPlayerId(name, out playerid))
            {
                return InternalDatabaseProvider.CreateEventItem(playerid, itemid, count);
            }

            return false;
        }

        #region Quests

        /// <summary>
        /// Get all available quests per given region
        /// </summary>
        /// <param name="target"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetAvailableQuestsByRegion(Character target, uint modelid)
        {
            return InternalDatabaseProvider.GetAvailableQuestsByRegion(target, modelid);
        }

        /// <summary>
        /// Gets a list of personal requests.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="region"></param>
        /// <param name="CurrentPersonalQuest"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<uint, uint>> GetPersonalAvailableQuestsByRegion(Character target, byte region, uint CurrentPersonalQuest)
        {
            return InternalDatabaseProvider.GetPersonalAvailableQuestsByRegion(target, region, CurrentPersonalQuest);
        }

        /// <summary>
        /// Completes a quest directly into the database.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="QuestId"></param>
        /// <returns></returns>
        public bool QuestComplete(uint charId, uint QuestId)
        {
            return InternalDatabaseProvider.QuestComplete(charId, QuestId);
        }

        #endregion Quests

        #region Blacklist

        /// <summary>
        /// Adds a certain character name on the friendlist.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool InsertAsFriend(uint charId, string friend)
        {
            return InternalDatabaseProvider.InsertAsFriend(charId, friend);
        }

        /// <summary>
        /// Deletes a certain character name from the friendlist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool DeleteFriend(uint charId, string friend)
        {
            return InternalDatabaseProvider.DeleteFriend(charId, friend);
        }

        /// <summary>
        /// Inserts a certain character on the blacklist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool InsertAsBlacklist(uint charId, string friend, byte reason)
        {
            return InternalDatabaseProvider.InsertAsBlacklist(charId, friend, reason);
        }

        /// <summary>
        /// Deletes a certain character from the blacklist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool DeleteBlacklist(uint charId, string friend)
        {
            return InternalDatabaseProvider.DeleteBlacklist(charId, friend);
        }

        #endregion Blacklist

        #region SQL Provider

        public IDataReader ExecuteDataReader(IQueryProvider query)
        {
            return InternalDatabaseProvider.ExecuteDataReader(query);
        }

        public IDataReader ExecuteDataReader(IQueryProvider query, CommandBehavior behavior)
        {
            return InternalDatabaseProvider.ExecuteDataReader(query, behavior);
        }

        public int ExecuteNonQuery(IQueryProvider query)
        {
            return InternalDatabaseProvider.ExecuteNonQuery(query);
        }

        public IQueryProvider GetQueryProvider()
        {
            return InternalDatabaseProvider.GetQueryProvider();
        }

        #endregion SQL Provider

        #region Junk

        /// <summary>
        /// Retrieves a list of item rewards that were given by
        /// to the specified character
        /// </summary>
        /// <param name="target">Character instance to retrieve</param>
        /// <returns>A collection of EventItemLists</returns>
        public Collection<EventItem> FindEventItemList(Character target)
        {
            return InternalDatabaseProvider.FindEventItemList(target);
        }

        /// <summary>
        /// Retrieves a even item reward based on the given unqiue id,
        /// </summary>
        /// <param name="target">Owner character of the item</param>
        /// <param name="RewardId">Unique id of the reward</param>
        /// <returns>a single eventitem reward</returns>
        public EventItem FindEventItemById(Character target, uint RewardId)
        {
            return InternalDatabaseProvider.FindEventItemById(target, RewardId);
        }

        /// <summary>
        /// Deletes a event item reward based on the given unique id.
        /// </summary>
        /// <param name="RewardId"></param>
        /// <returns></returns>
        public bool DeleteEventItemId(uint RewardId)
        {
            return InternalDatabaseProvider.DeleteEventItemId(RewardId);
        }

        #endregion Junk

        #endregion Wrapped Members

        #region Nested Types

        [Serializable]
        private struct DBQProvider : IInfoProvider2
        {
            #region Private Members

            private uint owner;
            private Character character;

            #endregion Private Members

            #region IInfoProvider2 Members

            IDataWeaponCollection IInfoProvider2.createWeaponCollection()
            {
                DataWeapon dataweapons = new DataWeapon(character);
                return dataweapons;
            }

            IDataCharacter IInfoProvider2.createDataCharacter()
            {
                DataCharacter collection = new DataCharacter(character);
                return collection;
            }

            IDataAdditionCollection IInfoProvider2.createAdditionCollection()
            {
                DataAdditionsCollection collection = new DataAdditionsCollection(character);
                return collection;
            }

            IDataSortableItemCollection IInfoProvider2.createInventoryCollection()
            {
                DataInventoryCollection collection = new DataInventoryCollection(character);
                return collection;
            }

            IDataSortableItemCollection IInfoProvider2.createStorageCollection()
            {
                DataStorageCollection collection = new DataStorageCollection(character);
                return collection;
            }

            IDataJobinformationCollection IInfoProvider2.createJobCollection()
            {
                DataJobCollection jobCollection = new DataJobCollection(character);
                return jobCollection;
            }

            IDataZoneInformationCollection IInfoProvider2.createDataZoneCollection()
            {
                DataZoneCollection zoneInformation = new DataZoneCollection(character);
                return zoneInformation;
            }

            IDataEquipmentCollection IInfoProvider2.createEquipmentCollection()
            {
                DataEquipmentCollection collection = new DataEquipmentCollection(character);
                return collection;
            }

            IDataSkillCollection IInfoProvider2.createSkillCollection()
            {
                DataSkillCollection collection = new DataSkillCollection(character);
                return collection;
            }

            IDatabaseQuestStream IInfoProvider2.createDatabaseQuestStream()
            {
                DatabaseQuestStream collection = new DatabaseQuestStream(character);
                return collection;
            }

            IDataSpecialSkillCollection IInfoProvider2.createDatabaseSpecialSkillCollection()
            {
                DataSpecialSkillCollection collection = new DataSpecialSkillCollection(character);
                return collection;
            }

            IDatabaseFriendList IInfoProvider2.createDatabaseFriendList()
            {
                DataFriendlistCollection collection = new DataFriendlistCollection(character);
                return collection;
            }

            IDatabaseBlacklist IInfoProvider2.createDatabaseBlacklist()
            {
                DataBlacklist collection = new DataBlacklist(character);
                return collection;
            }

            #endregion IInfoProvider2 Members

            #region Constructor

            public DBQProvider(Character character)
            {
                this.character = character;
                this.owner = 0;
            }

            uint IInfoProvider2.OwnerId
            {
                get
                {
                    return owner;
                }
                set
                {
                    if (owner > 0)
                        throw new NotImplementedException();
                    owner = value;
                }
            }

            public uint OwnerId
            {
                get
                {
                    return owner;
                }
                set
                {
                    if (owner > 0)
                        throw new NotImplementedException();
                    owner = value;
                }
            }

            #endregion Constructor
        }

        protected sealed class DatabaseQuestStream : IDatabaseQuestStream
        {
            #region Private Members

            private Character owner;

            #endregion Private Members

            #region IDatabaseQuestStream Members

            uint IDatabaseQuestStream.CharacterId
            {
                get { return owner.ModelId; }
            }

            byte[] IDatabaseQuestStream.questCollection
            {
                get
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream())
                        using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
                        {
                            ObjectiveList.Serialize(gzip, owner.QuestObjectives);
                            gzip.Flush();
                            gzip.Close();
                            return stream.GetBuffer();
                        }
                    }
                    catch (SerializationException)
                    {
                        return null;
                    }
                }
                set
                {
                    MemoryStream stream = null;
                    if (value == null) return;

                    try
                    {
                        stream = new MemoryStream(value);
                        using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            ObjectiveList list = null;
                            ObjectiveList.Deserialize(gzip, out list);
                            gzip.Close();
                            owner.QuestObjectives = list;
                        }
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (stream != null) stream.Dispose();
                    }
                }
            }

            #endregion IDatabaseQuestStream Members

            #region Constructor

            public DatabaseQuestStream(Character target)
            {
                owner = target;
            }

            #endregion Constructor
        }

        protected sealed class DataWeapon : IDataWeaponCollection
        {
            #region Private Members

            /// <summary>
            /// Container for the character
            /// </summary>
            private uint owner;

            /// <summary>
            /// Container for the weapon collection
            /// </summary>
            private WeaponCollection collection;

            #endregion Private Members

            #region Public Members

            public WeaponCollection Collection
            {
                get
                {
                    return collection;
                }
            }

            #endregion Public Members

            #region IDataWeaponCollection  Members

            uint IDataWeaponCollection.CharacterId
            {
                get { return owner; }
            }

            Weapon IDataWeaponCollection.this[int index]
            {
                get
                {
                    return collection[index];
                }
                set
                {
                    collection[index] = value;
                }
            }

            byte IDataWeaponCollection.UnlockedWeaponSlots
            {
                get
                {
                    return collection.UnlockedWeaponSlots;
                }
                set
                {
                    collection.UnlockedWeaponSlots = value;
                }
            }

            byte IDataWeaponCollection.PrimaryWeaponIndex
            {
                get
                {
                    return collection.PrimaryWeaponIndex;
                }
                set
                {
                    collection.PrimaryWeaponIndex = value;
                }
            }

            byte IDataWeaponCollection.SeconairyWeaponIndex
            {
                get
                {
                    return collection.SeconairyWeaponIndex;
                }
                set
                {
                    collection.SeconairyWeaponIndex = value;
                }
            }

            byte IDataWeaponCollection.ActiveWeaponIndex
            {
                get
                {
                    return collection.ActiveWeaponIndex;
                }
                set
                {
                    collection.ActiveWeaponIndex = value;
                }
            }

            #endregion IDataWeaponCollection  Members

            #region Constructor

            public DataWeapon(Character target)
            {
                owner = target.ModelId;
                collection = target.weapons;
            }

            public DataWeapon(uint characterid)
            {
                owner = characterid;
                collection = new WeaponCollection();
            }

            public DataWeapon(uint characterid, WeaponCollection weapons)
            {
                owner = characterid;
                collection = weapons;
            }

            #endregion Constructor
        }

        protected sealed class DataCharacter : IDataCharacter
        {
            #region Private Members

            private Character owner;

            #endregion Private Members

            #region IDataCharacter Members

            uint IDataCharacter.CharacterId
            {
                get { return owner.ModelId; }
                set { if (owner.ModelId > 0) throw new NotSupportedException(); owner.ModelId = value; }
            }

            string IDataCharacter.CharacterName
            {
                get
                {
                    return owner.Name;
                }
                set
                {
                    if (value != null)
                        owner.Name = value;
                    else
                        owner.Name = string.Empty;
                }
            }

            byte[] IDataCharacter.CharacterFace
            {
                get { return owner.FaceDetails; }
            }

            uint IDataCharacter.CharacterExperience
            {
                get
                {
                    return owner.Cexp;
                }
                set
                {
                    int prev = Singleton.CharacterConfiguration.CalculateMaximumHP(owner);
                    owner._status.MaxHP -= prev;
                    owner._level = Singleton.experience.FindClvlByCexp(value);
                    owner.Cexp = value;
                    int curr = Singleton.CharacterConfiguration.CalculateMaximumHP(owner);
                    owner._status.MaxHP += curr;
                }
            }

            uint IDataCharacter.JobExperience
            {
                get
                {
                    return owner.Jexp;
                }
                set
                {
                    owner.jlvl = Singleton.experience.FindJlvlByJexp(value);
                    owner.Jexp = value;
                }
            }

            byte IDataCharacter.Job
            {
                get
                {
                    return owner.job;
                }
                set
                {
                    int prev = Singleton.CharacterConfiguration.CalculateMaximumSP(owner);
                    owner._status.MaxSP -= prev;
                    owner.job = value;
                    int curr = Singleton.CharacterConfiguration.CalculateMaximumSP(owner);
                    owner._status.MaxSP += curr;
                }
            }

            ushort IDataCharacter.HP
            {
                get
                {
                    return owner.HP;
                }
                set
                {
                    owner.HP = value;
                }
            }

            ushort IDataCharacter.SP
            {
                get
                {
                    return owner.SP;
                }
                set
                {
                    owner.SP = value;
                }
            }

            byte IDataCharacter.LP
            {
                get
                {
                    return owner._status.CurrentLp;
                }
                set
                {
                    owner._status.CurrentLp = value;
                }
            }

            byte IDataCharacter.Oxygen
            {
                get
                {
                    return (byte)owner._status.CurrentOxygen;
                }
                set
                {
                    owner._status.CurrentOxygen = value;
                }
            }

            WorldCoordinate IDataCharacter.Position
            {
                get
                {
                    return new WorldCoordinate(owner.Position, owner.map);
                }
                set
                {
                    if (value.map == 0)
                    {
                        if (owner.savelocation.map == 0)
                        {
                            if (owner.race == 255)
                            {
                                //Throw exception when race is not set
                                throw new ArgumentException("race is not set ensure race setting is set prior having a fallback");
                            }
                            else if (owner.race == 0)
                            {
                                //Recover with norman settings
                                Saga.Factory.CharacterConfiguration.IDefaultCharacterSettings settings = Singleton.CharacterConfiguration.Normans;
                                owner.map = settings.DefaultLocation.map;
                                owner.Position = settings.DefaultLocation.coords;
                            }
                            else if (owner.race == 1)
                            {
                                //Recover with ellr settings
                                Saga.Factory.CharacterConfiguration.IDefaultCharacterSettings settings = Singleton.CharacterConfiguration.Ellr;
                                owner.map = settings.DefaultLocation.map;
                                owner.Position = settings.DefaultLocation.coords;
                            }
                            else if (owner.race == 2)
                            {
                                //Recover with dimago settings
                                Saga.Factory.CharacterConfiguration.IDefaultCharacterSettings settings = Singleton.CharacterConfiguration.Dimago;
                                owner.map = settings.DefaultLocation.map;
                                owner.Position = settings.DefaultLocation.coords;
                            }
                        }
                        else
                        {
                            owner.map = owner.savelocation.map;
                            owner.Position = owner.savelocation.coords;
                        }
                    }
                    else
                    {
                        owner.map = value.map;
                        owner.Position = value.coords;
                    }
                }
            }

            WorldCoordinate IDataCharacter.SavePosition
            {
                get
                {
                    return owner.savelocation;
                }
                set
                {
                    owner.savelocation = value;
                }
            }

            ushort IDataCharacter.Strength
            {
                get
                {
                    return owner.stats.CHARACTER.strength;
                }
                set
                {
                    ushort previous = owner.stats.CHARACTER.strength;
                    owner._status.MaxPAttack -= (ushort)(2 * previous);
                    owner._status.MinPAttack -= (ushort)(1 * previous);
                    owner._status.MaxHP -= (ushort)(10 * previous);
                    owner._status.MaxPAttack += (ushort)(2 * value);
                    owner._status.MinPAttack += (ushort)(1 * value);
                    owner._status.MaxHP += (ushort)(10 * value);
                    owner.stats.CHARACTER.strength = value;
                }
            }

            ushort IDataCharacter.Dexterity
            {
                get
                {
                    return owner.stats.CHARACTER.dexterity;
                }
                set
                {
                    ushort previous = owner.stats.CHARACTER.dexterity;
                    owner._status.BasePHitrate -= (ushort)(1 * previous);
                    owner._status.BasePHitrate += (ushort)(1 * value);
                    owner._status.BasePCritrate -= (ushort)(1 * previous);
                    owner._status.BasePCritrate += (ushort)(1 * value);
                    owner.stats.CHARACTER.dexterity = value;
                }
            }

            ushort IDataCharacter.Intellect
            {
                get
                {
                    return owner.stats.CHARACTER.intelligence;
                }
                set
                {
                    ushort previous = owner.stats.CHARACTER.intelligence;
                    owner._status.MaxMAttack -= (ushort)(6 * previous);
                    owner._status.MinMAttack -= (ushort)(3 * previous);
                    owner._status.BaseRHitrate -= (ushort)(1 * previous);
                    owner._status.MaxMAttack += (ushort)(6 * value);
                    owner._status.MinMAttack += (ushort)(3 * value);
                    owner._status.BaseRHitrate += (ushort)(1 * value);
                    owner.stats.CHARACTER.intelligence = value;
                }
            }

            ushort IDataCharacter.Concentration
            {
                get
                {
                    return owner.stats.CHARACTER.concentration;
                }
                set
                {
                    ushort previous = owner.stats.CHARACTER.concentration;
                    owner._status.MaxRAttack -= (ushort)(4 * previous);
                    owner._status.MinRAttack -= (ushort)(2 * previous);
                    owner._status.BasePHitrate -= (ushort)(2 * previous);
                    owner._status.MaxRAttack += (ushort)(4 * value);
                    owner._status.MinRAttack += (ushort)(2 * value);
                    owner._status.BasePHitrate += (ushort)(2 * value);
                    owner.stats.CHARACTER.concentration = value;
                }
            }

            ushort IDataCharacter.Luck
            {
                get
                {
                    return owner.stats.CHARACTER.luck;
                }
                set
                {
                    ushort previous = owner.stats.CHARACTER.luck;
                    owner._status.BasePEvasionrate -= (ushort)(1 * previous);
                    owner._status.BaseREvasionrate -= (ushort)(1 * previous);
                    owner._status.BaseMEvasionrate -= (ushort)(1 * previous);
                    owner._status.BasePEvasionrate += (ushort)(1 * value);
                    owner._status.BaseREvasionrate += (ushort)(1 * value);
                    owner._status.BaseMEvasionrate += (ushort)(1 * value);
                    owner.stats.CHARACTER.luck = value;
                }
            }

            ushort IDataCharacter.Remaining
            {
                get
                {
                    return owner.stats.REMAINING;
                }
                set
                {
                    owner.stats.REMAINING = value;
                }
            }

            uint IDataCharacter.Zeny
            {
                get
                {
                    return owner.ZENY;
                }
                set
                {
                    owner.ZENY = value;
                }
            }

            #endregion IDataCharacter Members

            #region Constructor

            public DataCharacter(Character target)
            {
                owner = target;
            }

            public DataCharacter(uint characterid)
            {
                owner = new Character(null, characterid, 0);
            }

            #endregion Constructor
        }

        protected sealed class DataAdditionsCollection : IDataAdditionCollection
        {
            #region Private Members

            private uint owner;
            private Addition additions;

            #endregion Private Members

            #region IDataAdditionCollection Members

            uint IDataAdditionCollection.CharacterId
            {
                get { return owner; }
            }

            bool IDataAdditionCollection.Create(uint addition, uint duration)
            {
                Saga.Factory.Additions.Info info;
                if (Singleton.Additions.TryGetAddition(addition, out info))
                {
                    if (duration > 0)
                    {
                        AdditionState state = new AdditionState(addition, duration, info);
                        state.canexpire = true;
                        additions.timed_additions.Add(state);
                    }
                    else
                    {
                        AdditionState state = new AdditionState(addition, duration, info);
                        additions.additions.Add(state);
                    }

                    return true;
                }

                return false;
            }

            IEnumerable<AdditionState> IDataAdditionCollection.Additions
            {
                get
                {
                    foreach (AdditionState state in additions)
                        yield return state;
                }
            }

            #endregion IDataAdditionCollection Members

            #region Constructor

            public DataAdditionsCollection(Character target)
            {
                owner = target.ModelId;
                additions = target._additions;
            }

            public DataAdditionsCollection(uint characterid)
            {
                this.owner = characterid;
                this.additions = new Addition();
            }

            #endregion Constructor
        }

        protected sealed class DataInventoryCollection : IDataSortableItemCollection
        {
            #region Private Members

            private Rag2Collection collection;
            private uint owner;
            private byte sortmode;

            #endregion Private Members

            #region IDataSortableItemCollection Members

            uint IDataSortableItemCollection.CharacterId
            {
                get { return owner; }
            }

            byte IDataSortableItemCollection.SortationMode
            {
                get
                {
                    return sortmode;
                }
                set
                {
                    sortmode = value;
                }
            }

            Rag2Collection IDataSortableItemCollection.Collection
            {
                get { return collection; }
            }

            #endregion IDataSortableItemCollection Members

            #region Constructor

            public DataInventoryCollection(Character target)
            {
                owner = target.ModelId;
                collection = target.container;
                //sortmode = target.
            }

            public DataInventoryCollection(uint characterId, Rag2Collection container)
            {
                owner = characterId;
                collection = container;
            }

            #endregion Constructor
        }

        protected sealed class DataStorageCollection : IDataSortableItemCollection
        {
            #region Private Members

            private Rag2Collection collection;
            private uint owner;
            private byte sortmode;

            #endregion Private Members

            #region IDataSortableItemCollection Members

            uint IDataSortableItemCollection.CharacterId
            {
                get { return owner; }
            }

            byte IDataSortableItemCollection.SortationMode
            {
                get
                {
                    return sortmode;
                }
                set
                {
                    sortmode = value;
                }
            }

            Rag2Collection IDataSortableItemCollection.Collection
            {
                get { return collection; }
            }

            #endregion IDataSortableItemCollection Members

            #region Constructor

            public DataStorageCollection(Character target)
            {
                owner = target.ModelId;
                collection = target.STORAGE;
                //sortmode = target.
            }

            public DataStorageCollection(uint characterId)
            {
                owner = characterId;
                collection = new Rag2Collection();
            }

            #endregion Constructor
        }

        protected sealed class DataJobCollection : IDataJobinformationCollection
        {
            #region Private Members

            private uint owner;
            private byte[] jobinfo;

            #endregion Private Members

            #region IDataJobinformationCollection Members

            uint IDataJobinformationCollection.CharacterId
            {
                get { return owner; }
            }

            byte[] IDataJobinformationCollection.Joblevels
            {
                get { return jobinfo; }
            }

            #endregion IDataJobinformationCollection Members

            #region Constructor

            public DataJobCollection(Character target)
            {
                owner = target.ModelId;
                jobinfo = target.CharacterJobLevel;
            }

            public DataJobCollection(uint target, byte[] jobInfo)
            {
                owner = target;
                jobinfo = jobInfo;
            }

            #endregion Constructor
        }

        protected sealed class DataZoneCollection : IDataZoneInformationCollection
        {
            #region Private Members

            private uint owner;
            private byte[] zoneinfo;

            #endregion Private Members

            #region IDateZoneInformationCollection Members

            uint IDataZoneInformationCollection.CharacterId
            {
                get { return owner; }
            }

            byte[] IDataZoneInformationCollection.ZoneInformation
            {
                get { return zoneinfo; }
            }

            #endregion IDateZoneInformationCollection Members

            #region Constructor

            public DataZoneCollection(Character target)
            {
                owner = target.ModelId;
                zoneinfo = target.ZoneInformation;
            }

            public DataZoneCollection(uint characterId, byte[] zoneInfo)
            {
                owner = characterId;
                zoneinfo = zoneInfo;
            }

            #endregion Constructor
        }

        protected sealed class DataEquipmentCollection : IDataEquipmentCollection
        {
            #region Private Members

            private uint owner;
            private Rag2Item[] equips;

            #endregion Private Members

            #region IDataEquipmentCollection Members

            uint IDataEquipmentCollection.CharacterId
            {
                get { return owner; }
            }

            Rag2Item[] IDataEquipmentCollection.Equipment
            {
                get { return equips; }
            }

            #endregion IDataEquipmentCollection Members

            #region Constructor

            public DataEquipmentCollection(Character target)
            {
                owner = target.ModelId;
                equips = target.Equipment;
            }

            public DataEquipmentCollection(uint characterId, Rag2Item[] equipment)
            {
                owner = characterId;
                equips = equipment;
            }

            #endregion Constructor
        }

        protected sealed class DataSkillCollection : IDataSkillCollection
        {
            #region Private Members

            private uint owner;
            private byte job;
            private List<Skill> skills;

            #endregion Private Members

            #region IDataSkillCollection Members

            uint IDataSkillCollection.CharacterId
            {
                get { return owner; }
            }

            byte IDataSkillCollection.Job
            {
                get { return job; }
            }

            List<Skill> IDataSkillCollection.Skills
            {
                get { return skills; }
            }

            #endregion IDataSkillCollection Members

            #region Constructor

            public DataSkillCollection(Character target)
            {
                owner = target.ModelId;
                skills = target.learnedskills;
                job = target.job;
            }

            #endregion Constructor
        }

        protected sealed class DataSpecialSkillCollection : IDataSpecialSkillCollection
        {
            #region Private Members

            private uint owner;
            private Skill[] skills;

            #endregion Private Members

            #region IDataSpecialSkillCollection Members

            public uint CharacterId
            {
                get { return owner; }
            }

            public Skill[] specialSkillCollection
            {
                get { return skills; }
            }

            #endregion IDataSpecialSkillCollection Members

            #region Constructor

            public DataSpecialSkillCollection(Character target)
            {
                owner = target.ModelId;
                skills = target.SpecialSkills;
            }

            #endregion Constructor
        }

        protected sealed class DataFriendlistCollection : IDatabaseFriendList
        {
            #region Private Members

            private uint owner;
            private List<string> friends;

            #endregion Private Members

            #region IDatabaseFriendList Members

            public uint CharacterId
            {
                get { return owner; }
            }

            List<string> IDatabaseFriendList.friends
            {
                get { return friends; }
            }

            #endregion IDatabaseFriendList Members

            #region Constructor

            public DataFriendlistCollection(Character target)
            {
                owner = target.ModelId;
                friends = target._friendlist;
            }

            #endregion Constructor
        }

        protected sealed class DataBlacklist : IDatabaseBlacklist
        {
            #region Private Members

            private uint owner;
            private List<KeyValuePair<string, byte>> blacklist;

            #endregion Private Members

            #region IDatabaseBlacklist Members

            public uint CharacterId
            {
                get { return owner; }
            }

            List<KeyValuePair<string, byte>> IDatabaseBlacklist.blacklist
            {
                get { return blacklist; }
            }

            #endregion IDatabaseBlacklist Members

            #region Constructor

            public DataBlacklist(Character target)
            {
                owner = target.ModelId;
                blacklist = target._blacklist;
            }

            #endregion Constructor
        }

        #endregion Nested Types
    }
}

namespace Saga.Data
{
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code>
    /// <![CDATA[public bool Connect(ConnectionInfo info)
    ///{
    ///    //REFERENCE OUR CONNECTION INFO
    ///    this.info = info;
    ///    //GENERATE A EXCEPTION QEUEE
    ///    List<MySqlException> Exceptions = new List<MySqlException>();
    ///    bool success = false;
    ///
    ///    //CREATE 5 CONNECTIONS
    ///    for (int i = 0; i < 5; i++)
    ///    {
    ///        try
    ///        {
    ///            MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder();
    ///            cb.UserID = info.username;
    ///            cb.Password = info.password;
    ///            cb.Port = info.port;
    ///            cb.Server = info.host;
    ///            cb.Database = info.database;
    ///
    ///            MySqlConnection conn = new MySqlConnection(cb.ConnectionString);
    ///            conn.Open();
    ///            System.Threading.Timer myTimer = new System.Threading.Timer(callback, conn, 300000, 300000);
    ///            ConnectionPool.Items = conn;
    ///            success = true;
    ///        }
    ///        catch (MySqlException e)
    ///        {
    ///            Exceptions.Add(e);
    ///        }
    ///    }
    ///
    ///    //ONLY OUTPUT EXCEPTIONS IF WE HAVE NO CONNECTION
    ///    if (success == false)
    ///    {
    ///        foreach (MySqlException e in Exceptions)
    ///        {
    ///            Trace.TraceError(e.ToString());
    ///        }
    ///    }
    ///
    ///    return success;
    ///}]]></code>
    /// </example>
    /// <remarks>
    /// This argument is parsed to the database plugin on the OnConnect event. You can use
    /// this information to establish a database connection wether your database is plain-text (csv)
    /// or a DBMS system such as MySQL.
    /// </remarks>
    public sealed class ConnectionInfo
    {
        /// <summary>
        /// Get's or set's the host of the database
        /// </summary>
        public string host;

        /// <summary>
        /// Get's or set's the password used to connect to the database
        /// </summary>
        public string password;

        /// <summary>
        /// Get's or set's the username used to connect to the database
        /// </summary>
        public string username;

        /// <summary>
        /// Get's or set's the databasename used to connect to the database
        /// </summary>
        public string database;

        /// <summary>
        /// Get's or set's the port used to connect to the database
        /// </summary>
        public uint port;

        /// <summary>
        /// Get's or set's the number of pooled connections
        /// </summary>
        public int pooledconnections;
    }
}