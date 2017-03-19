using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
			13.60487, 8.884898, -8.635887, -5.654573, -4.089039, 1.603052, -3.757544, 7.576688, 2.288497, 8.902033, 1.849116, -1.273531, -10.93834, -5.812613, 12.89801, 2.936879, -4.927489, -7.593657, 3.688386, 4.401144, -1.309046, 10.22238, 9.380809, -0.4886758, -1.750314, 3.453045, -1.882296, -10.45676, -0.6135027, -1.93418, 9.891644, -10.07681, 5.790706, -2.206334, -12.8038, -0.8844603, -4.794529, -0.9038593, -14.63157, 1.921247, -5.444273, -3.972237, -4.918005, 2.388006, 2.611881, -7.335958, 13.92663, 3.932114, 10.19557, 11.76798, -0.4599442, -2.06741, 0.3857583, 5.607525, -0.9200727, 2.699193, -8.174809, 1.494416, -2.416601, 7.878998, -3.730548, -13.83968, 9.139459, 9.119887, -11.33674, 4.229909, 5.508394, -0.4803945, 1.334958, 6.528176, 4.298055, 0.2587101, -3.187573, -6.061004, -19.069, -3.963905, 16, -16, 16.95995, 4.851724, 10.29429, -7.411299, 0.1125937, -9.812949, -8.197845, -7.104035, -0.3269861, 0.4837435, -19.32641, 9.625965, -3.08302, 5.559855, 8.386573, -3.750917, -1.91664, -7.606456, 16, 15.31863, -2.583349, 1.674326, -2.697098, 1.051957, 0.4025465, 4.394157, 0.353469, -4.234244, 0.771432, 0.5, 0.5, -2.727707, 5.042959, 4.962371, 2.452196, 3.367581, 2.468332, -0.4287907, 0.9687033, -0.1386612, 0.8735673, 2.024322, 
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
		net = new RNN (gen_alg.GetCurrentChrom()); 
		//net = new RNN (new Chromosome(param_list)); 

		Debug.Log ("weights: "+net.weights.ToString ());
		counter = 0;

		//SetAngles (new float[]{0f, 0f, 0.5f, 0.5f, 0.5f, 0.5f});
		//SetAngles (new float[]{0f, 0f, 0f, 0f, 0f, 0f});
	}
	
	// FixedUpdate is called every 0.02s
	void FixedUpdate () {
		net.Update ();
		SetAngles (net.GetOutputs());


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
		}

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


