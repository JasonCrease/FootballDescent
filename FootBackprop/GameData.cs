using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBackprop
{
    class GameData
    {
        public static int GameCount {
            get { return WhoPlayed.Count(); }
        }

        public static double[][] WhoPlayed;

        internal static double[] GetWhoPlayed(int gameNum)
        {
            return WhoPlayed[gameNum];
        }

        internal static double GetGameResult(int gameNum)
        {
            return 0.4;
        }
    }
}
