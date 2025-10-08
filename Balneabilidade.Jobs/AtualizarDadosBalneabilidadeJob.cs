using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace Balneabilidade.Jobs;

public class AtualizarDadosBalneabilidadeJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AtualizarDadosBalneabilidadeJob> _logger;

    public AtualizarDadosBalneabilidadeJob(IHttpClientFactory httpClientFactory, ILogger<AtualizarDadosBalneabilidadeJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task ExecutarAsync()
    {
        _logger.LogInformation("Iniciando tarefa de atualização de dados...");

        try
        {
            // 1. Buscar o link do PDF
            _logger.LogInformation("Buscando URL do PDF mais recente...");
            string? pdfUrl = await ObterUrlPdfRecente();

            if (string.IsNullOrEmpty(pdfUrl))
            {
                _logger.LogWarning("Nenhuma URL de PDF encontrada. A tarefa será encerrada.");
                return;
            }
            _logger.LogInformation("URL do PDF encontrada: {PdfUrl}", pdfUrl);

            // 2. Baixar o PDF
            _logger.LogInformation("Baixando o arquivo PDF...");
            string tempPdfPath = await BaixarPdfAsync(pdfUrl);
            _logger.LogInformation("PDF baixado com sucesso para: {TempPath}", tempPdfPath);

            // PRÓXIMOS PASSOS...
            // TODO: Chamar Python, ler JSON, salvar no banco...

            // Limpar o arquivo temporário
            File.Delete(tempPdfPath);
            _logger.LogInformation("Arquivo temporário '{TempPath}' removido.", tempPdfPath);

            _logger.LogInformation("Tarefa de atualização concluída com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro inesperado durante a execução da tarefa.");
        }
    }

    // Os métodos ObterUrlPdfRecente e BaixarPdfAsync continuam exatamente os mesmos
    private async Task<string?> ObterUrlPdfRecente()
    {
        const string urlBase = "https://www.sema.ma.gov.br";
        const string urlLaudos = $"{urlBase}/laudos-de-balneabilidade";
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(urlLaudos);
        var primeiroLinkPdfNode = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '.pdf')]");

        var href = primeiroLinkPdfNode.GetAttributeValue("href", string.Empty);
        return new Uri(new Uri(urlBase), href).ToString();
    }

    private async Task<string> BaixarPdfAsync(string url)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var pdfBytes = await httpClient.GetByteArrayAsync(url);
        string tempPdfPath = Path.Combine(Path.GetTempPath(), $"laudo_{Guid.NewGuid()}.pdf");
        await File.WriteAllBytesAsync(tempPdfPath, pdfBytes);
        return tempPdfPath;
    }
}