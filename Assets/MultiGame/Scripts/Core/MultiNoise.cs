using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

//References 
//https://www.mrl.nyu.edu/~perlin/doc/oscar.html#noise
//https://mzucker.github.io/html/perlin-noise-math-faq.html
//https://catlikecoding.com/unity/tutorials/noise/
//https://gamedev.stackexchange.com/questions/23625/how-do-you-generate-tileable-perlin-noise

namespace MultiGame {

	public static class MultiNoise {

		public static List<int> permutations = new List<int>();
		public static List<Vector2> vectors = new List<Vector2>();
		public static int seed = 42;

		static void PopulateVectors(int _resolution) {
			vectors.Clear();
			for (int i = 0; i < _resolution - 1; i++) {
				vectors.Add(new Vector2(Mathf.Cos(i * 2 * Mathf.PI / _resolution),Mathf.Sin(i * 2 * Mathf.PI / _resolution)));
			}
		}

		static void PopulatePermutations(int _seed) {
			SetSeed(_seed);
			PopulatePermutations(_seed, 256);
		}

		static void PopulatePermutations(int seed, int _resolution) {
			permutations.Clear();
			Random.InitState(seed);
			for (int p = 0; p < _resolution; p++)
				permutations.Add(p);

			int _tmp = 0;
			int _tmpIndex = 0;
			for (int i = 0; i < _resolution-1; i++) {
				_tmpIndex = Random.Range(i, _resolution);
				_tmp = permutations[i];
				permutations[i] = permutations[_tmpIndex];
				permutations[_tmpIndex] = _tmp;
			}
		}

		/// <summary>
		/// Clones the current permutation set and appends the clone to the original
		/// </summary>
		/// <param name="_clearTempAutomatically">Should we automatically free the extra copy of the list when we're done?</param>
		static void ClonePermutations(bool _clearTempAutomatically = true) {
			List<int> _tmpPermutations = new List<int>(permutations);
			permutations.AddRange(_tmpPermutations);
			if (_clearTempAutomatically)
				_tmpPermutations.Clear();
		}

		public class Surflet {
			public float distanceX = 0;
			public float distanceY = 0;
			public float x = 0;
			public float y = 0;
			public float polyX = 0;
			public float polyY = 0;
			public int hashed = 0;
			public Vector2 gradientVector = new Vector2();


			Surflet(float _x, float _y,float _gridX, float _gridY) {
				x = _x;
				y = _y;
				distanceX = Mathf.Abs(x - _gridX);
				distanceY = Mathf.Abs(y - _gridY);
				polyX = 1 - 6 * Mathf.Pow(distanceX, 5) + 15 * Mathf.Pow(distanceX, 4) - 10 * Mathf.Pow(distanceX, 3);
				polyY = 1 - 6 * Mathf.Pow(distanceY, 5) + 15 * Mathf.Pow(distanceY, 4) - 10 * Mathf.Pow(distanceY, 3);
				//hashed = permutations[permutations[(int)_gridX%pe
			}

			Surflet() {

			}
		}

		/*public static Surflet surflet = new Surflet();
		static Vector2 Noise(float _x, float _y, float _period) {
			return surflet.polyX * surflet.polyY * surflet.gradientVector;
		}
		*/
		public static void SetSeed(int _seed) {
			seed = _seed;
		}

	}

}