using AutoTTU.ML.ServicesML;
using AutoTTU.Service;
using Microsoft.AspNetCore.Mvc;

namespace AutoTTU.ML.ControllersML
{
    /// <summary>
    /// Controller responsável por gerenciar predições de IA para análise de risco
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class IAController : ControllerBase
    {
        private readonly IIAService _iaService;
        private readonly ICheckinService _checkinService;

        /// <summary>
        /// Inicializa uma nova instância do IAController
        /// </summary>
        /// <param name="iaService">Serviço de IA para predições</param>
        /// <param name="checkinService">Serviço de checkins para buscar dados</param>
        public IAController(IIAService iaService, ICheckinService checkinService)
        {
            _iaService = iaService;
            _checkinService = checkinService;
        }

        /// <summary>
        /// Prevê o risco de dano para todos os checkins realizados e calcula a média das probabilidades
        /// </summary>
        /// <returns>Lista de predições de risco para cada checkin e média das probabilidades</returns>
        [HttpPost("prever-danos")]
        public async Task<ActionResult> PreverDanos()
        {
            try
            {
                var checkins = await _checkinService.GetAllAsync();
                var resultados = new List<object>();
                var probabilidades = new List<float>();
                var quantidadeRiscoAlto = 0;

                foreach (var c in checkins)
                {
                    var pred = await _iaService.PreverRiscoAsync(c.Observacao ?? "");
                    probabilidades.Add(pred.Probability);
                    
                    if (pred.RiscoAlto)
                    {
                        quantidadeRiscoAlto++;
                    }
                    
                    resultados.Add(new
                    {
                        c.IdCheckin,
                        c.IdMoto,
                        c.Observacao,
                        RiscoAlto = pred.RiscoAlto,
                        Probabilidade = pred.Probability
                    });
                }

                // Calcula a média das probabilidades
                var mediaProbabilidade = probabilidades.Count > 0 
                    ? probabilidades.Average() 
                    : 0.0f;

                return Ok(new
                {
                    TotalCheckins = resultados.Count,
                    MediaProbabilidade = mediaProbabilidade,
                    QuantidadeRiscoAlto = quantidadeRiscoAlto,
                    PercentualRiscoAlto = resultados.Count > 0 
                        ? (quantidadeRiscoAlto * 100.0f / resultados.Count) 
                        : 0.0f,
                    Detalhes = resultados
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar predições de risco", error = ex.Message });
            }
        }

        /// <summary>
        /// Prevê o risco de dano para uma observação específica
        /// </summary>
        /// <param name="observacao">Observação a ser analisada</param>
        /// <returns>Predição de risco com probabilidade</returns>
        [HttpPost("prever-risco")]
        public async Task<ActionResult> PreverRisco([FromBody] string observacao)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(observacao))
                {
                    return BadRequest(new { message = "Observação não pode ser vazia" });
                }

                var predicao = await _iaService.PreverRiscoAsync(observacao);
                
                return Ok(new
                {
                    Observacao = observacao,
                    RiscoAlto = predicao.RiscoAlto,
                    Probabilidade = predicao.Probability
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar predição de risco", error = ex.Message });
            }
        }
    }
}
