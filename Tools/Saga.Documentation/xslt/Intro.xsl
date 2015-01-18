<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="page.xsl"/>
  
  <xsl:template match="Introduction">
    <table style="height:100%;" cellpadding="0" cellspacing="0">
	<tr><td><xsl:text disable-output-escaping='yes'><![CDATA[&nbsp;]]></xsl:text></td></tr>
	<tr><td style="height:440px; padding-left:280px; margin:0px; vertical-align:top; ">
	  <xsl:apply-templates/>
	</td></tr>
    </table>
  </xsl:template>
</xsl:stylesheet>