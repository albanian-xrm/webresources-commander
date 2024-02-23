using AlbanianXrm.WebResources.Commander.Repositories;
using AlbanianXrm.WebResources.DataModel;
using AlbanianXrm.WebResources.Tests;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace AlbanianXrm.WebResources
{
    public class WebResourceRepositoryTests : FakeXrmEasyTestsBase
    {
        [Theory]
        [InlineData("MySolution")]
        public void CanGetWebResourcesInSolution(string uniqueName)
        {
            // Arrange 
            var solution = new Solution(Guid.NewGuid())
            {
                UniqueName = uniqueName,
                FriendlyName = uniqueName
            };
            // Arrange 
            var otherSolution = new Solution(Guid.NewGuid())
            {
                UniqueName = uniqueName + "other",
                FriendlyName = uniqueName + "other"
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
                [SolutionComponent.Fields.SolutionId] = otherSolution.Id,
                [SolutionComponent.Fields.ComponentType] = new OptionSetValue((int)ComponentType.WebResource),
                [SolutionComponent.Fields.ObjectId] = otherWebResource.Id
            };

            _context.Initialize(new Entity[] { solution, webResource, solutionComponent, otherSolution, otherWebResource, otherSolutionComponent });
            var webResourceRepository = new WebResourceRepository(_service);

            // Act
            var webResources = webResourceRepository.GetWebResourcesInSolution(uniqueName);

            // Assert

            Assert.NotEmpty(webResources);
            Assert.Single(webResources);
            Assert.Equal("<s>Hi There</s>",
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        webResources
                            .First()
                            .GetAttributeValue<string>(WebResource.Fields.Content))));
        }
    }
}