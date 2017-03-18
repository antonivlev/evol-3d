using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chromosome {
	public float[] weights;
	public float[] biases;
	public float[] t_consts;

	public float dist;

	private MathNet.Numerics.Distributions.Normal weights_distr_mut = new Normal(0, 8);
	private MathNet.Numerics.Distributions.Normal biases_distr_mut = new Normal(0, 1.5);
	private MathNet.Numerics.Distributions.Normal t_distr_mut = new Normal(0, 3);

	private MathNet.Numerics.Distributions.Normal weights_distr_init = new Normal(0, 8);
	private MathNet.Numerics.Distributions.Normal biases_distr_init = new Normal(2.75, 1.5);
	private MathNet.Numerics.Distributions.Normal t_distr_init = new Normal(0, 3);


	public int mut_counter;

	public Chromosome() {
		mut_counter = 0;

		weights = new float[100];
		biases = new float[10];
		t_consts = new float[10];
		dist = Mathf.Infinity;

		for (int i = 0; i < 100; i++) {
			if (i < 10) {
				t_consts [i] = (float) t_distr_init.Sample();
				biases [i] = (float) biases_distr_init.Sample();
			}
			weights [i] = (float) weights_distr_init.Sample();
		}
	}

	public void Mutate() {
		mut_counter = 0;
		for (int i = 0; i < 120; i++) {
			int chance = Random.Range (0, 120);
			float w = (float)weights_distr_mut.Sample ();
			if (w < -16f) {
				w = -16f;
			} else if (w > 16f) {
				w = 16f;
			}

			float b = (float)biases_distr_mut.Sample ();
			if (b < -4f) {
				b = -4f;
			} else if (b > 4f) {
				b = 4f;
			}

			float t = (float)t_distr_mut.Sample ();
			if (t < 0.5f) {
				t = 0.5f;
			} else if (t > 2.75f) {
				t = 2.75f;
			}
				
			if (chance == 42) {
				mut_counter++;
				if (i < 100) {
					weights [i] = w;
				} else if (i < 110) {
					t_consts [i-100] = t;
				} else {
					biases [i - 110] = b;
				}					
			}
		}
	}

	public string ToTheString() {
		/*return 	"\n\tweights: [" + 	weights [0] + " " + weights [1] + " " + weights [2] + " " + weights [3] + "...]" +
				"\n\tbiases: [" + 	biases [0] + " " + biases [1] + " " + biases [2] + " " + biases [3] + "...]" +
				"\n\tt_consts: [" + 	t_consts [0] + " " + t_consts [1] + " " + t_consts [2] + " " + t_consts [3] + "...]";
		*/
		string chrom_params = "";
		for (int i = 100; i < 120; i++) {
			if (i < 100) {
				chrom_params += weights [i]+",";
			} else if (i < 110) {
				chrom_params += t_consts [i-100]+",";
			} else {
				chrom_params += biases[i-110]+",";
			}					
		}
			
		return chrom_params;
	}

	public Chromosome MakeClone() {
		Chromosome clone = new Chromosome ();

		//Make new arrays for clone params
		float[] clone_weights = new float[100];
		float[] clone_biases = new float[10];
		float[] clone_t_consts = new float[10];
		weights.CopyTo (clone_weights, 0);
		biases.CopyTo (clone_biases, 0);
		t_consts.CopyTo (clone_t_consts, 0);

		clone.weights = clone_weights;
		clone.biases = clone_biases;
		clone.t_consts = clone_t_consts;

		return clone;
	}
}
