using Microsoft.Extensions.FileSystemGlobbing;
using System.IO;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Linq;
using System;

namespace AlbanianXrm.WebResources
{
    public class MatcherTests
    {
        [Fact]
        public void CanMatchMultipleFiles()
        {
            // Arrange
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var options = new WebResourcesCommanderOptions()
            {
                GlobPatterns = new List<string>(new string[] { "albx_/**/*.js" }),
                SourceFolder = Path.Combine(executingFolder, "TestFiles", "Scenario01")
            };
            var matcher = options.GlobPatternsAsMatcher();

            // Act
            PatternMatchingResult result = matcher.Execute(
                                             new DirectoryInfoWrapper(
                                                 new DirectoryInfo(options.SourceFolder)));

            // Assert          
            Assert.True(result.HasMatches);
            Assert.Contains("albx_/js/account.events.js", result.Files.Select(s => s.Path));
            Assert.Contains("albx_/js/exclude.js", result.Files.Select(s => s.Path));
        }

        [Fact]
        public void CanExcludeFiles()
        {
            // Arrange
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var options = new WebResourcesCommanderOptions()
            {
                GlobPatterns = new List<string>(new string[] { "albx_/**/*.js", "!**/exclude.js" }),
                SourceFolder = Path.Combine(executingFolder, "TestFiles", "Scenario01")
            };
            var matcher = options.GlobPatternsAsMatcher();

            // Act
            PatternMatchingResult result = matcher.Execute(
                                             new DirectoryInfoWrapper(
                                                 new DirectoryInfo(options.SourceFolder)));

            // Assert          
            Assert.True(result.HasMatches);
            Assert.Contains("albx_/js/account.events.js", result.Files.Select(s => s.Path));
            Assert.DoesNotContain("albx_/js/exclude.js", result.Files.Select(s => s.Path));
        }
    }
}
