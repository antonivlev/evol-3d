using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNN {
	public Matrix<float> weights;
	public Vector<float> outputs;

	public Chromosome current_chrom;

	private Neuron[] neurons;

	public RNN(Chromosome chrom) {
		var M = Matrix<float>.Build;
		var V = Vector<float>.Build;

		current_chrom = chrom;

		weights = M.DenseOfRowMajor (10, 10, current_chrom.weights); 

		//Init neurons
		neurons = new Neuron[Main.num_neurons];
		for (int i = 0; i < Main.num_neurons; i++) {
			Neuron n = new Neuron ();
			n.SetParams (current_chrom.t_consts [i], current_chrom.biases [i]);
			neurons [i] = n;
		}

		outputs = V.DenseOfArray(new float[Main.num_neurons]);
	}

	public void Update() {
		//var M = Matrix<float>.Build;
		var V = Vector<float>.Build;

		for (int i = 0; i < Main.num_neurons; i++) {
			Neuron n = neurons [i];
			outputs [i] = n.GetOutput ();
		}

		Matrix<float> sums_matrix = outputs.ToRowMatrix() * weights;
		Vector<float> sums = V.DenseOfArray (sums_matrix.ToRowMajorArray ());
		//Vector<float> sums = weights * outputs;

		for (int i = 0; i < Main.num_neurons; i++) {
			Neuron n = neurons [i];
			float rate = (-n.activity + sums [i])*(1/n.t_const);
			n.SetRate (rate);
		}
	}


	public float[] GetOutputs() {
		float[] outputs_arr = outputs.ToArray ();
		return outputs_arr;
	}


	public float[] GetActivities() {
		float[] activities = new float[Main.num_neurons];
		for (int i = 0; i < Main.num_neurons; i++) {
			Neuron n = neurons [i];
			activities [i] = n.activity;
		}
		return activities;
	}


	public void ResetActivities() {
		for (var i = 0; i < Main.num_neurons; i++) {
			neurons [i].activity = 0;
		}
	}

	public void SetChrom(Chromosome chrom) {
		var M = Matrix<float>.Build;
		var V = Vector<float>.Build;

		current_chrom = chrom;

		weights = M.DenseOfRowMajor (10, 10, current_chrom.weights); 

		//Init neurons
		neurons = new Neuron[Main.num_neurons];
		for (int i = 0; i < Main.num_neurons; i++) {
			Neuron n = new Neuron ();
			n.SetParams (current_chrom.t_consts [i], current_chrom.biases [i]);
			neurons [i] = n;
		}

		outputs = V.DenseOfArray(new float[Main.num_neurons]);
	}
}
