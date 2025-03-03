using System;
using System.IdentityModel.Tokens.Jwt;
using App.Core.Interface;
using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.Core.Apps.User.Command
{
  


    // Our command that contains the ID token from the client
    public class LoginWithMicrosoftCommand : IRequest<ResponseDto>
    {
        public Domain.Model.TokenRequest IdToken { get; set; }
    }

    // The handler for the login command
    public class LoginWithMicrosoftCommandHandler : IRequestHandler<LoginWithMicrosoftCommand, ResponseDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly string _audience;
        // _issuer is used mainly for informational purposes; note that in multi-tenant apps, the token's issuer is dynamic.
        private readonly HttpClient _httpClient;

        public LoginWithMicrosoftCommandHandler(IAppDbContext appDbContext, IJwtService jwtService, IConfiguration configuration, HttpClient httpClient)
        {
            _appDbContext = appDbContext;
            _jwtService = jwtService;
            _configuration = configuration;
            _audience = _configuration["AzureJwt:Audience"];
            _httpClient = httpClient;
        }

        public async Task<ResponseDto> Handle(LoginWithMicrosoftCommand request, CancellationToken cancellationToken)
        {
            var token = request.IdToken.IdToken;

            if (string.IsNullOrWhiteSpace(token))
            {
                return new ResponseDto
                {
                    Status = 400,
                    Message = "Invalid token"
                };
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Read the token as a JwtSecurityToken
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jsonToken == null)
                {
                    return new ResponseDto
                    {
                        Status = 400,
                        Message = "Invalid token format"
                    };
                }

                // Extract the Key ID (kid) from the token header
                var kid = jsonToken.Header.Kid;
                // Extract the issuer from the token (e.g., "https://login.microsoftonline.com/{tenantId}/v2.0")
                var tokenIssuer = jsonToken.Issuer;

                // Dynamically resolve the JWKS endpoint based on the issuer.
                var jwksUrl = GetJwksUrlFromIssuer(tokenIssuer);
                var keys = await GetPublicKeys(jwksUrl);

                // Find the public key that matches the token's 'kid'
                var key = keys.Keys.FirstOrDefault(k => k.KeyId == kid);
                if (key == null)
                {
                    return new ResponseDto
                    {
                        Status = 400,
                        Message = "No matching key is found"
                    };
                }

                // Set up token validation parameters.
                // In a multi-tenant scenario, token issuer will vary.
                // You can choose to disable strict issuer validation or implement a custom IssuerValidator.
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = _audience,
                    // For multi-tenant, you might relax issuer validation:
                    ValidateIssuer = false,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Validate the token. This will throw an exception if validation fails.
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                // Extract claims
                var name = principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var email = principal.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new ResponseDto
                    {
                        Status = 400,
                        Message = "Invalid email address"
                    };
                }

                // Check if the user exists in your DB; if not, create a new user.
                var existingUser = await _appDbContext.Set<Domain.Entity.User>()
                                        .FirstOrDefaultAsync(u => u.UserEmail == email, cancellationToken);
                if (existingUser == null)
                {
                    existingUser = new Domain.Entity.User
                    {
                        UserName = name,
                        UserEmail = email,
                        apiKey = GenerateApiKey(),
                        RoleId = 2, // Adjust role ID as needed.
                    };
                    await _appDbContext.Set<Domain.Entity.User>().AddAsync(existingUser, cancellationToken);
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                }

                // Generate an access token using your custom JWT service.
                var accessToken = await _jwtService.GenerateToken(existingUser.Id, existingUser.UserName, existingUser.UserEmail, "User", existingUser.apiKey);

                return new ResponseDto
                {
                    Status = 200,
                    Message = "Token is valid",
                    Data = new { Name = name, Email = email, AccessToken = accessToken }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new ResponseDto
                {
                    Status = 500,
                    Message = $"Token validation failed: {ex.Message}"
                };
            }
        }

        // Dynamically fetch the JWKS from the appropriate tenant based on the token's issuer.
        private async Task<JsonWebKeySet> GetPublicKeys(string jwksUrl)
        {
            var response = await _httpClient.GetStringAsync(jwksUrl);
            return new JsonWebKeySet(response);
        }

        // Parse the issuer to extract the tenant ID and build the JWKS URL.
        private string GetJwksUrlFromIssuer(string issuer)
        {
            // Expecting issuer in the format: "https://login.microsoftonline.com/{tenantId}/v2.0"
            if (Uri.TryCreate(issuer, UriKind.Absolute, out var issuerUri))
            {
                // issuerUri.Segments: ["/", "{tenantId}/", "v2.0"]
                if (issuerUri.Segments.Length >= 2)
                {
                    var tenantId = issuerUri.Segments[1].TrimEnd('/');
                    return $"https://login.microsoftonline.com/{tenantId}/discovery/v2.0/keys";
                }
            }
            // Fallback to common if unable to parse.
            return "https://login.microsoftonline.com/common/discovery/v2.0/keys";
        }

        private static string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
