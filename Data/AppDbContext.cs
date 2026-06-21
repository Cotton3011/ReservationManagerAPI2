using Microsoft.EntityFrameworkCore;
using ReservationManagerAPI2.Entities;

namespace ReservationManagerAPI2.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Reservation> Reservation { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Reservation>()
				.HasOne(r => r.User) // Reservationは1人のUserを持つ
				.WithMany() // Userは複数のReservationを持てる
				.HasForeignKey(r => r.UserId); // ReservationのUserIdを外部キーとして設定
		}
	}
}
