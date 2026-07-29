using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Services;
using ReservationManagerAPI2.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReservationManagerAPI2.Tests
{
	public class ReservationServiceTests
	{
		static AppDbContext CreateContext()
		{
			//テストごとに独立したメモリ上DBを作成する
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			return new AppDbContext(options);
		}

		/// <summary>
		/// 既存予約と時間帯が重なる予約を作ろうとした場合
		/// </summary>
		/// <returns></returns>
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

			var service = new ReservationService(
				context, 
				NullLogger<ReservationService>.Instance);
			var request = new CreateReservationRequest
			{
				//既存予約の途中から始まるため、時間帯が重複する
				StartTime = existingStartTime.AddMinutes(30),
				EndTime = existingStartTime.AddMinutes(90),
				Memo = "重複する予約"
			};

			//別ユーザーであっても、同じ時間帯の予約は作成できない
			var exception = await Assert.ThrowsAsync<ConflictException>(
				() => service.CreateReservationRequest(2, request));

			Assert.Equal("すでに予約されています", exception.Message);
		}

		/// <summary>
		/// 存在しない自分の予約詳細を取得した場合
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task GetMyreservationByIdAsync_ReservationNotExist_ThrowsNotFoundException()
		{
			await using var context = CreateContext();
			var service = new ReservationService(
				context,
				NullLogger<ReservationService>.Instance);

			//存在しない予約IDでは404用の業務例外が発生する
			var exception = await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetMyReservationByIdAsync(userId: 1, id: 999));
		}

		/// <summary>
		/// 存在しない自分の予約をキャンセルした場合
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task CancelMyReservationAsync_ReservationDoesNotExist_ThrowsNotFoundException()
		{
			await using var context = CreateContext();
			var service = new ReservationService (
				context,
				NullLogger<ReservationService>.Instance);

			//自分の予約がなければ404
			var exception = await Assert.ThrowsAsync <NotFoundException>(
				() => service.CancelMyReservationAsync(userId: 1, id: 999));
			Assert.Equal("予約がありません", exception.Message);
		}

		/// <summary>
		/// キャンセル済みの予約を管理者が再度キャンセルした場合
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task CancelReservationAsync_ReservationIsCanceled_ThrowsConflictException()
		{
			await using var context = CreateContext();
			var now = DateTime.UtcNow;

			context.Reservations.Add(new Reservation
			{
				UserId = 1,
				StartTime = now.AddHours(1),
				EndTime = now.AddHours(2),
				Memo = "キャンセル済み予約",
				Status = ReservationStatus.Canceled,
				CreateAt = now,
				UpdateAt = now,
			});
			await context.SaveChangesAsync();

			var service = new ReservationService(
				context,
				NullLogger<ReservationService>.Instance);

			//キャンセル済み予約を再度キャンセルすると409用の業務例外が発生する
			var exception = await Assert.ThrowsAsync<ConflictException>(
				() => service.CancelReservationAsync(id: 1));

			Assert.Equal("この予約はすでにキャンセル済み", exception.Message);
		}
	}
}
