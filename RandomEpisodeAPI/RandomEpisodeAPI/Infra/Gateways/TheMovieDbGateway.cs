using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomEpisodeAPI.Models;

namespace RandomEpisodeAPI.Infra.Gateways
{
    public interface ITheMovieDbGateway
    {
        Task<Result<List<TvShowsResult>, TheMovieDbApiError?>> SearchTvShowByTitleAsync(Title title);
        Task<Result<List<TvShowsSeasonResult>>> GetSeasons(int id);
        Task<Result<EpisodeResult, TheMovieDbApiError?>> GetEpisode(int tvShowId, int season, int episode);
    }

    public class TheMovieDbGateway : ITheMovieDbGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<TmdbApiOptions> _tbmdbApiOptions;

        public TheMovieDbGateway(IHttpClientFactory httpClientFactory, IOptions<TmdbApiOptions> tbmdbApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _tbmdbApiOptions = tbmdbApiOptions;
        }

        public async Task<Result<List<TvShowsResult>, TheMovieDbApiError?>> SearchTvShowByTitleAsync(Title title)
        {
            var query = new Query(_tbmdbApiOptions.Value.ApiKey);
            query.Add("query", title.Value);
            var urlBuilder = new StringBuilder($"{_tbmdbApiOptions.Value.BaseUrl}");
            urlBuilder.Append($"search/tv{query}");

            using (var client = _httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString()))
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
                            var tvShowsResult = JsonConvert.DeserializeObject<List<TvShowsResult>>(resultsJson);
                            return Result.Ok<List<TvShowsResult>, TheMovieDbApiError?>(tvShowsResult);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return Result.Fail<List<TvShowsResult>, TheMovieDbApiError?>(TheMovieDbApiError.GenericError);
                        }
                    }
                }
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Fail<List<TvShowsResult>, TheMovieDbApiError?>(TheMovieDbApiError.NotFound);
                }
                return Result.Fail<List<TvShowsResult>, TheMovieDbApiError?>(TheMovieDbApiError.GenericError);
            }
        }

        public async Task<Result<List<TvShowsSeasonResult>>> GetSeasons(int id)
        {
            var query = new Query(_tbmdbApiOptions.Value.ApiKey);

            var urlBuilder = new StringBuilder($"{_tbmdbApiOptions.Value.BaseUrl}");
            urlBuilder.Append($"tv/{id}");
            urlBuilder.Append($"{query}");

            using (var client = _httpClientFactory.CreateClient())
            using(var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString()))
            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var content = response.Content)
                    {
                        var responseJson = await content.ReadAsStringAsync();
                        var parsedObj = JObject.Parse(responseJson);
                        var resultsJson = parsedObj["seasons"].ToString();
                        return Result.Ok(JsonConvert.DeserializeObject<List<TvShowsSeasonResult>>(resultsJson));
                        //return await GenerateRandomEpisode(tvShowsSeasonsResult, id);
                    }
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Fail<List<TvShowsSeasonResult>, TheMovieDbApiError>(TheMovieDbApiError.NotFound);
                }
                return Result.Fail<List<TvShowsSeasonResult>, TheMovieDbApiError>(TheMovieDbApiError.GenericError);
            }
        }

        public async Task<Result<EpisodeResult, TheMovieDbApiError?>> GetEpisode(int tvShowId, int season, int episode)
        {
            var query = new Query(_tbmdbApiOptions.Value.ApiKey);
            var urlBuilder = new StringBuilder($"{_tbmdbApiOptions.Value.BaseUrl}");
            urlBuilder.Append($"tv/{tvShowId}/");
            urlBuilder.Append($"season/{season}/");
            urlBuilder.Append($"episode/{episode}");
            urlBuilder.Append($"{query}");

            using (var client = _httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString()))
            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var content = response.Content)
                    {
                        var responseJson = await content.ReadAsStringAsync();
                        var episodeResult = JsonConvert.DeserializeObject<EpisodeResult>(responseJson);
                        return Result.Ok<EpisodeResult, TheMovieDbApiError?>(episodeResult);
                    }
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Fail<EpisodeResult, TheMovieDbApiError?>(TheMovieDbApiError.NotFound);
                }
                return Result.Fail<EpisodeResult, TheMovieDbApiError?>(TheMovieDbApiError.GenericError);
            }
        }
    }

    public enum TheMovieDbApiError
    {
        NotFound = 1,
        GenericError
    }
}
