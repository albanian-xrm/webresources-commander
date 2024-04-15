using System;

namespace AlbanianXrm.WebResources.DataModel
{
    public partial class Webresource
    {
        public static WebResource_WebResourceType GetTypeFromExtension(string extension, bool allowUnsupported)
        {
            if (extension == null) extension = "";

            if (extension == "" && allowUnsupported)
            {
                return WebResource_WebResourceType.Script_JScript;
            }

            switch (extension.ToLower())
            {
                case ".html":
                case ".htm":
                    return WebResource_WebResourceType.Webpage_HTML;

                case ".woff":
                case ".woff2":
                case ".ttf":
                    if (!allowUnsupported)
                        break;
                    return WebResource_WebResourceType.StyleSheet_CSS;
                case ".css":
                    return WebResource_WebResourceType.StyleSheet_CSS;

                case ".ts":
                case ".map":
                case ".mem":
                case ".wasm":
                case ".cur":
                case ".res":
                case ".xod":
                    if (!allowUnsupported)
                        break;
                    return WebResource_WebResourceType.Script_JScript;
                case ".js":
                    return WebResource_WebResourceType.Script_JScript;

                case ".json":
                case ".txt":
                    if (!allowUnsupported)
                        break;
                    return WebResource_WebResourceType.Data_XML;
                case ".xml":
                    return WebResource_WebResourceType.Data_XML;

                case ".png":
                    return WebResource_WebResourceType.PNGformat;

                case ".jpg":
                case ".jpeg":
                    return WebResource_WebResourceType.JPGformat;

                case ".gif":
                    return WebResource_WebResourceType.GIFformat;

                case ".xap":
                    return WebResource_WebResourceType.Silverlight_XAP;

                case ".xsl":
                case ".xslt":
                    return WebResource_WebResourceType.StyleSheet_XSL;

                case ".ico":
                    return WebResource_WebResourceType.ICOformat;

                case ".svg":
                    return WebResource_WebResourceType.Vectorformat_SVG;

                case ".resx":
                    return WebResource_WebResourceType.String_RESX;
            }

            throw new Exception($@"File extension '{extension}' cannot be mapped to a webresource type!");
        }


    }
}
