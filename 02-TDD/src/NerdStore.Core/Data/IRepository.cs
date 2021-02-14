using System;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Core.Data
{
    public interface IRepository<T> : IDisposable, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}