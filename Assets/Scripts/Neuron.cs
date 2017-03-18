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
		bias = Random.Range(-4f, 4f);
		t_const = Random.Range(0.5f, 5f);
		output = GetOutput();
	}

	public void SetRate(float r) {
		rate = r;
		activity += rate*Time.deltaTime;
	}

	public float GetOutput() {
		output = 1/(1 + Mathf.Exp (bias - activity)); 
		output = Mathf.Round (output * 100f) / 100f;
		return output;
	}

	public void SetParams(float t, float b) {
		t_const = t;
		bias = b;
	}
}
