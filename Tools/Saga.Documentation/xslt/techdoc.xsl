<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 
  <xsl:import href="page.xsl"/>

  <xsl:template match="paragraph">
    <p><xsl:value-of select="." /></p>
  </xsl:template>

  <xsl:template match="code">
    <code><pre class="code"><xsl:value-of select="."/></pre></code>
  </xsl:template>
</xsl:stylesheet>