using Saga.Configuration;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Saga.Factory
{
    public class ItemsFactory : FactoryBase
    {
        #region Ctor/Dtor

        public ItemsFactory()
        {
        }

        ~ItemsFactory()
        {
            this.item_drops = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        public Dictionary<uint, ItemInfo> item_drops;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            item_drops = new Dictionary<uint, ItemInfo>();
        }

        protected override void Load()
        {
            ItemSettings section = (ItemSettings)ConfigurationManager.GetSection("Saga.Factory.Items");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("ItemFactory", "Loading item information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Items");
            }
        }

        protected override void ParseAsXmlStream(Stream stream, ReportProgress ProgressReport)
        {
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                ItemInfo item = new ItemInfo();
                string value = null;

                while (reader.Read())
                {
                    ProgressReport.Invoke();
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "ROW": item = new ItemInfo(); continue;
                                default: value = null; break;
                            }
                            break;

                        case XmlNodeType.Text:
                            value = reader.Value;
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "ID": item.item = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "ADDITION": item.addition = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "PART": item.part = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "TYPE": item.type = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "PRICE": item.price = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "TRADE": item.trade = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "DROP": item.drop = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "NAME": item.name = value; break;
                                case "QUEST": item.quest = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_CLV": item.req_clvl = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_MALE": item.req_male = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_FEMALE": item.req_female = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_NORMAN": item.req_norman = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_ELLR": item.req_ellr = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_DIMAGO": item.req_dimago = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_STR": item.req_str = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_DEX": item.req_dex = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_INT": item.req_int = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_CON": item.req_con = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "REQ_LUC": item.req_luc = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "MAX_STACK": item.max_stack = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "MAX_DUR": item.max_durability = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "SKILL": item.skill = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "OPTION_ID": item.option_id = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "CATEGORIE": item.categorie = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "JOBREQUIREMENT":
                                    string[] values = value.Split(',');
                                    item.JobRequirement = new byte[]
                                    {
                                        byte.Parse(values[0],  NumberFormatInfo.InvariantInfo),  //NOVICE
                                        byte.Parse(values[1],  NumberFormatInfo.InvariantInfo),  //SWORDSMAN
                                        byte.Parse(values[3],  NumberFormatInfo.InvariantInfo),  //RECRUIT
                                        byte.Parse(values[2],  NumberFormatInfo.InvariantInfo),  //THIEF
                                        byte.Parse(values[4],  NumberFormatInfo.InvariantInfo),  //ENCHANTER
                                        byte.Parse(values[5],  NumberFormatInfo.InvariantInfo),  //CLOWN
                                        byte.Parse(values[6],  NumberFormatInfo.InvariantInfo),  //KNIGHT
                                        byte.Parse(values[7],  NumberFormatInfo.InvariantInfo),  //ASSASIN
                                        byte.Parse(values[8],  NumberFormatInfo.InvariantInfo),  //SPECIALIST
                                        byte.Parse(values[9],  NumberFormatInfo.InvariantInfo),  //SAGE
                                        byte.Parse(values[10],  NumberFormatInfo.InvariantInfo), //GAMBLER
                                        byte.Parse(values[11],  NumberFormatInfo.InvariantInfo), //FALCATA
                                        byte.Parse(values[12],  NumberFormatInfo.InvariantInfo), //FPRSYTHIE
                                        byte.Parse(values[13],  NumberFormatInfo.InvariantInfo), //NEMOPHILA
                                        byte.Parse(values[14],  NumberFormatInfo.InvariantInfo)  //VEILCHENBLAU
                                    };
                                    break;

                                case "ROW": goto Add;
                            }
                            break;
                    }

                    continue;
                Add:
                    this.item_drops.Add(item.item, item);
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public bool TryGetStackcount(uint id, out byte stackcount)
        {
            ItemInfo info;
            item_drops.TryGetValue(id, out info);
            stackcount = 0;
            bool isfound = info != null;
            if (isfound)
                stackcount = info.max_stack;
            return isfound;
        }

        public bool TryGetItem(uint id, out ItemInfo item)
        {
            return item_drops.TryGetValue(id, out item);
        }

        public bool TryGetItem(uint id, out Rag2Item item)
        {
            Rag2Item temp = new Rag2Item();
            bool result = item_drops.TryGetValue(id, out temp.info);
            if (result == true)
            {
                item = temp;
                item.clvl = (byte)item.info.req_clvl;
                item.durabillty = (int)item.info.max_durability;
            }
            else
            {
                item = null;
            }
            return result;
        }

        public bool TryGetItemWithCount(uint id, byte count, out Rag2Item item)
        {
            Rag2Item temp = new Rag2Item();
            bool result = item_drops.TryGetValue(id, out temp.info);
            if (result == true)
            {
                item = temp;
                item.clvl = (byte)item.info.req_clvl;
                item.durabillty = (int)item.info.max_durability;
                item.count = count;
            }
            else
            {
                item = null;
            }
            return result;
        }

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// Get the notification string.
        /// </summary>
        /// <remarks>
        /// We used notification strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_ITEM; }
        }

        /// <summary>
        /// Get the readystate string.
        /// </summary>
        /// <remarks>
        /// We used readystate strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_ITEM; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        [Serializable()]
        public class ItemInfo
        {
            /// <summary>
            /// Id of the item
            /// </summary>
            public uint item;

            /// <summary>
            /// Unknown
            /// </summary>
            public uint rate;

            /// <summary>
            /// Addition that is called when using the item
            /// </summary>
            public uint addition;

            /// <summary>
            /// Part of the item used for equipment
            /// </summary>
            public uint part;

            /// <summary>
            /// Type of item
            /// </summary>
            public uint type;

            /// <summary>
            /// Item market value
            /// </summary>
            public uint price;

            /// <summary>
            /// If it's item can be traded or not
            /// </summary>
            public uint trade;

            /// <summary>
            /// If it's item can be dropped or not
            /// </summary>
            public uint drop;

            /// <summary>
            /// Name of the item
            /// </summary>
            public string name;

            /// <summary>
            /// If it's a quest item or not
            /// </summary>
            public uint quest;

            /// <summary>
            /// Required character level
            /// </summary>
            public byte req_clvl;

            /// <summary>
            /// Required gender male
            /// </summary>
            public byte req_male;

            /// <summary>
            /// Required gender female
            /// </summary>
            public byte req_female;

            /// <summary>
            /// Required race norman
            /// </summary>
            public byte req_norman;

            /// <summary>
            /// Required race ellr
            /// </summary>
            public byte req_ellr;

            /// <summary>
            /// Required race dimango
            /// </summary>
            public byte req_dimago;

            /// <summary>
            /// Required strength needed to use this item
            /// </summary>
            public byte req_str;

            /// <summary>
            /// Required dexterity needed to use this item
            /// </summary>
            public byte req_dex;

            /// <summary>
            /// Required intellect needed to use this item
            /// </summary>
            public byte req_int;

            /// <summary>
            /// Required concentration needed to use this item
            /// </summary>
            public byte req_con;

            /// <summary>
            /// Required luck needed to use this item
            /// </summary>
            public byte req_luc;

            /// <summary>
            /// Required job requiresments to use this item
            /// </summary>
            public byte[] JobRequirement;

            /// <summary>
            /// Required summons
            /// </summary>
            public uint req_summons;

            /// <summary>
            /// Required addition for this item
            /// </summary>
            public uint req_additions;

            /// <summary>
            /// Maximum Stack count of this item
            /// </summary>
            public byte max_stack;

            /// <summary>
            /// Maximum durabillity
            /// </summary>
            public uint max_durability;

            /// <summary>
            /// Addition to call when you have this item in your
            /// inventory/equipment slot
            /// </summary>
            public uint option_id;

            /// <summary>
            /// Skill that is associated with this item
            /// </summary>
            public uint skill;

            /// <summary>
            /// Market registration categorie
            /// </summary>
            public uint categorie;
        }

        #endregion Nested Classes/Structures
    }
}