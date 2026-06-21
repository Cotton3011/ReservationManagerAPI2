using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;

namespace ReservationManagerAPI2.Entities
{
	public enum UserRole
	{
		User = 0,
		Admin = 1,
	}

	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public UserRole Role { get; set; } = UserRole.User;
		public DateTime CreateTime { get; set; } = DateTime.UtcNow;
	}
}
