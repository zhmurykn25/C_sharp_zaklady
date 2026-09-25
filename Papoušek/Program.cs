namespace Papoušek
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    break;

                Console.WriteLine(input);
            }
        }
    }
}
