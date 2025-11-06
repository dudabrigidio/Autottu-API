using AutoTTU.ML;

namespace AutoTTU.ML.ServicesML
{
    /// <summary>
    /// Interface que define os contratos de lógica de negócio para predições de IA
    /// </summary>
    public interface IIAService
    {
        /// <summary>
        /// Prevê o risco de dano com base na observação fornecida
        /// </summary>
        /// <param name="observacao">Observação do checkin a ser analisada</param>
        /// <returns>Predição com risco alto e probabilidade</returns>
        Task<CheckinPrediction> PreverRiscoAsync(string observacao);
    }
}

