using ChatMVC.Models;
using Grpc.Net.Client;
using GrpcServer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatController : Controller
{
    private readonly Chat.ChatClient _chatClient;
    private string usrname;

    private static List<ChatMessage> _messages = new List<ChatMessage>();

    private static List<string> _activeUsers = new List<string>();

    public ChatController(Chat.ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<IActionResult> Index(string username = null)
    {
        // If the username is provided, set it in the model
        var viewModel = new ChatViewModel
        {
            Messages = _messages,
            Username = username,
            ActiveUsers = _activeUsers
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string username, string text)
    {
        var message = new Message
        {
            Username = username,
            Text = text
        };

        await _chatClient.SendMessageAsync(message);

        // Add the message to the local list for display
        _messages.Add(new ChatMessage { Username = username, Text = text });

        if (usrname == null || usrname == ""){
            usrname = username;
        } 

        var viewModel = new ChatViewModel
        {
            Username = usrname,
            ActiveUsers = _activeUsers
        };

        // Redirect to refresh the view
        return RedirectToAction("Index", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> JoinRoom(string username){
        _activeUsers.Add(username);
        if (usrname == null || usrname == ""){
            usrname = username;
        } 

         var viewModel = new ChatViewModel
        {
            Username = usrname,
            ActiveUsers = _activeUsers
        };
        return RedirectToAction("Index", viewModel);
    }

     [HttpPost]
    public async Task<IActionResult> LeaveRoom(string username){
        _activeUsers.Remove(username);
         var viewModel = new ChatViewModel
        {
            Username = usrname,
            ActiveUsers = _activeUsers
        };
        return View();
    }


    
}
