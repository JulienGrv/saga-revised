<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="page">
<html>
  <head>
    <title>
       <xsl:value-of select="title"/>
    </title>
    <link rel="stylesheet" href="stylesheet/stylesheet.css" />
  </head>
  <body>
 	 <table class="container" cellspacing="0">
	 	<!-- Main content //-->
		<tr>
			<td style="padding:0px; margin:0px;">
			
	 <div class="overlay_top">
	 <div class="overlay_bottom">
       <table class="container"  cellspacing="0">
	 	<tr>
			<td class="container_corner_upper_left">
			</td>
			<td class="container_top">
			</td>
			<td class="container_corner_upper_right">
			</td>
		</tr>
		<tr>
			<td class="container_left">
			</td>
			<td valign="top">



				<div style="padding-left:130px; position:relative; top:-20px;">
					<a href="#" class="menulink">News</a> |
					<a href="#" class="menulink">Contact</a> |
					<a href="#" class="menulink">Registration</a> |
					<a href="#" class="menulink">About</a> |
					<a href="#" class="menulink">Screenshots</a> |
					<a href="#" class="menulink">Staff</a> |
					<a href="#" class="menulink">Sitemap</a> 
				</div>
				<div>
					<img class="pagetitle" src="Images/menu_large/screenshots.png" />
				</div>
				<div>
          <xsl:apply-templates/>
				</div>
				<div style="height:70px; width:100%;">
					<a href="#" class="footerlink">Top</a>		
				</div>
			</td>
			<td class="container_right">
			</td>
		</tr>
		<tr>
			<td class="container_corner_lower_left">
			</td>
			<td class="container_bottom">
			</td>
			<td class="container_corner_lower_right">
			</td>
		</tr>
	</table>


	</div>
	</div>

			</td>
		</tr>

		<!-- Footer -->
	   	<tr>
			<td class="page_footer">
				<img src="images/logo-rev-low.png" />
			</td>
		</tr>
	</table>
  </body>
</html>
    </xsl:template>


  <xsl:template match="title">
    <h1>
      <xsl:value-of select="."/>
    </h1>
  </xsl:template>
 
  <xsl:template match="luabindings">
    <p>

      This is the documentation about quest and npc scriping. As of this moment we'll keep this document
      up-to-date. For questfiles in the desired and validated layout we'll provide conversion tools to
      update this.
    </p>

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
      <h2>
        <a name="{name}"><xsl:value-of select="name" /></a>
      </h2>
      <div class="paragraph">

        <!-- Parameter String -->
        <code>
          <xsl:value-of select="parameterstring" />
        </code>

        <!-- Remarks -->
        <b>Remarks:</b>
        <p>
          <xsl:value-of select="remarks" disable-output-escaping="yes" />
        </p>

        <!-- Example -->
        <b>Example:</b>
        <p>The following code sample illustrates the use of <xsl:value-of select="name" />:</p>
        <code><pre><xsl:value-of select="example"/></pre></code>
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