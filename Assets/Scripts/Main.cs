using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	[Range(0.1f, 5f)]
	public float t0 = 0.5f;
	[Range(0.1f, 5f)]
	public float t1 = 0.5f;
	[Range(0.1f, 5f)]
	public float t2 = 0.5f;

	[Range(-4f, 4f)]
	public float b0 = 3f;
	[Range(-4f, 4f)]
	public float b1 = 3f;
	[Range(-4f, 4f)]
	public float b2 = 3f;


	private RNN net;

	// Use this for initialization
	void Start () {
		t0 = 0.5f;
		t1 = 1.5f;
		t2 = 0.5f;

		b0 = 3f;
		b1 = -2f;
		b2 = 3f;

		net = new RNN ();

		Debug.Log (net.weights.ToString ());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		net.SetParams (new float[,] {
			{t0, b0},
			{t1, b1},
			{t2, b2}
		});
		net.Update ();
		System.IO.File.AppendAllText("C:/UnityLogs/logRNN.txt", net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]+"\n");
		Debug.Log ("net outputs: "+net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]);
	}
}
