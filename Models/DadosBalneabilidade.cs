namespace BalneabilidadeMA.Models
{
    public class DadosBalneabilidade
    {
        public Guid Id { get; set; }
        public string Ponto { get; set; }

        public string Coordenada { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Localizacao { get; set; }

        public string Referencia { get; set; }

        public string Condicao { get; set; }
        public bool EstaProprioParaBanho { get; set; }
        public DateTime Data { get; set; }
    }
}