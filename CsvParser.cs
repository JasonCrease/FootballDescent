﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FootballDescent
{
    internal class CsvParser
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
                g.GoalDiff = double.Parse(x[i*10 + 1].Split(',')[2]);

                for (int a = 0; a < 5; a++)
                    g.TA[a] = ps[x[(i*10) + a + 1].Split(',')[0]];
                for (int b = 0; b < 5; b++)
                    g.TB[b] = ps[x[(i*10) + b + 6].Split(',')[0]];

                if (g.TA.All(z => z.GamesPlayed > 2) && g.TB.All(z => z.GamesPlayed > 2))
                    gs.Add(g);
            }

            Games = gs.ToArray();
            Players = ps.Select(z => z.Value).ToArray();

            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append(",");
            for (int i = 0; i < Players.Count(); i++)
            {
                sb.Append("," + Players[i].Name);
            }
            sb.AppendLine();

            foreach (Game g in Games)
            {
                sb.Append(g.GoalDiff + ",");
                for (int i = 0; i < Players.Count(); i++)
                {
                    if (g.TA.Contains(Players[i])) sb.Append(", 1");
                    else if (g.TB.Contains(Players[i])) sb.Append(",-1");
                    else sb.Append(", 0");
                }
                sb.AppendLine();
            }

            StreamWriter sw = new StreamWriter(".\\..\\..\\Matrix.csv");
            sw.Write(sb.ToString());
            sw.Close();
        }
    }

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
}
