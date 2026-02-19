namespace EquipamentosPesados.Dtos
{
    public record ResponseEquipamentoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Modelo { get; set; } = "";
        public decimal Horimetro { get; set; }
        public string StatusOperacional { get; set; } = "";
        public DateTime DataAquisicao { get; set; }
        public string LocalizacaoAtual { get; set; } = "";
    }
}