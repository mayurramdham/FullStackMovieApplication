using App.Core.Interface;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Apps.Movie.Command
{
    public class DeleteMovieCommand : IRequest<ResponseDto>
    {
        public int MovieId { get; set; }
    }

    public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, ResponseDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly ILogger<DeleteMovieCommandHandler> _logger;

        public DeleteMovieCommandHandler(IAppDbContext appDbContext, ILogger<DeleteMovieCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
        {
            try
            {
               

                var deleteMovieData = await _appDbContext.Set<Domain.Entity.Movie>().FindAsync(request.MovieId);
                if (deleteMovieData is null)
                {
                    _logger.LogWarning("Movie not found with the Id: {MovieId}", request.MovieId);
                    return new ResponseDto
                    {
                        Status = 404,
                        Message = "Movie not found with the Id"
                    };
                }

                deleteMovieData.isDeleted = true;
                await _appDbContext.SaveChangesAsync(cancellationToken);

                return new ResponseDto
                {
                    Status = 200,
                    Message = "Movie Deleted Successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the movie with Id: {MovieId}", request.MovieId);
                return new ResponseDto
                {
                    Status = 500,
                    Message = "An error occurred while deleting the movie",
                    Data = ex.Message
                };
            }
        }
    }
}
