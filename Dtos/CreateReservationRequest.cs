namespace ReservationManagerAPI2.Dtos
{
	public class CreateReservationRequest
	{
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public string Memo { get; set; } = string.Empty;
	}
}
