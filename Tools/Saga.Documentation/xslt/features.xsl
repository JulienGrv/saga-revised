<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 
  <xsl:import href="page.xsl"/>

  
  <xsl:template match="features">
    <xsl:for-each select="/page/features/feature">
	<img align="left" >
		<xsl:attribute name="src">
		images/features/
		<xsl:value-of select="@image"/>
		.png
		</xsl:attribute> 
	</img>
	<p><h3><xsl:value-of select="@title"/></h3></p>
	<p><xsl:value-of select="."/></p>		
	<br class="paragraph" />
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>