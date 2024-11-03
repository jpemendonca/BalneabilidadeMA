using BalneabilidadeMA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DadosBalneabilidadeService>();

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();

app.MapGet("api/Buscar", (DadosBalneabilidadeService _dadosBalneabilidadeService) =>
{
    try
    {
        var retorno = _dadosBalneabilidadeService.ListarDados();

        return retorno.Count > 0 ? Results.Ok(retorno) : Results.BadRequest("Erro ao obter dados do csv");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"{ex.Message}");
    }
});

app.Run();