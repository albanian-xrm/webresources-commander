using AlbanianXrm.WebResources.DataModel;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace AlbanianXrm.WebResources.Commander.Repositories
{
    internal class WebResourceRepository
    {
        private readonly IOrganizationService service;

        public WebResourceRepository(IOrganizationService service)
        {
            this.service = service;
        }

        public void CreateWebResourceAndAddToSolution(WebResource webResource, string solutionUniqueName)
        {
            webResource.Id = this.service.Create(webResource);
            var request = new AddSolutionComponentRequest
            {
                AddRequiredComponents = false,
                ComponentId = webResource.Id,
                ComponentType = (int)ComponentType.WebResource,
                SolutionUniqueName = solutionUniqueName
            };

            service.Execute(request);
        }

        public void UpdateWebResource(WebResource webResource)
        {
            this.service.Update(webResource);
        }

        public void DeleteWebResource(WebResource webResource)
        {
            this.service.Delete(webResource.LogicalName, webResource.Id);
        }

        public List<WebResource> GetWebResourcesInSolution(string solutionUniqueName)
        {
            var query = new QueryExpression(WebResource.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                    WebResource.Fields.Content,
                    WebResource.Fields.DisplayName,
                    WebResource.Fields.IsManaged,
                    WebResource.Fields.Name,
                    WebResource.Fields.WebResourceId,
                    WebResource.Fields.WebResourceType)
            };

            query.AddLink(SolutionComponent.EntityLogicalName, WebResource.Fields.WebResourceId, SolutionComponent.Fields.ObjectId)
                 .AddLink(Solution.EntityLogicalName, SolutionComponent.Fields.SolutionId, Solution.Fields.SolutionId)
                 .LinkCriteria
                 .AddCondition(Solution.Fields.UniqueName, ConditionOperator.Equal, solutionUniqueName);

            return service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<WebResource>()).ToList();
        }
    }
}
