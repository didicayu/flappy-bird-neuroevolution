using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class nn
{
    int input_nodes;
    int hidden_nodes;
    int output_nodes;

    float learning_rate = 0.1f;

    public Matrix weights_ih;
    public Matrix weights_ho;

    Matrix bias_h;
    Matrix bias_o;

    bool SoftMaxOn;

    public nn(int NumI, int NumH, int NumO, bool EnableSoftMax = false)
    {
        input_nodes = NumI;
        hidden_nodes = NumH;
        output_nodes = NumO;

        weights_ih = new Matrix(hidden_nodes, input_nodes);
        weights_ho = new Matrix(output_nodes, hidden_nodes);

        weights_ih.Randomize();
        weights_ho.Randomize();

        bias_h = new Matrix(hidden_nodes, 1);
        bias_o = new Matrix(output_nodes, 1);
        bias_h.Randomize();
        bias_o.Randomize();

        SoftMaxOn = EnableSoftMax;

        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
    }

    public float[] FeedForward(float[] input_array)
    {

        float[] sofmaxOutput;

        //Generating hidden outputs
        Matrix inputs = Matrix.fromArray(input_array);
        Matrix hidden = Matrix.Multiply(weights_ih, inputs);
        hidden.Add(bias_h);
        //activation function
        hidden.Map(sigmoid);

        //Generating Outputs
        Matrix output = Matrix.Multiply(weights_ho, hidden);
        output.Add(bias_o);
        output.Map(sigmoid);

        
        if(SoftMaxOn){
            sofmaxOutput = softmax(output.toArray());
            return sofmaxOutput; //MAKES SURE ALL THE OUTPUT VALUES ADD UP TO 1 FOR PROBABILTY PURPOSES
        }

        //Sending it back
        return output.toArray(); 

    }

    float sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    float dsigmoid(float y)
    {
        return y * (1 - y);
    }

    float[] softmax(float[] array)
    {
        float sum = 0;
        float[] output = new float[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            sum += Mathf.Exp(array[i]);
        }
        for (int j = 0; j < array.Length; j++)
        {
            output[j] = Mathf.Exp(array[j]) / sum;
        }

        return output;
    }

    public void Train(float[] input_array, float[] target_array)
    {

        //Generating hidden outputs
        Matrix inputs = Matrix.fromArray(input_array);
        Matrix hidden = Matrix.Multiply(weights_ih, inputs);

        hidden.Add(bias_h);
        //activation function
        hidden.Map(sigmoid);

        //Generating Outputs
        Matrix outputs = Matrix.Multiply(weights_ho, hidden);

        outputs.Add(bias_o);
        outputs.Map(sigmoid);

        Matrix TargetsMat = Matrix.fromArray(target_array);

        //Calculate the error
        //ERROR = TARGETS - OUTPUTS
        Matrix output_errors = Matrix.subtract(TargetsMat, outputs);

        //calculate gradient
        Matrix gradients = Matrix.Map(outputs, dsigmoid);
        gradients.Multiply(output_errors);

        gradients.Multiply(learning_rate);

        Matrix hidden_t = Matrix.Transpose(hidden);
        Matrix weight_ho_deltas = Matrix.Multiply(gradients, hidden_t);

        //adjust weights and biases by deltas
        weights_ho.Add(weight_ho_deltas);
        bias_o.Add(gradients);

        //calculate the hidden layer errors
        Matrix weights_ho_t = Matrix.Transpose(weights_ho);

        Matrix hidden_errors = Matrix.Multiply(weights_ho_t, output_errors);

        //calculate hidden gradient
        Matrix hidden_gradient = Matrix.Map(hidden, dsigmoid);

        //Debug.Log("cols mat A: " + hidden_gradient.cols + " rows mat A: "+ hidden_gradient.rows + " rows mat B: " + hidden_errors.rows + " cols mat B: " + hidden_errors.cols);

        hidden_gradient.Multiply(hidden_errors); //matrix dimension error
        hidden_gradient.Multiply(learning_rate);

        //calculate input to hidden deltas
        Matrix inputs_T = Matrix.Transpose(inputs);
        Matrix weight_ih_deltas = Matrix.Multiply(hidden_gradient, inputs_T);


        weights_ih.Add(weight_ih_deltas);
        bias_h.Add(hidden_gradient);


    }

    public void Mutate(Func<float,float> function){

        weights_ih.Map(function);
        weights_ho.Map(function);
        bias_h.Map(function);
        bias_o.Map(function);
        
    }

    public void SaveNeuralNetwork(string FileName)
    {
        FileName = Application.dataPath + "/" + FileName + ".csv";
        //Debug.Log(FileName);

        TextWriter tw = new StreamWriter(FileName, false);

        //Write input-hidden weights matrix
        tw.WriteLine("Input-Hidden");
        for (int i = 0; i < weights_ih.rows; i++)
        {
            for (int j = 0; j < weights_ih.cols; j++)
            {
                tw.Write(weights_ih.matrix[i, j] + ";");
            }
            tw.Write("\n");
        }
        tw.Write("\n");

        //Write hidden-output weights matrix
        tw.WriteLine("Hidden-Output");
        for (int i = 0; i < weights_ho.rows; i++)
        {
            for (int j = 0; j < weights_ho.cols; j++)
            {
                tw.Write(weights_ho.matrix[i, j] + ";");
            }
            tw.Write("\n");
        }
        tw.Write("\n");

        //Write Hidden Bias matrix
        tw.WriteLine("Hidden Bias");
        for (int i = 0; i < bias_h.toArray().Length; i++)
        {
            tw.Write(bias_h.toArray()[i] + ";");
        }
        tw.Write("\n");

        tw.Write("\n");

        //Write Output Bias matrix
        tw.WriteLine("Output Bias");
        for (int i = 0; i < bias_o.toArray().Length; i++)
        {
            tw.Write(bias_o.toArray()[i] + ";");
        }
        tw.Write("\n");

        tw.Close();
    }

    public void LoadNeuralNetwork(string FileName)
    {
        FileName = Application.dataPath + "/" + FileName + ".csv";

        if (File.Exists(FileName))
        {
            string[] Lines = System.IO.File.ReadAllLines(FileName);
            string[] Columns = Lines[1].Split(';');

            string[][] matrixData = new string[Lines.Length][];
    
            for (int i = 0; i <= Lines.Length - 1; i++)
            {
                matrixData[i] = Lines[i].Split(';');
                //Debug.Log(Lines[i]);
            }

            /* ----------->///CSV FILE DEBUG///<-----------

            for (int i = 0; i < matrixData.Length; i++)
            {
                for (int j = 0; j < matrixData[i].Length; j++)
                {
                    Debug.Log("i: " + i +" "+ "j: " + j + " " + matrixData[i][j]);
                }
            }
            -----------------------------------------------
            */

            //Input-Hidden Matrix Load
            for (int i = 0; i < weights_ih.rows; i++)
            {
                for (int j = 0; j < weights_ih.cols; j++)
                {
                    float.TryParse(matrixData[i+1][j], out weights_ih.matrix[i,j]);
                }
            }
            //Hidden-Output Matrix Load
            for (int i = 0; i < weights_ho.rows; i++)
            {
                for (int j = 0; j < weights_ho.cols; j++)
                {
                    float.TryParse(matrixData[i+3+weights_ih.rows][j], out weights_ho.matrix[i,j]);
                }
            }

            //Made to reverse ToArray() flattening effect when saving the biases
            Matrix bias_h_read = new Matrix(1,bias_h.rows);
            Matrix bias_o_read = new Matrix(1,bias_o.rows);

            //Hidden Bias
            for (int i = 0; i < bias_h.toArray().Length; i++)
            {
                float.TryParse(matrixData[5+weights_ih.rows+weights_ho.rows][i], out bias_h_read.matrix[0,i]);
            }
            //Output Bias
            for (int i = 0; i < bias_o.toArray().Length; i++)
            {
                float.TryParse(matrixData[8+weights_ih.rows+weights_ho.rows][i], out bias_o_read.matrix[0,i]);
            }

            bias_h = Matrix.Transpose(bias_h_read);
            bias_o = Matrix.Transpose(bias_o_read);

        }
        else
        {
            Debug.LogError("FILE DOES NOT EXIST!");
        }
    }
}
