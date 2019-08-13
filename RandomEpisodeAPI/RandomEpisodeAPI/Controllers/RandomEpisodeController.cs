using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomEpisodeAPI.Infra;
using RandomEpisodeAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RandomEpisodeAPI.Controllers
{
    [Route("[controller]/api/v1")]
    [ApiController]
    public class RandomEpisodeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<TmdbApiOptions> _tbmdbApiOptions;

        public RandomEpisodeController(IHttpClientFactory httpClientFactory, IOptions<TmdbApiOptions> tbmdbApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _tbmdbApiOptions = tbmdbApiOptions;
        }

        [HttpGet("tvshows")]
        [ProducesResponseType(typeof(List<GetTvShowsResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTvShowsByName([FromQuery] string title)
        {
            var urlBuilder = new StringBuilder($"{_tbmdbApiOptions.Value.BaseUrl}");
            urlBuilder.Append($"search/tv?api_key={_tbmdbApiOptions.Value.ApiKey}");
            urlBuilder.Append($"&query={title}");

            var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString());
            using (var client = _httpClientFactory.CreateClient())
            {
                using (var response = await client.SendAsync(request))
                 {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            try
                            {
                                var responseJson = await content.ReadAsStringAsync();
                                var parsedObj = JObject.Parse(responseJson);
                                var resultsJson = parsedObj["results"].ToString();
                                var tvShowsResult = JsonConvert.DeserializeObject<List<GetTvShowsResult>>(resultsJson);

                                return Ok(tvShowsResult);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                return BadRequest();
                            }
                        }
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
        }
    }
}
