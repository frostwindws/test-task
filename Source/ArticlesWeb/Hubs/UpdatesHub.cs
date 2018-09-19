using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ArticlesWeb.Hubs
{
    public class UpdatesHub : Hub
    {
        public async Task SendUpdate(string type, object data)
        {
            await Clients.All.SendAsync(type, data);
        }
    }
}
