using AutoTTU.ML;
using AutoTTU.Repository;
using Microsoft.ML;

namespace AutoTTU.ML.ServicesML
{
    /// <summary>
    /// Implementação do serviço para predições de IA
    /// Contém a lógica de Machine Learning para análise de risco
    /// </summary>
    public class IAService : IIAService
    {
        private readonly MLContext _mlContext;
        private readonly ICheckinRepository _checkinRepository;
        private readonly ITransformer _model;

        public IAService(ICheckinRepository checkinRepository)
        {
            _mlContext = new MLContext();
            _checkinRepository = checkinRepository;

            // Carrega e treina o modelo
            var dadosTreino = CarregarDadosTreino().GetAwaiter().GetResult();
            var dataView = _mlContext.Data.LoadFromEnumerable(dadosTreino);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(CheckInData.Observacao))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(CheckInData.RiscoAlto)));

            _model = pipeline.Fit(dataView);
        }

        /// <summary>
        /// Carrega dados do banco para treinamento do modelo
        /// </summary>
        private async Task<List<CheckInData>> CarregarDadosTreino()
        {
            var checkins = await _checkinRepository.GetAllAsync();
            
            var dadosTreino = checkins
                .Where(c => !string.IsNullOrWhiteSpace(c.Observacao))
                .Select(c => new CheckInData
                {
                    Observacao = c.Observacao,
                    RiscoAlto = c.Observacao.Contains("arranhado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("quebrado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("amassado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("riscado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("solto", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("danificado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("danificada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("ruim", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("caído", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("caida", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("quebrada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("amassada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("rachado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("rachada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("quebrada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("travado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("travada", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("desalinhado", StringComparison.OrdinalIgnoreCase)
                            || c.Observacao.Contains("desalinhada", StringComparison.OrdinalIgnoreCase)
                })
                .ToList();

            if (dadosTreino.Count < 2)
            {
                // Evita erro se não tiver dados suficientes - adiciona exemplos balanceados
                dadosTreino = new List<CheckInData>
                {
                    new() { Observacao = "sem observações", RiscoAlto = false },
                    new() { Observacao = "tanque arranhado", RiscoAlto = true }
                };
            }

            return dadosTreino;
        }

        /// <summary>
        /// Prevê o risco de dano com base na observação fornecida
        /// </summary>
        /// <param name="observacao">Observação do checkin a ser analisada</param>
        /// <returns>Predição com risco alto e probabilidade</returns>
        public Task<CheckinPrediction> PreverRiscoAsync(string observacao)
        {
            if (string.IsNullOrWhiteSpace(observacao))
            {
                return Task.FromResult(new CheckinPrediction
                {
                    RiscoAlto = false,
                    Probability = 0.0f
                });
            }

            var engine = _mlContext.Model.CreatePredictionEngine<CheckInData, CheckinPrediction>(_model);
            var resultado = engine.Predict(new CheckInData { Observacao = observacao });

            // Verifica palavras-chave diretamente como fallback se o modelo não detectar
            var palavrasRiscoAlto = new[] { "arranhado", "quebrado", "amassado", "riscado", "solto", 
                "danificado", "danificada", "ruim", "caído", "caida", "quebrada", "amassada", 
                "rachado", "rachada", "travado", "travada", "desalinhado", "desalinhada" };
            
            var temRiscoAlto = palavrasRiscoAlto.Any(palavra => 
                observacao.Contains(palavra, StringComparison.OrdinalIgnoreCase));

            // Se a detecção por palavras-chave indica risco alto, mas o modelo não detectou,
            if (temRiscoAlto && !resultado.RiscoAlto)
            {
                resultado.RiscoAlto = true;
                // Se a probabilidade está muito baixa, ajusta para um valor mais razoável
                if (resultado.Probability < 0.5f)
                {
                    resultado.Probability = 0.7f; 
                }
            }

            // Ajusta a probabilidade se ela estiver muito baixa mas o RiscoAlto é true
            if (resultado.RiscoAlto && resultado.Probability < 0.5f)
            {
                
                resultado.Probability = Math.Max(resultado.Probability, 0.6f);
            }

            return Task.FromResult(resultado);
        }
    }
}
