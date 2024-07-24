#region Helper

if (args?.Length > 0 && (args[0].Contains("help") || args[0].Contains("?")))
{
    Console.WriteLine("Ajuda - Publicador do OpenShift");
    Console.WriteLine("");
    Console.WriteLine("Inicie o aplicativo passando qual a configuração para ser usada.");
    Console.WriteLine($"Para usar o arquivo de configuração: appsettings.des-atendimento-medico-backend.json,");
    Console.WriteLine($"Execute: Publisher.OpenShift.exe des-atendimento-medico-backend");
    Console.WriteLine("");
    Console.WriteLine($"Se quiser usar somente o arquivo padrão (appsettings.json), chame o executável sem passar argumento.");
    Console.WriteLine($"Execute: Publisher.OpenShift.exe");
    Environment.Exit(0);
}

#endregion

new Publisher.OpenShift.App(args).Run();