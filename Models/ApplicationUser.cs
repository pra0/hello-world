using Microsoft.AspNetCore.Identity;

namespace Calculator.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<CalculationHistory> CalculationHistories { get; set; } = new List<CalculationHistory>();
    }
}