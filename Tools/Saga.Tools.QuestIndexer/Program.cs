using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Globalization;

namespace Saga.Tools.QuestIndexer
{
    class Program
    {

        static Dictionary<uint, QuestInformation> dict = new Dictionary<uint, QuestInformation>();
        static string filename = null;
        static string filename2 = null;

        static void Main(string[] args)
        {
            try
            {
                filename = args[0];
                ProcessAllQuests();
                ReverseQuestChain();
                GenerateQuestStub();
            }
            catch (Exception e)
            {
                Console.WriteLine("QuestIndexer cannot process due a fatal exception");
                Console.WriteLine("Please verify if you pass the arguments correct");
                Console.WriteLine(e);
            }            
        }

        static void ProcessAllQuests()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (XmlTextReader reader = new XmlTextReader(fs))
            {

                string value = null;
                QuestInformation ab = new QuestInformation();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            value = null;
                            switch (reader.Name.ToLowerInvariant())
                            {
                                case "row": ab = new QuestInformation(); break;
                            }
                            break;
                        case XmlNodeType.Text:
                            value = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name.ToLowerInvariant())
                            {
                                case "id": ab.QuestId = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "nextquest": ab.FollowUpQuest = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "reqclv": ab.ReqClvl = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "zeny": ab.Zeny = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "rewardcexp": ab.RewardCexp = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "rewardjexp": ab.RewardJexp = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "rewardwexp": ab.RewardWexp = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "item1": ab.Item1 = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "itemcount1": ab.ItemCount1 = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "item2": ab.Item2 = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "itemcount2": ab.ItemCount2 = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "row": dict.Add(ab.QuestId, ab); ab = null; break;
                            }
                            break;
                    }
                }
            }
        }

        static void ReverseQuestChain()
        {                  
            foreach (KeyValuePair<uint, QuestInformation> pair in dict)
            {
                ReverseChain(pair.Value);
            }
        }

        static void ReverseChain(QuestInformation ab)
        {
            if (ab.IsProcessed == true) return;
            if (ab.FollowUpQuest == 0) { ab.IsProcessed = true; return; }

            QuestInformation recursive;
            if (dict.TryGetValue(ab.FollowUpQuest, out recursive))
            {
                if (recursive.FollowUpQuest != ab.FollowUpQuest) ReverseChain(recursive);
                uint QuestId1 = ab.FollowUpQuest;
                uint QuestId2 = recursive.FollowUpQuest;

                ab.FollowUpQuest = QuestId2;
                recursive.FollowUpQuest = QuestId1;
                ab.IsProcessed = true;
            }
        }

        static void GenerateQuestStub()
        {
            Console.WriteLine("-- ---------------------------");
            Console.WriteLine("-- Dump data for list_quest");
            Console.WriteLine("-- ---------------------------");
            Console.WriteLine("");
            Console.WriteLine("LOCK TABLES `list_quests` WRITE;");
            foreach (KeyValuePair<uint, QuestInformation> pair in dict)
            Console.WriteLine("INSERT INTO `list_quests` (`QuestId`,`Region`,`Req_Clvl`, `Req_Jlvl`, `Req_Quest`) VALUES ('{0}','{1}','{2}','{3}','{4}') ON duplicate KEY UPDATE `Req_Clvl`='{2}', `Req_Quest`='{4}';", pair.Value.QuestId, 0, pair.Value.ReqClvl, 0, pair.Value.FollowUpQuest);
            Console.WriteLine("UNLOCK TABLES;");
        }
    }

    class QuestInformation
    {
        public bool IsProcessed = false;
        public uint QuestId;
        public uint FollowUpQuest;
        public byte ReqClvl;
    
        public uint Zeny;
        public uint RewardCexp;
        public uint RewardJexp;
        public uint RewardWexp;

        public uint Item1;
        public byte ItemCount1;
        public uint Item2;
        public byte ItemCount2;
    }
}
