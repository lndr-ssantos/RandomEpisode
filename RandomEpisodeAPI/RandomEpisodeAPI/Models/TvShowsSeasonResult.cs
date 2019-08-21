using Newtonsoft.Json;

namespace RandomEpisodeAPI.Models
{
    public class TvShowsSeasonResult 
    {
        [JsonProperty("episode_count")]
        public int EpisodeCount { get; set; }
        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }
    }
}
