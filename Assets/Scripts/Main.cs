using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Main : MonoBehaviour {
	public static int num_neurons;

	public float spring_const;
	public float damper;

	public static int run;


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
		num_neurons = 10;
		double[] p_list = new double[120]{
			7.513874, -13.31366, -5.891386, 3.89313, -13.35374, -5.470473, -28.48326, -0.8950226, 1.676227, -7.829721, 3.001554, 0.9742008, 6.85763, 12.07218, -1.542749, 6.42987, -6.777609, -12.28891, -11.6297, -3.275043, -0.3349386, 13.02803, -3.171752, 2.909704, 7.935295, 2.154472, 5.825492, 6.763169, 6.5986, 0.846593, 6.619426, 0.1207947, 7.378351, 5.546535, -0.8831579, -3.48157, 2.024852, -5.732147, 16.51345, -11.30504, -4.979899, 0.1936812, -7.330712, -2.363033, -5.555974, -11.48479, 6.027504, -0.5046204, -5.604547, 6.99612, 8.643599, -0.5479781, 1.765478, 12.20771, -14.86998, -10.28358, 1.827907, -1.541205, 8.282341, 14.34195, 0.6747534, 8.42286, -10.29843, -0.5448059, 6.860302, -7.558879, -3.571216, -3.13385, 12.60011, -0.07997914, -4.452103, 1.369113, 7.464754, -8.164039, -4.246214, -7.685563, -1.948951, 15.26882, -5.696559, -1.464601, 1.735656, 13.39631, 12.12768, -10.33641, 1.26995, 6.143419, -3.200521, -6.426282, -4.033798, -9.858385, 3.240334, 5.088798, -8.307716, 4.577082, 9.247719, -4.710557, 11.59537, 1.740587, 1.173246, 5.654369, -0.01115066, 3.742572, 0.01587177, 5.836065, -1.146728, -0.4543813, -0.09580191, -3.896233, -1.911579, 4.912868, -0.4526708, 3.798782, 4.286755, 2.614983, 3.773993, -1.182102, 3.293242, 3.547439, 4.012696, 0.9868906, 
		};
		float[] param_list = new float[120]; 
		for (int i = 0; i < 120; i++) {
			param_list [i] = (float) p_list [i];
		}

		run = 1;
		spring_const = 1000;
		damper = 100;

		//System.IO.File.Delete("C:/UnityLogs/best_costs.txt");

		//Assign joint references
		hip_right = upper_right.GetComponent<ConfigurableJoint> ();
		knee_right = lower_right.GetComponent<HingeJoint> ();
		hip_left = upper_left.GetComponent<ConfigurableJoint> ();
		knee_left = lower_left.GetComponent<HingeJoint> ();

		gen_alg = new GenAlg ();
		//net = new RNN (gen_alg.GetCurrentChrom()); 
		net = new RNN (new Chromosome(param_list)); 

		Debug.Log ("weights: "+net.weights.ToString ());
		counter = 0;

		//SetAngles (new float[]{0f, 0f, 0.5f, 0.5f, 0.5f, 0.5f});
		//SetAngles (new float[]{0f, 0f, 0f, 0f, 0f, 0f});
	}
	
	// FixedUpdate is called every 0.02s
	void FixedUpdate () {
		net.Update ();
		SetAngles (net.GetOutputs());

		/*
		if (gen_alg.gen_ind > 120) {
			gen_alg = new GenAlg ();
			run++;
		}

		//Conditions for terminating current chrom simulation
		counter += Time.deltaTime;
		if (counter > 40) {
			GoToNextChrom ();
		} else if (body.transform.position.y < 1.5) {
			GoToNextChrom ();
		} else if (Mathf.Abs (body.transform.rotation.x) > 100) {
			GoToNextChrom ();
		} else if (float.IsNaN (body.transform.position.x)) {
			GoToNextChrom ();		
		}		
		*/
		//Debugging
		//System.IO.File.AppendAllText("C:/UnityLogs/logRNN.txt", net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]+"\n");
		//Debug.Log ("net outputs: "+net.GetOutputs ()[0]+" "+net.GetOutputs ()[1]+" "+net.GetOutputs ()[2]+" "+net.GetOutputs ()[3]+" "+net.GetOutputs ()[4]+" "+net.GetOutputs ()[5]);

		//System.IO.File.AppendAllText("C:/UnityLogs/logRNN.txt", net.GetActivities ()[0]+" "+net.GetActivities ()[1]+" "+net.GetActivities ()[2]+" "+net.GetActivities ()[3]+" "+net.GetActivities ()[4]+" "+net.GetActivities ()[5]+"\n");
		//Debug.Log ("net activities: "+net.GetActivities ()[0]+" "+net.GetActivities ()[1]+" "+net.GetActivities ()[2]+" "+net.GetActivities ()[3]+" "+net.GetActivities ()[4]+" "+net.GetActivities ()[5]);
	}

	private void GoToNextChrom() {
		float fitness = Evaluate ();
		net.current_chrom.fitness = fitness;
		net.SetChrom (gen_alg.GetNextChrom ());

		Reset ();
		counter = 0;		
	}

	private void SetAngles(float[] net_outputs) {
		try {		
			JointSpring spring_right = new JointSpring ();
			spring_right.spring = spring_const;
			spring_right.damper = damper;
			spring_right.targetPosition = net_outputs [0] * 90;

			JointSpring spring_left = new JointSpring ();
			spring_left.spring = spring_const;
			spring_left.damper = damper;
			spring_left.targetPosition = net_outputs [1] * 90;

			//Debug.Log ("spring_right spring: "+spring_right.spring);
			//Debug.Log ("spring_left spring: "+spring_left.spring);

			Quaternion quat_right = Quaternion.Euler (net_outputs [2] * 130 - 65, 0, net_outputs [3] * 20 - 10);  
			Quaternion quat_left = Quaternion.Euler (net_outputs [4] * 130 - 65, 0, net_outputs [5] * 20 - 10);		
			//Quaternion quat_left = Quaternion.Euler (net_outputs [4] * 90 - 45, 0, net_outputs [5] * 90 - 45);		


			hip_right.targetRotation = quat_right;
			hip_left.targetRotation = quat_left;
			knee_right.spring = spring_right;
			knee_left.spring = spring_left;

			//Debug.Log ("knee_right spring: "+knee_right.spring.spring);
			//Debug.Log ("knee_left spring: "+knee_left.spring.spring);

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
		catch (Exception e) {
			Debug.Log ("error occured man");
			Debug.Log (e);
		}
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
		return (new Vector2(body.transform.position.x, body.transform.position.z) - new Vector2(1, 0)).magnitude;
	}
}


