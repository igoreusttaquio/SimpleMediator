using Cadastro.Commands;
using System.Threading.Tasks;

namespace Cadastro
{
    public interface IClienteService
    {
        Task<string> CadastrarCliente(CadastrarClienteCommand cliente);
        Task<bool> ExcluirCliente(ExcluirClienteCommand cliente);
    }
}