using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenAlg {
	private int gen_ind;
	public int gen_size;

	public List<Chromosome> gen;


	private int chrom_ind; 

	public GenAlg() {
		gen_ind = 1;
		gen_size = 10;
		chrom_ind = 0;
		gen = new List<Chromosome> ();
		//Fill gen with randomised chromosomes
		Debug.Log("-----chroms init-------");
		for (int i = 0; i < gen_size; i++) {
			Chromosome chr = new Chromosome ();
			Debug.Log (chr.biases[0]);
			gen.Add (chr);
		}
	}

	public void MakeNewGen() {
		//all chroms in gen have a cost at this point

		gen = gen.OrderByDescending( chrom => chrom.cost ).ToList();
		//Take top half of current generation, duplicate to make new gen
		gen = gen.Take (gen_size / 2)
			.Concat (gen.Take (gen_size / 2))
			.ToList();

		//Mutate new generation
		foreach (Chromosome chrom in gen) {
			chrom.Mutate ();
		}
			
		Debug.Log ("gen_ind: "+gen_ind+"  gen_length: " + gen.Count);
		gen_ind++;
	}

	public Chromosome GetNextChrom() {
		if (chrom_ind < gen_size-1) {
			chrom_ind++;
		} else {
			MakeNewGen ();
			chrom_ind = 0;
		}

		return gen [chrom_ind];
	}

	public Chromosome GetCurrentChrom() {
		return gen [chrom_ind];
	}
}
