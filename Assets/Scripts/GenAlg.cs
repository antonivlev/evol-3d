using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenAlg {
	public int gen_ind;
	public int gen_size;

	public List<Chromosome> gen;


	private int chrom_ind; 

	private int num_mutations;

	public GenAlg() {
		num_mutations = 0;

		gen_ind = 1;
		gen_size = 50;
		chrom_ind = 0;
		gen = new List<Chromosome> ();

		//Fill gen with randomised chromosomes
		for (int i = 0; i < gen_size; i++) {
			Chromosome chr = new Chromosome ();
			gen.Add (chr);
		}
	}

	public void MakeNewGen() {
		//all chroms in gen have a cost at this point
		gen = gen.OrderByDescending( chrom => chrom.fitness ).ToList();

		Debug.Log ("run: "+Main.run+"  gen: "+gen_ind+"  best cost: "+gen[0].fitness+"  total mutations: "+num_mutations);
		System.IO.File.AppendAllText("C:/UnityLogs/best_costs"+Main.run+".txt", gen[0].fitness+"\n");		
		System.IO.File.AppendAllText("C:/UnityLogs/best_params"+Main.run+".txt", gen[0].GetParamsString()+"\n");

		//Take top half of current generation, duplicate to make new gen
		gen = gen.Take (gen_size / 2).ToList(); 
		List<Chromosome> second_half = new List<Chromosome> ();
		foreach (Chromosome chr in gen) {
			second_half.Add (chr.MakeClone ());
		}
		gen = gen.Concat (second_half).ToList();

		//Mutate new generation
		num_mutations = 0;
		foreach (Chromosome chrom in gen) {
			chrom.Mutate ();
			num_mutations += chrom.mut_counter;
		}

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
