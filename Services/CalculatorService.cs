using Calculator.Models;

namespace Calculator.Services
{
    public interface ICalculatorService
    {
        CalculationResult Evaluate(string expression);
    }

    public class CalculatorService : ICalculatorService
    {
        public CalculationResult Evaluate(string expression)
        {
            try
            {
                // Clean the expression
                expression = expression.Trim();
                if (string.IsNullOrEmpty(expression))
                {
                    return new CalculationResult
                    {
                        IsError = true,
                        ErrorMessage = "Expression cannot be empty",
                        Expression = expression
                    };
                }

                // Basic security check - only allow numbers, operators, parentheses, and decimal points
                if (!IsValidExpression(expression))
                {
                    return new CalculationResult
                    {
                        IsError = true,
                        ErrorMessage = "Invalid characters in expression",
                        Expression = expression
                    };
                }

                // Use System.Data.DataTable.Compute for safe evaluation
                var table = new System.Data.DataTable();
                var result = table.Compute(expression, null);

                if (result == null || result == DBNull.Value)
                {
                    return new CalculationResult
                    {
                        IsError = true,
                        ErrorMessage = "Invalid expression",
                        Expression = expression
                    };
                }

                return new CalculationResult
                {
                    Result = Convert.ToDouble(result),
                    Expression = expression,
                    IsError = false
                };
            }
            catch (Exception ex)
            {
                return new CalculationResult
                {
                    IsError = true,
                    ErrorMessage = "Error evaluating expression: " + ex.Message,
                    Expression = expression
                };
            }
        }

        private static bool IsValidExpression(string expression)
        {
            // Only allow digits, operators (+, -, *, /), parentheses, decimal points, and spaces
            return expression.All(c => char.IsDigit(c) || 
                                      c == '+' || c == '-' || c == '*' || c == '/' || 
                                      c == '(' || c == ')' || c == '.' || c == ' ');
        }
    }
}