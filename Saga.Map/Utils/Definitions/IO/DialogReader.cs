﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Diagnostics;

namespace Saga.IO
{
    class DialogReader : IDisposable
    {

        private Stream _basestream;
        private XmlReader _reader;
        private bool _hasinfo;
        private string _svalue;
        private uint _uvalue;
        private string _name = string.Empty;

        #region Constructor / Deconctructor

        [DebuggerNonUserCode()]
        DialogReader(Stream stream)
        {
            _basestream = stream;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            _reader = XmlTextReader.Create(stream, settings);
        }

        [DebuggerNonUserCode()]
        ~DialogReader()
        {
            Dispose();
        }

        #endregion

        #region Private Methods

        [DebuggerNonUserCode()]
        private void ProcessItem()
        {   
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element: 
                    _svalue = string.Empty;
                    _uvalue = 0;
                    _hasinfo = false;
                    break;
                case XmlNodeType.Text:
                    _svalue = _reader.Value; 
                    break;
                case XmlNodeType.EndElement:
                    _name = _reader.Name;
                    _hasinfo = uint.TryParse(_svalue, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out _uvalue);
                    break;                        
            }
        }

        

        #endregion

        #region Public Properties

        public bool HasInformation
        {
            get
            {
                return _hasinfo;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public uint Value
        {
            get
            {
                return _uvalue;
            }
        }

        #endregion

        #region Public Methods

        [DebuggerNonUserCode()]
        public bool Read()
        {
            bool result = _reader.Read();
            ProcessItem();
            return result;
        }

        #endregion

        #region Public Static Methods

        [DebuggerNonUserCode()]
        public static DialogReader Open(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            DialogReader dialog = new DialogReader(fs);
            return dialog;
        }

        #endregion

        #region IDisposable Members

        [DebuggerNonUserCode()]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_reader != null) _reader.Close();
            if (_basestream != null) _basestream.Dispose();            
        }

        #endregion
    }
}
