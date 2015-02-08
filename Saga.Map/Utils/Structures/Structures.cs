using Saga.Configuration;
using Saga.Map;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Saga.Structures
{
    public struct CharDetails
    {
        public uint Jexp;
        public byte[] FaceDetails;
        public byte Map;
    }

    public struct CharCreationArgument
    {
        public uint UserId;
        public string CharName;
        public byte[] FaceDetails;
        public string WeaponName;
        public ushort WeaponAffix;
    }

    public struct CharInfo
    {
        public string name;
        public byte gender;
        public uint cexp;
        public uint charId;
        public byte map;
        public byte job;
    }

    public class MarketItemArgument
    {
        public uint id = 0;
        public uint price = 0;
        public byte grade = 0;
        public byte cat = 0;
        public string sender = "";
        public string itemname = "";
        public Rag2Item item = null;
        public DateTime expires = DateTime.Now;
    }

    [Serializable()]
    public struct WorldCoordinate
    {
        public Point coords;
        public byte map;

        public WorldCoordinate(Point coords, byte map)
        {
            this.coords = coords;
            this.map = map;
        }

        public float x
        {
            get
            {
                return coords.x;
            }
        }

        public float y
        {
            get
            {
                return coords.y;
            }
        }

        public float z
        {
            get
            {
                return coords.z;
            }
        }
    }

    public class SkillRotatorCollection
    {
        private static Random random = new Random();
        private List<Factory.Spells.Info> skillinfo;
        private List<uint> probabillity;
        private uint min_range;
        private uint max_range = 150;
        private uint pmin_range;
        private uint pmax_range;

        public uint MinRange
        {
            get
            {
                return min_range;
            }
        }

        public uint MaxRange
        {
            get
            {
                return max_range;
            }
        }

        public uint MinRangeE2
        {
            get
            {
                return pmin_range;
            }
        }

        public uint MaxRangeE2
        {
            get
            {
                return pmax_range;
            }
        }

        private SkillRotatorCollection()
        {
            skillinfo = new List<Saga.Factory.Spells.Info>();
            probabillity = new List<uint>();
        }

        public static SkillRotatorCollection Empty
        {
            get
            {
                return new SkillRotatorCollection();
            }
        }

        public static SkillRotatorCollection CreateFromFile(string file)
        {
            SkillRotatorCollection collection = new SkillRotatorCollection();
            using (XmlReader reader = new XmlTextReader(file))
            {
                string value = string.Empty;
                uint skill = 0;
                uint prob = 0;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "SKILL": uint.TryParse(reader["probability"], out prob); break;
                            }
                            break;

                        case XmlNodeType.Text:
                            value = reader.Value;
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "SKILL": uint.TryParse(value, out skill);
                                    collection.AddSkill(skill, prob);
                                    break;
                            }
                            break;
                    }
                }
            }

            return collection;
        }

        private void AddSkill(uint skill, uint prob)
        {
            Factory.Spells.Info info;
            if (Singleton.SpellManager.TryGetSpell(skill, out info))
            {
                skillinfo.Add(info);
                probabillity.Add(prob);

                if (info.minimumrange < min_range)
                {
                    min_range = info.minimumrange;
                    pmin_range = min_range * min_range;
                }
                if (info.maximumrange > max_range)
                {
                    max_range = info.maximumrange;
                    pmax_range = max_range * max_range;
                }
            }
        }

        public bool FindSkillForRange(double distance, out uint skill)
        {
            for (int i = 0; i < skillinfo.Count; i++)
            {
                Factory.Spells.Info info = skillinfo[i];
                if (distance > info.minimumrange && distance < info.maximumrange)
                {
                    if (random.Next(0, 10000) < probabillity[i])
                    {
                        skill = info.skillid;
                        return true;
                    }
                }
            }

            skill = 0;
            return false;
        }
    }

    public static class Server
    {
        static Server()
        {
            try
            {
                ServerVars section = ConfigurationManager.GetSection("Saga.ServerVars") as ServerVars;
                if (section != null)
                {
                    datadir = Path.Combine(Environment.CurrentDirectory, section.DataDirectory);
                }
            }
            catch (Exception x)
            {
                Trace.WriteLine(x.ToString());
            }
        }

        public static string AssemblyName
        {
            get
            {
                string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string fname = Path.GetFileNameWithoutExtension(file);
                return fname;
            }
        }

        private static string datadir;

        public static string SecurePath(string path)
        {
            string newpath = path;
            if (path.Length > 1 && path[0] == '~' && path[1] == '/')
            {
                newpath = Path.Combine(datadir, path.Substring(2, path.Length - 2));
            }

            return newpath;
        }

        public static string SecurePath(string format, params object[] objects)
        {
            string path = string.Format(format, objects);
            string newpath = path;
            if (path.Length > 1 && path[0] == '~' && path[1] == '/')
            {
                newpath = Path.Combine(datadir, path.Substring(2, path.Length - 2));
            }

            return newpath;
        }
    }
}