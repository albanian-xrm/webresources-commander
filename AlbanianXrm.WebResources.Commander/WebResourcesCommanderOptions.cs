using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;

namespace AlbanianXrm.WebResources
{
    public class WebResourcesCommanderOptions
    {
        /// <summary>
        /// List of Glob Patterns to include or exclude in the exploration of files in the Source Folder
        /// 
        /// <para>
        ///     Examples:
        /// </para>
        /// <example>
        /// <code>
        ///     GlobPatterns = ["albx_/js/*.js", "!albx_/js/exclude.js"] 
        /// </code>
        /// </example>
        /// </summary>
        public List<string> GlobPatterns { get; set; } = new List<string>();

        /// <summary>
        /// Target solution to sync the WebResources
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// Source folder of the local webresource files
        /// </summary>
        public string SourceFolder { get; set; }

        /// <summary>
        /// Webresources Prefix
        /// </summary>
        public string Prefix { get; set; }

        public bool UseTempFolder { get; set; }

        public Matcher GlobPatternsAsMatcher()
        {
            var matcher = new Matcher();
            foreach (var globPattern in GlobPatterns)
            {
                if (globPattern.StartsWith("!"))
                {
                    matcher.AddExclude(globPattern.Substring(1));
                }
                else
                {
                    matcher.AddInclude(globPattern);
                }
            }
            return matcher;
        }
    }
}
