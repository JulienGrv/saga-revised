using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Saga.IO
{
    public class GuideReader : IDisposable
    {
        private Stream _basestream;
        private XmlTextReader _reader;
        private string _svalue;
        private string _sattrvalue;

        private NodeType _type = NodeType.None;
        private float _x = 0;
        private float _y = 0;
        private float _z = 0;
        private uint _item = 0;
        private uint _menu = 0;

        #region Constructor / Deconctructor

        [DebuggerNonUserCode()]
        private GuideReader(Stream stream)
        {
            _basestream = stream;
            _reader = new XmlTextReader(stream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        [DebuggerNonUserCode()]
        ~GuideReader()
        {
            Dispose();
        }

        #endregion Constructor / Deconctructor

        #region Private Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.UInt32.TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.UInt32@)"), DebuggerNonUserCode()]
        private void ProcessItem()
        {
            _type = NodeType.None;
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (_reader.Name.ToUpperInvariant())
                    {
                        case "POINT":
                            _svalue = string.Empty;
                            _sattrvalue = string.Empty;
                            float.TryParse(_reader["x"], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out _x);
                            float.TryParse(_reader["y"], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out _y);
                            float.TryParse(_reader["z"], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out _z);
                            _sattrvalue = _reader["type"];
                            break;

                        case "GUIDE":
                            uint.TryParse(_reader["menu"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _menu);
                            break;
                    }
                    break;

                case XmlNodeType.Text:
                    _svalue = _reader.Value;
                    break;

                case XmlNodeType.EndElement:
                    if (_reader.Name.ToUpperInvariant() == "POINT")
                    {
                        if (uint.TryParse(_svalue, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _item))
                            switch (_sattrvalue.ToUpperInvariant())
                            {
                                case "NPC": _type = NodeType.Npc; break;
                                case "MAP": _type = NodeType.Position; break;
                                default: _type = NodeType.None; break;
                            }
                    }
                    break;
            }
        }

        #endregion Private Methods

        #region Public Properties

        public uint Menu
        {
            get
            {
                return _menu;
            }
        }

        public uint Item
        {
            get
            {
                return _item;
            }
        }

        public NodeType Type
        {
            get
            {
                return _type;
            }
        }

        public float X
        {
            get
            {
                return _x;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }
        }

        public float Z
        {
            get
            {
                return _z;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [DebuggerNonUserCode()]
        public bool Read()
        {
            bool result = _reader.Read();
            ProcessItem();
            return result;
        }

        #endregion Public Methods

        #region Public Static Methods

        [DebuggerNonUserCode()]
        public static GuideReader Open(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            GuideReader dialog = new GuideReader(fs);
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

        public enum NodeType { None, Npc, Position };

        #endregion Nested
    }
}