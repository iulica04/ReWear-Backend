using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IPasswordHasher passwordHasher;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            this.context = context;
            this.configuration = configuration;
            this.passwordHasher = passwordHasher;
        }
        public async Task<Result<Guid>> AddAsync(User user)
        {
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(user.Id);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while adding the user.", ex);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
           return await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
           return await context.Users.FindAsync(id);
        }
        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public Task UpdateAsync(User user)
        {
            context.Entry(user).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await context.Users
                .AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<LoginResult?> Login(string email, string password)
        {
            var user = context.Users
                .FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            // Check if the password is correct 
            if (!passwordHasher.Verify(user.PasswordHash, password))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResult
            {
                Token = tokenHandler.WriteToken(token),
                UserId = user.Id.ToString(),
            };
        }

        public async Task<LoginResult?> LoginWithGoogle(string email, string googleId)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email && u.GoogleId == googleId);
            if (user == null)
            {
                return null; // utilizatorul nu există
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResult
            {
                Token = tokenHandler.WriteToken(token),
                UserId = user.Id.ToString(),
            };
        }
    }
}
