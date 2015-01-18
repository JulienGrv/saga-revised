using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Saga.Shared.Definitions;
using Saga.Map.Utils.Definitions.Misc;
using System.Data;
using Saga.PrimaryTypes;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        public void LoadSkills(Character character, uint CharId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlDataReader reader = null;
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_60, connection);
                command.Parameters.AddWithValue("CharId", CharId);

                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {
                    Skill skill = new Skill();
                    skill.Id = reader.GetUInt32(0);
                    skill.Experience = reader.GetUInt32(1);
                    if (Singleton.SpellManager.TryGetSpell(skill.Id, out skill.info) && skill.info.requiredJobs[character.job - 1] == 1)
                        character.learnedskills.Add(skill);
                }
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }
        public IEnumerable<uint> GetAllLearnedSkills(MySqlConnection connection, Character target)
        {
            MySqlCommand command = new MySqlCommand(_query_61, connection);
            command.Parameters.AddWithValue("CharId", target.ModelId);
            MySqlDataReader reader = null;
            List<uint> Skills = new List<uint>();

            try
            {
                //LOAD ALL SKILL INFORMATION

                
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {
                    Skills.Add(reader.GetUInt32(0));
                }

                return Skills;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return Skills;
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
            }
        }


        public bool UpdateSkill(Character target, uint SkillId, uint Experience)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_62, connection);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                command.Parameters.AddWithValue("SkillId", SkillId);
                command.Parameters.AddWithValue("SkillExp", Experience);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //REPOOL OUR CONNECTION
                ConnectionPool.Release(connection);
            }
        }
        public bool InsertNewSkill(uint CharId, uint SkillId, byte job)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_63, connection);
                command.Parameters.AddWithValue("CharId", CharId);
                command.Parameters.AddWithValue("SkillId", SkillId);
                command.Parameters.AddWithValue("SkillExp", 0);
                command.Parameters.AddWithValue("Job", job);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //REPOOL OUR CONNECTION
                ConnectionPool.Release(connection);
            }
        }
        public bool InsertNewSkill(uint CharId, uint SkillId, byte job, uint Experience)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_63, connection);
                command.Parameters.AddWithValue("CharId", CharId);
                command.Parameters.AddWithValue("SkillId", SkillId);
                command.Parameters.AddWithValue("SkillExp", Experience);
                command.Parameters.AddWithValue("Job", job);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //REPOOL OUR CONNECTION
                ConnectionPool.Release(connection);
            }
        }
        public bool UpgradeSkill(Character target, uint OldSkillId, uint NewSkillId, uint Experience)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_64, connection);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                command.Parameters.AddWithValue("NewSkillId", NewSkillId);
                command.Parameters.AddWithValue("SkillId", OldSkillId);
                command.Parameters.AddWithValue("SkillExp", 0);
                command.Parameters.AddWithValue("Job", target.job);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                //REPOOL OUR CONNECTION
                ConnectionPool.Release(connection);
            }
        }
        public List<uint> GetJobSpeciaficSkills(MySqlConnection connection, Character target, byte job)
        {
            List<uint> MyList = new List<uint>();
            MySqlDataReader reader = null;
            try
            {
                //LOAD ALL SKILL INFORMATION
                MySqlCommand command = new MySqlCommand(_query_65, connection);
                command.Parameters.AddWithValue("CharId", target.ModelId);

                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                while (reader.Read())
                {
                    Skill skill = new Skill();
                    skill.Id = reader.GetUInt32(0);
                    if (Singleton.SpellManager.TryGetSpell(skill.Id, out skill.info) && 
                        skill.info.requiredJobs[job - 1] == 1)
                        MyList.Add(skill.Id);
                }

                return MyList;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return new List<uint>();
            }
            finally
            {
                //ALWAYS CLOSE THE READ RESULT
                if (reader != null) reader.Close();
            }
        }

    }
}
