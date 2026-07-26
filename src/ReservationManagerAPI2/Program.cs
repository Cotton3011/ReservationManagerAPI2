using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Data;
using ReservationManagerAPI2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ReservationManagerAPI2.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<AdminSeedService>();

//JWT
var jwtKey = builder.Configuration["Jwt:Key"]
?? throw new InvalidOperationException("JWT”é–§Œ®‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
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

//ŠÂ‹«•Ï”‚È‚Ç‚©‚çDBÚ‘±•¶š—ñ‚ğæ“¾‚µA–¢İ’è‚È‚ç‹N“®‚ğ~‚ß‚é
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("DBÚ‘±•¶š—ñ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");

//AppDbContext‚Ìİ’è‚ğ’Ç‰Á
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
