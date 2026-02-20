using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipamentosPesados.Data;
using EquipamentosPesados.Models;
using EquipamentosPesados.Dtos;


namespace EquipamentosPesados.Controllers
{
    [ApiController]
    [Route("api/equipamentos")]
    public class EquipamentoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EquipamentoController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipamento = await _db.Equipamentos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (equipamento == null) return NotFound();

            var dto = new ResponseEquipamentoDto
            {
                Id = equipamento.Id,
                Codigo = equipamento.Codigo,
                Tipo = equipamento.Tipo,
                Modelo = equipamento.Modelo,
                Horimetro = equipamento.Horimetro,
                StatusOperacional = equipamento.StatusOperacional,
                DataAquisicao = equipamento.DataAquisicao,
                LocalizacaoAtual = equipamento.LocalizacaoAtual
            };

            return Ok(dto);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var equipamentos = await _db.Equipamentos.AsNoTracking().ToListAsync();
            return Ok(equipamentos);
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var equipamento = await _db.Equipamentos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (equipamento == null) return NotFound();

            return Ok(new { Status = equipamento.StatusOperacional });
        }

        [HttpGet("{tipo}/tipo")]
        public async Task<IActionResult> GetByTipo(string tipo)
        {
            var equipamentos = await _db.Equipamentos
                .AsNoTracking()
                .Where(e => e.Tipo.ToLower() == tipo.ToLower())
                .ToListAsync();

            return Ok(equipamentos);
        }

        

        [HttpPost] 
        public async Task<IActionResult> Create([FromBody] CreateEquipamentoDto input)
        {

            if (string.IsNullOrEmpty(input.Codigo))
                return BadRequest("O campo 'Codigo' é obrigatório.");   
            if (string.IsNullOrEmpty(input.Tipo))
                return BadRequest("O campo 'Tipo' é obrigatório.");
            if (string.IsNullOrEmpty(input.Modelo))
                return BadRequest("O campo 'Modelo' é obrigatório.");
            if (string.IsNullOrEmpty(input.StatusOperacional))
                return BadRequest("O campo 'StatusOperacional' é obrigatório.");
            if (input.Horimetro <= 0)
                return BadRequest("O campo 'Horimetro' deve ser um valor positivo.");
            if (input.DataAquisicao > DateTime.Now)
                return BadRequest("O campo 'DataAquisicao' não pode ser uma data futura.");
            if (string.IsNullOrEmpty(input.LocalizacaoAtual))
                return BadRequest("O campo 'LocalizacaoAtual' é obrigatório.");
            if (!Enum.TryParse<StatusOperacional>(input.StatusOperacional, true, out _))
                return BadRequest("O campo 'StatusOperacional' deve ser um dos seguintes valores: Operacional, EmManutencao, ForaDeServico.");
            if (!Enum.TryParse<Tipo>(input.Tipo, true, out _))
                return BadRequest("O campo 'Tipo' deve ser um dos seguintes valores: Escavadeira, Caminhao, Carregadeira, Retroescavadeira, Trator, Guindaste.");
            if (await _db.Equipamentos.AnyAsync(e => e.Codigo == input.Codigo))
                return BadRequest("O campo 'Codigo' deve ser único. Já existe um equipamento com esse código.");

            var equipamento = new Equipamento
            {
                Codigo = input.Codigo,
                Tipo = input.Tipo,
                Modelo = input.Modelo,
                Horimetro = input.Horimetro,
                StatusOperacional = input.StatusOperacional,
                DataAquisicao = input.DataAquisicao,
                LocalizacaoAtual = input.LocalizacaoAtual
            };

            _db.Equipamentos.Add(equipamento);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = equipamento.Id }, equipamento);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipamentoDto input)
        {
            var equipamento = await _db.Equipamentos.FindAsync(id);
            if (equipamento == null) return NotFound();

            if (string.IsNullOrEmpty(input.Codigo))
                return BadRequest("O campo 'Codigo' é obrigatório.");
            if (string.IsNullOrEmpty(input.Tipo))
                return BadRequest("O campo 'Tipo' é obrigatório.");
            if (string.IsNullOrEmpty(input.Modelo))
                return BadRequest("O campo 'Modelo' é obrigatório.");
            if (string.IsNullOrEmpty(input.StatusOperacional))
                return BadRequest("O campo 'StatusOperacional' é obrigatório.");
            if (input.Horimetro <= 0)
                return BadRequest("O campo 'Horimetro' deve ser um valor positivo.");
            if (input.DataAquisicao > DateTime.Now)
                return BadRequest("O campo 'DataAquisicao' não pode ser uma data futura.");
            if (string.IsNullOrEmpty(input.LocalizacaoAtual))
                return BadRequest("O campo 'LocalizacaoAtual' é obrigatório.");
            if (!Enum.TryParse<StatusOperacional>(input.StatusOperacional, true, out _))
                return BadRequest("O campo 'StatusOperacional' deve ser um dos seguintes valores: Operacional, EmManutencao, ForaDeServico.");

            var equipamentoExistente = await _db.Equipamentos.FirstOrDefaultAsync(e => e.Codigo == input.Codigo && e.Id != id);
            if (equipamentoExistente != null)
                return BadRequest("O campo 'Codigo' deve ser único. Já existe um equipamento com esse código.");

            equipamento.Codigo = input.Codigo;
            equipamento.Tipo = input.Tipo;
            equipamento.Modelo = input.Modelo;
            equipamento.Horimetro = input.Horimetro;
            equipamento.StatusOperacional = input.StatusOperacional;
            equipamento.DataAquisicao = input.DataAquisicao;
            equipamento.LocalizacaoAtual = input.LocalizacaoAtual;

            await _db.SaveChangesAsync();
            
            var dto = new ResponseEquipamentoDto
            {
                Id = equipamento.Id,
                Codigo = equipamento.Codigo,
                Tipo = equipamento.Tipo,
                Modelo = equipamento.Modelo,
                Horimetro = equipamento.Horimetro,
                StatusOperacional = equipamento.StatusOperacional,
                DataAquisicao = equipamento.DataAquisicao,
                LocalizacaoAtual = equipamento.LocalizacaoAtual
            };

            return Ok(dto);
        }

        [HttpPatch("{id:int}/avancar-status")]
        public async Task<IActionResult> AvancarStatus(int id)
        {
            var equipamento = await _db.Equipamentos.FindAsync(id);
            if (equipamento == null) return NotFound();

            if (equipamento.StatusOperacional == "Operacional")
                equipamento.StatusOperacional = "ForaDeServico";
            else if (equipamento.StatusOperacional == "ForaDeServico")
                equipamento.StatusOperacional = "EmManutencao";
            else if (equipamento.StatusOperacional == "EmManutencao")
                equipamento.StatusOperacional = "Operacional";

            await _db.SaveChangesAsync();

            var dto = new ResponseEquipamentoDto
            {
                Id = equipamento.Id,
                Codigo = equipamento.Codigo,
                Tipo = equipamento.Tipo,
                Modelo = equipamento.Modelo,
                Horimetro = equipamento.Horimetro,
                StatusOperacional = equipamento.StatusOperacional,
                DataAquisicao = equipamento.DataAquisicao,
                LocalizacaoAtual = equipamento.LocalizacaoAtual
            };

            return Ok(dto);
        }

        [HttpPatch("{id:int}/atualizar-horimetro")]
        public async Task<IActionResult> AtualizarHorimetro(int id, [FromBody] UpdateHorimetroInput input)
        {
            var equipamento = await _db.Equipamentos.FindAsync(id);

            if (equipamento == null)
                return NotFound();

            if (input.Horimetro <= 0)
                return BadRequest("O campo 'Horimetro' deve ser um valor positivo.");

            equipamento.Horimetro = input.Horimetro;

            await _db.SaveChangesAsync();

            return Ok(equipamento);
        }

        [HttpPatch("{id:int}/atualizar-localizacao")]
        public async Task<IActionResult> AtualizarLocalizacao(int id, [FromBody] UpdateLocalizacaoInput input)
        {
            var equipamento = await _db.Equipamentos.FindAsync(id);

            if (equipamento == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(input.LocalizacaoAtual))
                return BadRequest("O campo 'LocalizacaoAtual' é obrigatório.");

            equipamento.LocalizacaoAtual = input.LocalizacaoAtual;

            await _db.SaveChangesAsync();

            return Ok(equipamento);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var equipamento = await _db.Equipamentos.FirstOrDefaultAsync(e => e.Id == id);
            if (equipamento == null) return NotFound();

            _db.Equipamentos.Remove(equipamento);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}