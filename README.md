## Características Técnicas Avançadas

### Modelo Físico Implementado

```yaml
Sistema de Referência 3D:
  - Coordenadas: Norte, Leste, Altitude
  - Vento: componentes Norte e Leste

Atmosfera Padrão:
  - Variação de temperatura: -0.0065 K/m
  - Variação de pressão: com altitude
  - Umidade relativa: incluída

Aerodinâmica:
  - Arrasto: dependente de Mach
  - Coeficiente balístico (BC): modelagem simplificada
  - Área transversal: do projétil

Forças Consideradas:
  - Gravidade: variação com altitude
  - Arrasto aerodinâmico
  - Força de Coriolis: rotação da Terra

Integração Numérica:
  - Método: Runge-Kutta 4ª ordem (O(h⁴))
  - Passo fixo: 0.01 segundos

Saída de Dados:
  - Visualização: gráfico de trajetória 2D
  - Tabela: 10 parâmetros de impacto
  - Exportação: CSV/TXT
  - Status: feedback em tempo real
```
