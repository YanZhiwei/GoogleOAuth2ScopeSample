using System.Security.Claims;
using GoogleOAuth2ScopeSample.Entities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoogleOAuth2ScopeSample.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(SignInManager<AppUser> signinMgr, UserManager<AppUser> userManager)
    {
        _signInManager = signinMgr;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("GoogleLogin")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
        return new ChallengeResult("Google", properties);
    }

    [AllowAnonymous]
    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return BadRequest();

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        if (result.Succeeded) return Ok();

        var user = new AppUser
        {
            Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
            UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
        };
        var identResult = await _userManager.CreateAsync(user);
        if (identResult.Succeeded)
        {
            identResult = await _userManager.AddLoginAsync(user, info);
            if (identResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }
        }

        return Forbid();
    }
}