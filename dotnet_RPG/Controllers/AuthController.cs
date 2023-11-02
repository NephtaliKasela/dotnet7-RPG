using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_RPG.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_RPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _AuthRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _AuthRepo = authRepo;
            
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO request)
        {
            var response = await _AuthRepo.Register(
                new User {Username = request.username}, request.password
            );
            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO request)
        {
            var response = await _AuthRepo.Login(request.username, request.password);
            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}