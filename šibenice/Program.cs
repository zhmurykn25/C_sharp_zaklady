namespace šibenice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Načtení slov ze souboru
            string cestslovniku = "slovnik.txt";

            // Pokud soubor není v aktuálním adresáři, hledej jej v projektovém adresáři
            if (!File.Exists(cestslovniku))
            {
                cestslovniku = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "šibenice", "slovnik.txt");
            }

            string[] slovnik = NacteniSlovnika(cestslovniku);

            if (slovnik.Length == 0)
            {
                Console.WriteLine("Chyba: Soubor slovnik.txt je prázdný!");
                return;
            }

            Random random = new Random();
            string slovo = slovnik[random.Next(slovnik.Length)].ToLower();

            HashSet<char> uhodnuta = new HashSet<char>();
            HashSet<char> chybna = new HashSet<char>();
            int pocetChyb = 0;
            int maxChyb = 6;

            Console.WriteLine("=== ŠIBENICE ===\n");

            while (pocetChyb < maxChyb)
            {
                VykresliSibenici(pocetChyb);
                VykresliSlovo(slovo, uhodnuta);
                Console.WriteLine($"\nChybná písmena: {string.Join(", ", chybna)}");
                Console.WriteLine($"Zbývá pokusů: {maxChyb - pocetChyb}\n");

                Console.Write("Uhádni písmeno: ");
                string vstup = Console.ReadLine().ToLower();

                if (string.IsNullOrEmpty(vstup) || vstup.Length != 1 || !char.IsLetter(vstup[0]))
                {
                    Console.WriteLine("Zadej jedno písmeno!\n");
                    continue;
                }

                char pismeno = vstup[0];

                if (uhodnuta.Contains(pismeno) || chybna.Contains(pismeno))
                {
                    Console.WriteLine("Toto písmeno jsi už zkusil!\n");
                    continue;
                }

                if (slovo.Contains(pismeno))
                {
                    uhodnuta.Add(pismeno);
                    Console.WriteLine("✓ Správně!\n");
                }
                else
                {
                    chybna.Add(pismeno);
                    pocetChyb++;
                    Console.WriteLine("✗ Špatně!\n");
                }

                // Kontrola výhry
                if (VsechnaUhodnuta(slovo, uhodnuta))
                {
                    VykresliSibenici(pocetChyb);
                    VykresliSlovo(slovo, uhodnuta);
                    Console.WriteLine("\n🎉 VÍTĚZSTVÍ! Uhádl jsi slovo: " + slovo);
                    break;
                }
            }

            if (pocetChyb >= maxChyb)
            {
                VykresliSibenici(pocetChyb);
                Console.WriteLine("\n💀 PROHRA! Slovo bylo: " + slovo);
            }
        }

        static void VykresliSibenici(int chyby)
        {
            Console.Clear();
            string[] sibenice = {
                "  +---+\n  |   |\n      |\n      |\n      |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n      |\n      |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n  |   |\n      |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n /|   |\n      |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n /|\\  |\n      |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n /|\\  |\n /    |\n      |\n=========",
                "  +---+\n  |   |\n  O   |\n /|\\  |\n / \\  |\n      |\n========="
            };

            Console.WriteLine(sibenice[Math.Min(chyby, 6)]);
        }

        static void VykresliSlovo(string slovo, HashSet<char> uhodnuta)
        {
            Console.Write("\nSlovo: ");
            foreach (char c in slovo)
            {
                if (uhodnuta.Contains(c))
                    Console.Write(c + " ");
                else
                    Console.Write("_ ");
            }
            Console.WriteLine();
        }

        static bool VsechnaUhodnuta(string slovo, HashSet<char> uhodnuta)
        {
            foreach (char c in slovo)
            {
                if (!uhodnuta.Contains(c))
                    return false;
            }
            return true;
        }

        static string[] NacteniSlovnika(string cesta)
        {
            try
            {
                if (!File.Exists(cesta))
                {
                    Console.WriteLine($"Chyba: Soubor '{cesta}' neexistuje!");
                    return Array.Empty<string>();
                }

                // Přečti všechny řádky a odstraň prázdné řádky
                string[] slova = File.ReadAllLines(cesta)
                    .Select(slovo => slovo.Trim())
                    .Where(slovo => !string.IsNullOrEmpty(slovo))
                    .ToArray();

                return slova;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při čtení souboru: {ex.Message}");
                return Array.Empty<string>();
            }
        }
    }
}
