using System;
using System.Linq;
using FluentAssertions;
using NerdStore.Core.DomainObjects;
using NerdStore.Venda.Domain;
using Xunit;

namespace NerdStore.Vendas.Domain.Testes
{
    [Collection(nameof(PedidoCollection))]
    public class PedidoTeste
    {
        private readonly PedidoTesteFixture _pedidoFixture;

        public PedidoTeste(PedidoTesteFixture pedidoFixture)
        {
            _pedidoFixture = pedidoFixture;
        }

        [Fact(DisplayName = "Deve atualizar valor quando adicionar novo pedido")]
        [Trait("Pedido", "Adicionar")]
        public void DeveAdicionarUmNovoPedido()
        {
            // Arrange

            var pedido = new Pedido(Guid.NewGuid());
            var item = _pedidoFixture.GerarItem(2);

            // Act
            pedido.AdicionarItem(item);

            // Assert
            pedido.ValorTotal
                .Should()
                .Be(200);
        }

        [Fact(DisplayName = "Deve adicionar o mesmo produto no pedido")]
        [Trait("Pedido", "Adicionar")]
        public void DeveAdicionarOMesmoItemNoPedido()
        {
            // Arrange

            var produtoId = Guid.NewGuid();
            var pedido = new Pedido(Guid.NewGuid());
            var produtoCom2Qtd = new PedidoItem(produtoId, "Produto de teste", 2, 100);
            var produtoCom1Qtd = new PedidoItem(produtoId, "Produto de teste", 1, 100);

            // Act
            pedido.AdicionarItem(produtoCom2Qtd);
            pedido.AdicionarItem(produtoCom1Qtd);

            // Assert


            pedido.Itens.Count
                .Should()
                .Be(1, "foi adicionado o mesmo produto " +
                       "mais com uma quantidade diferente");

            pedido.Itens
                .FirstOrDefault(x => x.ProdutoId == produtoId)
                ?.Quantidade
                .Should()
                .Be(3, "foi adicionado o mesmo produto duas vezes. " +
                       "Na primeira com dois item na segunda com um item");

            pedido.ValorTotal
                .Should()
                .Be(300);
        }

        [Fact(DisplayName = "Deve retornar uma exception para um item que excedeu o limite de unidades")]
        [Trait("Pedido", "Adicionar")]
        public void DeveRetonarExceptionItemExcedeuAQuantidadeMaximaDeUnidades()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var item1 = new PedidoItem(produtoId, "Pedido teste", Pedido.QUANTIDADE_MAXIMA_ITENS, 100);
            var item2 = new PedidoItem(produtoId, "Pedido teste", 1, 100);

            // Act 

            pedido.AdicionarItem(item1);

            // Assert
            var mensagemDeErro =
                $"Excedeu a quantidade maxima de unidade. O maximo é {Pedido.QUANTIDADE_MAXIMA_ITENS}";

            pedido.Invoking(x => x.AdicionarItem(item2))
                .Should().Throw<DomainException>()
                .WithMessage(mensagemDeErro);
        }


