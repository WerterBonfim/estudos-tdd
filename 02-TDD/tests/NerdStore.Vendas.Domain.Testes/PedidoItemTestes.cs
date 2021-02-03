using System;
using FluentAssertions;
using NerdStore.Core.DomainObjects;
using NerdStore.Venda.Domain;
using Xunit;

namespace NerdStore.Vendas.Domain.Testes
{
    [Collection(nameof(PedidoCollection))]
    public class PedidoItemTestes
    {
        private readonly PedidoTesteFixture _pedidoFixture;

        public PedidoItemTestes(PedidoTesteFixture pedidoFixture)
        {
            _pedidoFixture = pedidoFixture;
        }

        [Fact(DisplayName = "Deve retornar uma exception para um item com a quantidade inferior ao permitido")]
        [Trait("Pedido Item", "Adicionar")]
        public void DeveRetonarExceptionItemComUmaQuantidadeInferiorAoPermitido()
        {
            // Arrange
            var quantidadeInvalida = Pedido.QUANTIDADE_MINIMA_ITENS - 1;
            Action act = () => _pedidoFixture.GerarItem(quantidadeInvalida);

            // Act && Assert

            var mensagemDeErro =
                $"Défict na quantidade minima de unidade. O minimo é {Pedido.QUANTIDADE_MINIMA_ITENS}";

            act.Should().Throw<DomainException>()
                .WithMessage(mensagemDeErro);
        }
    }
}