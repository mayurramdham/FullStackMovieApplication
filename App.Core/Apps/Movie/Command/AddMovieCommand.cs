using App.Core.Interface;
using AutoMapper;
using Domain.Model;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Apps.Movie.Command
{
    public class AddMovieCommand : IRequest<ResponseDto>
    {
        public MovieDto MovieDto { get; set; }
    }

    public class AddMovieCommandHandler : IRequestHandler<AddMovieCommand, ResponseDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMovieCommandHandler> _logger;

        public AddMovieCommandHandler(IAppDbContext appDbContext, IImageService imageService, IMapper mapper, ILogger<AddMovieCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _imageService = imageService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(AddMovieCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var movieData = request.MovieDto;

                var checkUser = await _appDbContext.Set<Domain.Entity.User>().FirstOrDefaultAsync(m => m.Id == movieData.Id);
                if(checkUser is null)
                {
                    return new ResponseDto
                    {

                        Message = "User not exists with th id",
                      
                    };
                };
               
                var existingMovie = await _appDbContext.Set<Domain.Entity.Movie>()
                                    .FirstOrDefaultAsync(m => m.MovieTitle == movieData.MovieTitle && m.isDeleted == false && m.Id==movieData.Id, cancellationToken);

                if (existingMovie != null)
                {
                    return new ResponseDto
                    {
                        Status = 403,
                        Message = "Movie Already Exists"
                    };
                }
                //if (existingMovie is null)
                //{
                //    return new ResponseDto
                //    {
                //        Status = 404,
                //        Message = "Movie Not Found"
                //    };
                //}


                var moviePoster = await _imageService.Upload(movieData.PosterImage);
                if (moviePoster is ResponseDto uploadResponse && uploadResponse.Status != 200)
                {
                    return uploadResponse;
                }

                string uploadedMoviePosterUrl = moviePoster.Data.ToString();
                var moviesData = _mapper.Map<Domain.Entity.Movie>(movieData); // used automapper to map the data
                moviesData.PosterImage = uploadedMoviePosterUrl;
             //   movieData.MovieLink = movieData.MovieLink;
                await _appDbContext.Set<Domain.Entity.Movie>().AddAsync(moviesData, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);

                return new ResponseDto
                {
                    Status = 200,
                    Message = "Movie Added Successfully",
                    Data = moviesData
                };
            }
            catch (Exception ex)
            {

                {
                    _logger.LogError(ex, "An error occurred while adding the movie with Id: {MovieDto}", request.MovieDto);
                    return new ResponseDto
                    {
                        Status = 500,
                        Message = "An error occurred while adding the movie",
                        Data = ex.Message
                    };
                }
            }
        }
    }
}
