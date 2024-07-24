using Microsoft.Extensions.DependencyInjection;
using Publisher.OpenShift.Domain.Services;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace Publisher.OpenShift
{
    public class App
    {
        public App(string[] args)
        {
            if (args?.Length > 0)
                nomeConfiguracao = $".{args[0]}";
        }

        private string nomeConfiguracao { get; set; } = "";

        public void Run()
        {
            var logs = new StringBuilder();

            try
            {
                var host = Startup.AppStartup(nomeConfiguracao);
                var integracao = ActivatorUtilities.CreateInstance<PublicacaoService>(host.Services);

                Log.Information(".- - - - - - - - - - - - - - - - - - - - - - - - - - -.");
                Log.Information("|               Publicador do OpenShift               |");
                Log.Information($"|                    Versão: {GetVersion()}                    |");
                Log.Information($"|          Desenvolvido por: Jairo Monteiro           |");
                Log.Information($"'- - - - - - - - - - - - - - - - - - - - - - - - - - -'{Environment.NewLine}");

                logs.AppendLine($"Processo Integração RQA - Versão: {GetVersion()}").AppendLine();
                integracao.LogsAplicacao = logs;
                integracao.ProcessarPublicacao();
            }
            catch (ApplicationException ex)
            {
                Log.Error($"Erro na execução. Detalhes: {ex.Message}");

                logs.AppendLine($"[Descrição]: Erro na aplicação");
                logs.AppendLine($"[Message]: {ex.Message}");
                logs.AppendLine($"[StackTrace]: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Erro desconhecido");

                logs.AppendLine($"[Descrição]: Erro desconhecido");
                logs.AppendLine($"[Message]: {ex.Message}");
                logs.AppendLine($"[StackTrace]: {ex.StackTrace}");
            }
            finally
            {
                Log.Information($"Fechando aplicação{Environment.NewLine}{Environment.NewLine}");
                Log.CloseAndFlush();
                Environment.Exit(0);
            }

            static string GetVersion()
            {
                var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                return FileVersionInfo.GetVersionInfo(executingAssembly.Location).FileVersion;
            }
        }
    }
}