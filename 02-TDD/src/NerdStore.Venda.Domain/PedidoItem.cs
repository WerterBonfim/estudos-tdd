using System;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Venda.Domain
{
    public class PedidoItem : Entity, IEquatable<PedidoItem>
    {
        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; }
        public decimal ValorUnitario { get; }
        public int Quantidade { get; private set; }
        public string Titulo { get; }
        public decimal SubTotal => CalcularSubTotal();
        
        // EF Referencia
        public Pedido Pedido { get; set; }

        public PedidoItem()
        {
            
        }

        public PedidoItem(Guid produtoId, string titulo, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            Titulo = titulo;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            
            if (quantidade < Pedido.QUANTIDADE_MINIMA_ITENS)
            {
                var mensagemDeErro =
                    $"Défict na quantidade minima de unidade. " +
                    $"O minimo é {Pedido.QUANTIDADE_MINIMA_ITENS}";
                throw new DomainException(mensagemDeErro);
            }
        }

        private decimal CalcularSubTotal()
        {
            return Quantidade * ValorUnitario;
        }

        internal void AdicionarQuantidade(int quantidade)
        {
            Quantidade += quantidade;
        }


        #region [ Implementanção do IEquatable ]
        public bool Equals(PedidoItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProdutoId.Equals(other.ProdutoId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PedidoItem) obj);
        }

        public override int GetHashCode()
        {
            return ProdutoId.GetHashCode();
        }
        
        #endregion

        public void AtualizarQuantidade(int quantidade)
        {
            Quantidade = quantidade;
        }
    }
}