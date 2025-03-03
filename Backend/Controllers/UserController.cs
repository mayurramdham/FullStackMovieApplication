
using App.Core.Apps.User.Command;
using App.Core.Apps.User.Query;
using Domain.Model;
using Domain.Model.ValidationDTO;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
   // [Authorize(AuthenticationSchemes = "CustomAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> AddUser(UserDTO userDTO)
        {
            var test = await Task.FromResult(0);

            _logger.LogInformation("Get Method");
            var validator = new UserDtoValidator();
            var result = validator.Validate(userDTO);

            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for adding the user: {@UserDTO}", userDTO);
                return BadRequest(new {Errors=result.Errors.Select(x=>x.ErrorMessage).ToList() });
            }
            var register = await _mediator.Send(new CreateUserCommand { UserDTO = userDTO });
            return Ok(register);

          }

        [HttpPost("/loginUser")]
        public async Task<IActionResult> LoginAgent(LoginDTO model)
        {
            var test = await Task.FromResult(0);

            var validator = new LoginDtoValidator();
            var result = validator.Validate(model);

            if(!result.IsValid)
            {
                _logger.LogWarning("Validation failed for login the user: {@LoginDTO}", model);
                return BadRequest(new { Errors = result.Errors.Select(x => x.ErrorMessage).ToList() });
            }
            var userLogin = await _mediator.Send(new UserLoginQuery { loginDTO = model });
            return Ok(userLogin);
        }

        [HttpGet("GetAllUser")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllUser()
        {
            var allUser = await _mediator.Send(new getAllUserQuery());
            return Ok(allUser);
        }


    }
}
