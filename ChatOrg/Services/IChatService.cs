using System.Threading.Tasks;

namespace ChatOrg.Services
{
    public interface IChatService
    {
        Task<string> SendMessageAsync(string message);
    }
}
