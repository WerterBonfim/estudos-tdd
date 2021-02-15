using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApi;
using NerdStore.WebApp.Testes.Config;
using Xunit;

namespace NerdStore.WebApp.Testes.Config
{
    [CollectionDefinition(nameof(IntegrationTestsFixtureCollection))]
    public class IntegrationTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<StartupApiTeste>> {}
    
    public class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly LojaAppFactory<TStartup> Factory;
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                
            };
            
            Factory = new LojaAppFactory<TStartup>();
            Client = Factory.CreateClient(options);
        }


        public void Dispose()
        {
            Client.Dispose();
        }
    }
}