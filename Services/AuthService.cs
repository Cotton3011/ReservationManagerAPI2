using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Entities;

namespace ReservationManagerAPI2.Services
{
	public class AuthService
	{
		readonly AppDbContext _context;
		readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
		readonly JwtService _jwtService;

		public AuthService(AppDbContext context, JwtService jwtService)
		{
			_context = context;
			_jwtService = jwtService;
		}

		public async Task<bool> Register(RegisterRequest request)
		{
			var existsUser = await _context.Users
				.AnyAsync(u => u.UserName == request.UserName);

			if (existsUser)
			{
				return false;
			}

			var user = new User
			{
				UserName = request.UserName,
				Role = UserRole.User,
			};

			user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<LoginResponse?> LoginAsync(LoginRequest request)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.UserName);

			if (user == null) 
			{
				return null;
			}

			var verifyResult = _passwordHasher.VerifyHashedPassword(
				user,
				user.PasswordHash,
				request.Password
				);
			if (verifyResult == PasswordVerificationResult.Failed)
			{
				return null;
			}
			var tokenResult = _jwtService.CreateToken(user);

			return new LoginResponse
			{
				Token = tokenResult.Token,
				ExpiresAt = tokenResult.ExpiresAt,
			};
		}
	}
}
