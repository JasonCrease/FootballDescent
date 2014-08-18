using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBackprop
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

    class GameData
    {
        public static int GameCount {
            get { return WhoPlayed.Length; }
        }

        public static double[][] WhoPlayed;
        public static double[] Results;

        internal static double[] GetWhoPlayed(int gameNum)
        {
            return WhoPlayed[gameNum];
        }

        internal static double GetGameResult(int gameNum)
        {
            return Results[gameNum];
        }

        public static int PlayerCount
        {
            get { return WhoPlayed[0].Length; }
        }

        public static Game[] Games { get; private set; }
        public static Player[] Players { get; private set; }
        private static List<Game> gs = new List<Game>();
        private static Dictionary<string, Player> ps = new Dictionary<string, Player>();

        public static void Build()
        {
            StreamReader sr = new StreamReader(".\\..\\..\\..\\Results.csv");
            string contents = sr.ReadToEnd().Replace("\r", "");
            sr.Dispose();

            string[] x = contents.Split('\n');

            for (int i = 1; i < 1011; i++)
            {
                string name = x[i].Split(',')[0];
                if (!ps.ContainsKey(name))
                {
                    ps[name] = new Player(name, 0);
                    ps[name].GamesPlayed = 1;
                }
                else
                {
                    ps[name].GamesPlayed++;
                }
            }

            for (int i = 0; i < 101; i++)
            {
                Game g = new Game();
                g.GoalDiff = double.Parse(x[i * 10 + 1].Split(',')[2]);

                for (int a = 0; a < 5; a++)
                    g.TA[a] = ps[x[(i * 10) + a + 1].Split(',')[0]];
                for (int b = 0; b < 5; b++)
                    g.TB[b] = ps[x[(i * 10) + b + 6].Split(',')[0]];

                if (g.TA.All(z => z.GamesPlayed > 4) && g.TB.All(z => z.GamesPlayed > 4))
                    gs.Add(g);
            }

            Games = gs.ToArray();
            Players = ps.Select(z => z.Value).Where(z => z.GamesPlayed > 4).ToArray();

            int playerCount = Players.Count();
            Results = new double[Games.Count()];
            WhoPlayed = new double[Games.Count()][];

            for (int i = 0; i < Games.Count(); i++)
            {
                Game g = Games[i];
                Results[i] = (g.GoalDiff / 30f);

                WhoPlayed[i] = new double[playerCount];
                for (int j = 0; j < playerCount; j++)
                {
                    if (g.TA.Contains(Players[j])) WhoPlayed[i][j] = 1;
                    else if (g.TB.Contains(Players[j])) WhoPlayed[i][j] = -1;
                    else WhoPlayed[i][j] = 0;
                }
            }
        }
    }
}
