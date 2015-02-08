using Saga.Map;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Common
{
    public static class Special
    {
        public static IEnumerable<uint> GetShopingList(uint npcid)
        {
            string file = String.Format("../Data/shops/{0}.xml", npcid);
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                using (XmlTextReader reader = new XmlTextReader(fs))
                {
                    reader.ReadStartElement();
                    while (reader.ReadToFollowing("item"))
                    {
                        uint ItemId = 0;
                        if (uint.TryParse(reader.ReadElementString("item"), out ItemId))
                            yield return ItemId;
                    }
                }
            }
        }

        public static IEnumerable<ushort> GetWarperTable(uint npcid)
        {
            //HELPER VARABLES
            string file = Saga.Structures.Server.SecurePath("~/warpers/{0}.xml", npcid);
            string WarperChildNode = "warp";

            //GO READ THE CONTENTS OF THE FILE
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                using (XmlTextReader reader = new XmlTextReader(fs))
                {
                    reader.ReadStartElement();
                    while (reader.ReadToFollowing(WarperChildNode))
                    {
                        ushort current;
                        if (ushort.TryParse(reader.ReadElementString(), out current))
                            yield return current;
                    }
                }
            }
        }

        public static IEnumerable<KeyValuePair<uint, Rag2Item>> GetLootList(uint npc)
        {
            //HELPER VARABLES
            string file = String.Format("../Data/npc-loots/{0}.xml", npc);
            string WarperChildNode = "item";
            string RateNode = "rate";

            //GO READ THE CONTENTS OF THE FILE
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                using (XmlTextReader reader = new XmlTextReader(fs))
                {
                    reader.ReadStartElement();
                    while (reader.ReadToFollowing(WarperChildNode))
                    {
                        Rag2Item item = null;
                        uint rate;
                        uint itemid;

                        uint.TryParse(reader[RateNode], out rate);
                        if (uint.TryParse(reader.ReadElementString(), out itemid) &&
                            Singleton.Item.TryGetItem(itemid, out item))
                        {
                            KeyValuePair<uint, Rag2Item> pair = new KeyValuePair<uint, Rag2Item>(rate, item);
                            yield return pair;
                        }
                    }
                }
            }
        }
    }
}