using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RandomEpisodeAPI.Infra.Gateways;
using RandomEpisodeAPI.Models;
using RandomEpisodeAPI.Models.TvShows;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace RandomEpisodeAPI.Controllers
{
    [Route("/api/v1")]
    [ApiController]
    public class TvShowsController : ControllerBase
    {
        private readonly ITheMovieDbGateway _theMovieDbGateway;

        public TvShowsController(ITheMovieDbGateway theMovieDbGateway)
        {

            _theMovieDbGateway = theMovieDbGateway;
        }

        [HttpGet("tvshows")]
        [ProducesResponseType(typeof(List<TvShowsResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTvShowsByName([FromQuery][Required] string title)
        {
            var titleResult = Title.Create(title);

            if (titleResult.IsFailure)
            {
                ModelState.AddModelError(nameof(title), titleResult.Error);
            }

            if (ModelState.IsValid)
            {
                var result = await _theMovieDbGateway.SearchTvShowByTitleAsync(titleResult.Value);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                //If there are many errors, maybe it's a good idea to create a Map
                if (result.Error.Value == TheMovieDbApiError.NotFound)
                {
                    return NotFound();
                }

                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Erro inesperado"));
            }

            return BadRequest(ModelState);
        }

        [HttpGet("tvshows/{id}/random-episode")]
        public async Task<IActionResult> GetRandomEpisode([Required]int id)
        {
            var seasonResult = await _theMovieDbGateway.GetSeasons(id);

            if (seasonResult.IsFailure)
            {
                return BadRequest();
            }

            var (season, episode) = Pick(seasonResult.Value);

            var episodeResult = await _theMovieDbGateway.GetEpisode(id, season, episode);

            if (episodeResult.IsSuccess)
            {
                return Ok(episodeResult.Value);
            }

            //If there are many errors, maybe it's a good idea to create a Map
            if (episodeResult.Error.Value == TheMovieDbApiError.NotFound)
            {
                return NotFound();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Erro inesperado"));
        }

        private (int,int) Pick(List<TvShowsSeasonResult> tvShowsSeasonsResult)
        {
            int indexNumber, episodeNumber = 0;
            bool loop = true;
            do
            {
                var indexPicker = new NumberPicker(tvShowsSeasonsResult.Count);
                indexNumber = indexPicker.Value - 1; 
                if (tvShowsSeasonsResult[indexNumber].EpisodeCount > 0)
                {
                    var episodePicker = new NumberPicker(tvShowsSeasonsResult[indexNumber].EpisodeCount);
                    episodeNumber = episodePicker.Value;
                    loop = false;
                }
            } while (loop);
            return (indexNumber, episodeNumber);
        }
    }
}
