using Werter.EstudosTdd.Features.Core;

namespace Werter.EstudosTdd.Features.Clientes
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Cliente ObterPorEmail(string email);
    }
}