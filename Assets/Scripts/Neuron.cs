using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron {
	public float activity;
	public float rate;

	public float t_const;
	public float bias;

	public float output;

	public Neuron() {
		activity = 0f;
		rate = 0f;
		bias = 3f;
		t_const = 0.5f;
		output = GetOutput();
	}

	public void SetRate(float r) {
		rate = r;
		activity += rate;
	}

	public float GetOutput() {
		output = 1/(1 + Mathf.Exp (bias - activity)); 
		return output;
	}

	public void SetParams(float t, float b) {
		t_const = t;
		bias = b;
	}
}
