using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Services;
using ReservationManagerAPI2.Exceptions;

namespace ReservationManagerAPI2.Tests
{
	public class ReservationServiceTests
	{
		static AppDbContext CreateContext()
		{
			//テストごのに独立したメモリ上DBを作成する
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			return new AppDbContext(options);
		}

		[Fact]
		public async Task CreateReservationRequest_ReservationOverlaps_ReturnsError()
		{
			await using var context = CreateContext();
			var now = DateTime.UtcNow;
			var existingStartTime = now.AddHours(1);

			//すでに確定している予約をDBへ用意する
			context.Reservations.Add(new Reservation
			{
				UserId = 1,
				StartTime = existingStartTime,
				EndTime = existingStartTime.AddHours(1),
				Memo = "既存予約",
				Status = ReservationStatus.Reserved,
				CreateAt = now,
				UpdateAt = now,
			});
			await context.SaveChangesAsync();

			var service = new ReservationService(context);
			var request = new CreateReservationRequest
			{
				//既存予約の途中から始まるため、時間帯が重複する
				StartTime = existingStartTime.AddMinutes(30),
				EndTime = existingStartTime.AddMinutes(90),
				Memo = "重複する予約"
			};

			//別ユーザーであっても、同じ時間帯の予約は作成できない
			var exception = await Assert.ThrowsAsync<ConflictException>(() => service.CreateReservationRequest(2, request));
			Assert.Equal("すでに予約されています", exception.Message);
		}
	}
}
