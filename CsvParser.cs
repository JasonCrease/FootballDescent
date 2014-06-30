using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootballDescent
{
    class CsvParser
    {
        public Game[] Games { get; private set; }
        public Player[] Players { get; private set; }
        private List<Game> gs = new List<Game>();
        private Dictionary<string, Player> ps = new Dictionary<string, Player>(); 

        public void Go()
        {
            StreamReader sr = new StreamReader(".\\..\\..\\Results.csv");
            string contents = sr.ReadToEnd().Replace("\r", "");
            sr.Dispose();

            string[] x = contents.Split('\n');

            for (int i = 1; i < 481; i++)
            {
                string name = x[i].Split(',')[0];
                if(!ps.ContainsKey(name))
                {
                    ps[name] = new Player(name, 0);
                    ps[name].GamesPlayed = 1;
                }
                else
                {
                    ps[name].GamesPlayed++;
                }
            }

            for (int i = 0; i < 48; i++ )
            {
                Game g = new Game();
                g.GoalDiff = double.Parse(x[i * 10 + 1].Split(',')[2]);

                for (int a = 0; a < 5; a++)
                    g.TA[a] = ps[x[i*10 + a + 1].Split(',')[0]];
                for (int b = 0; b < 5; b++)
                    g.TB[b] = ps[x[i*10 + b + 6].Split(',')[0]];
                gs.Add(g);
            }

            Games = gs.ToArray();
            Players = ps.Select(z => z.Value).ToArray();
        }
    }
}
