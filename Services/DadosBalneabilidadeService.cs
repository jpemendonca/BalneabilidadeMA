using BalneabilidadeMA.Models;
using Dapper;
using Microsoft.Data.Sqlite;
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
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM DadosBalneabilidade";
                    var dados = connection.Query<DadosBalneabilidade>(query).ToList();

                    // Filtrar duplicatas pela propriedade Ponto
                    var dadosDistintos = dados.GroupBy(x => x.Ponto).Select(g => g.First()).ToList();

                    return dadosDistintos;
                }
            }
            catch (Exception ex)
            {
                // Tratar exceção (log, rethrow ou retornar erro)
                throw new Exception("Erro ao listar dados: " + ex.Message);
            }
        }

    }
}