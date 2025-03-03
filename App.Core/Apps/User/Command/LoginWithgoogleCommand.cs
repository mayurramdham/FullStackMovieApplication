using System.IdentityModel.Tokens.Jwt;
using App.Core.Interface;
using Domain.Model;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class LoginWithGoogleCommand : IRequest<ResponseDto>
{
    public TokenRequest TokenRequest { get; set; }
}

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, ResponseDto>
{
    private readonly IAppDbContext _appDbContext;
    private readonly IJwtService _jwtService;

    public LoginWithGoogleCommandHandler(IAppDbContext appDbContext, IJwtService jwtService)
    {
        _appDbContext = appDbContext;
        _jwtService = jwtService;
    }

    public async Task<ResponseDto> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var firebaseLogin = request.TokenRequest;

        if (string.IsNullOrWhiteSpace(firebaseLogin.IdToken))
        {
            return new ResponseDto
            {
                Status = 404,
                Message = "Invalid token"
            };
        }

        try
        {
            // Decode the token and get claims
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseLogin.IdToken);
            var claims = decodedToken.Claims;

            var userId = decodedToken.Uid;
            var name = claims.GetValueOrDefault("name", "Unknown").ToString();
            var email = claims.GetValueOrDefault("email", null)?.ToString();
            var pictureUrl = claims.GetValueOrDefault("picture", null)?.ToString();

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseDto
                {
                    Message = "Invalid token, email missing",
                    Status = 400
                };
            }

            // Check if the user already exists in the database
            var existingUser = await _appDbContext.Set<Domain.Entity.User>()
                                                  .FirstOrDefaultAsync(u => u.UserEmail == email);
            if (existingUser == null)
            {
                existingUser = new Domain.Entity.User
                {
                    UserEmail = email,
                    UserName = name,
                    apiKey = GenerateApiKey(),
                    RoleId=2
                };

                await _appDbContext.Set<Domain.Entity.User>().AddAsync(existingUser);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }

            var accessToken = await _jwtService.GenerateToken(existingUser.Id, existingUser.UserName, existingUser.UserName, "User", existingUser.apiKey);

            return new ResponseDto
            {
                Status = 200,
                Message = "Login successful",
                Data = accessToken
            };
        }
        catch (FirebaseAuthException ex)
        {
            return new ResponseDto
            {
                Status = 400,
                Message = $"Firebase token verification failed: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                Status = 500,
                Message = ex.Message
            };
        }
    }

        private static string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }
}
