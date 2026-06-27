using ReservationManagerAPI2.Entities;

namespace ReservationManagerAPI2.Dtos
{
	public class ReservationResponse
	{
		public int Id { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Memo { get; set; } = string.Empty;
		public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;
		public DateTime CreateAt { get; set; }
		public DateTime UpdateAt { get; set; }
	}
}
