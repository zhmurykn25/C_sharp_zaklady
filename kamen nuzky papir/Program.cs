namespace kamen_nuzky_papir
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            while (true)
            {
                Console.WriteLine("\n1 - Kamen");
                Console.WriteLine("2 - Nuzky");
                Console.WriteLine("3 - Papir");
                Console.WriteLine("0 - Konec");
                Console.Write("Vyber: ");

                string choice = Console.ReadLine();

                if (choice == "0")
                    break;

                if (choice != "1" && choice != "2" && choice != "3")
                {
                    Console.WriteLine("Spatny vstup!");
                    continue;
                }

                int computer = random.Next(1, 4);

                Console.WriteLine($"Ty jsi vybral: {GetName(choice)}");
                Console.WriteLine($"Pocitac si vybral: {GetName(computer.ToString())}");

                string result = GetResult(choice, computer.ToString());
                Console.WriteLine(result);
            }
        }

        static string GetName(string choice)
        {
            return choice switch
            {
                "1" => "Kamen",
                "2" => "Nuzky",
                "3" => "Papir",
                _ => "Neznamy"
            };
        }

        static string GetResult(string player, string computer)
        {
            if (player == computer)
                return "Remiza!";

            if ((player == "1" && computer == "2") ||
                (player == "2" && computer == "3") ||
                (player == "3" && computer == "1"))
                return "Vyhral jsi!";

            return "Vyhral pocitac!";
        }
    }
}
