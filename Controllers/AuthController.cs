using Microsoft.AspNetCore.Mvc;
using ReservationManagerAPI2.Dtos;
using ReservationManagerAPI2.Services;

namespace ReservationManagerAPI2.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		readonly AuthService _authService;
		public AuthController(AuthService service)
		{
			_authService = service;
		}

		[HttpGet("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var result = await _authService.Register(request);

			if (!result)
			{
				return BadRequest("このユーザー名はすでに登録されています");
			}

			return Ok("ユーザー登録が完了しました");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var response = await _authService.LoginAsync(request);

			if (response == null)
			{
				return Unauthorized("ユーザー名またはパスワードが正しくありません。");
			}

			return Ok(response);
		}
	}
}

