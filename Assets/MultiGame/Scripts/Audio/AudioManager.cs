using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	public GameObject target;
	public float masterVolume = 1.0f;
	public float sfxVolume = 1.0f;
	[HideInInspector]
	public MusicManager musicManager;
	[HideInInspector]
	public GameObject[] audioManagers;
	private bool destroyMe; //should I be destroyed if there is another AudioManager?
	
	void Start () {
		audioManagers = GameObject.FindGameObjectsWithTag("AudioManager");
		if (audioManagers.Length > 0) {
			for (int i = 1; i < audioManagers.Length; i += 1) {
				Destroy(audioManagers[i]);
			}
		}
		DontDestroyOnLoad(gameObject);
		musicManager = GetComponentInChildren<MusicManager>();
	}
	
	void Update () {
		if (target != null)
			transform.position = target.transform.position;
		else
			target = GameObject.FindGameObjectWithTag("Player");
	}
}
