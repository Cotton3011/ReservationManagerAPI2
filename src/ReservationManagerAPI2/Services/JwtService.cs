using Microsoft.IdentityModel.Tokens;
using ReservationManagerAPI2.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReservationManagerAPI2.Services
{
	public class JwtService
	{
		readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public (string Token, DateTime ExpiresAt) CreateToken(User user)
		{
			//JWTセクションのKeyを取得　※未設定の場合は例外
			var jwtKey = _configuration["Jwt:Key"] 
				?? throw new InvalidOperationException("JWT秘密鍵が設定されていません");

			//有効期限設定 DateTime.UtcNowを使うことで、サーバーのタイムゾーン差によるズレを避ける
			var expiresAt = DateTime.UtcNow.AddMinutes(
				_configuration.GetValue<int>("Jwt:ExpireMinutes"));

			//JTWの中に入れるユーザー情報を作る
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Id.ToString()), //ログイン中のユーザーIDとして後で取り出す
				new(ClaimTypes.Name, user.UserName),
				new(ClaimTypes.Role, user.Role.ToString())
			};

			//文字列の秘密鍵をバイト配列に変換し、JWT署名用のキーを作る
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			//作成したキーを使って、HMAC SHA256方式で署名用する設定を作る
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			//JWT本体を作成
			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials);

			return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
		}
	}
}
