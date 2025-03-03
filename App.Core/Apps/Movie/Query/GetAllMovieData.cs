using App.Core.Interface;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Apps.Movie.Query
{
    public class GetAllMovieData:IRequest<ResponseDto>
    {
       
    }
    public class GetAllMovieDataHandler:IRequestHandler<GetAllMovieData,ResponseDto>
    {
        private readonly IAppDbContext _appDbContext;
        public GetAllMovieDataHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ResponseDto> Handle(GetAllMovieData request, CancellationToken cancellationToken)
        {
            var getAllMovie = await _appDbContext.Set<Domain.Entity.Movie>()
                                    .Where(m => m.isDeleted == false).ToListAsync();
            return new ResponseDto
            {
                Status = 200,
                Message = "All movie data",
                Data = getAllMovie
            };
            
        }
    }
}
