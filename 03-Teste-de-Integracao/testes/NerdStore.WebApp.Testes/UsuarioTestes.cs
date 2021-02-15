using System.Threading.Tasks;
using NerdStore.WebApi;
using NerdStore.WebApp.Testes.Config;
using Xunit;

namespace NerdStore.WebApp.Testes
{
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class UsuarioTestes
    {
        private readonly IntegrationTestsFixture<StartupApiTeste> _fixture;

        public UsuarioTestes(IntegrationTestsFixture<StartupApiTeste> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Deve realizar um cadastro de usuario com sucesso")]
        [Trait("Cadastro", "Inserir")]
        public async Task DeveCadastrarUmNovoUsuarioComSucesso()
        {
            // Arrange

            var response = await _fixture.Client.GetAsync("/Identity");
            response.EnsureSuccessStatusCode();
            // Act

            // Assert
        }
    }
}