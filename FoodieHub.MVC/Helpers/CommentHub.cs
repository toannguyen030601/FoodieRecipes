using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FoodieHub.MVC.Helpers
{
    public class CommentHub : Hub
    {
        public async Task SendComment(string message, string avatar, string fullName)
        {
            await Clients.All.SendAsync("ReceiveComment", message, avatar, fullName);
        }
    }
}
