using App.Core.Apps.User.Command;
using App.Core.Interface;
using Domain.Model;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Infrastructure.Service;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    //[Authorize(AuthenticationSchemes = "Firebase")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly IJwtService _jwtService;
        private readonly IMediator _mediator;

        public AuthController(FirebaseAuthService firebaseAuthService, IJwtService jwtService,IMediator mediator)
        {
           _jwtService = jwtService;
            _mediator = mediator;

            // Ensure Firebase app instance is initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("C:\\Users\\mayurramdham\\Desktop\\SmartData\\Assesment\\Assesment_8MovieApplication\\MayurRamdham\\MovieFullStackApplication\\Backend\\Movie_Application\\Backend\\firebase-adminsdk.json")
                });
            }
        }


       
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLoginCommand(Domain.Model.TokenRequest tokenRequest)
        {

            var userLogin = await _mediator.Send(new LoginWithGoogleCommand { TokenRequest = tokenRequest });
            return Ok(userLogin);             
        }

        [HttpPost("microsoft")]
        public async Task<IActionResult> MicrosoftLoginCommand([FromBody] Domain.Model.TokenRequest IdToken)
        {
            var userLogin = await _mediator.Send(new LoginWithMicrosoftCommand { IdToken = IdToken });
            //if(userLogin.Status!=200)
            //{
            //   return BadRequest();
            //}
            return Ok(userLogin);
        }
    }
}
    
