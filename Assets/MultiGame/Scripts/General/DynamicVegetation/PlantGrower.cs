using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantGrower : MonoBehaviour {

	[Tooltip("Growth curve of the plant. The length of this curve can be adjusted to extend growth time.")]
	public AnimationCurve sizeOverTime = AnimationCurve.EaseInOut(0f, 0f, 5f, 1f);
	[Tooltip("Size over time of the blooms. Length should be 1")]
	public AnimationCurve bloomOverTime = AnimationCurve.EaseInOut(0f, 0f, 5f, 1f);
	[Tooltip("Size of leaves over their cycle, length should be 1")]
	public AnimationCurve vegetationOverTime = AnimationCurve.EaseInOut(0f, 0f, 5f, 1f);

	[Tooltip("How long does a year last?")]
	public float annualCycle = 100f;

	[Tooltip("Where do the flowers bloom from?")]
	public List<GameObject> flowerPositions = new List<GameObject>();
	[System.NonSerialized]
	private List<GameObject> flowers = new List<GameObject>();

	[Tooltip("Where do the leaves/veggies come from?")]
	public List<GameObject> vegetationPositions = new List<GameObject>();
	[System.NonSerialized]
	private List<GameObject> veggies = new List<GameObject>();

	[Tooltip("How long into the annual cycle do we grow leaves?")]
	[Range(0.01f, 0.95f)]
	public float vegetationFraction = 0.2f;
	[Tooltip("How long into the annual cycle do the leaves wilt?")]
	[Range(0.01f, 0.95f)]
	public float vegetationWiltingFraction = 0.8f;
	[Tooltip("How many seconds does wilting vary?")]
	public float vegetationWiltingVariance = 2.0f;
	[Tooltip("How long into the annual cycle do we blossom?")]
	[Range(0.01f, 0.95f)]
	public float blossomingFraction = 0.5f;
	[Tooltip("How long into the annual cycle do the blooms wilt?")]
	[Range(0.01f, 0.95f)]
	public float blossomWiltingFraction = 0.6f;
	[Tooltip("How many seconds does the wilting vary?")]
	public float blossomWiltingVariance = 2.0f;
	[Tooltip("How long into the annual cycle do we grow seeds?")]
	[Range(0.01f, 0.95f)]
	public float seedingFraction = 0.85f;

	public GameObject flowerPrefab;
	public GameObject deadFlowerPrefab;
	public GameObject vegetationPrefab;
	public GameObject deadVegetationPrefab;
	public GameObject seedPrefab;
	public GameObject deadPlantPrefab;

	[System.NonSerialized]
	private bool bloomed = false;
	[System.NonSerialized]
	private bool bloomsWilted = false;
	[System.NonSerialized]
	private bool vegetationWilted = false;
	[System.NonSerialized]
	private bool seeded = false;
	[System.NonSerialized]
	private bool vegetated = false;

	[System.NonSerialized]
	private float startTime;
	[System.NonSerialized]
	private float yearStart;
	[System.NonSerialized]
	private float bloomStartTime;
	[System.NonSerialized]
	private float vegetationStartTime;

	[Tooltip("Output useful information to the console")]
	public bool debug = false;
	
	void Start () {
		if (annualCycle <1) {
			Debug.LogError("Plant Grower " + gameObject.name + " needs an annual cycle greater than 1 second!");
			enabled = false;
			return;
		}
		startTime = Time.time;
		yearStart = Time.time;
	}

	void Update () {
			transform.localScale = Vector3.one * sizeOverTime.Evaluate((Time.time - startTime) / (sizeOverTime[ sizeOverTime.length-1].time));

		if( flowerPositions.Count > 0) {
			if (CheckCanBloom()) {
				bloomed = true;
				Blossom();
			} else {
				if (bloomed) {
					EvaluateBloom();
				}
			}

			if (CheckCanWiltFlowers()) {
				StartCoroutine(WiltFlowers());
				bloomsWilted = true;
			}
		}

		if( vegetationPositions.Count > 0) {
			if (CheckCanVegetate()) {
				vegetated = true;
				Vegetate();
			} else {
				if (vegetated) {
					EvaluateVegetation();
				}
			}

			if (CheckCanWiltVegetation()) {
				StartCoroutine(WiltVegetation());
				vegetationWilted = true;
			}
		}

		if (CheckCanSeed() && seedPrefab != null) {
			seeded = true;
			GameObject _newSeed;
			foreach (GameObject _origin in flowerPositions) {
				_newSeed = Instantiate(seedPrefab, _origin.transform.position, _origin.transform.rotation) as GameObject;
			}
		}

		if (Time.time - yearStart > annualCycle)
			ResetAnnualCycle();

	}

	void Blossom () {
		if (debug)
			Debug.Log("Blossom");
		bloomStartTime = Time.time;
		GameObject _bloom;

		for (int i = 0; i < flowerPositions.Count; i++) {
			_bloom = Instantiate(flowerPrefab, flowerPositions[i].transform.position, flowerPositions[i].transform.rotation) as GameObject;
			_bloom.transform.SetParent(flowerPositions[i].transform);
			_bloom.transform.localScale = Vector3.zero;
			flowers.Add(_bloom);
		}

//		foreach (GameObject _flowerPos in flowerPositions)
//			_flowerPos.transform.localScale = Vector3.zero;
	}

	void Vegetate () {
		if (debug)
			Debug.Log("Vegetate");
		vegetationStartTime = Time.time;
		GameObject _leaf;

		for (int i = 0; i < vegetationPositions.Count; i++) {
			_leaf = Instantiate(vegetationPrefab, vegetationPositions[i].transform.position, vegetationPositions[i].transform.rotation) as GameObject;
			_leaf.transform.SetParent(vegetationPositions[i].transform);
			_leaf.transform.localScale = Vector3.zero;
			veggies.Add(_leaf);
		}

//		foreach (GameObject _vegPos in vegetationPositions)
//			_vegPos.transform.localScale= Vector3.zero;
	}

	IEnumerator WiltFlowers () {
		if (debug)
			Debug.Log ("Wilting flowers");

		GameObject _bloom;
		for (int i = 0; i < flowerPositions.Count; i++) {
			yield return new WaitForSeconds(Random.Range(0f, blossomWiltingVariance));
			if (flowerPositions[i].transform.childCount > 0) {
				Destroy(flowers[i]);
				flowers.RemoveAt(i);
				if (deadFlowerPrefab != null)
					Instantiate(deadFlowerPrefab, flowerPositions[i].transform.position, flowerPositions[i].transform.rotation);
			}
		}
	}

	IEnumerator WiltVegetation () {
		if (debug)
			Debug.Log ("Wilting vegetation");

		for (int i = 0; i < vegetationPositions.Count; i++) {
			yield return new WaitForSeconds(Random.Range(0f, vegetationWiltingVariance));
			if (vegetationPositions[i].transform.childCount > 0) {
				Destroy(veggies[i]);
				veggies.RemoveAt(i);
				if (deadVegetationPrefab != null)
					Instantiate(deadVegetationPrefab, vegetationPositions[i].transform.position, vegetationPositions[i].transform.rotation);
			}
		}
	}

	void EvaluateBloom () {
		for (int i = 0; i < flowers.Count; i++) {
			flowers[i].transform.localScale = Vector3.one * bloomOverTime.Evaluate((Time.time - bloomStartTime) / (bloomOverTime[bloomOverTime.length - 1].time));
		}
	}

	void EvaluateVegetation () {
		for (int i = 0; i < veggies.Count; i++) {
			veggies[i].transform.localScale = Vector3.one * vegetationOverTime.Evaluate((Time.time - vegetationStartTime) / (vegetationOverTime[vegetationOverTime.length - 1].time));
		}
	}

	bool CheckCanBloom() {
		bool _ret = false;

		if (Time.time - yearStart > (annualCycle * blossomingFraction) && !bloomed)
			_ret = true;

		return _ret;
	}

	bool CheckCanSeed() {
		bool _ret = false;
		
		if (Time.time - yearStart > (annualCycle * seedingFraction) && !seeded)
			_ret = true;
		
		return _ret;
	}

	bool CheckCanVegetate() {
		bool _ret = false;
		
		if (Time.time - yearStart > (annualCycle * vegetationFraction) && !vegetated)
			_ret = true;
		
		return _ret;
	}

	bool CheckCanWiltFlowers () {
		bool _ret = false;

		if (Time.time - yearStart > (annualCycle * blossomWiltingFraction) && !bloomsWilted)
			_ret = true;

		return _ret;
	}

	bool CheckCanWiltVegetation () {
		bool _ret = false;
		
		if (Time.time - yearStart > (annualCycle * vegetationWiltingFraction) && !vegetationWilted)
			_ret = true;
		
		return _ret;
	}

	void ResetAnnualCycle () {
		bloomed = false;
		vegetated = false;
		seeded = false;
		bloomsWilted = false;
		vegetationWilted = false;
		yearStart = Time.time;
	}
}
