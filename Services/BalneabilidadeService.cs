using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Recognition;
using HtmlAgilityPack;
using System.Drawing;
using System.Security.Policy;
using System.Text;

namespace BalneabilidadeMA.Services
{
    public static class BalneabilidadeService
    {
        public static async Task BaixarPDFMaisRecente()
        {
            try
            {
                using HttpClient client = new HttpClient();

                var response = await client.GetStringAsync("https://www.sema.ma.gov.br/laudos-de-balneabilidade");

                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                var pdfLink = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '.pdf') and contains(@href, 'https')]");

                if (pdfLink == null)
                {
                    Console.WriteLine("Nenhum link PDF encontrado na página.");
                    return;
                }

                string pdfUrl = pdfLink.GetAttributeValue("href", string.Empty);

                // Download do pdf
                byte[] pdfBytes = await client.GetByteArrayAsync(pdfUrl);
                string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents", "Balneabilidade.pdf");

                await File.WriteAllBytesAsync(fileName, pdfBytes);

                TransformarTabelaPDFParaCSV();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }

        public static void TransformarTabelaPDFParaCSV()
        {
            const float DPI = 72;

            var contentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents");
            var pdfFile = Directory.EnumerateFiles(contentDirectory, "*.pdf").FirstOrDefault();

            if (pdfFile == null)
            {
                return;
            }

            var fs = File.OpenRead(pdfFile);

            var pdfDoc = new GcPdfDocument();

            pdfDoc.Load(fs);

            var tableBounds = new RectangleF(0, 2.5f * DPI, 8.5f * DPI, 7.0f * DPI);

            var tableExtrctOpt = new TableExtractOptions();
            var GetMinimumDistanceBetweenRows = tableExtrctOpt.GetMinimumDistanceBetweenRows;

            tableExtrctOpt.GetMinimumDistanceBetweenRows = (list) =>
            {
                var res = GetMinimumDistanceBetweenRows(list);
                return res * 0.7f;
            };

            var data = new List<List<string>>();

            for (int i = 0; i < pdfDoc.Pages.Count; ++i)
            {
                // Obter a tabela dentro dos limites especificados
                var itable = pdfDoc.Pages[i].GetTable(tableBounds, tableExtrctOpt);
                if (itable != null)
                {
                    for (int row = 0; row < itable.Rows.Count; ++row)
                    {
                        data.Add(new List<string>());
                        for (int col = 0; col < itable.Cols.Count; ++col)
                        {
                            var cell = itable.GetCell(row, col);
                            if (cell == null)
                            {
                                data.Last().Add("");  // Adiciona vazio para células nulas
                            }
                            else
                            {
                                string texto = cell.Text.Trim();
                                data.Last().Add($"\"{texto}\"");  // Adiciona o texto da célula e remove espaços extras
                            }
                        }
                    }
                }
            }

            SalvarTabelaEmCSV(data);
        }

        public static void SalvarTabelaEmCSV(List<List<string>> data)
        {
            // Salvando os dados extraídos em CSV
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents", "tabela_extraida.csv");

            File.WriteAllLines(
                fileName,
                data.Where(l_ => l_.Any(s_ => !string.IsNullOrEmpty(s_)))
                    .Select(d_ => string.Join(',', d_)),
                Encoding.UTF8);

            Console.WriteLine($"Arquivo CSV salvo em: {fileName}");
        }
    }
}