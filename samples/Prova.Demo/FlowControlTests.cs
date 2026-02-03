using System;
using System.Globalization;
using System.Threading.Tasks;
using Prova;


namespace Prova.Demo
{
    public class FlowControlTests
    {
        private static int _repeatCounter = 0;

        [Fact]
        [Repeat(3)]
        public void Repeat_Attribute_Runs_Test_Multiple_Times()
        {
            _repeatCounter++;
            Console.WriteLine($"Repeat execution: {_repeatCounter}/3");
        }

        [Fact]
        [Culture("fr-FR")]
        public void Culture_Attribute_Sets_Current_Culture()
        {
            var culture = CultureInfo.CurrentCulture;
            Console.WriteLine($"Current Culture: {culture.Name}");
            if (culture.Name != "fr-FR")
            {
                throw new InvalidOperationException($"Expected culture fr-FR but got {culture.Name}");
            }
        }

        [Fact]
        [Culture("es-ES")]
        public void Culture_Attribute_Sets_Current_UICulture()
        {
             var uiCulture = CultureInfo.CurrentUICulture;
             Console.WriteLine($"Current UI Culture: {uiCulture.Name}");
             if (uiCulture.Name != "es-ES")
             {
                 throw new InvalidOperationException($"Expected UI culture es-ES but got {uiCulture.Name}");
             }
        }
    }
}
