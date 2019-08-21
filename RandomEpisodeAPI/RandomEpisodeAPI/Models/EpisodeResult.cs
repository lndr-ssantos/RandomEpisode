using Newtonsoft.Json;

namespace RandomEpisodeAPI.Models
{
    public class EpisodeResult 
    {
        public string Name { get; set; }
        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }
        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }
        public string Overview { get; set; }
    }
}
