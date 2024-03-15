using Microsoft.AspNetCore.SignalR;

namespace CSCCSTRDB.Hubs
{
	public interface IMatchClient
	{
		[HubMethodName("ReceiveMatchUpdate")]
		Task PushMatchUpdateAsync(string user, string message);
	}

	public class MatchHub : Hub<IMatchClient>
	{
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.PushMatchUpdateAsync(user, message);
		}

		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await base.OnDisconnectedAsync(exception);
		}
	}
}
