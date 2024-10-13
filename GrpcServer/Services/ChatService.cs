using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class ChatService : Chat.ChatBase
    {
        private static readonly List<IServerStreamWriter<Message>> _clients = new();
        private static readonly Dictionary<string, IServerStreamWriter<Message>> _activeUsers = new();

        public override Task<Empty> SendMessage(Message request, ServerCallContext context)
        {
            var msg = new Message
            {
                Username = request.Username,
                Text = request.Text
            };

            // Create a list to collect disconnected clients
            var disconnectedClients = new List<IServerStreamWriter<Message>>();

            // Iterate over clients and attempt to send the message
            foreach (var client in _clients)
            {
                try
                {
                    client.WriteAsync(msg).Wait();
                }
                catch
                {
                    // If sending fails, add the client to disconnected list
                    disconnectedClients.Add(client);
                }
            }

            // Remove disconnected clients
            foreach (var client in disconnectedClients)
            {
                _clients.Remove(client);
            }

            return Task.FromResult(new Empty());
        }

        public override async Task JoinStream(Empty request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            var username = context.RequestHeaders.FirstOrDefault(h => h.Key == "username")?.Value;
            if (!string.IsNullOrEmpty(username) && !_activeUsers.ContainsKey(username))
            {
                _activeUsers.Add(username, responseStream);
                _clients.Add(responseStream); // Add client to the list
            }

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000); // Keep the stream alive
                }
            }
            finally
            {
                // This block runs when the user disconnects
                if (!string.IsNullOrEmpty(username))
                {
                    _activeUsers.Remove(username); // Remove user from active users
                }

                _clients.Remove(responseStream); // Remove the client from the list
            }
        }

       
    }
}
