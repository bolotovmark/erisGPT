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

      
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }


        public class Root
        {
            public List<Message> messages { get; set; } = new List<Message>();
        }

        class responseBody
        {

            public string prompt { get; set; } = "";
            public string model { get; set; } = "llama3:latest";
            public bool stream { get; set; } = true;

            public List<Message> messages { get; set; } = new List<Message>();
        }

        public async Task Send(string text, string model, string messages, string uid)
        {
            





            var urlString = "http://10.59.4.6:11434";


            responseBody _responseBody = new responseBody();


            if (messages != "[]")
            {
                var root = JsonSerializer.Deserialize<List<Message>>(messages);

                _responseBody.messages = root;
            }

           



            StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\context.txt");

            _responseBody.prompt = text;
            _responseBody.model = model;

            var uri = new Uri(urlString);





            var clients = new List<Client>();




            var context = reader.ReadToEnd();





            var guid = Guid.Parse(uid);
            var client = Program.Clients.FirstOrDefault(x => x.Uid == guid);

            if (client == null)
                return;



            client.Clients = Clients;


            

            if (client.Chat == null)
            {
                client.ApIClient = new OllamaApiClient(uri);
                client.Chat = client.ApIClient.Chat(async stream => await Update(stream, client)); //await Update(stream, Clients) 
                client.ApIClient.SelectedModel = _responseBody.model;

                client.Clients = Clients;


                var chats = await client.Chat.Send(_responseBody.prompt);
            }
            else
            {
               
                var chats = await client.Chat.Send(_responseBody.prompt);
            }
                



            

           


       

        }


        public async Task SendUpdate(string text, string model, string messages, string uid)
        {






            var urlString = "http://10.59.4.6:11434" + "/api/embeddings";


            responseBody _responseBody = new responseBody();


            if (messages != "[]")
            {
                var root = JsonSerializer.Deserialize<List<Message>>(messages);

                _responseBody.messages = root;
            }

          
            HttpClient httpClient = new HttpClient();




            _responseBody.prompt = text;
            _responseBody.model = model;

            StringContent jsonContent = new(JsonSerializer.Serialize(_responseBody), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, urlString);

            request.Content = jsonContent;

            var response = httpClient.SendAsync(request);

            string responseText = "";

            try
            {



                responseText = await response.Result.Content.ReadAsStringAsync();

                await Clients.Caller.SendAsync("Update", responseText, true);

            }
            catch (Exception ex)
            {


            }









        }


        public async Task Update(ChatResponseStream stream, Client client)
        {

            var msg = stream.Message?.Content.Replace("\n", "<br>") ?? "";
            
            await client.Clients.Caller.SendAsync("Update", msg, stream.Done);
        }


     

    }
}
