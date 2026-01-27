namespace Prova.Sample
{
    /// <summary>
    /// Sample calculations service.
    /// </summary>
    public class Calculations
    {
        /// <summary>Adds two numbers.</summary>
        public static int Add(int a, int b) => a + b;
        
        /// <summary>Multiplies two numbers.</summary>
        public static int Multiply(int a, int b) => a * b;

        /// <summary>Divides two numbers asynchronously.</summary>
        public static async Task<int> DivideAsync(int a, int b)
        {
            await Task.Delay(10);
            if (b == 0) throw new DivideByZeroException();
            return a / b;
        }
    }
}
