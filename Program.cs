using BalneabilidadeMA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ManipuladorCSVService>();
builder.Services.AddRazorPages();

// Executa a funcao de tempo em tempo
//_ = new Timer(async x => await BalneabilidadeService.BaixarPDFMaisRecente(), null, TimeSpan.FromMinutes(0), TimeSpan.FromHours(12));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();

app.MapGet("/Buscar", (ManipuladorCSVService _manipuladorService) =>
{
    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents", "tabela_extraida.csv");

    try
    {
        var retorno = _manipuladorService.BuscarDadosTabelaExtraida(fileName);

        if (retorno != null)
        {
            return Results.Ok(retorno);
        }

        return Results.BadRequest("Erro ao obter dados do csv");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"{ex.Message}");
    }
});

app.Run();