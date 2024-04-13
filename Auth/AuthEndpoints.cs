using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using O9d.AspNet.FluentValidation;
using WebApi.Auth.Entity;
using WebApi.Data.Dtos.Auth;

namespace WebApi.Auth;

public static class AuthEndpoints
{
    public static void AddAuthApi(this WebApplication app)
    {
        RouteGroupBuilder authGroup = app.MapGroup("/api/auth").WithValidationFilter().WithTags("Auth");
        authGroup.MapPost("register", async ([Validate] RegisterDto registerDto, UserManager<User> userManager, JwtTokenService jwtTokenService) =>
        {
            User? user = await userManager.FindByEmailAsync(registerDto.Email);
            if (user is not null) return Results.UnprocessableEntity("User already exists");

            User newUser = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.Username,
            };
            IdentityResult result = await userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded) return Results.UnprocessableEntity(result.Errors.Select(x => x.Description));

            await userManager.AddToRoleAsync(newUser, UserRoles.BasicUser);

            IList<string> roles = await userManager.GetRolesAsync(newUser);
            string accessToken = jwtTokenService.CreateAccessToken(newUser, roles);
            string refreshToken = jwtTokenService.CreateRefreshToken(newUser.Id);

            newUser.RefreshToken = refreshToken;
            await userManager.UpdateAsync(newUser);

            return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken));
        }).WithName("Register");

        authGroup.MapPost("login", async (UserManager<User> userManager, JwtTokenService jwtTokenService, [Validate] LoginDto loginDto) =>
        {
            User? user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return Results.UnprocessableEntity("Email or password was incorrect");

            bool isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid) return Results.UnprocessableEntity("Username or password was incorrect");

            IList<string> roles = await userManager.GetRolesAsync(user);
            string accessToken = jwtTokenService.CreateAccessToken(user, roles);
            string refreshToken = jwtTokenService.CreateRefreshToken(user.Id);

            user.RefreshToken = refreshToken;
            await userManager.UpdateAsync(user);

            return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken));
        }).WithName("Login");

        authGroup.MapPost("refresh-token", async (UserManager<User> userManager, JwtTokenService jwtTokenService, [Validate] RefreshAccessTokenDto refreshAccessTokenDto) =>
        {
            if (!jwtTokenService.TryParseRefreshToken(refreshAccessTokenDto.RefreshToken, out ClaimsPrincipal? claims))
            {
                return Results.UnprocessableEntity();
            }

            string? userId = claims!.FindFirstValue(JwtRegisteredClaimNames.Sub);
            User? user = await userManager.FindByIdAsync(userId);
            if (user is null || user.RefreshToken is null || user.RefreshToken != refreshAccessTokenDto.RefreshToken)
            {
                return Results.UnprocessableEntity("Invalid token or user not logged in");
            }

            IList<string> roles = await userManager.GetRolesAsync(user);
            string accessToken = jwtTokenService.CreateAccessToken(user, roles);
            string refreshToken = jwtTokenService.CreateRefreshToken(user.Id);

            user.RefreshToken = refreshToken;
            await userManager.UpdateAsync(user);

            return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken));
        }).WithName("RefreshAccessToken");

        authGroup.MapPost("logout", async (UserManager<User> userManager, [Validate] LogoutDto logoutDto) =>
        {
            User? user = await userManager.FindByNameAsync(logoutDto.Username);
            if (user is null) return Results.UnprocessableEntity("User not found");

            user.RefreshToken = null;
            await userManager.UpdateAsync(user);

            return Results.Ok("Logout successful");
        }).WithName("Logout");
    }
}