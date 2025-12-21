using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
           _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallBack()
        {
            return Ok();
        }
    }
}
