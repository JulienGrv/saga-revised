using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;

namespace Saga.Tools.QuestExtractor
{

    class Program
    {
        static string questdirectory = null;
        static string filename = null;

        static void Main(string[] args)
        {
            try
            {
                questdirectory = args[0];
                filename = args[1];
                ProcessAllQuests();
            }
            catch (Exception e)
            {
                Console.WriteLine("QuestExtractor cannot process due a fatal exception");
                Console.WriteLine("Please verify if you pass the arguments correct");
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Extraction Completed");
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
                                case "row": GenerateQuestStub(ab); ab = null; break;
                            }
                            break;
                    }
                }
            }
        }

        static void GenerateQuestStub(QuestInformation questinfo)
        {
            if (questinfo == null || questinfo.QuestId == 0) return;
            string tmpFile = Path.GetTempFileName();
            string tmpFile2 = Path.Combine(questdirectory, String.Format("{0}.lua", questinfo.QuestId));
           
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License");
                writer.WriteLine("-- http://creativecommons.org/licenses/by-nc-sa/3.0/");
                writer.WriteLine("-- Generated By Quest Extractor on {0}", DateTime.Now);
                writer.WriteLine();
                writer.WriteLine(String.Format("local QuestID = {0};", questinfo.QuestId));
                writer.WriteLine(String.Format("local ReqClv = {0};", questinfo.ReqClvl));
                writer.WriteLine(String.Format("local ReqJlv = 0;"));
                writer.WriteLine(String.Format("local NextQuest = {0};", questinfo.FollowUpQuest));
                writer.WriteLine(String.Format("local RewZeny = {0};", questinfo.Zeny));
                writer.WriteLine(String.Format("local RewCxp = {0};", questinfo.RewardCexp));
                writer.WriteLine(String.Format("local RewJxp = {0};", questinfo.RewardJexp));
                writer.WriteLine(String.Format("local RewWxp = {0}; ", questinfo.RewardWexp));
                writer.WriteLine(String.Format("local RewItem1 = {0}; ", questinfo.Item1));
                writer.WriteLine(String.Format("local RewItem2 = {0}; ", questinfo.Item2));
                writer.WriteLine(String.Format("local RewItemCount1 = {0}; ", questinfo.ItemCount1));
                writer.WriteLine(String.Format("local RewItemCount2 = {0}; ", questinfo.ItemCount2));
                writer.WriteLine();
                writer.WriteLine("-- Modify steps below for gameplay");

                if (File.Exists(tmpFile2))
                {
                    using (FileStream fs2 = new FileStream(tmpFile2, FileMode.Open, FileAccess.Read))
                    using (StreamReader reader = new StreamReader(fs2))
                    {
                        bool Process = false;
                        while (reader.Peek() > -1)
                        {
                            string line = reader.ReadLine();
                            if (line == "-- Modify steps below for gameplay")
                            {
                                Process = true;
                                continue;
                            }
                            if (Process == true)
                            {
                                writer.WriteLine(line);
                            }
                        }
                    }
                }
                else
                {
                    writer.WriteLine();
                    writer.WriteLine("function QUEST_START()");
                    writer.WriteLine("\t-- Initialize all quest steps");
                    writer.WriteLine("\t-- Initialize all starting navigation points");
                    writer.WriteLine("\treturn 0;");
                    writer.WriteLine("end");
                    writer.WriteLine();
                    
                    writer.WriteLine("function QUEST_FINISH()");
                	writer.WriteLine("\tSaga.GiveZeny(RewZeny);");
	                writer.WriteLine("\tSaga.GiveExp( RewCxp, RewJxp, RewWxp);");
                	writer.WriteLine("\treturn 0;");
                    writer.WriteLine("end");
                    writer.WriteLine();

                    writer.WriteLine("function QUEST_CANCEL()");                  
	                writer.WriteLine("\treturn 0;");
                    writer.WriteLine("end");
                    writer.WriteLine();

                    writer.WriteLine("function QUEST_CHECK()");
                    writer.WriteLine("\t-- Check all steps for progress");
                    writer.WriteLine("\tlocal CurStepID = Saga.GetStepIndex();");
                    writer.WriteLine("\t-- TODO: Add code to check all progress");
                    writer.WriteLine("\treturn -1;");
                    writer.WriteLine("end");
                }
            }
            File.Copy(tmpFile, tmpFile2, true);
            File.Delete(tmpFile);
        }
    }

    class QuestInformation
    {
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
