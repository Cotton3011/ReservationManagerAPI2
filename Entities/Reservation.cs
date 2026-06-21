namespace ReservationManagerAPI2.Entities
{
	public enum ReservationStatus
	{
		Reserved = 0,
		Cancelled = 1,
	}
	public class Reservation
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Memo { get; set; } = string.Empty;
		public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;
		public DateTime CreateAt { get; set; }
		public DateTime UpdateAt { get; set; }
		public User? User { get; set; }
	}
}
