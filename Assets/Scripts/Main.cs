using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	public float spring_const;
	public float damper;

	public GameObject body;
	public GameObject upper_right;
	public GameObject lower_right;
	public GameObject upper_left;
	public GameObject lower_left;

	private ConfigurableJoint 	hip_right;
	private HingeJoint 			knee_right;

	private ConfigurableJoint 	hip_left;
	private HingeJoint 			knee_left;

	private float counter;

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
		spring_const = 1000;
		damper = 100;
		t0 = 0.5f;
		t1 = 1.5f;
		t2 = 0.5f;

		b0 = 3f;
		b1 = -2f;
		b2 = 3f;

		hip_right = upper_right.GetComponent<ConfigurableJoint> ();
		knee_right = lower_right.GetComponent<HingeJoint> ();
		hip_left = upper_left.GetComponent<ConfigurableJoint> ();
		knee_left = lower_left.GetComponent<HingeJoint> ();
	
		net = new RNN ();

		Debug.Log (net.weights.ToString ());
		counter = 0;
		SetAngles (new float[]{0f, 1f, 0.25f, 0.5f, 0.5f, 0.5f});
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		net.SetNeuronParams (new float[,] {
			{t0, b0},
			{t1, b1},
			{t2, b2}
		});
		net.Update ();

		counter += Time.deltaTime;
		if (counter > 3) {
			Reset ();
			counter = 0;
		}

		SetAngles (net.GetOutputs());

		//Debugging
		//System.IO.File.AppendAllText("C:/UnityLogs/logRNN.txt", net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]+"\n");
		//Debug.Log ("net outputs: "+net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]);

		//System.IO.File.AppendAllText("C:/UnityLogs/logRNN.txt", net.GetActivities ()[0]+" "+net.GetActivities ()[1]+" "+net.GetActivities ()[2]+" "+net.GetActivities ()[3]+" "+net.GetActivities ()[4]+" "+net.GetActivities ()[5]+"\n");
		//Debug.Log ("net activities: "+net.GetActivities ()[0]+" "+net.GetActivities ()[1]+" "+net.GetActivities ()[2]+" "+net.GetActivities ()[3]+" "+net.GetActivities ()[4]+" "+net.GetActivities ()[5]);
	}

	private void SetAngles(float[] net_outputs) {
		JointSpring spring_right = new JointSpring ();
		spring_right.spring = spring_const;
		spring_right.damper = damper;
		spring_right.targetPosition = net_outputs [0] * 90;

		JointSpring spring_left = new JointSpring ();
		spring_left.spring = spring_const;
		spring_left.damper = damper;
		spring_left.targetPosition = net_outputs [1] * 90;

		Quaternion quat_right = Quaternion.Euler (net_outputs [2] * 90 - 45, 0, net_outputs [3] * 90 - 45); 
		Quaternion quat_left = Quaternion.Euler (net_outputs [4] * 90 - 45, 0, net_outputs [5] * 90 - 45);

		hip_right.targetRotation = quat_right;
		hip_left.targetRotation = quat_left;
		knee_right.spring = spring_right;
		knee_left.spring = spring_left;
	}

	private void Reset() {
		//Set all target angles to zero
		Quaternion quat = Quaternion.Euler (0, 0, 0);
		JointSpring spring = new JointSpring ();
		spring.spring = spring_const;
		spring.damper = damper;

		hip_right.targetRotation  = quat;
		knee_right.spring = spring;

		hip_left.targetRotation = quat;
		knee_left.spring = spring;
	
		body.transform.position = new Vector3 (1, 5, 0);
		body.transform.rotation = quat;

		upper_right.transform.position = new Vector3 (2, 4, 0);
		upper_right.transform.rotation = quat;
		lower_right.transform.position = new Vector3 (2, 1, 0);
		lower_right.transform.rotation = quat;

		upper_left.transform.position = new Vector3 (0, 4, 0);
		upper_left.transform.rotation = quat;
		lower_left.transform.position = new Vector3 (0, 1, 0);
		lower_left.transform.rotation = quat;
	
		ResetVelocity (body);
		ResetVelocity (upper_right);
		ResetVelocity (lower_right);
		ResetVelocity (upper_left);
		ResetVelocity (lower_left);
	}

	private void ResetVelocity(GameObject obj) {
		Rigidbody rb = obj.GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
	}
}


