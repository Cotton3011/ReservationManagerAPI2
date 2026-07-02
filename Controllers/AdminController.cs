using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Entities;
using ReservationManagerAPI2.Services;

namespace ReservationManagerAPI2.Controllers
{
	[ApiController]
	[Authorize(Roles = "Admin")]
	[Route("api/[controller]")]
	public class AdminController : ControllerBase
	{
		readonly ReservationService _reservationService;
		public AdminController(ReservationService reservationService) 
		{
			_reservationService = reservationService;
		}

		public async Task<IActionResult> GetAllreservations()
		{
			var result = _reservationService.GetReservation();

			return Ok(result);
		}

	}
}
