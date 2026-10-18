using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace HomeHub.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/register", (RegisterRequest request, IUserRepository repository) =>
            {
                var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var password = request.Password ?? string.Empty;

                if (!MailAddress.TryCreate(email, out _) || password.Length < 8)
                {
                    return Results.BadRequest(new
                    {
                        error = "Introduce un correo válido y una contraseña de al menos 8 caracteres."
                    });
                }

                if (repository.GetByEmail(email) is not null)
                    return Results.Conflict();

                var user = new User
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };

                try
                {
                    user.Id = repository.Save(user);
                }
                catch (PostgresException exception) when (exception.SqlState == "23505")
                {
                    return Results.Conflict();
                }

                return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email });
            });

            app.MapPost("/api/auth/login", (LoginRequest request, IUserRepository repository, IConfiguration configuration) =>
            {
                var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var password = request.Password ?? string.Empty;

                var user = repository.GetByEmail(email);
                if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return Results.Unauthorized();

                var response = new AuthResponse
                {
                    Token = CreateToken(user, configuration),
                    Email = user.Email
                };

                return Results.Ok(response);
            });
        }

        private static string CreateToken(User user, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"]!;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
