<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  
  <xsl:import href="page.xsl"/>
 
  <xsl:template match="tasks">
    <h2 id="Tasks">Tasks:</h2>
    <div class="paragraph">
      <b>Todo:</b>
      <ul>
        <xsl:for-each select="/page/tasks/task">
          <li>
            <xsl:value-of select="." />
          </li>          
        </xsl:for-each>
      </ul>
    </div>        
  </xsl:template>
  
</xsl:stylesheet>