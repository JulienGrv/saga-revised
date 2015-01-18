<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 
  <xsl:import href="page.xsl"/>

  <xsl:template match="title">
    <h1>
      <xsl:value-of select="."/>
    </h1>
  </xsl:template>
 
  <xsl:template match="documentation">
    <p>
      This is the technical documentation of our saga system. These documents are meant for both 
	scripters as developers. As refereneces for how the system works.
    </p>

    <h2>Table of Contents</h2>
    <div class="paragraph_menu">
      <p>
        <ol>
          <xsl:for-each select="document">
          <xsl:sort select="title"/>
            <li>
		  <a>
            	<xsl:attribute name="href">
				<xsl:value-of select="@path"/>.html
			</xsl:attribute> 
			<xsl:value-of select="@title" />
		  </a>
            </li>
          </xsl:for-each>
        </ol>
      </p>
    </div>
  </xsl:template>
</xsl:stylesheet>