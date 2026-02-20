using System.ComponentModel.DataAnnotations.Schema;

namespace EquipamentosPesados.Models
{

    [Table("equipamentos_pesados")]
    public class Equipamento
    {   
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; } = "";
        [Column("tipo")]
        public string Tipo { get; set; } = "";
        [Column("modelo")]
        public string Modelo { get; set; } = "";
        [Column("horimetro")]
        public decimal Horimetro { get; set; }
        [Column("status_operacional")]
        public string StatusOperacional { get; set; } = "";
        [Column("data_aquisicao")]
        public DateTime DataAquisicao { get; set; }
        [Column("localizacao_atual")]
        public string LocalizacaoAtual { get; set; } = "";
    }
}