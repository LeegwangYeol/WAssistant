using System.Threading.Tasks;
using ChatOrg.Chat;

namespace ChatOrg.Services
{
    public class ChatService : IChatService
    {
        private readonly ExcuteAPI _api;
        
        public ChatService()
        {
            _api = new ExcuteAPI();
        }
        
        public async Task<string> SendMessageAsync(string message)
        {
            // 비동기로 API 호출
            return await ExcuteAPI.SendMessageAsync(message);
        }
    }
}
