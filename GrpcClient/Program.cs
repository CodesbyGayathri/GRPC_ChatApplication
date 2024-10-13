// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using Grpc.Net.Client;
using GrpcServer;

var channel = GrpcChannel.ForAddress("http://localhost:5114");
var client = new Chat.ChatClient(channel);

var cts = new CancellationTokenSource();

var listenTask = Task.Run(async () =>{
    using var stream =  client.JoinStream(new Empty());

    while (await stream.ResponseStream.MoveNext(cts.Token)){
        var message = stream.ResponseStream.Current;
        Console.WriteLine($"{message.Username}: {message.Text}");
    }
});

Console.WriteLine("Enter your Name:");
string UserName = Console.ReadLine();


while (true){
    string messageText = Console.ReadLine();
    var msg = new Message{ Username = UserName, Text = messageText };
    await client.SendMessageAsync(msg);
}


