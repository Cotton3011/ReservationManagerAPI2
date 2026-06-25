namespace ReservationManagerAPI2.Dtos
{
	public class LoginResponse
	{
		public string Token { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; } = DateTime.UtcNow; //有効期限
	}
}
