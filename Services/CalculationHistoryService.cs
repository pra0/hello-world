using Calculator.Data;
using Calculator.Models;
using Microsoft.EntityFrameworkCore;

namespace Calculator.Services
{
    public interface ICalculationHistoryService
    {
        Task SaveCalculationAsync(string userId, string expression, double result);
        Task<List<CalculationHistory>> GetUserHistoryAsync(string userId, int maxRecords = 10);
    }

    public class CalculationHistoryService : ICalculationHistoryService
    {
        private readonly ApplicationDbContext _context;

        public CalculationHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveCalculationAsync(string userId, string expression, double result)
        {
            try
            {
                // Get current count for this user
                var currentCount = await _context.CalculationHistories
                    .CountAsync(h => h.UserId == userId);

                // If we're at or above the limit, remove oldest entries
                if (currentCount >= 10)
                {
                    var toRemove = await _context.CalculationHistories
                        .Where(h => h.UserId == userId)
                        .OrderBy(h => h.CreatedAt)
                        .Take(currentCount - 9) // Keep only 9, so we can add 1 more
                        .ToListAsync();

                    _context.CalculationHistories.RemoveRange(toRemove);
                }

                // Add new calculation
                var calculation = new CalculationHistory
                {
                    UserId = userId,
                    Expression = expression,
                    Result = result,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CalculationHistories.Add(calculation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - history saving shouldn't break calculator functionality
                Console.WriteLine($"Error saving calculation history: {ex.Message}");
            }
        }

        public async Task<List<CalculationHistory>> GetUserHistoryAsync(string userId, int maxRecords = 10)
        {
            try
            {
                return await _context.CalculationHistories
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(maxRecords)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving calculation history: {ex.Message}");
                return new List<CalculationHistory>();
            }
        }
    }
}