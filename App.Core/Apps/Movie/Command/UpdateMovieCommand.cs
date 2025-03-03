using App.Core.Interface;
using AutoMapper;
using Domain.Entity;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Apps.Movie.Command
{
    public class UpdateMovieCommand : IRequest<ResponseDto>
    {
        public UpdateMovieDto movieDto { get; set; }
    }

    public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, ResponseDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMovieCommandHandler> _logger;

        public UpdateMovieCommandHandler(IAppDbContext appDbContext, IMapper mapper, IImageService imageService, ILogger<UpdateMovieCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
        {
            try
            {
                

                var updateMovieDto = request.movieDto;

                // Check if the movie exists
                var movieEntity = await _appDbContext.Set<Domain.Entity.Movie>().FindAsync(updateMovieDto.MovieId);
                if (movieEntity == null)
                {
                    _logger.LogWarning("Movie not found with the Id: {MovieId}", updateMovieDto.MovieId);
                    return new ResponseDto
                    {
                        Status = 404,
                        Message = "Movie not found"
                    };
                }

                string imgPathHolding = movieEntity.PosterImage;
               
                // Map the only updateable data
                _mapper.Map(updateMovieDto, movieEntity);
                if(movieEntity.PosterImage is null) movieEntity.PosterImage = imgPathHolding;
                ResponseDto moviePoster = null;
                if (updateMovieDto.PosterImage != null)
                {
                    moviePoster = await _imageService.Upload(updateMovieDto.PosterImage);
                }

                if (moviePoster is ResponseDto uploadResponse && uploadResponse.Status != 200)
                {
                    return uploadResponse;
                }

                string uploadedMoviePosterUrl = moviePoster?.Data?.ToString();

                if (!string.IsNullOrEmpty(uploadedMoviePosterUrl))
                {
                    movieEntity.PosterImage = uploadedMoviePosterUrl;
                }
                else
                {
                    movieEntity.PosterImage = movieEntity.PosterImage;
                }

                //movieEntity.MovieTitle = updateMovieDto.MovieTitle;
                //movieEntity.ReleaseYear = updateMovieDto.ReleaseYear;
                //movieEntity.MovieLink = updateMovieDto?.MovieLink;
              
                _appDbContext.Set<Domain.Entity.Movie>().Update(movieEntity);
                 await _appDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie updated successfully: {MovieId}", updateMovieDto.MovieId);
                return new ResponseDto
                {
                    Status = 200,
                    Message = "Movie updated successfully",
                    Data = movieEntity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the movie with Id: {MovieId}", request.movieDto.MovieId);
                return new ResponseDto
                {
                    Status = 500,
                    Message = "An error occurred while updating the movie",
                    Data = ex.Message
                };
            }
        }
    }
}
