using System.Threading.Tasks;

namespace Sporty.Services.Groups.Contracts
{
    internal interface IAuthServerConnect
    {
        Task<string> RequestClientCredentialsTokenAsync();
    }
}
