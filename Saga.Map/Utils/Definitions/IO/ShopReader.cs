using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Saga.IO
{
    public class ShopReader : IDisposable
    {
        private Stream _basestream;
        private XmlTextReader _reader;
        private bool _hasinfo;
        private string _svalue;
        private string _sattrvalue;
        private uint _itemid;
        private byte _count;

        #region Constructor / Deconctructor

        [DebuggerNonUserCode()]
        private ShopReader(Stream stream)
        {
            _basestream = stream;
            _reader = new XmlTextReader(stream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        [DebuggerNonUserCode()]
        ~ShopReader()
        {
            Dispose();
        }

        #endregion Constructor / Deconctructor

        #region Private Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Byte.TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.Byte@)"), DebuggerNonUserCode()]
        private void ProcessItem()
        {
            _hasinfo = false;
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (_reader.Name.ToUpperInvariant() == "ITEM")
                    {
                        _svalue = string.Empty;
                        _sattrvalue = string.Empty;
                        _itemid = 0;
                        _count = 0;
                        _sattrvalue = _reader["count"];
                    }
                    break;

                case XmlNodeType.Text:
                    _svalue = _reader.Value;
                    break;

                case XmlNodeType.EndElement:
                    if (_reader.Name.ToUpperInvariant() == "ITEM")
                    {
                        _hasinfo = uint.TryParse(_svalue, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _itemid);
                        byte.TryParse(_sattrvalue, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _count);
                    }
                    break;

                case XmlNodeType.Whitespace:
                    break;

                default:
                    _hasinfo = false;
                    break;
            }
        }

        #endregion Private Methods

        #region Public Properties

        public bool HasInformation
        {
            get
            {
                return _hasinfo;
            }
        }

        public byte Count
        {
            get
            {
                return _count;
            }
        }

        public uint Value
        {
            get
            {
                return _itemid;
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
        public static ShopReader Open(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            ShopReader dialog = new ShopReader(fs);
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
    }
}