using Microsoft.ML.Data;

namespace AutoTTU.ML
{
    public class CheckInData
    {
        [LoadColumn(0)]
        public string Observacao { get; set; }

        [LoadColumn(1)]
        public bool RiscoAlto { get; set; } // 1 = risco alto, 0 = risco baixo
    }

    public class CheckinPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool RiscoAlto { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
