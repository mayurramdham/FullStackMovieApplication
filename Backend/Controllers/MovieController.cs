using App.Core.Apps.Movie.Command;
using App.Core.Apps.Movie.Query;
using Domain.Entity;
using Domain.Model;
using Domain.Model.ValidationDTO;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
  //  [Authorize(AuthenticationSchemes = "CustomAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MovieController> _logger;
        public MovieController(IMediator mediator, ILogger<MovieController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("addMovie")]
        [Authorize(Roles="Admin", Policy = "CustomAuthPolicy")]
        public async Task<IActionResult> AddMovies(MovieDto movieDto)
        {
         
             var validator = new MovieDtoValidator();
             var result = validator.Validate(movieDto);

            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for MovieDto: {@MovieDto}", movieDto);
                return BadRequest(new { Errors = result.Errors.Select(x => x.ErrorMessage).ToList() });
            }
            var addMovie = await _mediator.Send(new AddMovieCommand { MovieDto = movieDto });
            return Ok(addMovie);

        }
        [HttpPut("updateMovie")]
        [Authorize(Roles = "Admin", Policy = "CustomAuthPolicy")]
        public async Task<IActionResult> updateMovies([FromForm] UpdateMovieDto updateMovieDto)
        {
            var validator = new UpdateMovieValidator();
            var result = validator.Validate(updateMovieDto);

            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateMovieDto: {@UpdateMovieDto}", updateMovieDto);
                return BadRequest(new { Errors = result.Errors.Select(x => x.ErrorMessage).ToList() });
            }

            var updateMovie = await _mediator.Send(new UpdateMovieCommand { movieDto = updateMovieDto });
            return Ok(updateMovie);
        }

        [HttpDelete("deleteMovie")]
        [Authorize(Roles = "Admin", Policy = "CustomAuthPolicy")]
        public async Task<IActionResult> deleteMovies(int movieId)
        {
            var deleteMovie = await _mediator.Send(new DeleteMovieCommand { MovieId = movieId });
            _logger.LogInformation("Movie deleted successfully: {MovieId}", movieId);
            return Ok(deleteMovie);
        }
   
        [HttpGet("getAllMovie")]
        public async Task<IActionResult> getAllMovies([FromQuery] string s, [FromQuery] string apikey, [FromQuery] int UserId)
        {            
            // Call the query with the search string
            var allMovie = await _mediator.Send(new GetAllMovieQuery { s = s, apiKey=apikey,UserId=UserId });
            return Ok(allMovie);
        }

        [HttpGet("getAllMovieData")]
        public async Task<IActionResult> getAllMoviesData()
        {
            var totalmovie = await _mediator.Send(new GetAllMovieData {  });
            return Ok(totalmovie);
        }
       
        [HttpGet("AllMovieBydId/{Id}")]
        [Authorize(Roles = "Admin", Policy = "CustomAuthPolicy")]
    
        public async Task<IActionResult> getAllMovieBydId(int Id)
        {
            var allMovies=await _mediator.Send(new GetMovieByAdminIdQuery {Id=Id });
            return Ok(allMovies);
        }

    }
}
