using App.Core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Apps.Movie.Query
{
    public class GetMovieByAdminIdQuery : IRequest<object>
    {
        public int Id;
    }
    public class GetMovieByAdminIdQueryHandler : IRequestHandler<GetMovieByAdminIdQuery, object>
    {
        private readonly IAppDbContext _appDbContext;
        public GetMovieByAdminIdQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<object> Handle(GetMovieByAdminIdQuery request, CancellationToken cancellationToken)
        {
            var loginId = request.Id;

            var movie = await _appDbContext.Set<Domain.Entity.Movie>().Where(m => m.Id == loginId && m.isDeleted == false).ToListAsync();
            if (movie is null)
            {
                return new
                {
                    status = 404,
                    message = "movie not found with this Id"
                };
            }
            var response = new
            {
                status = 200,
                movieData = movie,
                message = "All movie data by userId"
            };
            return response;
        }
    }
}
