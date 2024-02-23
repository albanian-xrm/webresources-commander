using AlbanianXrm.WebResources.Commander;
using AlbanianXrm.WebResources.Commander.Repositories;
using AlbanianXrm.WebResources.DataModel;
using AlbanianXrm.WebResources.Tests;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AlbanianXrm.WebResources
{
    public class WebResourcesCommanderTests : FakeXrmEasyTestsBase
    {
        public WebResourcesCommanderTests(): base() {
            string solutionUniqueName = "MySolution";

            // Arrange 
            var solution = new Solution(Guid.NewGuid())
            {
                UniqueName = solutionUniqueName,
                FriendlyName = solutionUniqueName
            };
            var webResource = new WebResource(Guid.NewGuid())
            {
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("<s>Hi There</s>")),
                Name = "albx_/t.txt",
                WebResourceType = WebResource_WebResourceType.Data_XML
            };
            var otherWebResource = new WebResource(Guid.NewGuid())
            {
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("<s>Hi There Other</s>")),
                Name = "albx_/other.txt",
                WebResourceType = WebResource_WebResourceType.Data_XML
            };
            var solutionComponent = new SolutionComponent(Guid.NewGuid())
            {
                [SolutionComponent.Fields.SolutionId] = solution.Id,
                [SolutionComponent.Fields.ComponentType] = new OptionSetValue((int)ComponentType.WebResource),
                [SolutionComponent.Fields.ObjectId] = webResource.Id
            };

            var otherSolutionComponent = new SolutionComponent(Guid.NewGuid())
            {
                [SolutionComponent.Fields.SolutionId] = solution.Id,
                [SolutionComponent.Fields.ComponentType] = new OptionSetValue((int)ComponentType.WebResource),
                [SolutionComponent.Fields.ObjectId] = otherWebResource.Id
            };

            _context.Initialize(new Entity[] { solution, webResource, solutionComponent, otherWebResource, otherSolutionComponent });
        }

        [Fact]
        public void CanSaveWebresourcesToTempFolder()
        {
            // Arrange
            string solutionUniqueName = "MySolution";
            var webResourcesCommander = new WebResourcesCommander(_service, new WebResourcesCommanderOptions()
            {
                Solution = solutionUniqueName,
                UseTempFolder = true,
            });
            var repository = new WebResourceRepository(_service);


            // Act
            webResourcesCommander.webResources = repository.GetWebResourcesInSolution(solutionUniqueName);
            webResourcesCommander.SaveWebResourcesInTempFolder();
            var fileTContent = File.ReadAllText(webResourcesCommander.tempWebResources[ "albx_/t.txt"]);

            // Assert
            Assert.Equal("<s>Hi There</s>", fileTContent);
        }

        [Fact]
        public void CanKeepWebresourcesInMemory()
        {
            // Arrange
            string solutionUniqueName = "MySolution";
            var webResourcesCommander = new WebResourcesCommander(_service, new WebResourcesCommanderOptions()
            {
                Solution = solutionUniqueName,
                UseTempFolder = false
            });
            var repository = new WebResourceRepository(_service);


            // Act
            webResourcesCommander.webResources = repository.GetWebResourcesInSolution(solutionUniqueName);
            webResourcesCommander.SaveWebResourcesInMemory();
            string fileTContent;
            using(var reader = new StreamReader(webResourcesCommander.memoryWebResources["albx_/t.txt"]))
            {
                fileTContent = reader.ReadToEnd();
            }

            // Assert
            Assert.Equal("<s>Hi There</s>", fileTContent);
        }

        [Fact]
        public void CanCalculateOperationsWithWebresourcesInMemory()
        {
            // Arrange
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string solutionUniqueName = "MySolution";
            var webResourcesCommander = new WebResourcesCommander(_service, new WebResourcesCommanderOptions()
            {
                GlobPatterns = new List<string>(new string[] { "albx_/**/*.js", "albx_/**/*.txt" }),
                SourceFolder = Path.Combine(executingFolder, "TestFiles", "Scenario02"),
                Solution = solutionUniqueName,
                UseTempFolder = false
            });
            var repository = new WebResourceRepository(_service);

            // Act
            webResourcesCommander.GetLocalWebResources();
            webResourcesCommander.webResources = repository.GetWebResourcesInSolution(solutionUniqueName);
            webResourcesCommander.SaveWebResourcesInMemory();
            webResourcesCommander.CalculateOperations();

            // Assert
            Assert.Equal(Operation.Create, webResourcesCommander.pendingOperations["albx_/js/account.events.js"]);
            Assert.Equal(Operation.Delete, webResourcesCommander.pendingOperations["albx_/other.txt"]);
            Assert.Equal(Operation.NoAction, webResourcesCommander.pendingOperations["albx_/t.txt"]);
        }
    }
}
