using Microsoft.EntityFrameworkCore;
using GameDbManager.API.Models;
using GameDbManager.API.Models.Items;

namespace GameDbManager.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Accessory> Accessories { get; set; }
        public DbSet<Armor> Armors { get; set; }
        public DbSet<Etc> Etcs { get; set; }
        public DbSet<Jewelry> Jewelries { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Stat> Stats { get; set; }  // Añadido DbSet para Stat

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>()
                .HasDiscriminator<string>("ItemType")
                .HasValue<Accessory>("Accessory")
                .HasValue<Armor>("Armor")
                .HasValue<Etc>("Etc")
                .HasValue<Jewelry>("Jewelry")
                .HasValue<Weapon>("Weapon");

            // Definición para Stat
            modelBuilder.Entity<Stat>().ToTable("Stats");
        }
    }
}
