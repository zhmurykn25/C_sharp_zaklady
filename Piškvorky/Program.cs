namespace Piškvorky
{
    internal class Program
    {
        static char[] pole = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        static void Main(string[] args)
        {
            Console.WriteLine("=== PIŠKVORKY 3x3 ===\n");
            Console.WriteLine("Ty hrašš za X, počítač za O\n");

            bool hraTrvá = true;

            while (hraTrvá)
            {
                VykresliPole();

                // Hráč hraje
                Console.Write("Tvůj tah (1-9): ");
                string vstup = Console.ReadLine();

                if (!int.TryParse(vstup, out int tah) || tah < 1 || tah > 9)
                {
                    Console.WriteLine("Neplatný vstup!\n");
                    continue;
                }

                if (pole[tah - 1] != ((char)('0' + tah)))
                {
                    Console.WriteLine("Toto pole je obsazeno!\n");
                    continue;
                }

                pole[tah - 1] = 'X';

                // Kontrola výhry hráče
                if (KontrolaraVyhry('X'))
                {
                    VykresliPole();
                    Console.WriteLine("\n🎉 VÍTĚZSTVÍ! Vyhráls!\n");
                    hraTrvá = false;
                    break;
                }

                // Kontrola remízy
                if (JeRemíza())
                {
                    VykresliPole();
                    Console.WriteLine("\n🤝 REMÍZA!\n");
                    hraTrvá = false;
                    break;
                }

                // Počítač hraje
                Console.WriteLine("\nPočítač hraje...\n");
                int tahPocitace = NajdiNejlepsiTah();
                pole[tahPocitace] = 'O';

                // Kontrola výhry počítače
                if (KontrolaraVyhry('O'))
                {
                    VykresliPole();
                    Console.WriteLine("\n💀 PROHRA! Počítač vyhrál!\n");
                    hraTrvá = false;
                    break;
                }

                // Kontrola remízy
                if (JeRemíza())
                {
                    VykresliPole();
                    Console.WriteLine("\n🤝 REMÍZA!\n");
                    hraTrvá = false;
                    break;
                }
            }

            Console.Write("Chceš hrát znovu? (a/n): ");
            if (Console.ReadLine().ToLower() == "a")
            {
                pole = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                Main(args);
            }
        }

        static void VykresliPole()
        {
            Console.Clear();
            Console.WriteLine("=== PIŠKVORKY ===\n");
            Console.WriteLine($" {pole[0]} | {pole[1]} | {pole[2]} ");
            Console.WriteLine("---+---+---");
            Console.WriteLine($" {pole[3]} | {pole[4]} | {pole[5]} ");
            Console.WriteLine("---+---+---");
            Console.WriteLine($" {pole[6]} | {pole[7]} | {pole[8]} \n");
        }

        static bool KontrolaraVyhry(char hrac)
        {
            // Řádky
            if ((pole[0] == hrac && pole[1] == hrac && pole[2] == hrac) ||
                (pole[3] == hrac && pole[4] == hrac && pole[5] == hrac) ||
                (pole[6] == hrac && pole[7] == hrac && pole[8] == hrac))
                return true;

            // Sloupce
            if ((pole[0] == hrac && pole[3] == hrac && pole[6] == hrac) ||
                (pole[1] == hrac && pole[4] == hrac && pole[7] == hrac) ||
                (pole[2] == hrac && pole[5] == hrac && pole[8] == hrac))
                return true;

            // Diagonály
            if ((pole[0] == hrac && pole[4] == hrac && pole[8] == hrac) ||
                (pole[2] == hrac && pole[4] == hrac && pole[6] == hrac))
                return true;

            return false;
        }

        static bool JeRemíza()
        {
            foreach (char c in pole)
            {
                if (c >= '1' && c <= '9')
                    return false;
            }
            return true;
        }

        static int NajdiNejlepsiTah()
        {
            // Pokud počítač může vyhrát, vyhraje
            for (int i = 0; i < 9; i++)
            {
                if (pole[i] >= '1' && pole[i] <= '9')
                {
                    pole[i] = 'O';
                    if (KontrolaraVyhry('O'))
                    {
                        pole[i] = (char)('1' + i);
                        return i;
                    }
                    pole[i] = (char)('1' + i);
                }
            }

            // Pokud hráč může vyhrát, zablokuj ho
            for (int i = 0; i < 9; i++)
            {
                if (pole[i] >= '1' && pole[i] <= '9')
                {
                    pole[i] = 'X';
                    if (KontrolaraVyhry('X'))
                    {
                        pole[i] = (char)('1' + i);
                        return i;
                    }
                    pole[i] = (char)('1' + i);
                }
            }

            // Preferuj střed
            if (pole[4] >= '1' && pole[4] <= '9')
                return 4;

            // Preferuj rohy
            int[] rohy = { 0, 2, 6, 8 };
            foreach (int roh in rohy)
            {
                if (pole[roh] >= '1' && pole[roh] <= '9')
                    return roh;
            }

            // Jinak zvol prvovolné prázdné pole
            for (int i = 0; i < 9; i++)
            {
                if (pole[i] >= '1' && pole[i] <= '9')
                    return i;
            }

            return -1;
        }
    }
}
