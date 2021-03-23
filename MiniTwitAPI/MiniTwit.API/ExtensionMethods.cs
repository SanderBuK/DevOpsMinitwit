using System.Threading.Tasks;
using Prometheus;

namespace MiniTwit.API
{
    public static class ExtensionMethods
    {
        public static async Task IncAsync(this Gauge gauge)
        {
            await Task.Run(() => gauge.Inc());
        }
        
        public static async Task DecAsync(this Gauge gauge)
        {
            await Task.Run(() => gauge.Dec());
        }
        
        public static async Task IncToAsync(this Gauge gauge, int to)
        {
            await Task.Run(() => gauge.IncTo(to));
        }
    }
}