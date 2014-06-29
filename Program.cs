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

        public Player(string name, double quality)
        {
            Name = name;
            Quality = quality;
        }
    }

    class Game
    {
        public double GoalDiff { get; private set; }
        public Player[] TA = new Player[3];
        public Player[] TB = new Player[3];

        public Game(int goalDiff, Player[] players, int a0, int a1, int a2, int b0, int b1, int b2)
        {
            GoalDiff = goalDiff;
            TA[0] = players[a0];
            TA[1] = players[a1];
            TA[2] = players[a2];
            TB[0] = players[b0];
            TB[1] = players[b1];
            TB[2] = players[b2];
        }
    }

    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            Game[] games;
            Player[] players;
            InitGamesAndPlayers(out games, out players);

            for (int goes = 0; goes < 2; goes++)
            {
                for (int i = 0; i < 1000000; i++)
                    GradDown(games);

                foreach(Player p in players.OrderByDescending(x => x.Quality))
                    Console.WriteLine("{0} {1}", p.Name, p.Quality.ToString("0.000"));
            }

            Console.ReadLine();
        }

        private static void GradDown(Game[] games)
        {
            Game g = games[random.Next(0, games.Length)];

            double pred = 0f;

            for (int i = 0; i < 3; i++)
            {
                pred += g.TA[i].Quality;
                pred -= g.TB[i].Quality;
            }

            double diff = (g.GoalDiff - pred) / 100f;

            for (int i = 0; i < 3; i++)
            {
                g.TA[i].Quality += diff;
                g.TB[i].Quality -= diff;
            }
        }

        private static void InitGamesAndPlayers(out Game[] games, out Player[] p)
        {
            p = new Player[10];
            p[0] = new Player("A", 0);
            p[1] = new Player("B", 0);
            p[2] = new Player("C", 0);
            p[3] = new Player("D", 0);
            p[4] = new Player("E", 0);
            p[5] = new Player("F", 0);
            p[6] = new Player("G", 0);
            p[7] = new Player("H", 0);
            p[8] = new Player("I", 0);
            p[9] = new Player("J", 0);

            games = new Game[15];
            games[0] = new Game(2, p, 0, 1, 2, 3, 4, 5);
            games[1] = new Game(1, p, 1, 2, 3, 4, 5, 6);
            games[2] = new Game(0, p, 2, 3, 4, 5, 6, 7);
            games[3] = new Game(-1, p, 3, 4, 5, 6, 7, 8);
            games[4] = new Game(-2, p, 4, 5, 6, 7, 8, 9);
            games[5] = new Game(2, p, 0, 1, 6, 3, 4, 5);
            games[6] = new Game(1, p, 1, 2, 3, 4, 5, 6);
            games[7] = new Game(0, p, 2, 7, 4, 5, 6, 7);
            games[8] = new Game(-1, p, 9, 2, 5, 6, 7, 8);
            games[9] = new Game(-2, p, 2, 5, 6, 7, 8, 0);
            games[10] = new Game(2, p, 9, 8, 2, 6, 4, 5);
            games[11] = new Game(1, p, 1, 9, 3, 0, 5, 6);
            games[12] = new Game(0, p, 9, 8, 4, 1, 6, 7);
            games[13] = new Game(-1, p, 3, 0, 2, 6, 7, 8);
            games[14] = new Game(-2, p, 4, 5, 1, 7, 2, 9);
        }
    }
}
