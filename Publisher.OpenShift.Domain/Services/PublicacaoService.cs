using Publisher.OpenShift.Common.Configurations;
using Publisher.OpenShift.Domain.Interfaces;
using Serilog;
using SimpleBrowser;
using System.Diagnostics;
using System.Text;

namespace Publisher.OpenShift.Domain.Services
{
    public sealed class PublicacaoService : IPublicacaoService
    {
        private readonly GitSettings _gitSettings;
        private readonly ProcessSettings _processSettings;
        private readonly OpenShiftSettings _openShiftSettings;
        private readonly PackagesNugetSettings _packagesNugetSettings;
        private const string CMD_GIT = "/C {0}:&cd \"{1}\" &{2}";

        public StringBuilder LogsAplicacao { get; set; }

        public PublicacaoService(
            GitSettings gitSettings,
            ProcessSettings processSettings,
            OpenShiftSettings openShiftSettings,
            PackagesNugetSettings packagesNugetSettings)
        {
            _gitSettings = gitSettings;
            _processSettings = processSettings;
            _openShiftSettings = openShiftSettings;
            _packagesNugetSettings = packagesNugetSettings;
        }

        public void ProcessarPublicacao()
        {
            var momentoInicio = DateTime.Now;

            Logar($"Iniciando processo do Projeto: {_openShiftSettings.Projeto}{Environment.NewLine}");

            if (_packagesNugetSettings.CopiarPackagesNuget)
            {
                Logar($"0 - Copiando pacotes Nuget para o SabespNugetHomolog:");
                Logar($"Pasta Origem: {_packagesNugetSettings.PastaOrigem}");
                Logar($"Pasta Destino: {_packagesNugetSettings.PastaDestino}");
                CopiarArquivosPackagesNuget();
                Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");
            }

            Logar($"1 - Definindo branchs do processo (checkout, fetch e pull):");
            Logar($"Branch Git Origem: {_gitSettings.BranchOrigem}");
            Logar($"Branch Git Destino: {_gitSettings.BranchDestino}");
            DefinindoBranchsGit();
            Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");

            Logar($"2 - Apagando arquivos da pasta destino:");
            Logar(_processSettings.PastaDestino);
            ApagarArquivosPastaDestino();
            Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");

            Logar($"3 - Copiando arquivos da pasta origem:");
            Logar(_processSettings.PastaOrigem);
            CopiarArquivosPastaOrigem();
            Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");

            Logar($"4 - Comitando e Subindo arquivos no Git Destino:");
            Logar(_gitSettings.Repositorio);
            Logar($"Branch: {_gitSettings.BranchDestino}");
            ComitarArquivosGit();
            Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");

            Logar($"4 - Iniciando StartBuild no OpenShift:");
            Logar(_openShiftSettings.Url);
            Logar($"Namespace: {_openShiftSettings.Namespace}");
            Logar($"Projeto: {_openShiftSettings.Projeto}");
            IniciarStartBuildOpenShift();
            Logar($"Tempo total decorrido: {DateTime.Today.AddSeconds((DateTime.Now - momentoInicio).TotalSeconds):HH:mm:ss}{Environment.NewLine}");

            Logar("Final do Processo de Publicação");
        }

