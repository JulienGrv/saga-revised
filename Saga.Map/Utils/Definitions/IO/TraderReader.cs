using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Saga.IO
{
    internal class TraderReader : IDisposable
    {
        #region Private Members

        private Stream _basestream;
        private XmlTextReader _reader;
        private string _svalue;

        /// <summary>
        /// List of node options
        /// </summary>
        private Dictionary<uint, NodeOption> nodeoptions =
            new Dictionary<uint, NodeOption>();

        /// <summary>
        /// Get the production owner
        /// </summary>
        private byte part = 0;

        /// <summary>
        /// Contains the current nodetype
        /// </summary>
        public NodeType _nodetype = NodeType.None;

        /// <summary>
        /// Contains the menu of the current trade
        /// </summary>
        public uint _menuid = 0;

        /// <summary>
        /// Contains the trade id of the current item
        /// </summary>
        public uint _trade = 0;

        /// <summary>
        /// Contains the group id of the current item
        /// </summary>
        public uint _group = 0;

        /// <summary>
        /// Contains the item count of the current item
        /// </summary>
        public byte _count = 0;

        /// <summary>
        /// Contains the item count of the current item
        /// </summary>
        public uint _item = 0;

        #endregion Private Members

        #region Constructor / Deconctructor

        [DebuggerNonUserCode()]
        private TraderReader(Stream stream)
        {
            _basestream = stream;
            _reader = new XmlTextReader(stream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        [DebuggerNonUserCode()]
        ~TraderReader()
        {
            Dispose();
        }

        #endregion Constructor / Deconctructor

        #region Private Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.UInt32.TryParse(System.String,System.UInt32@)"), DebuggerNonUserCode()]
        private void ProcessHeader()
        {
            string type = string.Empty;
            string fee = string.Empty;
            string refund = string.Empty;
            string val = string.Empty;
            NodeOption option;

            do
            {
                _nodetype = NodeType.None;
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (_reader.Name.ToUpperInvariant())
                        {
                            case "TRADES":
                                uint.TryParse(_reader["id"], out  _menuid);
                                break;

                            case "TRADE":
                                type = _reader["type"];
                                fee = _reader["fee"];
                                refund = _reader["refund"];
                                if (type != null) type = type.ToUpperInvariant();
                                break;
                        }
                        break;

                    case XmlNodeType.Text:
                        val = _reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        switch (_reader.Name.ToUpperInvariant())
                        {
                            case "TRADE":
                                uint menu;
                                option = new NodeOption();
                                uint.TryParse(val, out menu);
                                uint.TryParse(fee, out option.fee);
                                uint.TryParse(refund, out option.refund);
                                if (type == "SINGLE")
                                    option.node = NodeTypeSpeciafier.Single;
                                else if (type == "GROUPED")
                                    option.node = NodeTypeSpeciafier.Grouped;
                                nodeoptions[menu] = option;
                                break;

                            case "TRADES":
                                part = 1;
                                break;
                        }

                        break;
                }
            }
            while (part == 0 && _reader.Read());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.UInt32.TryParse(System.String,System.UInt32@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Byte.TryParse(System.String,System.Byte@)"), DebuggerNonUserCode()]
        private void ProcessBody()
        {
            if (part == 0) ProcessHeader();
            _nodetype = NodeType.None;
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (_reader.Name.ToUpperInvariant())
                    {
                        case "ITEM":
                            uint.TryParse(_reader["trade"], out _trade);
                            uint.TryParse(_reader["group"], out _group);
                            byte.TryParse(_reader["count"], out _count);
                            break;
                    }
                    break;

                case XmlNodeType.Text:
                    _svalue = _reader.Value;
                    break;

                case XmlNodeType.EndElement:
                    switch (_reader.Name.ToUpperInvariant())
                    {
                        case "ITEM":
                            if (uint.TryParse(_svalue, out _item))
                                if (part == 1)
                                {
                                    _nodetype = NodeType.Production;
                                }
                                else if (part == 2)
                                {
                                    _nodetype = NodeType.Supplement;
                                }
                            break;

                        case "PRODUCTION":
                            part = 2;
                            break;
                    }

                    break;
            }
        }

        #endregion Private Methods

        #region Public Properties

        public NodeOption this[uint index]
        {
            get
            {
                NodeOption option;
                nodeoptions.TryGetValue(index, out option);
                return option;
            }
        }

        public NodeType Type
        {
            get
            {
                return _nodetype;
            }
        }

        public uint Menu
        {
            get
            {
                return _menuid;
            }
        }

        public uint Trade
        {
            get
            {
                return _trade;
            }
        }

        public int Group
        {
            get
            {
                return (int)_group;
            }
        }

        public byte ItemCount
        {
            get
            {
                return (byte)_count;
            }
        }

        public uint ItemId
        {
            get
            {
                return _item;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [DebuggerNonUserCode()]
        public bool Read()
        {
            bool result = _reader.Read();
            ProcessBody();
            return result;
        }

        #endregion Public Methods

        #region Public Static Methods

        [DebuggerNonUserCode()]
        public static TraderReader Open(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            TraderReader dialog = new TraderReader(fs);
            return dialog;
        }

        #endregion Public Static Methods

        #region IDisposable Members

        [DebuggerNonUserCode()]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_reader != null) _reader.Close();
            if (_basestream != null) _basestream.Dispose();
        }

        #endregion IDisposable Members

        #region Nested

        public enum NodeType { None, Production, Supplement };

        public enum NodeTypeSpeciafier { Single, Grouped };

        public struct NodeOption
        {
            public NodeTypeSpeciafier node;
            public uint fee;
            public uint refund;
        }

        #endregion Nested
    }
}