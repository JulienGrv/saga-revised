<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 
  <xsl:import href="page.xsl"/>

  <xsl:template match="title">
    <h1>
      <xsl:value-of select="."/>
    </h1>
  </xsl:template>

  <xsl:template match="paragraph">
    <p><xsl:value-of select="." /></p>
  </xsl:template>
 
  <xsl:template match="luabindings">
    <h2>Functions by alphabet</h2>
    <div class="paragraph_menu">
      <p>
        <ol>
          <xsl:for-each select="/page/luabindings/function">
          <xsl:sort select="name"/>
            <li>
              <a href="#{name}"><xsl:value-of select="name" /></a>
            </li>
          </xsl:for-each>
        </ol>
      </p>
    </div>

    <xsl:for-each select="/page/luabindings/function">
      <xsl:sort select="name"/>
      <h2>
        <a name="{name}"><xsl:value-of select="name" /></a>
      </h2>
      <div class="paragraph">

        <!-- Parameter String -->
        <code>
          <xsl:value-of select="parameterstring" />
        </code>

        <!-- Remarks -->
        <p><b>Remarks:</b></p>
        <p>
          <xsl:value-of select="remarks" disable-output-escaping="yes" />
        </p>

        <!-- Example -->
        <b>Example:</b>
        <p>The following code sample illustrates the use of <xsl:value-of select="name" />:</p>
        <code><pre class="code"><xsl:value-of select="example"/></pre></code>
      </div>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match="resources">
    <p>

      This is the documentation for resources. Which dll's are required to get the server working/running.
      Where you can download these dll's we advice to be used.
    </p>

    <h2>Resources by alphabet</h2>
    <div class="paragraph_menu">
      <p>
        <ol>
          <xsl:for-each select="/page/resources/resource">
            <xsl:sort select="name"/>
            <li>
              <a href="#{name}">
                <xsl:value-of select="name" />
              </a>
            </li>
          </xsl:for-each>
        </ol>
      </p>
    </div>

    <xsl:for-each select="/page/resources/resource">
      <h2>
        <a name="{name}">
          <xsl:value-of select="name" />
        </a>
      </h2>
      <div class="paragraph">

        <!-- Description -->
        <b>Description:</b>
        <p>
          <xsl:value-of select="description" />
          <br />
          <a href="{downloaduri}">Download</a>
        </p>




        <!-- Dll check -->
        <b>Dll check:</b>
        <p>
          <ul>
            <xsl:for-each select="dllcheck/dll">
              <li>
                <xsl:value-of select="." />
              </li>
            </xsl:for-each>
          </ul>
          <xsl:value-of select="remarks" disable-output-escaping="yes" />
        </p>
      </div>
    </xsl:for-each>
  </xsl:template>  

</xsl:stylesheet>