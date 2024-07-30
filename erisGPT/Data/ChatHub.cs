using erisGPT.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace erisGPT.Data
{
    public class ChatHub : Hub
    {
        // // json 
        // public class Message
        // {
        //     public string role { get; set; }
        //     public string content { get; set; }
        //     public List<string> images { get; set; }
        // }
        // public class Root
        // {
        //     public List<Message> messages { get; set; } = new List<Message>();
        // }
        //
        //
        // class responseBody
        // {
        //
        //     public string prompt { get; set; } = "";
        //     public string model { get; set; } = "llama3:latest";
        //     public bool stream { get; set; } = true;
        //     public List<string> images { get; set; } = new List<string>();
        //     public List<Message> messages { get; set; } = new List<Message>();
        // }

        public async Task SendImg(string text, string imagesJson, string uid)
        {
            Console.WriteLine(imagesJson);
            var urlString = "http://10.59.4.6:11434";
            var uri = new Uri(urlString);
            var guid = Guid.Parse(uid);
            var client = Program.Clients.FirstOrDefault(x => x.Uid == guid);
            var imagesList = JsonSerializer.Deserialize<List<string>>(imagesJson);
            
            
            if (client == null)
                return;
            
            if (client.Chat == null)
            {
                client.ApIClient = new OllamaApiClient(uri);
                client.ApIClient.SelectedModel = "llava:34b";
                client.Chat = client.ApIClient.Chat(async stream => await Update(stream, client)); //await Update(stream, Clients) 
                
                client.Clients = Clients;
            }
            
            if(client.ApIClient.SelectedModel != "llava:34b" )
            {
                client.ApIClient.SelectedModel = "llava:34b";
            }
            
            var chats = await client.Chat.Send(text, imagesList);
        }
        
        public async Task Send(string text, string uid)
        {
            var urlString = "http://10.59.4.6:11434";
            var uri = new Uri(urlString);
            var guid = Guid.Parse(uid);
            var client = Program.Clients.FirstOrDefault(x => x.Uid == guid);
            
            if (client == null)
                 return;
            
            if (client.Chat == null)
            {
                client.ApIClient = new OllamaApiClient(uri);
                client.ApIClient.SelectedModel = "llama3:80b";
                client.Chat = client.ApIClient.Chat(async stream => await Update(stream, client)); //await Update(stream, Clients) 
                
                client.Clients = Clients;
            }
            
            if(client.ApIClient.SelectedModel != "llama3:80b" )
            {
                client.ApIClient.SelectedModel = "llama3:80b";
            }
            
            var chats = await client.Chat.Send(text);
            // var urlString = "http://10.59.4.6:11434";
            // responseBody _responseBody = new responseBody();
            //
            // List<string> imagesList = JsonSerializer.Deserialize<List<string>>(imagesJSON);
            // _responseBody.images = imagesList;
            // foreach (var VARIABLE in imagesList)
            // {
            //     Console.WriteLine(VARIABLE);
            // }
            //
            // _responseBody.model = "llama3:80b";
            // if (imagesJSON != "")
            // {
            //     _responseBody.model = "llava:34b";
            // }
            //
            // // if (messages != "[]")
            // // {
            // //     var root = JsonSerializer.Deserialize<List<Message>>(messages);
            // //     _responseBody.messages = root;
            // // }
            // var root = JsonSerializer.Deserialize<List<Message>>(messages);
            // _responseBody.messages = root;
            //
            //
            // StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"/context.txt");
            //
            // _responseBody.prompt = text;
            // _responseBody.model = model;
            //
            // var uri = new Uri(urlString);
            //
            // var clients = new List<Client>();
            // var context = reader.ReadToEnd();
            //
            // var guid = Guid.Parse(uid);
            // var client = Program.Clients.FirstOrDefault(x => x.Uid == guid);
            //
            // if (client == null)
            //     return;
            //
            // if (client.Chat == null)
            // {
            //     client.ApIClient = new OllamaApiClient(uri);
            //     client.Chat = client.ApIClient.Chat(async stream => await Update(stream, client)); //await Update(stream, Clients) 
            //     client.ApIClient.SelectedModel = _responseBody.model;
            //
            //     client.Clients = Clients;
            // }
            // var chats = await client.Chat.Send(_responseBody.prompt, _responseBody.images);
        }


        // public async Task SendUpdate(string text, string model, string messages, string uid)
        // {
        //     var urlString = "http://10.59.4.6:11434" + "/api/embeddings";
        //
        //
        //     responseBody _responseBody = new responseBody();
        //
        //
        //     if (messages != "[]")
        //     {
        //         var root = JsonSerializer.Deserialize<List<Message>>(messages);
        //
        //         _responseBody.messages = root;
        //     }
        //
        //   
        //     HttpClient httpClient = new HttpClient();
        //     
        //     _responseBody.prompt = text;
        //     _responseBody.model = model;
        //
        //     StringContent jsonContent = new(JsonSerializer.Serialize(_responseBody), Encoding.UTF8, "application/json");
        //     var request = new HttpRequestMessage(HttpMethod.Post, urlString);
        //     request.Content = jsonContent;
        //     var response = httpClient.SendAsync(request);
        //     string responseText = "";
        //
        //     try
        //     {
        //         responseText = await response.Result.Content.ReadAsStringAsync();
        //
        //         await Clients.Caller.SendAsync("Update", responseText, true);
        //
        //     }
        //     catch (Exception ex)
        //     {
        //         
        //     }
        // }


        public async Task Update(ChatResponseStream stream, Client client)
        {
            var msg = stream.Message?.Content ?? "";
            await client.Clients.Caller.SendAsync("Update", msg, stream.Done);
        }
    }
}
