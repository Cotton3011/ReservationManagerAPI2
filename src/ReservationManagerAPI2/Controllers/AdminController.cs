using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Services;

namespace ReservationManagerAPI2.Controllers
{
	[ApiController]
	[Route("api/admin/reservations")]
	[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		readonly ReservationService _reservationService;
		public AdminController(ReservationService reservationService)
		{
			_reservationService = reservationService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllReservations()
		{
			var reservations = await _reservationService.GetAllReservationsForAdminAsync();

			return Ok(reservations);
		}

		[HttpPatch("{id}/cancel")]
		public async Task<IActionResult> CancelReservation(int id)
		{
			await _reservationService.CancelReservationAsync(id);
			return Ok("予約をキャンセルしました");
		}
	}
}
