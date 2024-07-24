# Publisher.OpenShift

Aplicação para fazer a publicação dos produtos na plataforma OpenShift.
Essa aplicação realiza todos os passos necessários para publicar o projeto no respectivo container, bem como a gravação dos logs de cada processo.
Os passos são os seguintes:
-> 0 - Copiando pacotes Nuget para o SabespNugetHomolog;
-> 1 - Definindo branchs do processo (checkout, fetch e pull);
-> 2 - Apagando arquivos da pasta destino;
-> 3 - Copiando arquivos da pasta origem;
-> 4 - Comitando e Subindo arquivos no Git Destino;
-> 5 - Iniciando StartBuild no OpenShift;

Ele é totalmente adaptável para várias publicações (projeto), desde que cada projeto tenha sua configuração (appsettings.json) específica.


# Ajuda - Publicador do OpenShift

Inicie o aplicativo passando qual a configuração para ser usada.
Para usar o arquivo de configuração: appsettings.des-atendimento-medico-backend.json, execute: Publisher.OpenShift.exe des-atendimento-medico-backend

Se quiser usar somente o arquivo padrão (appsettings.json), chame o executável sem passar argumento.
Execute: Publisher.OpenShift.exe


# Tipo e Versão

Windows Application / .NET Core 6.0
