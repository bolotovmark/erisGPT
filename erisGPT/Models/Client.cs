using Microsoft.AspNetCore.SignalR;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erisGPT.Models
{
    public class Client
    {
        public Guid Uid { get; set; }
        public OllamaApiClient ApIClient { get; set; }
        public Chat Chat { get; set; }
        public IHubCallerClients Clients { get; set; }

        public DateTime LastTime { get; set; } = DateTime.Now;
    }
}
