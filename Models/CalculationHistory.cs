using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class CalculationHistory
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string Expression { get; set; } = string.Empty;
        
        [Required]
        public double Result { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ApplicationUser User { get; set; } = null!;
    }
}