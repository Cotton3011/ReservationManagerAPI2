using System.ComponentModel.DataAnnotations;

namespace ReservationManagerAPI2.Dtos
{
	public class RegisterRequest
	{
		[Required]
		public string UserName { get; set; } = string.Empty;
		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;
	}
}
