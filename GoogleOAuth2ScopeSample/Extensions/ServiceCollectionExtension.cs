using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;

namespace GoogleOAuth2ScopeSample.Extensions;

public static class ServiceCollectionExtension
{
    public static AuthenticationBuilder AddGoogleScope(this AuthenticationBuilder builder, params string[] scopes)
    {
        builder.AddGoogle(opts =>
        {
            opts.ClientId = "{clientId}";
            opts.ClientSecret = "{ClientSecret}";
            opts.SignInScheme = IdentityConstants.ExternalScheme;
            if (scopes.Any())
                foreach (var scope in scopes)
                    opts.Scope.Add(scope);
            opts.SaveTokens = true;
            opts.AccessType = "offline"; // Request a refresh token
            opts.Events = new OAuthEvents
            {
                OnCreatingTicket = context =>
                {
                    var accessToken = context.AccessToken;
                    var refreshToken = context.RefreshToken;
                    var identity = (ClaimsIdentity)context.Principal.Identity;
                    Console.WriteLine($"accessToken:{accessToken},refreshToken:{refreshToken}");
                    return Task.CompletedTask;
                },
                OnRedirectToAuthorizationEndpoint = context =>
                {
                    context.RedirectUri += "&prompt=consent";
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                }
            };
        });
        return builder;
    }
}