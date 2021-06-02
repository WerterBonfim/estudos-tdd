using System;
using FluentValidation;
using FluentValidation.Results;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Venda.Domain
{
    public class Voucher : Entity
    {
        public string Codigo { get; }
        public decimal Desconto { get; }
        public decimal? PercentualDesconto { get; }
        public int Quantidade { get; }
        public DateTime Validade { get; }
        public bool Ativo { get; }
        public bool Utilizado { get; }
        public TipoDescontoVoucher TipoDesconto { get; private set; }

        public Voucher()
        {
            
        }

        public Voucher(
            string codigo, decimal desconto,
            decimal? percentualDesconto, int quantidade, 
            DateTime validade, TipoDescontoVoucher tipoDesconto,
            bool ativo = true, bool utilizado = false)
        {
            Codigo = codigo;
            Desconto = desconto;
            PercentualDesconto = percentualDesconto;
            Quantidade = quantidade;
            Validade = validade;
            TipoDesconto = tipoDesconto;
            Ativo = ativo;
            Utilizado = utilizado;
        }


        public ValidationResult ValidarSeAplicavel()
        {
            return new ValidacaoVoucher().Validate(this);
        }
    }

    public sealed class ValidacaoVoucher : AbstractValidator<Voucher>
    {
        public const string MensagemCodigoInvalido = "Voucher sem código valido.";
        public const string MensagemExpirado = "Este voucher está expirado";
        public const string MensagemInativo = "Este voucher não é mais valido";
        public const string MensagemJaUtilizado = "Este voucher não esta mais disponível";
        public const string MensagemIndisponivel = "Este voucher não está mais disponível";
        public const string MensagemErroValorDesconto = "O valor do desconto precisa ser superior a 0";
        public const string MensagemErroPercentualDesconto =
            "O valor da porcentagem de desconto precisa ser superior a 0";

        public ValidacaoVoucher()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty()
                .WithMessage(MensagemCodigoInvalido);

            RuleFor(x => x.Validade)
                .Must(DataVencimentoSuperiorAtual)
                .WithMessage(MensagemExpirado);

            RuleFor(x => x.Ativo)
                .NotEqual(false)
                .WithMessage(MensagemInativo);

            RuleFor(x => x.Utilizado)
                .NotEqual(true)
                .WithMessage(MensagemJaUtilizado);

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage(MensagemIndisponivel);

            When(x => x.TipoDesconto == TipoDescontoVoucher.Valor, () =>
            {
                RuleFor(x => x.Desconto)
                    .NotEmpty()
                    .WithMessage(MensagemErroValorDesconto)
                    .GreaterThan(0)
                    .WithMessage(MensagemErroValorDesconto);
            });

            When(x => x.TipoDesconto == TipoDescontoVoucher.Porcentagem, () =>
            {
                RuleFor(x => x.PercentualDesconto)
                    .NotEmpty()
                    .WithMessage(MensagemErroPercentualDesconto)
                    .GreaterThan(0)
                    .WithMessage(MensagemErroPercentualDesconto);
            });
        }

        private static bool DataVencimentoSuperiorAtual(DateTime dataValidade)
        {
            return dataValidade >= DateTime.Now;
        }
    }
}