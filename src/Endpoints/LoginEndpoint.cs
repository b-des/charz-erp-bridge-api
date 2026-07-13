using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;

namespace CharzPiexApi.Endpoints;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, DateTime Expiry);

public class LoginEndpoint(IConfiguration config) : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Mock Authentication Check (Replace this with database validation)
        if (req.Username != "" || req.Password != "")
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Create User Claims (Identity data baked into the token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, req.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "Admin"),
            new Claim("CustomUserAttribute", "GoldMember")
        };

        // Generate the Crypto Security Key
        var jwtSettings = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.UtcNow.AddDays(12);

        // Create and Write the Token Structure
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Return the Token to the client
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = jwtSettings["Key"]!;
            o.ExpireAt = expiry;
            o.User.Roles.Add("Manager", "Auditor");
            o.User.Claims.Add(("UserName", req.Username));
            o.User["UserId"] = "001"; //indexer based claim setting
        });

        // await Send.OkAsync(
        //     new
        //     {
        //         req.Username,
        //         Token = jwtToken
        //     });
        await Send.OkAsync(new LoginResponse(tokenString, expiry), ct);
    }
}