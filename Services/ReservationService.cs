using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

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
	}
}
