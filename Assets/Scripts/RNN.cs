using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNN {
	private int num_neurons;

	public Matrix<float> weights;
	public Vector<float> outputs;

	public Neuron[] neurons;

	public RNN() {
		var M = Matrix<float>.Build;
		var V = Vector<float>.Build;

		num_neurons = 10;

		/*weights = M.DenseOfArray(new float[,]{
			{10,  1,  3},
			{1,  10, -2},
			{3,  -2,  3}
		});*/

		//weights = M.Random (10, 10);

		weights = M.Dense (10, 10, (i, j) => Random.Range (-16f, 16f)); 

		//Init neurons
		neurons = new Neuron[num_neurons];
		for (int i = 0; i < num_neurons; i++) {
			neurons[i] = new Neuron();
		}

		outputs = V.DenseOfArray(new float[num_neurons]);
		Debug.Log ("weights: " + weights.ToString ());
	}

	public void Update() {
		Vector<float> sums = weights * outputs;
		for (int i = 0; i < num_neurons; i++) {
			Neuron n = neurons [i];
			outputs [i] = n.GetOutput ();
			float rate = (-n.activity + sums [i])*1/n.t_const;
			n.SetRate (rate);
		}
	}

	public float[] GetOutputs() {
		float[] outputs_arr = outputs.ToArray ();
		return outputs_arr;
	}

	public void SetParams(float[,] param_list) {
		for (int i = 0; i < param_list.GetLength(0); i++) {
			neurons [i].SetParams (param_list [i, 0], param_list [i, 1]);
		}
	}
}
