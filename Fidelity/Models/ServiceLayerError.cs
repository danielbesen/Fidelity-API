using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Models
{
    public class ServiceLayerError
    {
        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}