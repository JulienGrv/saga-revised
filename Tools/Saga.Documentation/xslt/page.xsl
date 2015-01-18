<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="page" >
<html>
  <head>
    <title>
       <xsl:value-of select="title"/>
    </title>
    
	<xsl:choose>
	     <xsl:when test="@style">
			<link rel="stylesheet">
			<xsl:attribute name="href">stylesheet/<xsl:value-of select="@style"/>.css</xsl:attribute>
			</link>
    		</xsl:when>  
    		<xsl:otherwise><link rel="stylesheet" href="stylesheet/stylesheet.css" /></xsl:otherwise>
	</xsl:choose>	
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
				<div style="padding-left:130px; position:relative; top:10px;">
					<xsl:call-template name="Links"/>
				</div>
			</td>
			<td class="container_corner_upper_right">
			</td>
		</tr>
		<tr>
			<td class="container_left">
			</td>
			<td valign="top">




				<div>
					<xsl:if test="@logo">
						<img class="pagetitle">
						<xsl:attribute name="src">Images/menu_large/<xsl:value-of select="@logo"/>.png</xsl:attribute>
						</img>
					</xsl:if>
				</div>


	<xsl:choose>
	     <xsl:when test="@container = 'false'">
	   		<xsl:apply-templates/>
    		</xsl:when>  
    		<xsl:otherwise>
	   		<div><xsl:apply-templates/></div>
		</xsl:otherwise>
	</xsl:choose>

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
    <h2><xsl:value-of select="."/></h2>
  </xsl:template>

<xsl:template name="Links">
<a href="Introduction.html" class="menulink">Main</a> |
  <a href="eula.html" class="menulink">Eula</a> |
  <a href="changelog.html" class="menulink">Changelog</a> |
  <a href="features.html" class="menulink">Features</a> |
  <a href="todo.html" class="menulink">Project Status</a> |
  <a href="toc.techdoc.html" class="menulink">Technical Documentation</a>
</xsl:template>

<xsl:template match="paragraph" >
  <xsl:if test="@title">
 	<p><b>
   	   <xsl:value-of select="@title"/>
	</b></p>
  </xsl:if>
  <p>
 	<xsl:value-of select="."/>
  </p>	
</xsl:template>



</xsl:stylesheet>
