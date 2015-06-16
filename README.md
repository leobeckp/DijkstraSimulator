# Descrição do projeto
Simulador do Algoritmo de Dijkstra para grafos, com opção de exibição passo-a-passo.
Desenvolvido em C#, para apresentação em seminário da disciplina SSC0630 - Inteligência Artificial, ministrada pelo Prof. Dr. João Luís Garcia Rosa, na Universidade de São Paulo, campus São Carlos.

# Requerimentos
- .NET Framework 4.0;
- Biblioteca GraphViz (http://www.graphviz.org/), incluída.
- GraphViz C# Wrapper (https://github.com/JamieDixon/GraphViz-C-Sharp-Wrapper), incluído.

# Binários
Na seção "Releases" há algumas versões já compiladas, com os requerimentos inclusos (Menos o .NET Framework 4.0). De preferência, baixe o arquivo com maior versão.

# Como compilar
Para compilar, basta abrir o arquivo Dijkstra.sln (ou Dijkstra/Dijkstra.csproj) no Microsoft Visual Studio e escolher a configuração para compilação (Debug ou Release). Os binários se encontrarão na pasta Dijkstra/bin/[Debug] ou [Release], junto com suas dependências (pasta GraphViz e GraphVizWrapper.dll).

# Como utilizar
A interface do programa é bem intuitiva. Pode-se adicionar, remover ou editar nós clicando nos botões abaixo da lista de nós existentes no grafo. Em cada caso, uma nova janela se abrirá, necessitando apenas que o usuário informe os dados pedidos. Os grafos podem ser salvos e carregados de algum arquivo no disco, no formato específico do programa (.grf).

A simulação é feita escolhendo os parâmetros (nó de início e nó de término), e pode-se optar por obter o resultado de uma vez (“Executar”) ou visualizar passo-a-passo a solução (“Próximo passo” avança a simulação, enquanto “Passo anterior” a retrocede). A solução será gráfica e aparecerá ao lado, de acordo com a legenda especificada em “Legenda”, no menu superior.

Também há a opção de obter o tempo de execução do algoritmo para o grafo carregado com três diferentes implementações da lista de nós, clicando em “Tempo de Execução” no menu superior. Uma janela se abrirá, onde é possível selecionar o nó de início e a quantidade de iterações, usada para fazer a média do tempo de execução, ou seja, o algoritmo é repetido n vezes para se obter uma maior precisão no cálculo.

# Exemplos
Na pasta "Exemplos" há um grafo já pronto, contendo a representação de algumas cidades da Romênia e suas distâncias em linha reta.

# Autores
- Leonardo Beck Prates, Nº USP 7962121
- Marlon Jordan Francisco, Nº USP 8668945
- Mateus Ribeiro Vanzella, Nº USP 8504181
- Vitor Bertozzi da Silva, Nº USP 8622401