        private void ApagarArquivosPastaDestino()
        {
            var momentoProcesso = DateTime.Now;
            var arquivosPastas = Directory.GetFileSystemEntries(_processSettings.PastaDestino);

            foreach (string arquivoPasta in arquivosPastas)
            {
                if (PodeApagar(arquivoPasta) == true)
                    continue;

                if (EhPasta(arquivoPasta))
                    Directory.Delete(arquivoPasta, true);
                else
                    File.Delete(arquivoPasta);
            }

            Logar($"Tempo decorrido na exclusão: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private void CopiarArquivosPastaOrigem()
        {
            var momentoProcesso = DateTime.Now;
            var arquivosPastas = Directory.GetFileSystemEntries(_processSettings.PastaOrigem);

            CopiarArquivos(_processSettings.PastaOrigem, arquivosPastas);
            
            Logar($"Tempo decorrido na cópia: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private void ComitarArquivosGit()
        {
            var momentoProcesso = DateTime.Now;

            var commitMessage = $"Commit automático em {DateTime.Now} [Publicador OpenShift]";
            var letraUnidade = _processSettings.PastaDestino.Remove(1);
            var comandosGit = $"git add --all . &git commit -m \"{commitMessage}\" &git push";
            var arguments = string.Format(CMD_GIT, letraUnidade, _processSettings.PastaDestino, comandosGit);
            ProcessStart(arguments);

            Logar($"Tempo decorrido no commit e push: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private void DefinindoBranchsGit()
        {
            var momentoProcesso = DateTime.Now;

            var letraUnidade = _processSettings.PastaOrigem.Remove(1);
            var comandosGit = $"git checkout {_gitSettings.BranchOrigem} &git fetch origin &git pull";
            var arguments = string.Format(CMD_GIT, letraUnidade, _processSettings.PastaOrigem, comandosGit);
            ProcessStart(arguments);

            letraUnidade = _processSettings.PastaDestino.Remove(1);
            arguments = string.Format(CMD_GIT, letraUnidade, _processSettings.PastaDestino, comandosGit);
            ProcessStart(arguments);

            Logar($"Tempo decorrido no commit e push: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private void IniciarStartBuildOpenShift()
        {
            var momentoProcesso = DateTime.Now;
            
            var tokenOpenShift = GetTokenOpenShift();
            var letraUnidade = _openShiftSettings.PastaOpenShiftCLI.Remove(1);
            var comandosOc = $"{tokenOpenShift} &y &oc project {_openShiftSettings.Namespace} &oc start-build {_openShiftSettings.Projeto}";
            var arguments = string.Format(CMD_GIT, letraUnidade, _openShiftSettings.PastaOpenShiftCLI, comandosOc);
            ProcessStart(arguments);

            Logar($"Tempo decorrido no commit e push: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private static void ProcessStart(string arguments)
        {
            ProcessStartInfo procStartInfo = new("cmd", arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = new() { StartInfo = procStartInfo };
            proc.Start();
            proc.WaitForExit();
        }

        private void CopiarArquivos(string pastaCopiar, string[] arquivosPastas)
        {
            foreach (string arquivoPasta in arquivosPastas)
            {
                if (PodeCopiar(arquivoPasta) == true)
                    continue;

                if (EhPasta(arquivoPasta))
                {
                    CopiarArquivos(arquivoPasta, Directory.GetFileSystemEntries(arquivoPasta));
                    continue;
                }

                var pastaSalvar = pastaCopiar.Replace(_processSettings.PastaOrigem, _processSettings.PastaDestino);
                var diretorio = new DirectoryInfo(pastaSalvar);
                if (!diretorio.Exists)
                    diretorio.Create();

                var nomeArquivo = arquivoPasta.Substring(pastaCopiar.Length + 1);
                var destinoSalvar = Path.Combine(pastaSalvar, nomeArquivo);

                if (!File.Exists(destinoSalvar))
                    File.Copy(arquivoPasta, destinoSalvar, true);
            }
        }

        private bool? PodeApagar(string arquivoPasta)
        {
            if (EhPasta(arquivoPasta))
                return _processSettings?.PastasNaoApagar?.Any(x => arquivoPasta == Path.Combine(_processSettings.PastaDestino, x));

            return _processSettings?.ArquivosNaoApagar?.Any(x => arquivoPasta == Path.Combine(_processSettings.PastaDestino, x));
        }

        private bool? PodeCopiar(string arquivoPasta)
        {
            if (EhPasta(arquivoPasta))
                return _processSettings?.PastasNaoCopiar?.Any(x => arquivoPasta == Path.Combine(_processSettings.PastaOrigem, x));

            return _processSettings?.ArquivosNaoCopiar?.Any(x => arquivoPasta == Path.Combine(_processSettings.PastaOrigem, x));
        }

        private static bool EhPasta(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private void Logar(string mensagem)
        {
            Log.Information(mensagem);
            LogsAplicacao.AppendLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} {mensagem}");
        }

        private string GetTokenOpenShift()
        {
            var objBrowser = new Browser
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
            };

            objBrowser.Navigate("https://oauth-openshift.apps.ad2.sabesp.com.br/oauth/authorize?client_id=openshift-browser-client&redirect_uri=https%3A%2F%2Foauth-openshift.apps.ad2.sabesp.com.br%2Foauth%2Ftoken%2Fdisplay&response_type=code");
            objBrowser.Navigate("https://oauth-openshift.apps.ad2.sabesp.com.br/login/LDAP?then=%2Foauth%2Fauthorize%3Fclient_id%3Dopenshift-browser-client%26idp%3DLDAP%26redirect_uri%3Dhttps%253A%252F%252Foauth-openshift.apps.ad2.sabesp.com.br%252Foauth%252Ftoken%252Fdisplay%26response_type%3Dcode");

            //Pega os controles
            var txtUsername = objBrowser.Find(ElementType.TextField, FindBy.Name, "username");
            var txtPassword = objBrowser.Find(ElementType.TextField, FindBy.Name, "password");
            var btnLogIn = objBrowser.Find(ElementType.Button, FindBy.Value, "Log in");

            txtUsername.Value = _openShiftSettings.Autenticacao.Login;
            txtPassword.Value = _openShiftSettings.Autenticacao.Senha;
            btnLogIn.Click();

            var btnDisplayToken = objBrowser.Find(ElementType.Button, FindBy.Value, "\r\n      Display Token\r\n    ");
            btnDisplayToken.Click();

            var token = objBrowser.CurrentHtml;
            token = token[token.IndexOf("--token=sha256")..];
            token = token.Remove(token.IndexOf("</span>"));

            var server = objBrowser.CurrentHtml;
            server = server[server.IndexOf("--server=https")..];
            server = server.Remove(server.IndexOf("</span>"));

            objBrowser.Close();

            return $"oc login {token} {server}";
        }

        private void CopiarArquivosPackagesNuget()
        {
            var momentoProcesso = DateTime.Now;
            var arquivosPastasTodos = Directory.GetFileSystemEntries(_packagesNugetSettings.PastaOrigem, "*.nupkg");
            List<string> arquivosPastas = new();

            _packagesNugetSettings.PackagesCopiar
                .ForEach(pacote => arquivosPastas.AddRange(arquivosPastasTodos.Where(x => x.Contains(pacote))));

            if (arquivosPastas.Count == 0)
            {
                Logar("Nenhum pacote Nuget para copiar");
                return;
            }
            
            CopiarArquivosPackagesNuget(_packagesNugetSettings.PastaOrigem, arquivosPastas);
            LimparCachePacotesNuget();

            Logar($"Tempo decorrido na cópia: {DateTime.Today.AddMilliseconds((DateTime.Now - momentoProcesso).TotalMilliseconds):HH:mm:ss.fff}");
        }

        private void CopiarArquivosPackagesNuget(string pastaCopiar, List<string> arquivosPastas)
        {
            foreach (string arquivoPasta in arquivosPastas)
            {
                var pastaSalvar = pastaCopiar.Replace(_packagesNugetSettings.PastaOrigem, _packagesNugetSettings.PastaDestino);
                var nomeArquivo = arquivoPasta[(pastaCopiar.Length + 1)..];
                var destinoSalvar = Path.Combine(pastaSalvar, nomeArquivo);
                var arquivoExcluir = Path.Combine(_packagesNugetSettings.PastaOrigem, nomeArquivo);

                if (!File.Exists(destinoSalvar))
                    File.Copy(arquivoPasta, destinoSalvar, true);

                File.Delete(arquivoExcluir);
            }
        }

        private void LimparCachePacotesNuget()
        {
            var objBrowser = new Browser
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
            };

            // Limpar Cache
            objBrowser.Navigate(_packagesNugetSettings.UrlServidor);
            objBrowser.Navigate(_packagesNugetSettings.UrlLimparCache);

            Thread.Sleep(2000);

            // Restaurar Pacotes
            objBrowser.Navigate(_packagesNugetSettings.UrlServidor);
            objBrowser.Navigate(_packagesNugetSettings.UrlRestaurarPacotes);

            // Fecha o navegador
            objBrowser.Close();
        }
    }
}