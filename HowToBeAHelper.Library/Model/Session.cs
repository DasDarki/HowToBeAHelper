using Newtonsoft.Json;

namespace HowToBeAHelper.Model
{
    public class Session
    {
        [JsonProperty("session")]
        public string ID { get; set; }

        [JsonProperty("sessionName")]
        public string Name { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("user")]
        public string UserID { get; set; }

        [JsonProperty("isPlayer")]
        public bool IsPlayer { get; set; }
    }
}
