using System.Threading.Tasks;

namespace Sporty.Services.Users.Contracts
{
    internal interface IAuthServerConnect
    {
        Task<string> RequestClientCredentialsTokenAsync();
    }
}
