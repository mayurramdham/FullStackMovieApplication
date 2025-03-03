using App.Core.Interface;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Apps.User.Query
{
    public class getAllUserQuery : IRequest<object>
    {
    }
    public class getAllUserQueryHandler : IRequestHandler<getAllUserQuery, object>
    {
        private readonly IAppDbContext _appDbContext;
        public getAllUserQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> Handle(getAllUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allUser = await _appDbContext.Set<Domain.Entity.User>().ToListAsync(cancellationToken);
                return new ResponseDto
                {
                    Status = 200,
                    Message = "All user data",
                    Data = allUser
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Status = 500,
                    Message = ex.Message,
                };

            }
        }
    }
}
