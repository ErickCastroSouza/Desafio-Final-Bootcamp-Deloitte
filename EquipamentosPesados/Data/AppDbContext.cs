using Microsoft.EntityFrameworkCore;
using EquipamentosPesados.Models;

namespace EquipamentosPesados.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Equipamento> Equipamentos => Set<Equipamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Equipamento>(entity =>
            {
                entity.ToTable("equipamentos_pesados");

                entity.HasKey(x => x.Id);

                
                entity.Property(e => e.Codigo)
                    .HasColumnName("codigo")
                    .HasMaxLength(50)
                    .IsRequired();


                entity.HasIndex(e => e.Codigo)
                    .IsUnique();


                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(120)
                    .IsRequired();


                entity.Property(e => e.Modelo)
                    .HasColumnName("modelo")
                    .HasMaxLength(120)
                    .IsRequired();


                entity.Property(e => e.Horimetro)
                    .HasColumnName("horimetro")
                    .HasColumnType("numeric(5,2)")
                    .IsRequired();;


                entity.Property(e => e.StatusOperacional)
                    .HasColumnName("status_operacional")
                    .HasMaxLength(50)
                    .IsRequired();;


                entity.Property(e => e.DataAquisicao)
                    .HasColumnName("data_aquisicao")
                    .IsRequired();;


                entity.Property(e => e.LocalizacaoAtual)
                    .HasColumnName("localizacao_atual")
                    .HasMaxLength(200)
                    .IsRequired();
            });
        }
    }
}