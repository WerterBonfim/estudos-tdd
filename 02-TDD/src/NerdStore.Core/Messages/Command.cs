using System;
using FluentValidation.Results;
using MediatR;

namespace NerdStore.Core.Messages
{
    public abstract class Command : Message, IRequest<bool>
    {
        public DateTime Timestamp { get; set; }
        public ValidationResult Validacoes { get; set; }

        public Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool EstaValido();
    }
}