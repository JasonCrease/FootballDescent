using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictor
{
    public class Descender : IPredictor
    {
        static Random random = new Random();
        private IEnumerable<Player> Players { get; set; }

        private static void PrintAverageError(IEnumerable<Game> games, IEnumerable<Player> players)
        {
            foreach (Player p in players.Where(x => x.Quality != 0).OrderByDescending(x => x.Quality))
                Console.WriteLine("{0, 11} {1, 5}", p.Name, p.Quality.ToString("0.00"));

            double totalError = 0d;

            foreach (Game g in games)
            {
                double pred = 0d;

                for (int i = 0; i < 5; i++)
                {
                    pred += g.TA[i].Quality;
                    pred -= g.TB[i].Quality;
                }

                double diff = g.GoalDiff - pred;
                totalError += diff * diff;
            }

            Console.WriteLine("MSE is: {0:0.000}", totalError / (double)games.Count());
        }

        private static void GradDown(Game[] games)
        {
            Game g = games[random.Next(0, games.Length)];

            double pred = 0f;

            for (int i = 0; i < 5; i++)
            {
                pred += g.TA[i].Quality;
                pred -= g.TB[i].Quality;
            }

            double diff = g.GoalDiff - pred;
            diff = diff / 1000f;

            int gameBoundary = 0;
            double teamAValids = 0;
            double teamBValids = 0;

            for (int i = 0; i < 5; i++)
                if (g.TA[i].GamesPlayed > gameBoundary)
                    teamAValids++;
            for (int i = 0; i < 5; i++)
                if (g.TB[i].GamesPlayed > gameBoundary)
                    teamBValids++;

            for (int i = 0; i < 5; i++)
            {
                if (g.TA[i].GamesPlayed > gameBoundary)
                    g.TA[i].Quality += (diff / teamAValids);

                if (g.TB[i].GamesPlayed > gameBoundary)
                    g.TB[i].Quality -= (diff / teamBValids);
            }
        }

        public void Configure(IEnumerable<Game> games, IEnumerable<Player> players)
        {
            Game[] gameArray = games.ToArray();
            Players = players;

            for (int i = 0; i < 4000000; i++)
                GradDown(gameArray);
        }

        public double Predict(Game g)
        {
            double pred = 0f;

            for (int i = 0; i < 5; i++)
            {
                pred += g.TA[i].Quality;
                pred -= g.TB[i].Quality;
            }

            return pred;
        }

        public void PrintDebug()
        {
            foreach (Player p in Players.Where(x => x.Quality != 0).OrderByDescending(x => x.Quality))
                Console.WriteLine("{0, 11} {1, 5}", p.Name, p.Quality.ToString("0.00"));
        }
    }
}
