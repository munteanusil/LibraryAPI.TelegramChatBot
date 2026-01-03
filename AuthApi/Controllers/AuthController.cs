using Google.Apis.Auth;
using Library.Application.Interfaces;
using Library.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.FeatureFilters;
using System.ComponentModel.DataAnnotations;


namespace AuthApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IGoogleService _googleService;
        private readonly GoogleConfigurations _googleConfiguration;

        public AuthController(IGoogleService googleService,
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IOptions<GoogleConfigurations> googleConfiguration)
        {
            _googleService = googleService;
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _googleConfiguration = googleConfiguration.Value;
            
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            var url = _googleService.GetRedirectLink();
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallBack(string code,CancellationToken cancelationToken  )
        {
            GoogleJsonWebSignature.Payload payload;
            var idToken = await _googleService.GetIdTocken(code);
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken,new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleConfiguration.ClientId }
            });

            var user = await _userRepository.GetUserByEmail(payload.Email);
            if(user == null)
            {
                user = new Library.Domain.Entities.User
                {
                    Email = payload.Email,
                };
                await _userRepository.CreateUser(user); 
            }
            var token = _jwtTokenGenerator.GenerateToken(user);
            return Ok(token);
        }
    }
}
