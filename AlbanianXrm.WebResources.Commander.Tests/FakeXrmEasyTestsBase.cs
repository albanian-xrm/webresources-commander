using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Middleware;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy.Middleware.Crud;

namespace AlbanianXrm.WebResources.Tests
{
    public class FakeXrmEasyTestsBase
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;

        public FakeXrmEasyTestsBase()
        {
            _context = MiddlewareBuilder
                            .New()
                            .AddCrud()

                            .UseCrud()

                            // Here we are saying we're using FakeXrmEasy (FXE) under a commercial context
                            // For more info please refer to the license at https://dynamicsvalue.github.io/fake-xrm-easy-docs/licensing/license/
                            // And the licensing FAQ at https://dynamicsvalue.github.io/fake-xrm-easy-docs/licensing/faq/
                            .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                            .Build();

            _service = _context.GetOrganizationService();
        }
    }
}
