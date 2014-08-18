using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBackprop
{
    class Program2
    {
        static void Main2(string[] args)
        {
            Console.WriteLine("\nBegin Neural Network training using Back-Propagation demo\n");

            Random rnd = new Random(1); // for random weights. not used.

            double[] xValues = new double[3] { 1.0, -2.0, 3.0 }; // inputs
            double[] yValues; // outputs
            double[] tValues = new double[2] { 0.1234, 0.8766 }; // target values

            Console.WriteLine("The fixed input xValues are:");
            Helpers.ShowVector(xValues, 1, 8, true);

            Console.WriteLine("The fixed target tValues are:");
            Helpers.ShowVector(tValues, 4, 8, true);

            int numInput = 3;
            int numHidden = 4;
            int numOutput = 2;
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + (numHidden + numOutput);

            Console.WriteLine("Creating a " + numInput + "-input, " + numHidden + "-hidden, " + numOutput + "-output neural network");
            Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
            Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");

            BackPropNeuralNet bnn = new BackPropNeuralNet(numInput, numHidden, numOutput);

            //Console.WriteLine("\nGenerating random initial weights and bias values");
            //double[] initWeights = new double[numWeights];
            //for (int i = 0; i < initWeights.Length; ++i)
            //  initWeights[i] = (0.1 - 0.01) * rnd.NextDouble() + 0.01;

            Console.WriteLine("\nCreating arbitrary initial weights and bias values");
            double[] initWeights = new double[26] {
          0.001, 0.002, 0.003, 0.004,
          0.005, 0.006, 0.007, 0.008,
          0.009, 0.010, 0.011, 0.012,

          0.013, 0.014, 0.015, 0.016,

          0.017, 0.018,
          0.019, 0.020,
          0.021, 0.022,
          0.023, 0.024,

          0.025, 0.026 };

            Console.WriteLine("\nInitial weights and biases are:");
            Helpers.ShowVector(initWeights, 3, 8, true);

            Console.WriteLine("Loading neural network initial weights and biases into neural network");
            bnn.SetWeights(initWeights);

            double learnRate = 0.5;  // learning rate - controls the maginitude of the increase in the change in weights.
            double momentum = 0.1; // momentum - to discourage oscillation.
            Console.WriteLine("Setting learning rate = " + learnRate.ToString("F2") + " and momentum = " + momentum.ToString("F2"));

            int maxEpochs = 10000;
            double errorThresh = 0.00001;
            Console.WriteLine("\nSetting max epochs = " + maxEpochs + " and error threshold = " + errorThresh.ToString("F6"));

            int epoch = 0;
            double error = double.MaxValue;
            Console.WriteLine("\nBeginning training using back-propagation\n");

            while (epoch < maxEpochs) // train
            {
                if (epoch % 20 == 0) Console.WriteLine("epoch = " + epoch);

                yValues = bnn.ComputeOutputs(xValues);
                error = Helpers.Error(tValues, yValues);
                if (error < errorThresh)
                {
                    Console.WriteLine("Found weights and bias values that meet the error criterion at epoch " + epoch);
                    break;
                }
                bnn.UpdateWeights(tValues, learnRate, learnRate);
                ++epoch;
            } // train loop

            double[] finalWeights = bnn.GetWeights();
            Console.WriteLine("");
            Console.WriteLine("Final neural network weights and bias values are:");
            Helpers.ShowVector(finalWeights, 5, 8, true);

            yValues = bnn.ComputeOutputs(xValues);
            Console.WriteLine("\nThe yValues using final weights are:");
            Helpers.ShowVector(yValues, 8, 8, true);

            double finalError = Helpers.Error(tValues, yValues);
            Console.WriteLine("\nThe final error is " + finalError.ToString("F8"));

            Console.WriteLine("\nEnd Neural Network Back-Propagation demo\n");
            Console.ReadLine();

        } // Main

    } // Program


}
