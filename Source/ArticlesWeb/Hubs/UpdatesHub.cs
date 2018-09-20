using System;
using Microsoft.AspNetCore.SignalR;

namespace ArticlesWeb.Hubs
{
    /// <summary>
    /// Хаб для подписки на обновления.
    /// Поскольку общение одностороннее, то хаб  методов не содержит
    /// </summary>
    public class UpdatesHub : Hub
    {
    }
}
