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

            for (int goes = 0; goes < 1; goes++)
            {
                for (int i = 0; i < 10000000; i++)
                    GradDown(parser.Games);

                foreach (Player p in parser.Players.OrderByDescending(x => x.Quality))
                    Console.WriteLine("{0, 12} {1}", p.Name, p.Quality.ToString("0.000"));
            }

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

            double diff = (g.GoalDiff - pred) / 1000f;

            for (int i = 0; i < 5; i++)
            {
                if(g.TA[i].GamesPlayed > 5)
                    g.TA[i].Quality += diff;
                if (g.TB[i].GamesPlayed > 5)
                    g.TB[i].Quality -= diff;
            }
        }
    }
}
