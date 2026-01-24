using EventPhotographer.App.Users.Dto;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.App.Users.Mappers;
using EventPhotographer.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Users.Controllers;

public class AuthController : ApiController
{
    private readonly UserManager<User> _userManager;

    private readonly SignInManager<User> _signInManager;

    public AuthController(
        AppDbContext context,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Route("login")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserLoginResponseDto>> Login(
        [FromBody]LoginRequestDto loginRequest,
        [FromServices]IValidator<LoginRequestDto> validator)
    {
        await validator.ValidateAndThrowAsync(loginRequest);

        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null
            || !await _signInManager.CanSignInAsync(user))
        {
            return Unauthorized();
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            loginRequest.Password,
            loginRequest.RememberMe,
            false
        );

        if (!signInResult.Succeeded)
        {
            return Unauthorized();
        }

        return Ok(UserMapper.ToLoginResponseDto(user));
    }

    [Route("register")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserLoginResponseDto>> Register(
        [FromBody]RegisterRequestDto request,
        [FromServices]IValidator<RegisterRequestDto> validator)
    {
        await validator.ValidateAndThrowAsync(request);

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _signInManager.SignInAsync(user, false);

        return Ok(UserMapper.ToLoginResponseDto(user));
    }

    [Route("logout")]
    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return Ok();
    }

    [Route("user")]
    [HttpGet]
    public async Task<ActionResult<UserLoginResponseDto>> GetUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(UserMapper.ToLoginResponseDto(user));
    }
}