        [Fact(DisplayName = "Deve retornar uma exception ao tentar atualizar um item inexistente")]
        [Trait("Pedido", "Atualizar")]
        public void DeveRetornarExceptionAoTentarAtualizarItemInexistente()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid());
            var itemPedido = new PedidoItem(Guid.NewGuid(), "teste", 3, 100);


            // Act && Assert
            pedido.Invoking(x => x.AtualizarItem(itemPedido))
                .Should().Throw<DomainException>()
                .WithMessage("Não existe esse item no pedido");
        }

        [Fact(DisplayName = "Deve atualizar a quantidade de items, passando um novo item com mesmo produto")]
        [Trait("Pedido", "Atualizar")]
        public void DeveFazerAlgumaCoisa()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "teste", 3, 100);
            pedido.AdicionarItem(pedidoItem);
            var pedidoAtualizado = new PedidoItem(produtoId, "teste", 2, 100);


            // Act
            pedido.AtualizarItem(pedidoAtualizado);

            // Assert
            pedido.Itens.First(x => x.ProdutoId == produtoId)
                .Quantidade
                .Should()
                .Be(pedidoAtualizado.Quantidade,
                    "deve atualizar o item baseado no novo item passado");
        }

        [Fact(DisplayName = "Deve calcular corretamente o valor total, dados dois itens, sendo que um foi atualizado")]
        [Trait("Pedido", "Atualizar")]
        public void DeveFazerCalcularCorrentamenteDoisItensUmAtualizado()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid());
            var pedidoId = Guid.NewGuid();

            var teclado = new PedidoItem(Guid.NewGuid(), "Teclado Adamantium RGB", 1, 400);
            var mouse = new PedidoItem(pedidoId, "Mouse Adamantium RGB", 1, 100);

            pedido.AdicionarItem(teclado);
            pedido.AdicionarItem(mouse);

            var mouseAtualizado = new PedidoItem(pedidoId, "Mouse Adamantium RGB", 2, 100);
            var total = teclado.SubTotal + mouseAtualizado.SubTotal;

            // Act
            pedido.AtualizarItem(mouseAtualizado);

            // Assert
            total.Should()
                .Be(pedido.ValorTotal);
        }

        [Fact(DisplayName =
            "Deve retornar uma exception, foi atualizado um item com uma quantidade maior que o permitido")]
        [Trait("Pedido", "Atualizar")]
        public void DeveRetornarUmaExceptionQtdMaximaExcedidaAoTentarAtualizar()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid());
            var pedidoId = Guid.NewGuid();
            var teclado = new PedidoItem(pedidoId, "Teclado Adamantium RGB", 1, 400);

            pedido.AdicionarItem(teclado);

            var tecladoAtualizado = new PedidoItem(
                pedidoId,
                "Teclado Adamantium RGB",
                Pedido.QUANTIDADE_MAXIMA_ITENS + 1,
                400);

            var mensagemDeErro =
                $"Excedeu a quantidade maxima de unidade. O maximo é {Pedido.QUANTIDADE_MAXIMA_ITENS}";

            // Act && Assert
            pedido.Invoking(x => x.AtualizarItem(tecladoAtualizado))
                .Should()
                .Throw<DomainException>()
                .WithMessage(mensagemDeErro);
        }

        [Fact(DisplayName = "Deve retornar uma exception ao tentar remover um item que não existe")]
        [Trait("Pedido", "Remover")]
        public void DeveRetornarUmaExceptoinItemNãoExiste()
        {
            // Arrange
            var pedidoCom2Itens = _pedidoFixture.GerarPedidoComItens();
            var item = _pedidoFixture.GerarItem();

            // Act && Assert
            pedidoCom2Itens.Invoking(x => x.RemoverItem(item))
                .Should()
                .Throw<DomainException>()
                .WithMessage("Não existe esse item no pedido");
        }

        [Fact(DisplayName = "Deve atualizar o valor total após remover um item do pedido")]
        [Trait("Pedido", "Remover")]
        public void DeveAtualizarOValorAposRemoverUmItemDoPedido()
        {
            // Arrange
            var pedido = _pedidoFixture.GerarPedidoComItens(5);
            var itemParaRemover = pedido.Itens.First();

            // Act

            pedido.RemoverItem(itemParaRemover);

            // Assert
            pedido.ValorTotal.Should()
                .Be(400,
                    "pedido tinha 5 item totalizando 500, removeu um item, novo valor deve ser 400");
        }

        [Fact(DisplayName = "Deve calcular o desconto de um voucher valido")]
        [Trait("Pedido", "Voucher")]
        public void DeveCalcularODescontoParaUmVoucherValido()
        {
            // Arrange

            var pedido = _pedidoFixture.GerarPedidoComItens(1, 100);
            var voucher = _pedidoFixture.GerarVoucherValido();

            // Act

            var validationResult = pedido.AplicarVoucher(voucher);
            var valorPedidoEsperado = pedido.Itens
                .Sum(x => x.SubTotal) - voucher.Desconto;

            // Assert

            validationResult.IsValid.Should()
                .BeTrue();

            pedido.ValorTotal.Should()
                .Be(valorPedidoEsperado);
        }

        [Fact(DisplayName = "Deve notificar erro ao tentar aplicar um voucher inválido")]
        [Trait("Pedido", "Voucher")]
        public void DeveNotificarUmErroAoAplicarVoucherInvalido()
        {
            // Arrange
            var pedido = _pedidoFixture.GerarPedidoComUmItem();
            var voucherInvalido = _pedidoFixture.GerarVoucherInvalido();

            // Act

            var result = pedido.AplicarVoucher(voucherInvalido);

            // Assert

            result.IsValid.Should()
                .BeFalse();

            result.Errors
                .Select(x => x.ErrorMessage)
                .Should()
                .Contain(ValidacaoVoucher.MensagemExpirado)
                .And.Contain(ValidacaoVoucher.MensagemInativo)
                .And.Contain(ValidacaoVoucher.MensagemIndisponivel)
                .And.Contain(ValidacaoVoucher.MensagemCodigoInvalido)
                .And.Contain(ValidacaoVoucher.MensagemJaUtilizado)
                .And.Contain(ValidacaoVoucher.MensagemErroValorDesconto);
        }

        [Fact(DisplayName = "Deve aplicar uma porcentagem de desconto através do voucher")]
        [Trait("Pedido", "Voucher")]
        public void DeveAplicarPorcentagemDeDescontoAtravesDoVoucher()
        {
            // Arrange
            var pedido = _pedidoFixture.GerarPedidoComUmItem();
            var voucher = _pedidoFixture
                .GerarVoucherValido(TipoDescontoVoucher.Porcentagem, 50);

            // Act

            var result = pedido.AplicarVoucher(voucher);
            var total = pedido.Itens.Sum(x => x.SubTotal);
            var valorEsperadoDoPedido = (total * voucher.PercentualDesconto) / 100;

            // Assert

            result.IsValid.Should()
                .BeTrue();

            pedido.ValorTotal.Should()
                .Be(valorEsperadoDoPedido);
        }

        [Fact(DisplayName = "Total do pedido deve ser 0 para um voucher que excede o valor total")]
        [Trait("Pedido", "Voucher")]
        public void TotalDoPedidoDeveSer0VoucherExcedeOValor()
        {
            // Arrange
            var pedido = _pedidoFixture.GerarPedidoComUmItem();
            var voucher = new Voucher(
                "Presente",
                200,
                null,
                1,
                DateTime.Now.AddDays(1),
                TipoDescontoVoucher.Valor);

            // Act

            var result = pedido.AplicarVoucher(voucher);

            // Assert

            result.IsValid
                .Should()
                .BeTrue();

            pedido.ValorTotal
                .Should()
                .Be(0, "Valor do desconto é maior que total do pedido");
        }

        [Fact(DisplayName = "Deve recalcular o valor total corretamente após adicionar outro item no pedido")]
        [Trait("Pedido", "Voucher")]
        public void DeveRecalcularCorretamenteOValorTotalAposAdicionarUmNovoItemNoPedidoComVoucher()
        {
            // Arrange
            var pedido = _pedidoFixture.GerarPedido();
            var item1 = _pedidoFixture.GerarItem();
            var vouchar = _pedidoFixture.GerarVoucherInvalido();
            var item2 = _pedidoFixture.GerarItem();
            var valorEsperado = (item1.SubTotal + item2.SubTotal) - vouchar.Desconto; 

            // Act
            pedido.AdicionarItem(item1);
            pedido.AplicarVoucher(vouchar);
            pedido.AdicionarItem(item2);

            // Assert

            pedido.ValorTotal
                .Should()
                .Be(valorEsperado);



        }
    }
}