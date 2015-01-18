<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  
  <xsl:import href="page.xsl"/>
 
  <xsl:template match="log">
	<table>
          <xsl:apply-templates/>
	</table>
  </xsl:template>

  <xsl:template match="logentry">
	<tr><td valign="top" width="150">
	<h3 style="margin:0px; padding:0px;">Revision <xsl:value-of select="@revision" /></h3>
	</td><td valign="middle">
	<xsl:value-of select="./msg" /><br/>
	</td></tr>
  </xsl:template>
  
</xsl:stylesheet>