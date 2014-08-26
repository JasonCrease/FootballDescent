using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Predictor;

namespace Runner
{
    class Program
    {
        static double totalSquaredError = 0;
        static double totalLinearError = 0;
        static double totalSignError = 0;
        static double n = 0;

        static void Main(string[] args)
        {
            for (int i = 0; i < 250; i++)
            {
                Console.WriteLine("\n\n***  Trial {0}  ***\n\n", i);
                CsvParser parser = new CsvParser();
                IEnumerable<Game> games = parser.LoadWithGamesOmmitted(4);

                IPredictor predictor = new Descender();
                predictor.Configure(parser.Games, parser.Players);
                predictor.PrintDebug();

                PrintMseError(games, predictor);
            }

            Console.ReadLine();
        }

        private static void PrintMseError(IEnumerable<Game> games, IPredictor predictor)
        {
            foreach (Game g in games)
            {
                double pred = predictor.Predict(g);

                totalSquaredError += (pred - g.GoalDiff) * (pred - g.GoalDiff);
                totalLinearError += Math.Abs(pred - g.GoalDiff);

                if (Math.Sign(g.GoalDiff) == 0) totalSignError += 0.5d;
                else if (Math.Sign(pred) != Math.Sign(g.GoalDiff)) totalSignError += 1d;
                n++;

                Console.WriteLine("Predicted {0:0.00} was {1:0.00}", pred, g.GoalDiff);
            }

            Console.WriteLine("MSE {0:0.00}", totalSquaredError / (double)n);
            Console.WriteLine("MLE {0:0.00}", totalLinearError / (double)n);
            Console.WriteLine("Total Sign Error {0:0.00}", totalSignError / (double)n);
        }
    }
}
