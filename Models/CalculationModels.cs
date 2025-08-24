using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class CalculationRequest
    {
        [Required]
        public string Expression { get; set; } = string.Empty;
    }
    
    public class CalculationResult
    {
        public double Result { get; set; }
        public string Expression { get; set; } = string.Empty;
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}