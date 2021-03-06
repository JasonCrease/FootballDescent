﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballDescent
{

    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            CsvParser parser = new CsvParser();
            parser.Go();

            for (int i = 0; i < 5000000; i++)
                GradDown(parser.Games);

            PrintAverageError(parser);

            foreach (Player p in parser.Players.Where(x => x.Quality != 0).OrderByDescending(x => x.Quality))
                Console.WriteLine("{0, 11} {1, 5}", p.Name, p.Quality.ToString("0.00"));
                //Console.WriteLine("{0},{1}", p.Name, p.Quality.ToString("0.00"));

            Console.ReadLine();
        }

        private static void PrintAverageError(CsvParser parser)
        {
            double totalError = 0d;

            foreach (Game g in parser.Games)
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

            Console.WriteLine("MSE is: {0:0.000}", totalError / (double)parser.Games.Length);
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

            double diff = g.GoalDiff - pred;
            //diff = diff * diff * Math.Sign(g.GoalDiff - pred);
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
    }
}
