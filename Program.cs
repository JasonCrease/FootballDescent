using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballDescent
{
    class Player
    {
        public string Name { get; private set; }
        public double Quality;
        public int GamesPlayed = 0;

        public Player(string name, double quality)
        {
            Name = name;
            Quality = quality;
        }
    }

    class Game
    {
        public double GoalDiff { get; set; }
        public Player[] TA = new Player[5];
        public Player[] TB = new Player[5];
    }

    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            CsvParser parser = new CsvParser();
            parser.Go();

            for (int i = 0; i < 5000000; i++)
                GradDown(parser.Games);

            foreach (Player p in parser.Players.Where(x => x.Quality != 0).OrderByDescending(x => x.Quality))
                Console.WriteLine("{0, 11} {1, 5}", p.Name, p.Quality.ToString("0.00"));

            Console.ReadLine();
        }

        private static void GradDown(Game[] games)
        {
            Game g = games[random.Next(0, games.Length)];

            double pred = 0f;

            //if (!g.TA.All(x => x.GamesPlayed > 3)) return;
            //if (!g.TB.All(x => x.GamesPlayed > 3)) return;

            for (int i = 0; i < 5; i++)
            {
                pred += g.TA[i].Quality;
                pred -= g.TB[i].Quality;
            }

            double diff = Math.Abs(g.GoalDiff) - Math.Abs(pred);
            diff = diff * diff;
            diff = diff * Math.Sign(g.GoalDiff - pred);
            diff = diff / 10000f;

            for (int i = 0; i < 5; i++)
            {
                g.TA[i].Quality += diff;
                g.TB[i].Quality -= diff;
            }
        }
    }
}
