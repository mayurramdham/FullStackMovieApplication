using App.Core.Interface;
using BCrypt.Net;
using Domain.Model;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Apps.User.Command
{
    public class CreateUserCommand : IRequest<object>
    {
        public UserDTO UserDTO { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IAppDbContext appDbContext, ILogger<CreateUserCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<object> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
               

                var user = request.UserDTO;

                var existingUser = await _appDbContext.Set<Domain.Entity.User>().FirstOrDefaultAsync(u => u.UserEmail == user.UserEmail);
                if (existingUser != null)
                {
                    _logger.LogWarning("User already exists with the email: {UserEmail}", user.UserEmail);
                    return new ResponseDto
                    {
                        Status = 404,
                        Message = "User Already Exists"
                    };
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 13);
                var newUser = user.Adapt<Domain.Entity.User>();
                newUser.apiKey = GenerateApiKey();
                await _appDbContext.Set<Domain.Entity.User>().AddAsync(newUser);  //adding the user
                await _appDbContext.SaveChangesAsync(cancellationToken);  //saving the user

               
                return new ResponseDto
                {
                    Status = 200,
                    Message = "User Added Successfully",
                    Data = newUser
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user with email: {UserEmail}", request.UserDTO.UserEmail);
                return new ResponseDto
                {
                    Status = 500,
                    Message = "An error occurred while creating the user",
                    Data = ex.Message
                };
            }
        }

        public static string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
