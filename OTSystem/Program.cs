using EasyModbus;

namespace OTSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IndustrialControlSystem isc = new IndustrialControlSystem();
            isc.Run();
        }
    }
}
