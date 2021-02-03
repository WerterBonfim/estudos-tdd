using System;

namespace NerdStore.Core.DomainObjects
{
    public class DomainException : Exception
    {
        public DomainException(string mensagem) : base(mensagem)
        {
            
        }
        
    }
}