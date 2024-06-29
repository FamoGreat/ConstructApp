using Microsoft.AspNetCore.SignalR;

namespace ConstructApp.Services
{
    public class ExpenseHub : Hub
    {
        public async Task SendExpenseUpdate(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveExpenseUpdate", user, message);
        }
    }
}
