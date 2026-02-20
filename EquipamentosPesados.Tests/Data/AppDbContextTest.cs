using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using EquipamentosPesados.Data;
using EquipamentosPesados.Models;
using Xunit;

namespace EquipamentosPesados.Tests
{
    public class AppDbContextTests
    {
        private AppDbContext GetDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            
            context.Database.EnsureCreated();
            
            return context;
        }

        [Fact]
        public async Task DbContext_DeveSalvarEquipamento_ComCamposValidos()
        {
            using var context = GetDbContext();
            var equipamento = new Equipamento
            {
                Codigo = "EQ-001",
                Tipo = "Escavadeira",
                Modelo = "Cat 320",
                Horimetro = 120.50m,
                StatusOperacional = "Operacional",
                DataAquisicao = DateTime.Now,
                LocalizacaoAtual = "Setor Norte"
            };


            context.Equipamentos.Add(equipamento);
            await context.SaveChangesAsync();


            var salvo = await context.Equipamentos.FirstOrDefaultAsync(e => e.Codigo == "EQ-001");
            Assert.NotNull(salvo);
            Assert.Equal("EQ-001", salvo.Codigo);
        }

        [Fact]
        public async Task DbContext_DeveLancarExcecao_QuandoCodigoForDuplicado()
        {
            using var context = GetDbContext();
            var eq1 = new Equipamento { Codigo = "UNIQUE", Tipo = "X", Modelo = "Y", StatusOperacional = "Z", LocalizacaoAtual = "L", Horimetro = 1, DataAquisicao = DateTime.Now };
            var eq2 = new Equipamento { Codigo = "UNIQUE", Tipo = "A", Modelo = "B", StatusOperacional = "C", LocalizacaoAtual = "D", Horimetro = 2, DataAquisicao = DateTime.Now };

            context.Equipamentos.Add(eq1);
            await context.SaveChangesAsync();


            context.Equipamentos.Add(eq2);

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task DbContext_DeveLancarExcecao_QuandoCampoObrigatorioForNulo()
        {

            using var context = GetDbContext();
            var equipamentoInvalido = new Equipamento 
            { 
                Codigo = null!, 
                Tipo = "Escavadeira" 
            };

            // Act & Assert
            context.Equipamentos.Add(equipamentoInvalido);
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public void DbContext_DeveAplicarConfiguracoesDeTabela()
        {
            // Arrange
            using var context = GetDbContext();
            var entityType = context.Model.FindEntityType(typeof(Equipamento));

            // Assert
            Assert.Equal("equipamentos_pesados", entityType?.GetTableName());
            Assert.Equal("public", entityType?.GetSchema());
            
            var codigoProp = entityType?.FindProperty("Codigo");
            Assert.Equal(50, codigoProp?.GetMaxLength());
            Assert.False(codigoProp?.IsNullable);
        }
    }
}