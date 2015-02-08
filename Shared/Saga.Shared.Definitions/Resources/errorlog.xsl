<?xml version='1.0'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="page">
      <html>
        <head>
          <title>
            <xsl:value-of select="title" />
          </title>
          <style>
            BODY {
            padding: 20px;
            }

            BODY, DIV, P, TD {
            font-size:1em;
            color: rgb(83,100,130);
            }

            H1
            {
            color:#000000;
            font-size: 20px;
            }

            H2
            {
            color:#000000;
            font-size: 16px;
            }

            CODE {
            padding: 3px;
            background-color: #FFFFFF;
            border: 1px solid #C9D2D8;
            font-size: 1em;
            margin-bottom: 10px;
            display: block;
            font: 0.9em Monaco, "Andale Mono","Courier New", Courier, mono;
            line-height: 1.3em;
            }

            .paragraph_menu {
            background: rgb(205, 220, 235) ;
            padding:10px;
            }

            .paragraph {
            background: rgb(236,243,247);
            padding:10px;
            }

            a:link	{ color: #105289; text-decoration: none; }
            a:visited	{ color: #105289; text-decoration: none; }
            a:hover	{ color: #D31141; text-decoration: underline; }
            a:active	{ color: #368AD2; text-decoration: none; }

            .top {
            text-align:right;
            }

            ol  {
            color: black;
            font-size:13px;
            line-height:1.3em;
            }
          </style>
        </head>
        <body>
          <xsl:apply-templates />
        </body>
      </html>
    </xsl:template>

  <xsl:template match="title">
    <h1>
      <xsl:value-of select="." />
    </h1>
  </xsl:template>

  <xsl:template match="errorlog">
    <p>

      This is the documentation about quest and npc scriping. As of this moment we'll keep this document
      up-to-date. For questfiles in the desired and validated layout we'll provide conversion tools to
      update this.
    </p>

    <h2>Functions by alphabet</h2>
    <div class="paragraph_menu">
      <p>
        <ol>
          <xsl:for-each select="/page/errorlog/error">
          <xsl:sort select="name" />
            <li>
              <a href="#{name}"><xsl:value-of select="name" /></a>
            </li>
          </xsl:for-each>
        </ol>
      </p>
    </div>

    <xsl:for-each select="/page/errorlog/error">
      <h2>
        <a name="{name}"><xsl:value-of select="name" /></a>
      </h2>
      <div class="paragraph">
        <!-- Full error-->
        <code><pre><xsl:value-of select="example" /></pre></code>
      </div>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>