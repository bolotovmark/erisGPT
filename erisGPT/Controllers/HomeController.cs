using erisGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace erisGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {

            var guid = Guid.NewGuid();
            Program.Clients.Add(new Client { Uid = guid });

            
            
            ViewBag.Uid = guid.ToString();
            ViewBag.MainUrl = configuration.GetSection("urls").GetSection("main").Value;
            return View();
        }

        public IActionResult Learn() => Index();

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //class responseBody
        //{
        //    public int max_context_length { get; set; } = 1600;
        //    public int max_length { get; set; } = 256;
        //    public string prompt { get; set; } = "";
        //    public bool quiet { get; set; } = true;
        //    public float rep_pen { get; set; } = 0;
        //    public int rep_pen_range { get; set; } = 320;
        //    public float rep_pen_slope { get; set; } = 0.7f; //1.1
        //    public float temperature { get; set; } = 0.7f;
        //    public int tfs { get; set; } = 1;
        //    public float top_a { get; set; } = 0;
        //    public float top_k { get; set; } = 100;
        //    public float top_p { get; set; } = 0.92f;
        //    public int typical { get; set; } = 1;
        //}



        public class Message {
            public string role { get; set; }
            public string content { get; set; }
        }


        public class Root
        {
            public List<Message> messages { get; set; } = new List<Message>();
        }

        class responseBody {
         
            public string prompt { get; set; } = "";
            public string model { get; set; } = "llama3:latest";
            public bool stream { get; set; } = true;

            public List<Message> messages { get; set; } = new List<Message>();
        }


        public async Task<JsonResult> SendMessage(string prompt, string messages)
        {
            var urlString = configuration.GetSection("urls").GetSection("main").Value + "/api/generate/";


            responseBody _responseBody = new responseBody();


            if (messages != "[]")
            {
                var root = JsonSerializer.Deserialize<List<Message>>(messages);

                _responseBody.messages = root;
            }

            StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\context.txt");

            _responseBody.prompt = prompt;


            var uri = new Uri(urlString);

            var ollama = new OllamaApiClient(uri);


            ollama.SelectedModel = _responseBody.model;

           

            var chat = ollama.Chat(stream => { var ss = stream.Message?.Content ?? ""; } );
            try
            {
                await chat.Send(_responseBody.prompt);
            }
            catch (Exception ex)
            {

                throw;
            }
            

            string responseText = "";

            
          


            return Json(responseText);
        }

        //[HttpPost]
        //public async Task<JsonResult> SendMessage(string prompt, string messages)
        //{
        //    var urlString = configuration.GetSection("urls").GetSection("main").Value + "/api/generate";

        //    HttpClient httpClient = new HttpClient();



        //    responseBody _responseBody = new responseBody();


        //    if (messages != "[]")
        //    {
        //        var root = JsonSerializer.Deserialize<List<Message>>(messages);

        //        _responseBody.messages = root;
        //    }

        //    StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\context.txt");

        //    _responseBody.prompt = prompt + " Отвечай всегда только на русском";

        //    StringContent jsonContent = new(JsonSerializer.Serialize(_responseBody), Encoding.UTF8, "application/json");

        //    var request = new HttpRequestMessage(HttpMethod.Post, urlString);

        //    request.Content = jsonContent;

        //    var response = httpClient.SendAsync(request);

        //    string responseText = "";

        //    try
        //    {



        //        responseText =  await response.Result.Content.ReadAsStringAsync();
        //    }
        //    catch (Exception ex)
        //    {


        //    }



        //    return Json(responseText);
        //}





    }
}
