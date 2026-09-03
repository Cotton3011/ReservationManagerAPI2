using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ReservationManagerAPI2.Middlewares;
using Microsoft.OpenApi.Models;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Application Insightsの接続文字列を取得する
var applicationInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
//Azure側で接続文字列が設定されている時だけ監視を有効にする
if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
{
	//AzureMonitorへログメトリクスリクエスト情報を送信する
	builder.Services.AddOpenTelemetry().UseAzureMonitor();
}
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<AdminSeedService>();

//JWT
var jwtKey = builder.Configuration["Jwt:Key"]
?? throw new InvalidOperationException("JWT秘密鍵が設定されていません");
builder.Services.AddAuthentication(options => 
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(jwtKey)),
		ClockSkew = TimeSpan.Zero
	};
});
builder.Services.AddAuthorization();

//環境変数などからDB接続文字列を取得し、未設定なら起動を止める
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("DB接続文字列が設定されていません");

//AppDbContextの設定を追加
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
	//SwaggerにJWTの送信方法を定義する
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authenrization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "ログインで取得したJWTを入力してください"
	});

	//Bearer認証をAPI呼び出しで使用できるようにする
	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme()
			{
				Reference = new OpenApiReference()
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//Admin
using (var scope = app.Services.CreateScope())
{
	var adminSeedService = scope.ServiceProvider.GetRequiredService<AdminSeedService>();
	await adminSeedService.SeedAsync();
}

//HTTPアクセスをHTTPSへ誘導する
app.UseHttpsRedirection();
//ルートURLへのアクセス時にindex.htmlを既定ファイルとして探す
app.UseDefaultFiles();
//wwwroot配下のHTML、CSS、JavaScriptをブラウザへ配信する
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
