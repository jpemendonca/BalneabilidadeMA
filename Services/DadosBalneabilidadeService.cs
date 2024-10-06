using BalneabilidadeMA.Models;
using Dapper;
using Npgsql;

namespace BalneabilidadeMA.Services
{
    public class DadosBalneabilidadeService
    {
        private readonly string _connectionString;

        public DadosBalneabilidadeService(IConfiguration configuration)
        {
            if (configuration is not null)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }
        }

        public void InserirDados(DadosBalneabilidade dados)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                INSERT INTO DadosBalneabilidade
                (Id, Ponto, Coordenada, Latitude, Longitude, Localizacao, Referencia, Condicao, EstaProprioParaBanho, Data)
                VALUES
                (@Id, @Ponto, @Coordenada, @Latitude, @Longitude, @Localizacao, @Referencia, @Condicao, @EstaProprioParaBanho, @Data)";

                connection.Execute(query, dados);
            }
        }

        public List<DadosBalneabilidade> ListarDados()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM DadosBalneabilidade";
                return connection.Query<DadosBalneabilidade>(query).DistinctBy(x => x.Ponto).ToList();
            }
        }
    }
}