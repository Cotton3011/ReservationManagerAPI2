using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ReservationManagerAPI2.Services
{
	public class ReservationService
	{
		readonly AppDbContext _context;
		public ReservationService(AppDbContext appDbContext) 
		{
			_context = appDbContext;
		}

		public async Task<(bool Success, string? ErrorMessage, Reservation? Reservation)>
			CreateReservationRequest(int userId, CreateReservationRequest request) 
		{
			if (request.EndTime <= request.StartTime)
			{
				return (false, "終了日時は開始日時より後にしてください", null);
			}

			if (request.StartTime <= DateTime.UtcNow) 
			{
				return (false, "過去日時は予約不可です", null);
			}

			//重複確認
			var isOverlap = await _context.Reservations.AnyAsync(r =>
			r.Status == ReservationStatus.Reserved
			&& r.StartTime < request.EndTime
			&& r.EndTime > request.StartTime);
			
			if (isOverlap)
			{
				return (false, "すでに予約されています", null);
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

			return (true, "予約成功", reservation);
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

		public async Task<ReservationResponse?> GetMyReservationByIdAsync(int userId, int id)
		{
			return await _context.Reservations.
				Where(r => r.UserId == userId && r.Id == id).
				Select(r => new ReservationResponse
				{
					Id = r.Id,
					StartTime = r.StartTime,
					EndTime = r.EndTime,
					Memo = r.Memo,
					Status = r.Status,
					CreateAt = r.CreateAt,
					UpdateAt= r.UpdateAt,
				}).FirstOrDefaultAsync();
		}

		public async Task<(bool Success, bool NotFound, string? Message)> CancelMyReservationAsync(int userId, int id)
		{
			var reservation = await _context.Reservations.
				FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

			if (reservation is null)
			{
				return (false, true, "予約がありません");
			}

			if (reservation.Status == ReservationStatus.Canceled)
			{
				return (false, false, "キャンセル済みです");
			}

			reservation.Status = ReservationStatus.Canceled;
			reservation.UpdateAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			return (true, false, null);
		}

		public async Task<List<AdminReservationResponse>> GetAllReservationsForAdminAsync()
		{
			return await _context.Reservations.
				Include(r => r.User).
				OrderBy(r => r.StartTime).
				Select(r => new AdminReservationResponse
				{
					Id = r.Id,
					dateTime = DateTime.UtcNow,
				}).ToListAsync();
		}
	}
}
