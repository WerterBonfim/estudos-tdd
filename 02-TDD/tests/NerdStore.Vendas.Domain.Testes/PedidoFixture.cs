using System;
using System.Linq;
using NerdStore.Venda.Domain;
using Xunit;

namespace NerdStore.Vendas.Domain.Testes
{
    
    
    [CollectionDefinition(nameof(PedidoCollection))]
    public class PedidoCollection : ICollectionFixture<PedidoTesteFixture>
    {
    }

    public class PedidoTesteFixture : IDisposable
    {
        public Pedido GerarPedido()
        {
            return new Pedido(Guid.NewGuid());
        }
        
        public Pedido GerarPedidoComUmItem(int qtdItens = 1, decimal valor = 100)
        {
            var pedido = GerarPedido();
            pedido.AdicionarItem(GerarItem());
            return pedido;
        }

        public PedidoItem GerarItem(int qtd = 1, decimal valor = 100)
        {
            return new PedidoItem(Guid.NewGuid(), "Item valido", qtd, valor);
        }

        public Pedido GerarPedidoComItens(int qtdItens = 2, decimal valor = 100)
        {
            var itens = Enumerable.Range(0, qtdItens)
                .Select(x => GerarItem(1, valor))
                .ToList();

            var pedido = GerarPedido();

            foreach (var item in itens)
                pedido.AdicionarItem(item);

            return pedido;
        }

        public Voucher GerarVoucherInvalido()
        {
            return new Voucher(
                "",
                0,
                null,
                0,
                DateTime.Now.AddDays(-1),
                TipoDescontoVoucher.Valor,
                false,
                true);
        }

        public Voucher GerarVoucherValido(
            TipoDescontoVoucher tipo = TipoDescontoVoucher.Valor,
            decimal valorPercentualOuDesconto = 15)
        {

            var desconto = valorPercentualOuDesconto;
            decimal? percentual = null;
            if (tipo == TipoDescontoVoucher.Porcentagem)
            {
                percentual = valorPercentualOuDesconto;
                desconto = 0;
            }

            return new Voucher(
                "CODIGO-TESTE-15",
                desconto, 
                percentual,
                1,
                DateTime.Now.AddDays(1),
                tipo
            );
        }

        public void Dispose()
        {
        }
    }
}