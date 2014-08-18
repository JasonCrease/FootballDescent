using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBackprop
{
    class Program
    {
        static void Main(string[] args)
        {
            CsvParser csvParser = new CsvParser();
            csvParser.Go();

            int playerCount = 25;
            Random rnd = new Random(1);

            double[] whoPlayed = new double[playerCount];
            double[] playerSkills = new double[playerCount];
            double[] yValues; // outputs

            int numInput = playerCount;
            int numHidden = playerCount;
            int numOutput = 1;
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + (numHidden + numOutput);

            Console.WriteLine("Creating a " + numInput + "-input, " + numHidden + "-hidden, " + numOutput + "-output neural network");
            Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
            Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");

            BackPropNeuralNet bnn = new BackPropNeuralNet(numInput, numHidden, numOutput);

            Console.WriteLine("\nGenerating random initial weights and bias values");
            double[] initWeights = new double[numWeights];
            for (int i = 0; i < initWeights.Length; ++i)
              initWeights[i] = (0.1 - 0.01) * rnd.NextDouble() + 0.01;

            Console.WriteLine("\nInitial weights and biases are:");
            Helpers.ShowVector(initWeights, 3, 8, true);

            Console.WriteLine("Loading neural network initial weights and biases into neural network");
            bnn.SetWeights(initWeights);

            double learnRate = 0.5;  // learning rate - controls the maginitude of the increase in the change in weights.
            double momentum = 0.1; // momentum - to discourage oscillation.
            Console.WriteLine("Setting learning rate = " + learnRate.ToString("F2") + " and momentum = " + momentum.ToString("F2"));

            int maxEpochs = 40000;
            double errorThresh = 0.00001;
            Console.WriteLine("\nSetting max epochs = " + maxEpochs + " and error threshold = " + errorThresh.ToString("F6"));

            int epoch = 0;
            double error = double.MaxValue;
            Console.WriteLine("\nBeginning training using back-propagation\n");

            while (epoch < maxEpochs) // train
            {
                int gameNum = rnd.Next(GameData.GameCount);
                whoPlayed = GameData.GetWhoPlayed(gameNum);
                double result = GameData.GetGameResult(gameNum);
                double[] xValues = Helpers.MultiplyVectors(whoPlayed, playerSkills);

                if (epoch % 100 == 0) Console.WriteLine("epoch = " + epoch);

                error = GetTotalError(playerSkills, bnn);
                if (error < errorThresh)
                {
                    Console.WriteLine("Found weights and bias values that meet the error criterion at epoch " + epoch);
                    break;
                }
                bnn.UpdateWeights(new [] {result}, learnRate, learnRate);
                ++epoch;
            } // train loop

            double[] finalWeights = bnn.GetWeights();
            Console.WriteLine("");
            Console.WriteLine("Final neural network weights and bias values are:");
            Helpers.ShowVector(finalWeights, 5, 8, true);


            Console.ReadLine();
        }

        private static double GetTotalError(double[] playerSkills, BackPropNeuralNet bnn)
        {
            double[] whoPlayed;
            double totalError = 0d;
            for (int i = 0; i < GameData.GameCount; i++)
            {
                whoPlayed = GameData.GetWhoPlayed(i);
                double[] xValues = Helpers.MultiplyVectors(whoPlayed, playerSkills);
                double predictedResult = bnn.ComputeOutputs(xValues)[0];
                double actualResult = GameData.GetGameResult(i);
                totalError += (predictedResult - actualResult) * (predictedResult - actualResult);
            }
            //Console.WriteLine("Total error is: {0}", totalError);
            return totalError;
        }
    } // Program


}
