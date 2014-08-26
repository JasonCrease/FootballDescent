using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBackprop
{
    class Program
    {
        private const int testSize = 10;

        static void Main(string[] args)
        {
            Random rnd = new Random();
            GameData.Build();

            int numInput = GameData.PlayerCount;
            int numHidden = 12; //(int)(GameData.PlayerCount * 0.1d);
            int numOutput = 1;
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + (numHidden + numOutput);

            Console.WriteLine("Creating a " + numInput + "-input, " + numHidden + "-hidden, " + numOutput + "-output neural network");
            Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
            Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");

            BackPropNeuralNet bnn = new BackPropNeuralNet(numInput, numHidden, numOutput);

            Console.WriteLine("\nGenerating random initial weights and bias values");
            double[] initWeights = new double[numWeights];
            for (int i = 0; i < initWeights.Length; ++i)
                initWeights[i] = (rnd.NextDouble() - 0.5d) * 0.1d;

            Console.WriteLine("\nInitial weights and biases are:");
            Helpers.ShowVector(initWeights, 3, 8, true);

            Console.WriteLine("Loading neural network initial weights and biases into neural network");
            bnn.SetWeights(initWeights);

            double learnRate = 0.02;  // learning rate - controls the maginitude of the increase in the change in weights.
            double momentum = 0.0001; // momentum - to discourage oscillation.
            Console.WriteLine("Setting learning rate = " + learnRate.ToString("F2") + " and momentum = " + momentum.ToString("F2"));

            int maxEpochs = 1000000;
            double errorThresh = 0.00001;
            Console.WriteLine("\nSetting max epochs = " + maxEpochs + " and error threshold = " + errorThresh.ToString("F6"));

            int epoch = 0;
            double error = double.MaxValue;
            Console.WriteLine("\nBeginning training using back-propagation\n");

            while (epoch < maxEpochs) // train
            {
                int gameNum = rnd.Next(GameData.GameCount - testSize);  //leave 10 games for prediction
                double[] whoPlayed = GameData.GetWhoPlayed(gameNum);
                double result = GameData.GetGameResult(gameNum);
                double[] predictedResult = bnn.ComputeOutputs(whoPlayed);

                bnn.UpdateWeights(new[] { result }, learnRate, learnRate);
                ++epoch;

                if (epoch % 1000 == 0)
                {
                    error = GetTotalError(bnn, false);
                    if (error < errorThresh)
                    {
                        Console.WriteLine("Found weights and bias values that meet the error criterion at epoch " + epoch);
                        break;
                    }
                    Console.WriteLine("epoch = " + epoch);
                    Console.WriteLine("error = " + error);
                }
            } // train loop

            double[] finalWeights = bnn.GetWeights();
            Console.WriteLine("");
            Console.WriteLine("Final neural network weights and bias values are:");
            Helpers.ShowVector(finalWeights, 5, 8, true);

            GetTotalError(bnn, true);

            Console.ReadLine();

            //int[] team1 = new int[5];
            //int[] team2 = new int[5];

            //team1[0] = GameData.GetIndexOfPlayer("Luke");
            //team1[1] = GameData.GetIndexOfPlayer("Daniel");
            //team1[2] = GameData.GetIndexOfPlayer("MC");
            //team1[3] = GameData.GetIndexOfPlayer("Christian");
            //team1[4] = GameData.GetIndexOfPlayer("Pete");

            //team2[0] = GameData.GetIndexOfPlayer("Ben");
            //team2[1] = GameData.GetIndexOfPlayer("Elwood");
            //team2[2] = GameData.GetIndexOfPlayer("Nigel");
            //team2[3] = GameData.GetIndexOfPlayer("Clive");
            //team2[4] = GameData.GetIndexOfPlayer("Harry");

            //double[] teamsVector = BuildTeam(team1, team2);

            //double[] guess = bnn.ComputeOutputs(teamsVector);

            //Console.WriteLine("Guessed result {0:0.00}", GameData.RevConv(guess[0]));

            //Console.ReadLine();
        }

        private static double[] BuildTeam(int[] team1, int[] team2)
        {
            double[] retVector = new double[GameData.PlayerCount];
            for (int i = 0; i < GameData.PlayerCount; i++)
            {
                if(team1.Contains(i)) retVector[i] = 1;
                else if(team2.Contains(i)) retVector[i] = -1;
                else retVector[i] = 0;
            }

            return retVector;
        }

        private static double GetTotalError(BackPropNeuralNet bnn, bool print)
        {
            double totalError = 0d, totalErrorOnTestData = 0d;
            int score = 0, outof = 0;

            for (int i = 0; i < GameData.GameCount; i++)
            {
                double[] whoPlayed = GameData.GetWhoPlayed(i);
                double predictedResult = GameData.RevConv(bnn.ComputeOutputs(whoPlayed)[0]);
                double actualResult = GameData.RevConv(GameData.GetGameResult(i));
                totalError += (predictedResult - actualResult) * (predictedResult - actualResult);

                if (print)
                {
                    if (i == GameData.GameCount - testSize)
                        Console.WriteLine("---------------------");

                    Console.WriteLine("Predicted {0:0.00} was {1:0.00} ", predictedResult, actualResult);

                    if (i >= GameData.GameCount - testSize)
                    {
                        totalErrorOnTestData += Math.Abs(predictedResult - actualResult) * Math.Abs(predictedResult - actualResult);
                        if (Math.Sign(predictedResult) == Math.Sign(actualResult))
                            score++;
                        outof++;
                    }
                }
            }

            if (print)
            {
                Console.WriteLine("MSE is: {0:0.000}", totalError / (double)GameData.GameCount);
                Console.WriteLine("MSE test is: {0:0.000}", totalErrorOnTestData / (double)outof);
                Console.WriteLine("Accuracy: {0:0.0}%", (score * 100) / outof);
            }

            return totalError;
        }
    } // Program


}
