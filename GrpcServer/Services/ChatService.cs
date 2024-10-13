using System;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class ChatService : Chat.ChatBase
    {
        private static readonly List<IServerStreamWriter<Message>> _clients = new();
        public override Task<Empty> SendMessage(Message request, ServerCallContext context)
        {
            var msg = new Message{
                Username = request.Username,
                Text = request.Text
            };

            foreach (var client in _clients){
                client.WriteAsync(msg);
            }
            return Task.FromResult(new Empty());
        }

        public override async Task JoinStream(Empty request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            _clients.Add(responseStream);
            while (!context.CancellationToken.IsCancellationRequested){
                await Task.Delay(1000);
            }

            _clients.Remove(responseStream);
        }
    }
}
