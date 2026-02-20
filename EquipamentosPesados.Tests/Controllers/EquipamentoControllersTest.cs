using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipamentosPesados.Controllers;
using EquipamentosPesados.Data;
using EquipamentosPesados.Models;
using EquipamentosPesados.Dtos;
using Xunit;

namespace EquipamentosPesados.Tests
{
    public class EquipamentoControllerTests
    {
        // Helper para criar o contexto em memória
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task GetById_RetornaOk_QuandoEquipamentoExiste()
        {
            // Arrange
            var db = GetDatabaseContext();
            var equipamento = new Equipamento { Id = 1, Codigo = "EQ01", Tipo = "Escavadeira", Modelo = "CAT320", StatusOperacional = "Operacional", Horimetro = 100, LocalizacaoAtual = "Obra A", DataAquisicao = DateTime.Now };
            db.Equipamentos.Add(equipamento);
            await db.SaveChangesAsync();

            var controller = new EquipamentoController(db);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ResponseEquipamentoDto>(okResult.Value);
            Assert.Equal("EQ01", dto.Codigo);
        }

        [Fact]
        public async Task GetById_RetornaNotFound_QuandoEquipamentoNaoExiste()
        {
            // Arrange
            var db = GetDatabaseContext();
            var controller = new EquipamentoController(db);

            // Act
            var result = await controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_RetornaBadRequest_QuandoCodigoJaExiste()
        {
            // Arrange
            var db = GetDatabaseContext();
            db.Equipamentos.Add(new Equipamento { Codigo = "DUPLICADO", Tipo = "Caminhao", Modelo = "Tector", StatusOperacional = "Operacional", Horimetro = 10, LocalizacaoAtual = "Base", DataAquisicao = DateTime.Now });
            await db.SaveChangesAsync();

            var controller = new EquipamentoController(db);
            var novoEquipamento = new CreateEquipamentoDto 
            { 
                Codigo = "DUPLICADO", // Mesmo código
                Tipo = "Escavadeira",
                Modelo = "CAT",
                StatusOperacional = "Operacional",
                Horimetro = 50,
                LocalizacaoAtual = "Obra B",
                DataAquisicao = DateTime.Now
            };

            // Act
            var result = await controller.Create(novoEquipamento);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Já existe um equipamento com esse código", badRequest.Value.ToString());
        }

        [Fact]
        public async Task AvancarStatus_DeveCiclarStatusCorretamente()
        {
            // Arrange
            var db = GetDatabaseContext();
            var equipamento = new Equipamento { Id = 10, Codigo = "ST01", StatusOperacional = "Operacional", Tipo = "Trator", Modelo = "D6", Horimetro = 100, LocalizacaoAtual = "Pátio", DataAquisicao = DateTime.Now };
            db.Equipamentos.Add(equipamento);
            await db.SaveChangesAsync();

            var controller = new EquipamentoController(db);

            // Act: Operacional -> ForaDeServico
            await controller.AvancarStatus(10);
            
            // Assert
            var equipamentoAtualizado = await db.Equipamentos.FindAsync(10);
            Assert.NotNull(equipamentoAtualizado);
            Assert.Equal("ForaDeServico", equipamentoAtualizado.StatusOperacional);
        }

        [Fact]
        public async Task Delete_RetornaNoContent_E_RemoveDoBanco()
        {
            // Arrange
            var db = GetDatabaseContext();
            db.Equipamentos.Add(new Equipamento { Id = 5, Codigo = "DEL", Tipo = "Guindaste", Modelo = "X", StatusOperacional = "Operacional", Horimetro = 10, LocalizacaoAtual = "Porto", DataAquisicao = DateTime.Now });
            await db.SaveChangesAsync();

            var controller = new EquipamentoController(db);

            // Act
            var result = await controller.Delete(5);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await db.Equipamentos.FindAsync(5));
        }
    }
}