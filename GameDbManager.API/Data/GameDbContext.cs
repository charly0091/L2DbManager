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
        public DbSet<ItemStat> ItemStats { get; set; }
        public DbSet<ItemSkill> ItemSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de TPH (Table Per Hierarchy)
            modelBuilder.Entity<Item>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Accessory>("Accessory")
                .HasValue<Armor>("Armor")
                .HasValue<Etc>("Etc")
                .HasValue<Jewelry>("Jewelry")
                .HasValue<Weapon>("Weapon");

            // Configuración de relaciones
            modelBuilder.Entity<ItemStat>()
                .HasOne(s => s.Item)
                .WithMany(i => i.Stats)
                .HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemSkill>()
                .HasOne(s => s.Item)
                .WithMany(i => i.Skills)
                .HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}