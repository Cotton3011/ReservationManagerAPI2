using ReservationManagerAPI2.Entities;

namespace ReservationManagerAPI2.Dtos
{
	public class AdminReservationResponse
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; } = string.Empty;
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Memo { get; set; } = string.Empty;
		public ReservationStatus Status { get; set; }
		public DateTime CreateAt { get; set; }
		public DateTime UpdateAt { get; set; }
	}
}
