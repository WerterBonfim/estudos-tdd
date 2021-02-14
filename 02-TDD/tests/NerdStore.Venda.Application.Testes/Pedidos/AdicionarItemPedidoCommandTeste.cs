using System;
using System.Linq;
using FluentAssertions;
using NerdStore.Venda.Application.Commands;
using Xunit;

namespace NerdStore.Venda.Application.Testes.Pedidos
{
    public class AdicionarItemPedidoCommandTeste
    {
        [Fact(DisplayName = "O command para adicionar um novo item deve ser válido")]
        [Trait("PedidoCommand", "Adicionar")]
        public void CommandParaAdicionarItemDeveSerValido()
        {
            // Arrange
            var command = new AdicionarItemPedidoCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Produto teste",
                2,
                100
                );

            // Act

            var estaValido = command.EstaValido();

            // Assert

            estaValido
                .Should()
                .BeTrue();
        }

        [Fact(DisplayName = "Deve notificar erros para o command que está inválido")]
        [Trait("PedidoCommand", "Adicionar")]
        public void DeveNotificarErrosParaCommandInvalido()
        {
            // Arrange

            var command = new AdicionarItemPedidoCommand(
                Guid.Empty,
                Guid.Empty,
                "",
                0,
                0
            );

            // Act

            var estaValido = command.EstaValido();
            var erros = command.Validacoes
                .Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            // Assert

            estaValido.Should().BeFalse();

            erros.Should()
                .Contain(ValidacaoParaAdicionarItemPedido.IdClienteErroMsg)
                .And.Contain(ValidacaoParaAdicionarItemPedido.IdProdutoErroMsg)
                .And.Contain(ValidacaoParaAdicionarItemPedido.NomeErroMsg)
                .And.Contain(ValidacaoParaAdicionarItemPedido.QtdMinErroMsg)
                .And.Contain(ValidacaoParaAdicionarItemPedido.ValorErroMsg);

        }
    }
}