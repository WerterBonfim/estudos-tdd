using System;
using System.Linq;
using FluentAssertions;
using NerdStore.Venda.Domain;
using Xunit;

namespace NerdStore.Vendas.Domain.Testes
{
    public class VoucherTestes
    {
        [Fact(DisplayName = "Deve notificar que voucher é valido")]
        [Trait("Voucher", "Validacao")]
        public void DeveNotificarQueVoucherEValido()
        {
            // Arrange
            var voucher = new Voucher(
                "PROMO-15-REAIS", 
                15, 
                null, 
                1, 
                DateTime.Now.AddDays(1),
                TipoDescontoVoucher.Valor);

            // Act

            var eValido = voucher.ValidarSeAplicavel();
            // Assert
            eValido.Errors.Count
                .Should()
                .Be(0);
        }

        [Fact(DisplayName = "Deve mostrar 6 notificações de erros para um voucher inválido")]
        [Trait("Voucher", "Validacao")]
        public void DeveRetornar6NotificacoesDeErrosParaUmVoucherInvalido()
        {
            // Arrange
            var voucher = new Voucher(
                "", 
                0, 
                null, 
                0, 
                DateTime.Now.AddDays(-1),
                TipoDescontoVoucher.Valor,
                false,
                true);

            // Act

            var validationResult = voucher.ValidarSeAplicavel();
            
            // Assert
            validationResult.IsValid
                .Should()
                .BeFalse();
            
            validationResult.Errors.Count
                .Should()
                .Be(7);

            var mensagensDeErros = validationResult.Errors
                .Select(x => x.ErrorMessage);

            mensagensDeErros.Should()
                .Contain(ValidacaoVoucher.MensagemExpirado)
                .And.Contain(ValidacaoVoucher.MensagemInativo)
                .And.Contain(ValidacaoVoucher.MensagemIndisponivel)
                .And.Contain(ValidacaoVoucher.MensagemCodigoInvalido)
                .And.Contain(ValidacaoVoucher.MensagemJaUtilizado)
                .And.Contain(ValidacaoVoucher.MensagemErroValorDesconto);



        }
    }
}