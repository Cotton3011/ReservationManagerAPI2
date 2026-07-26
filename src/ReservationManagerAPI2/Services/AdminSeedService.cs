using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Entities;

namespace ReservationManagerAPI2.Services
{
	public class AdminSeedService
	{
		readonly AppDbContext _context;
		readonly IConfiguration _configuration;
		readonly PasswordHasher<User> _passwordHasher = new();

		public AdminSeedService(AppDbContext appDbContext, IConfiguration configuration) 
		{
			_context = appDbContext;	
			_configuration = configuration;
		}

		public async Task SeedAsync()
		{
			var existsAdmin = await _context.Users.
				AnyAsync(u => u.Role == UserRole.Admin);

			if (existsAdmin)
			{
				return;
			}

			var userName = _configuration["AdminUser:UserName"];
			var password = _configuration["AdminUser:Password"];

			if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
			{
				return;
			}

			var adminUser = new User()
			{
				UserName = userName,
				Role = UserRole.Admin,
				CreateTime = DateTime.UtcNow,
			};

			adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, password);

			_context.Users.Add(adminUser);
			await _context.SaveChangesAsync();
		}
	}
}
