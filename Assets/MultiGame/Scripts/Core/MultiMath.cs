using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

//References:
//https://easings.net/

namespace MultiGame {

	public static class MultiMath {

		public static float EaseOutSine(float _start, float _end, float _time) {
			return (Mathf.Sin(Mathf.PI * _time)*.5f) * (_end - _start) + _start;
		}

		public static float EaseInSine(float _start, float _end, float _time) {
			return (1-Mathf.Cos(Mathf.PI * _time) * .5f) * (_end - _start) + _start;
		}

		/// <summary>
		/// Return a smoothed value somewhere between _start and _end
		/// </summary>
		/// <param name="_start">The starting value</param>
		/// <param name="_end">The ending value</param>
		/// <param name="_time">The current progress from start to end, beginning at 0 and ending at 1</param>
		/// <returns></returns>
		public static float EaseInOutSine(float _start, float _end, float _time) {
			return ((-(Mathf.Cos(Mathf.PI * _time)) * .5f) * (_end-_start)) + _start;
		}

		public static float EaseInCubic(float _start, float _end, float _time) {
			return (Mathf.Clamp01(Mathf.Pow(_time, 3) ) * (_end - _start) + _start);
		}

		public static float EaseOutCubic(float _start, float _end, float _time) {
			return 1 - (Mathf.Clamp01(Mathf.Pow(_time, 3)) * (_end - _start) + _start);
		}

		public static float EaseInOutCubic(float _start, float _end, float _time) {
			return _time < 0.5f ? EaseInCubic(_start, _end, _time) : EaseOutCubic(_start, _end, _time);
		}

		public static Texture2D Perlin2D(int resolution, int numSurflets, int randomSeed) {

			if (numSurflets >= resolution) {
				Debug.LogError("MultiGame Perlin2D must have fewer surflets than pixels. Please decrease the relative surflet density.");
				return null;
			}

			float _gridSpacing = resolution / numSurflets;
			Vector2[,] _surflets = new Vector2[numSurflets, numSurflets];
			float[,] _values = new float[resolution,resolution];

			Random.InitState(randomSeed);

			for (int u = 0; u < numSurflets; u++) {
				for (int v = 0; v < numSurflets; v++) {
					_surflets[u, v] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
				}
			}
			//[u0,v0][u1,v1]
			//[u2,v2][u3,v3]
			//each u,v represents a surflet gradient vector
			//we calculate the distance to each from the sample point and store it
			//Then we return the dot product of the two vectors
			//Repeat for all neighboring points and interpolate the values to get the final result
			for (int x = 0; x < resolution; x++) {
				for (int y = 0; y < resolution; y++) {
					//TODO
					//_values[x,y] = Vector2.Dot();
				}
			}

			return new Texture2D(resolution,resolution);
		}

		// Thanks to Sebastian Lague and www.iquilezles.org/www/articles/smin/smin.htm
		/// <summary>
		/// Smooth the values a and b by k
		/// </summary>
		/// <param name="a">The first value we wish to interpolate</param>
		/// <param name="b">The second value we wish to interpolate</param>
		/// <param name="k">The interpolation factor</param>
		/// <param name="invert">Should we return the max instead of the min?</param>
		/// <returns>A smoothed value between a and b</returns>
		public static float SmoothMin(float a, float b, float k, bool invert = false) {
			k = invert ? -k : k;
			float h = Mathf.Clamp01((b - a * k) / (2 * k));

			return a * h + b * (1 - h) - k * h * (1 - h); ;
		}

		/// <summary>
		/// Get the point opposite this one on the object's local X axis from a raycast hit
		/// </summary>
		/// <param name="_hinfo">The RaycastHit we wish to extract a local opposing position from</param>
		/// <returns></returns>
		public static Vector3 GetOpposingPoint(RaycastHit _hinfo) {
			return Vector3.Scale(new Vector3(-1, 1, 1), _hinfo.transform.root.InverseTransformPoint(_hinfo.point));
		}

		/*
		public class Perlin2D {
			public int resolution = 512;
			public int numSurflets = 4;
			public List<Vector2> surflets = new List<Vector2>();
			public float[,] values;

			public Perlin2D () {
				float _quotient = resolution / numSurflets;
				values = new float[resolution, resolution];
				//if ((float)resolution % (float)numSurflets != 0)
				//	Debug.LogWarning("Resolution "+ resolution +" is not evenly divisible by numSurflets "+ numSurflets +" ! This may create tiling artifacts.");
				for (int x = 0; x < resolution; x++) {
					for (int y = 0; y < resolution; y++) {
						
					}
				}
			}
		}
		*/

	}
}