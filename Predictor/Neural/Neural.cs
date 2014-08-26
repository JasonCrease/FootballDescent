using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictor
{
    public class Neural : IPredictor
    {
        private int m_GameCount = 0;
        private int m_PlayerCount = 0;

        private double[][] WhoPlayed;
        private double[] Results;

        private double[] GetWhoPlayed(int gameNum)
        {
            return WhoPlayed[gameNum];
        }

        private double GetGameResult(int gameNum)
        {
            return Results[gameNum];
        }

        private BackPropNeuralNet m_Bnn;
        private Player[] m_Players;

        public void Configure(IEnumerable<Game> games, IEnumerable<Player> players)
        {
            m_Players = players.ToArray();
            m_GameCount = games.Count();
            m_PlayerCount = m_Players.Length;

            Random rnd = new Random();
            BuildGameData(games.ToArray(), m_Players);

            int numInput = m_PlayerCount;
            int numHidden = 6;
            int numOutput = 1;
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + (numHidden + numOutput);

            Console.WriteLine("Creating a " + numInput + "-input, " + numHidden + "-hidden, " + numOutput + "-output neural network");
            Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
            Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");

            m_Bnn = new BackPropNeuralNet(numInput, numHidden, numOutput);

            Console.WriteLine("\nGenerating random initial weights and bias values");
            double[] initWeights = new double[numWeights];
            for (int i = 0; i < initWeights.Length; ++i)
                initWeights[i] = (rnd.NextDouble() - 0.5d) * 0.1d;

            //Console.WriteLine("\nInitial weights and biases are:");
            //Helpers.ShowVector(initWeights, 3, 8, true);

            Console.WriteLine("Loading neural network initial weights and biases into neural network");
            m_Bnn.SetWeights(initWeights);

            double learnRate = 0.03;  // learning rate - controls the maginitude of the increase in the change in weights.
            double momentum = learnRate / 10d; // momentum - to discourage oscillation.
            Console.WriteLine("Setting learning rate = " + learnRate.ToString("F2") + " and momentum = " + momentum.ToString("F2"));

            int maxEpochs = 8000000;
            double errorThresh = 0.01;
            Console.WriteLine("\nSetting max epochs = " + maxEpochs + " and error threshold = " + errorThresh.ToString("F6"));

            int epoch = 0;
            double error = double.MaxValue;
            Console.WriteLine("\nBeginning training using back-propagation\n");

            while (epoch < maxEpochs) // train
            {
                int gameNum = rnd.Next(m_GameCount);  //leave 10 games for prediction
                double[] whoPlayed = GetWhoPlayed(gameNum);
                double result = GetGameResult(gameNum);
                double[] predictedResult = m_Bnn.ComputeOutputs(whoPlayed);

                m_Bnn.UpdateWeights(new[] { result }, learnRate, momentum);
                ++epoch;

                if (epoch % 20000 == 0)
                {
                    error = GetTotalError(m_Bnn, true);

                    if (error < errorThresh)
                    {
                        Console.WriteLine("Found weights and bias values that meet the error criterion at epoch " + epoch);
                        break;
                    }
                    Console.WriteLine("epoch = " + epoch);
                    Console.WriteLine("error = " + error);
                }
            } // train loop

            double[] finalWeights = m_Bnn.GetWeights();
            Console.WriteLine("");
            //Console.WriteLine("Final neural network weights and bias values are:");
            //Helpers.ShowVector(finalWeights, 5, 8, true);
        }

        private void BuildGameData(Game[] games, Player[] players)
        {
            m_PlayerCount = players.Count();
            Results = new double[games.Count()];
            WhoPlayed = new double[games.Count()][];

            for (int i = 0; i < games.Count(); i++)
            {
                Game g = games[i];
                Results[i] = CsvParser.Conv(g.GoalDiff);

                WhoPlayed[i] = new double[m_PlayerCount];
                for (int j = 0; j < m_PlayerCount; j++)
                {
                    if (g.TA.Contains(players[j])) WhoPlayed[i][j] = 1;
                    else if (g.TB.Contains(players[j])) WhoPlayed[i][j] = -1;
                    else WhoPlayed[i][j] = 0;
                }
            }
        }

        private double[] BuildTeam(int[] team1, int[] team2)
        {
            double[] retVector = new double[m_PlayerCount];
            for (int i = 0; i < m_PlayerCount; i++)
            {
                if(team1.Contains(i)) retVector[i] = 1;
                else if(team2.Contains(i)) retVector[i] = -1;
                else retVector[i] = 0;
            }

            return retVector;
        }

        private double GetTotalError(BackPropNeuralNet bnn, bool print)
        {
            double totalError = 0d, totalErrorOnTestData = 0d;
            int score = 0, outof = 0;

            for (int i = 0; i < m_GameCount; i++)
            {
                double[] whoPlayed = GetWhoPlayed(i);
                double predictedResult = CsvParser.RevConv(bnn.ComputeOutputs(whoPlayed)[0]);
                double actualResult = CsvParser.RevConv(GetGameResult(i));
                totalError += (predictedResult - actualResult) * (predictedResult - actualResult);
            }

            if (print)
            {
                Console.WriteLine("MSE is: {0:0.000}", totalError / (double)m_GameCount);
            }

            return totalError;
        }


        public double Predict(Game g)
        {
            double[] whoPlayed = new double[m_PlayerCount];

            for (int j = 0; j < m_PlayerCount; j++)
            {
                if (g.TA.Contains(m_Players[j])) whoPlayed[j] = 1;
                else if (g.TB.Contains(m_Players[j])) whoPlayed[j] = -1;
                else whoPlayed[j] = 0;
            }
            double predictedResult = CsvParser.RevConv(m_Bnn.ComputeOutputs(whoPlayed)[0]);

            return predictedResult;
        }

        public void PrintDebug()
        {

        }
    } // Program


}
