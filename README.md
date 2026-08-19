/// Características Técnicas Avançadas
/// Modelo Físico Implementado:

/*
    Sistema de Referência 3D
        - Coordenadas Norte, Leste, Altitude
        - Vento em componentes Norte e Leste
*/

/*
    Atmosfera Padrão
        - Variação de temperatura com altitude (-0.0065 K/m)
        - Variação de pressão com altitude
        - Umidade relativa incluída
*/

/*
    Aerodinâmica
        - Arrasto dependente de Mach
        - Coeficiente balístico (BC) para modelagem simplificada
        - Área transversal do projétil
*/

/*
    Forças Consideradas
        - Gravidade (variação com altitude)
        - Arrasto aerodinâmico
        - Força de Coriolis (rotação da Terra)
*/

/*
    Integração Numérica
        - Método Runge-Kutta 4ª ordem (precisão O(h⁴))
        - Passo fixo de 0.01 segundos
*/

/*
    Saída de Dados:
        - Visualização: Gráfico de trajetória 2D
        - Tabela de Resultados: 10 parâmetros de impacto
        - Exportação: CSV/TXT com todos os dados
        - Status: Feedback em tempo real da simulação
*/
