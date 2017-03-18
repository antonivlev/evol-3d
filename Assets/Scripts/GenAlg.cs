using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenAlg {
	private int gen_ind;
	public int gen_size;

	public List<Chromosome> gen;


	private int chrom_ind; 

	private int num_mutations;

	public GenAlg() {
		num_mutations = 0;

		gen_ind = 1;
		gen_size = 120;
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
		System.IO.File.AppendAllText("C:/UnityLogs/gen_desc.txt", gen_ind+"-------------------------------"+"\n");
		gen = gen.OrderByDescending( chrom => chrom.dist ).ToList();

		foreach (Chromosome chrom in gen) {
			System.IO.File.AppendAllText("C:/UnityLogs/gen_desc.txt", chrom.dist+"\n");		
		}
			
		Debug.Log ("gen: "+gen_ind+"  best cost: "+gen[0].dist+"  total mutations: "+num_mutations);
		System.IO.File.AppendAllText("C:/UnityLogs/best_costs.txt", gen[0].dist+"\n");
		//System.IO.File.AppendAllText("C:/UnityLogs/top_chroms.txt", gen[0].ParamsToString+"\n");
	

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

		//Prints all chromosomes in generation
		//string[] gen_str = gen.Select(c => c.ToTheString()).ToArray();
		//Debug.Log (string.Join("\n", gen_str));
		//System.IO.File.AppendAllText("C:/UnityLogs/params.txt", string.Join("\n", gen_str)+"\n");

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
