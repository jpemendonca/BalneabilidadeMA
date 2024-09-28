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

                        dados.Latitude = ConvertDMSToDecimal(dados.Coordenada.Split(" ")[0]);
                        dados.Longitude = ConvertDMSToDecimal(dados.Coordenada.Split(" ")[1]);

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

        public static double ConvertDMSToDecimal(string dms)
        {
            // Remover caracteres especiais (º, ’, ”) para facilitar a conversão
            dms = dms.Replace("º", " ")
                     .Replace("’", " ")
                     .Replace("”", " ")
                     .Trim();

            // Separar os valores de graus, minutos, segundos e direção
            string[] dmsParts = dms.Split(' ');
            double degrees = double.Parse(dmsParts[0], CultureInfo.InvariantCulture);
            double minutes = double.Parse(dmsParts[1], CultureInfo.InvariantCulture);
            double seconds = double.Parse(dmsParts[2], CultureInfo.InvariantCulture);
            char direction = dmsParts[3][0]; // 'S', 'N', 'O' ou 'E'

            // Calcular a conversão para graus decimais
            double decimalDegrees = degrees + (minutes / 60.0) + (0 / 3600.0);

            // Inverter o sinal se for sul ou oeste
            if (direction == 'S' || direction == 'O')
            {
                decimalDegrees *= -1;
            }

            return decimalDegrees;
        }

        public static double TruncarParaQuatroDecimais(double valor)
        {
            return Math.Truncate(valor * 10000) / 10000;
        }
    }
}