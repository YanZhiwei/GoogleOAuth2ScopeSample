using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using AuthenticationOptions = GoogleOAuth2ScopeSample.Configuration.AuthenticationOptions;

namespace GoogleOAuth2ScopeSample.Extensions;

public static class ServiceCollectionExtension
{
    public static AuthenticationBuilder AddGoogleScope(this AuthenticationBuilder builder,
        IConfigurationSection googleSection, params string[] scopes)
    {
        if (googleSection == null)
            throw new ArgumentNullException(nameof(googleSection));
        var authenticationConfig = googleSection.Get<AuthenticationOptions>();
        if (authenticationConfig == null)
            throw new ArgumentNullException(nameof(authenticationConfig));
        if (authenticationConfig.Google == null)
            throw new ArgumentNullException(nameof(authenticationConfig.Google));
        builder.AddGoogle(opts =>
        {
            opts.ClientId = authenticationConfig.Google.ClientId;
            opts.ClientSecret = authenticationConfig.Google.ClientSecret;
            opts.SignInScheme = IdentityConstants.ExternalScheme;
            if (scopes?.Any() ?? false)
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