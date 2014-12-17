using UnityEngine;
using System.Collections;

public class Health : MultiModule {
	
	public float hp = 100.0f;
	public float maxHP = 100.0f;
	public bool autodestruct = true;
	public GameObject[] deathPrefabs;
	public GameObject deathCam;//optional camera to be spawned, which watches the first death prefab
	
	public GUISkin guiSkin;
	public bool showHealthBarGUI = false;
	public Rect healthBar;
	public bool autoHide = true;//if health is full, hide this bar
	public Color barColor = Color.white;
	public MessageManager.ManagedMessage healthGoneMessage;
	
	void Start () {
		hp = maxHP;
		if (healthGoneMessage.target == null)
			healthGoneMessage.target = gameObject;
	}
	
	void Update () {
		if (hp <= 0)
			Die();
		if (hp > maxHP)
			hp = maxHP;
	}
	
	void OnGUI () {
		if (hp == maxHP && autoHide)
			return;
		if (showHealthBarGUI) {
			GUI.skin = guiSkin;
			GUI.color = barColor;
			GUILayout.BeginArea(new Rect(healthBar.x * Screen.width, healthBar.y * Screen.height, (healthBar.width * Screen.width) * (hp / maxHP), healthBar.height * Screen.height));
			GUILayout.Box("Health");
			GUILayout.EndArea();
		}
	}
	
	public void Die() {
		MessageManager.Send( healthGoneMessage);
		if (!autodestruct)
			return;
		

		
		Destroy(gameObject);
	}

	void OnDestroy () {
		if (deathPrefabs.Length > 0)
		for (int i = 0; i < deathPrefabs.Length; i++) {
			GameObject dFab = Instantiate(deathPrefabs[i], transform.position, transform.rotation) as GameObject;
			
			if (i == 0 && deathCam != null) {//set the optional deathCam to watch this one!
				Transform mainCam = transform.Find("Main Camera");
				GameObject dCam = Instantiate(deathCam, mainCam.transform.position, mainCam.transform.rotation) as GameObject;
				dCam.GetComponent<SmoothLookAt>().target = dFab.transform;
				
			}
		}
	}
	
	public void ModifyHealth (float val) {
		hp += val;
		if (hp <= 0.0f) {
			if (deathPrefabs.Length > 0) {
				foreach (GameObject pfab in deathPrefabs) {
					Instantiate(pfab, transform.position, transform.rotation);
				}
			}
			Destroy(gameObject);
		}
	}
}
//Copyright 2014 William Hendrickson
