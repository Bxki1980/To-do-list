using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using To_do_list.Data;
using To_do_list.Models;

namespace To_do_list.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DataContext _dataContext;
        private readonly string _jwtSecret;

        public LoginRepository(DataContext context, IConfiguration configuration)
        {
            _dataContext = context;
            _jwtSecret = configuration["Jwt:Key"]; // Retrieve the secret key from appsettings.json
        }

        // Authenticate the user and generate JWT token
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // Retrieve user from the database
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserName == username && u.Password == password);

            // If the user doesn't exist or credentials are incorrect, return null
            if (user == null)
                return null;

            // Generate the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserID", user.ID.ToString()) // Adding custom claim to identify user
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time (1 hour)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "YourIssuer", // Optional: Specify Issuer if needed
                Audience = "YourAudience" // Optional: Specify Audience if needed
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Create a new user account
        public async Task CreateAccountAsync(UserModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password is required");

            if (await _dataContext.Users.AnyAsync(x => x.UserName == user.UserName))
                throw new ArgumentException($"Username '{user.UserName}' is already taken");

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);
            if (user != null)
            {
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _dataContext.Users.OrderBy(u => u.ID).ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int id, UserModel updatedUser)
        {
            var user = await _dataContext.Users.FindAsync(id);
            if (user != null)
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.UserName = updatedUser.UserName;

                if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                {
                    user.Password = updatedUser.Password;
                }

                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
