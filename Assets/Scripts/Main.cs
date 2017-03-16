using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	public static int num_neurons = 10;

	public float spring_const = 1000;
	public float damper = 100;

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

	private RNN net;
	private GenAlg gen_alg;

	// Use this for initialization
	void Start () {
		//Assign joint references
		hip_right = upper_right.GetComponent<ConfigurableJoint> ();
		knee_right = lower_right.GetComponent<HingeJoint> ();
		hip_left = upper_left.GetComponent<ConfigurableJoint> ();
		knee_left = lower_left.GetComponent<HingeJoint> ();

		gen_alg = new GenAlg ();
		net = new RNN (gen_alg.GetCurrentChrom()); 

		Debug.Log ("weights: "+net.weights.ToString ());
		counter = 0;
	}
	
	// FixedUpdate is called every 0.02s
	void FixedUpdate () {
		net.Update ();
		SetAngles (net.GetOutputs());

		counter += Time.deltaTime;
		if (counter > 5) {
			float cost = Evaluate ();
			net.current_chrom.cost = cost;
			net.SetChrom(gen_alg.GetNextChrom ());

			Reset ();
			counter = 0;
		}



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

		//Debugging
		/*
		float a0 = net_outputs [0] * 90;
		float a1 = net_outputs [1] * 90;
		float a2 = net_outputs [2] * 90;
		float a3 = net_outputs [3] * 90 - 45;
		float a4 = net_outputs [4] * 90 - 45;
		float a5 = net_outputs [5] * 90 - 45;

		Debug.Log ("angles: "+a0+" "+a1+" "+a2+" "+a3+" "+a4+" "+a5);		
		*/
	}

	private void Reset() {
		//Places legs at initial position

		//Set all joint target angles to zero
		Quaternion quat = Quaternion.Euler (0, 0, 0);
		JointSpring spring = new JointSpring ();
		spring.spring = spring_const;
		spring.damper = damper;

		hip_right.targetRotation  = quat;
		knee_right.spring = spring;

		hip_left.targetRotation = quat;
		knee_left.spring = spring;
	
		//Set rotations of all body parts to zero
		body.transform.rotation = quat;
		upper_right.transform.rotation = quat;
		lower_right.transform.rotation = quat;
		upper_left.transform.rotation = quat;
		lower_left.transform.rotation = quat;

		//Set positions of all parts to inital ones
		body.transform.position = new Vector3 (1, 5, 0);
		upper_right.transform.position = new Vector3 (2, 4, 0);
		lower_right.transform.position = new Vector3 (2, 1, 0);
		upper_left.transform.position = new Vector3 (0, 4, 0);
		lower_left.transform.position = new Vector3 (0, 1, 0);
	
		//Set velocity of all parts to zero
		ResetVelocity (body);
		ResetVelocity (upper_right);
		ResetVelocity (lower_right);
		ResetVelocity (upper_left);
		ResetVelocity (lower_left);

		//Set net activities to zero
		net.ResetActivities ();
	}

	private void ResetVelocity(GameObject obj) {
		Rigidbody rb = obj.GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
	}

	private float Evaluate() {
		return 4f;
	}
}


