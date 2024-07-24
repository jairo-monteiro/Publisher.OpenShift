# Publisher.OpenShift

Aplicação para fazer a publicação dos produtos na plataforma OpenShift.
Essa aplicação realiza todos os passos necessários para publicar o projeto no respectivo container, bem como a gravação dos logs de cada processo.
Os passos são os seguintes:<br>
-> 0 - Copiando pacotes Nuget para o SabespNugetHomolog;<br>
-> 1 - Definindo branchs do processo (checkout, fetch e pull);<br>
-> 2 - Apagando arquivos da pasta destino;<br>
-> 3 - Copiando arquivos da pasta origem;<br>
-> 4 - Comitando e Subindo arquivos no Git Destino;<br>
-> 5 - Iniciando StartBuild no OpenShift;

Ele é totalmente adaptável para várias publicações (projeto), desde que cada projeto tenha sua configuração (appsettings.json) específica.
<br>


# Ajuda - Publicador do OpenShift

Inicie o aplicativo passando qual a configuração para ser usada.
Para usar o arquivo de configuração: appsettings.des-atendimento-medico-backend.json, execute: Publisher.OpenShift.exe des-atendimento-medico-backend

Se quiser usar somente o arquivo padrão (appsettings.json), chame o executável sem passar argumento.
Execute: Publisher.OpenShift.exe
<br>


# Tipo e Versão

Windows Application / .NET Core 6.0
