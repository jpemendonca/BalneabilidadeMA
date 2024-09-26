using System.Globalization;
using System.Text;
using BalneabilidadeMA.ViewModels;
using Microsoft.VisualBasic.FileIO;

namespace BalneabilidadeMA.Services
{
    public class ManipuladorCSVService
    {
        public List<DadosTabelaViewModel> BuscarDadosTabelaExtraida(string fileName)
        {
			try
			{
                var retorno = new List<DadosTabelaViewModel>();

                using (var parser = new TextFieldParser(fileName))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TrimWhiteSpace = true;

                    parser.ReadFields();

                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();

                        var dados = new DadosTabelaViewModel
                        {
                            Ponto = fields[0],
                            Coordenada = fields[1],
                            Localizacao = fields[2],
                            Referencia = fields[3],
                            Condicao = fields[4],
                        };

                        dados.EstaProprioParaBanho = dados.Condicao == "PRÓPRIO";

                        retorno.Add(dados);
                    }
                }

                return retorno;
            }
			catch (Exception ex)
			{
                throw new Exception("Erro ao buscar dados da tabela extraída: " + ex.Message, ex);
            }
        }
    }
}