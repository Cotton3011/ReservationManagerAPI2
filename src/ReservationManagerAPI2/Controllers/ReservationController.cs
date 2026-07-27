using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Services;
using System.Security.Claims;

namespace ReservationManagerAPI2.Controllers
{
	[ApiController]
	[Route("api/reservations")]
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
			var reservation = await _service.CreateReservationRequest(usrId, request);
			
			return StatusCode(StatusCodes.Status201Created, reservation);
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

		[HttpGet("{id}")]
		public async Task<IActionResult> GeGetMyReservationByIdtId(int id)
		{
			var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if(!int.TryParse(userIdText, out int userId))
			{
				return Unauthorized();
			}
			var reservation = await _service.GetMyReservationByIdAsync(userId, id);

			return Ok(reservation);
		}

		[HttpPatch("{id}/cancel")]
		public async Task<IActionResult> CancelMyReservation(int id)
		{
			var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdText, out int userId))
			{
				return Unauthorized();
			}

			//業務エラーはMiddlewareが404/409へ変換する
			await _service.CancelMyReservationAsync(userId, id);
			return Ok("予約をキャンセルしました");
		}
	}
}
