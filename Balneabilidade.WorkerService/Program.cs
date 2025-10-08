using Balneabilidade.Jobs;
using Balneabilidade.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<AtualizarDadosBalneabilidadeJob>(); 
builder.Services.AddHostedService<ColetaBalneabilidadeWorker>(); 

var host = builder.Build();
host.Run();