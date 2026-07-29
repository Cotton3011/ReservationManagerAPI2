using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Data;
using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Exceptions;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ReservationManagerAPI2.Services
{
	public class ReservationService
	{
		readonly AppDbContext _context;
		readonly ILogger<ReservationService> _logger;

		public ReservationService(AppDbContext appDbContext,
			ILogger<ReservationService> logger) 
		{
			_context = appDbContext;
			_logger = logger;
		}

		public async Task<Reservation>CreateReservationRequest(
			int userId, 
			CreateReservationRequest request) 
		{
			if (request.EndTime <= request.StartTime)
			{
				throw new BadRequestException("終了日時は開始日時より後にしてください");
			}

			if (request.StartTime <= DateTime.UtcNow) 
			{
				throw new BadRequestException("過去日時は予約不可です");
			}

			//重複確認
			var isOverlap = await _context.Reservations.AnyAsync(r =>
			r.Status == ReservationStatus.Reserved
			&& r.StartTime < request.EndTime
			&& r.EndTime > request.StartTime);
			
			if (isOverlap)
			{
				_logger.LogWarning("重複予約を検出しました UserId: {UserId}, StartTime: {StartTime}, EndTime: {EndTime}",
					userId,
					request.StartTime,
					request.EndTime);

				throw new ConflictException("すでに予約されています");
			}

			var now = DateTime.UtcNow;

			var reservation = new Reservation
			{
				UserId = userId,
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				Memo = request.Memo,
				Status = ReservationStatus.Reserved,
				CreateAt = now,
				UpdateAt = now,
			};

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
			_logger.LogInformation("予約作成 ReservationId: {ReservationId}, UserId: {UserId}",
				reservation.Id,
				userId);

			return reservation;
		}

		public async Task< List<ReservationResponse>>GetReservation(int userId)
		{
			//ログイン中ユーザー本人の予約だけを一覧取得
			return await _context.Reservations.
				Where(r => r.UserId == userId).		//条件に合うデータのみ
				OrderBy(r => r.StartTime).			//予約開始日時順に並べる
				Select(r => new ReservationResponse //DTOに変換
				{
					Id = r.Id,
					StartTime = r.StartTime,
					EndTime = r.EndTime,
					Memo = r.Memo,
					Status = r.Status,
					CreateAt= r.CreateAt,
					UpdateAt= r.UpdateAt,
				}).
				ToListAsync();
		}

		//ログイン中のユーザー自身の予約詳細を取得する
		public async Task<ReservationResponse> GetMyReservationByIdAsync(int userId, int id)
		{
			var reservation = await _context.Reservations
				.Where(r => r.UserId == userId && r.Id == id)
				.Select(r => new ReservationResponse
				{
					Id = r.Id,
					StartTime = r.StartTime,
					EndTime = r.EndTime,
					Memo = r.Memo,
					Status = r.Status,
					CreateAt = r.CreateAt,
					UpdateAt = r.UpdateAt,
				}).FirstOrDefaultAsync();

			if (reservation is null)
			{
				throw new NotFoundException("予約が見つかりません");
			}
			return reservation;
		}

		public async Task CancelMyReservationAsync(int userId, int id)
		{
			var reservation = await _context.Reservations.
				FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

			if (reservation is null)
			{
				throw new NotFoundException("予約がありません");
			}

			if (reservation.Status == ReservationStatus.Canceled)
			{
				throw new ConflictException("キャンセル済みです");
			}

			reservation.Status = ReservationStatus.Canceled;
			reservation.UpdateAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			_logger.LogInformation("ユーザーが予約をキャンセルしました ReservationId: {ReservationId}, UserId: {UserId}",
				reservation.Id,
				userId);
		}

		public async Task<List<AdminReservationResponse>> GetAllReservationsForAdminAsync()
		{
			return await _context.Reservations.
				Include(r => r.User).
				OrderBy(r => r.StartTime).
				Select(r => new AdminReservationResponse
				{
					Id = r.Id,
					UserId = r.UserId,
					UserName = r.User != null ? r.User.UserName : string.Empty,
					StartTime = r.StartTime,
					EndTime = r.EndTime,
					Memo = r.Memo,
					Status = r.Status,
					CreateAt = r.CreateAt,
					UpdateAt = r.UpdateAt
				}).ToListAsync();
		}

		public async Task CancelReservationAsync(int id)
		{
			var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);

			if (reservation is null)
			{
				throw new NotFoundException("予約がありません");
			}

			if (reservation.Status == ReservationStatus.Canceled)
			{
				throw new ConflictException("この予約はすでにキャンセル済み");
			}

			reservation.Status = ReservationStatus.Canceled;
			reservation.UpdateAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			_logger.LogInformation("ユーザーが予約をキャンセルしました ReservationId: {ReservationId}",
				reservation.Id);
		}
	}
}
