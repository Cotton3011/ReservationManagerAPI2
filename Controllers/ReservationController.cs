using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Services;
using System.Security.Claims;

namespace ReservationManagerAPI2.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ReservationController : ControllerBase
	{
		readonly ReservationService _service;
		public ReservationController(ReservationService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateReservationRequest request)
		{
			var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdText, out var usrId))
			{
				return Unauthorized();
			}
			var result = await _service.CreateReservationRequest(usrId, request);
			
			if (!result.Success)
			{
				return BadRequest(result.ErrorMessage);
			}

			return StatusCode(StatusCodes.Status201Created, result.Reservation);
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdText, out int userId))
			{
				return Unauthorized();
			}
			var reservations = await _service.GetReservation(userId);

			return Ok(reservations);
		}
	}
}
