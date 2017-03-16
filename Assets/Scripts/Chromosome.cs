using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chromosome {
	public float[] weights;
	public float[] biases;
	public float[] t_consts;

	public float cost;

	public Chromosome() {
		weights = new float[100];
		biases = new float[10];
		t_consts = new float[10];
		cost = Mathf.Infinity;

		for (int i = 0; i < 100; i++) {
			if (i < 10) {
				t_consts [i] = Random.Range (-0.5f, 5f);
				biases [i] = Random.Range (-4f, 4f);
			}
			weights [i] = Random.Range (-16f, 16);
		}
	}

	public void Mutate() {
	}
}
