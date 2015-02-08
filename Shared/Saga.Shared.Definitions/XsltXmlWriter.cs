using System;
using System.Text;
using System.Xml;

public sealed class XsltXmlWriter : XmlTextWriter
{
    private string stylesheet;

    //Constructors - add more as needed
    public XsltXmlWriter(string filename,
      Encoding enc, string stylesheet)
        : base(filename, enc)
    {
        this.stylesheet = stylesheet;
    }

    public override void WriteStartElement(string prefix,
      string localName, string ns)
    {
        if (WriteState == WriteState.Prolog ||
            WriteState == WriteState.Start)
        {
            //It's time to add the PI
            base.WriteProcessingInstruction("xml-stylesheet",
              String.Format("type=\"text/xsl\" href=\"{0}\"", stylesheet));
        }
        base.WriteStartElement(prefix, localName, ns);
    }
}