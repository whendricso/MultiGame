using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class ModernGun : MonoBehaviour {
	
	public enum AimCorrectionTypes {None, Raycast, DistantPoint};
	public AimCorrectionTypes aimCorrection = AimCorrectionTypes.None;
	public bool showAmmoGUI = true;
	public Rect guiArea;
	public GUISkin guiSkin;
	public GameObject projectile;
	public GameObject reloadMessageReceiver;
	public int magazineMax = 32;
	[HideInInspector]
	public int magazineCount;
	[HideInInspector]
	public bool reloading = false;
	public int clipType = 0;
	public KeyCode reload = KeyCode.R;
	public float reloadTime = 2.2f;
	public GameObject muzzleTransform;
	public GameObject muzzleFlash;
	public GameObject muzzleFlashSpawnTransform;
	public float flashDuration = 0.125f;
	public AudioClip fireSound;
	public AudioClip reloadSound;
	public AudioClip ammoExhaustedClick;
	public float refireTime = 0.4f;
	public float muzzleSpreadMin = 2.0f;
	public float muzzleSpreadMax = 10.0f;
	public float roundSpreadCost = 1.4f;
	public float refocusRate = 1.2f;
	private float currentSpread;
	private float refireCounter;
	private Vector3 muzzleOrientation;
	public Texture2D crosshairs;
	public float crossSpreadScalar = 1.0f;
	
	private bool saved = false;
	
	void Start () {
		//PlayerPrefs.DeleteAll();
//		GameObject player = GameObject.FindGameObjectWithTag("Player");
		//ClipInventory clipInv = player.GetComponent<ClipInventory>();
		if (PlayerPrefs.HasKey(gameObject.name + "magazineCount")) {
			magazineCount = PlayerPrefs.GetInt(gameObject.name + "magazineCount");
			Debug.Log("Loaded " + magazineCount);
			
		}
		currentSpread = (muzzleSpreadMin + muzzleSpreadMax)/2;
		refireCounter = refireTime;
		if (muzzleTransform == null)
			Debug.LogError("ModernGun requires a muzzle transform!");
		muzzleOrientation = muzzleTransform.transform.localEulerAngles;
	}
	
	void OnGUI () {
		if (crosshairs == null)
			return;
		float adjustedWidth = crosshairs.width * crossSpreadScalar;
		float adjustedHeight = crosshairs.height * crossSpreadScalar;
		GUI.DrawTexture(new Rect(((Screen.width/2) - ((currentSpread * adjustedWidth)/2)), (Screen.height/2) - ((currentSpread * adjustedHeight)/2), (adjustedWidth * currentSpread), (adjustedHeight * currentSpread) ),crosshairs,ScaleMode.ScaleToFit);
		
		if (showAmmoGUI) {
			GUILayout.Window( 10000, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), AmmoWindow, "");
		}
		
	}
	
	void AmmoWindow (int id) {
		GUILayout.Label(magazineCount + " : " + magazineMax);
	}
	
	void Update () {
		if (Input.GetKeyDown(reload))
			Reload();
		if (currentSpread > muzzleSpreadMin)
			currentSpread -= Time.deltaTime * refocusRate;
		refireCounter -= Time.deltaTime;
		if (Screen.lockCursor) {
			if ((Input.GetMouseButton(0) && refireCounter <= 0))
				Fire ();
		}
	}
	
	public void Fire() {
		RaycastHit hinfo;
		if (aimCorrection == AimCorrectionTypes.Raycast) {
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0.0f));
			bool didHit = Physics.Raycast(ray, out hinfo);//Physics.(muzzleTransform.transform.position, Vector3.forward, out hinfo);
			if (didHit) {
				muzzleTransform.transform.LookAt(hinfo.point);
			}
			if (!didHit) {
				GameObject distantPoint = new GameObject("DistantPoint");
				GameObject pointCaster = new GameObject("PointCaster");
				pointCaster.transform.parent = Camera.main.transform;
				pointCaster.transform.localPosition = Vector3.zero;
				pointCaster.transform.localRotation = Quaternion.identity;
				distantPoint.transform.parent = pointCaster.transform;
				distantPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 1500.0f);
				muzzleTransform.transform.LookAt(distantPoint.transform.position);
				Destroy(pointCaster);
			}
		}
		if (aimCorrection == AimCorrectionTypes.DistantPoint) {
			GameObject distantPoint = new GameObject("DistantPoint");
			GameObject pointCaster = new GameObject("PointCaster");
			pointCaster.transform.parent = Camera.main.transform;
			pointCaster.transform.localPosition = Vector3.zero;
			pointCaster.transform.localRotation = Quaternion.identity;
			distantPoint.transform.parent = pointCaster.transform;
			distantPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 1500.0f);
			muzzleTransform.transform.LookAt(distantPoint.transform.position);
			Destroy(pointCaster);
		}
		
		if (magazineCount <= 0 && !reloading) {
			if (ammoExhaustedClick != null)
				audio.PlayOneShot(ammoExhaustedClick);
			if (reloadMessageReceiver != null)
				reloadMessageReceiver.SendMessage("Reload",SendMessageOptions.DontRequireReceiver);
			if (reloadMessageReceiver != gameObject)
				Reload();
			return;
		}
		if (magazineCount > 0 && !reloading) {
			magazineCount -= 1;
			currentSpread += roundSpreadCost;
			if (currentSpread > muzzleSpreadMax)
				currentSpread = muzzleSpreadMax;
			if (aimCorrection == AimCorrectionTypes.None)
				muzzleTransform.transform.localEulerAngles = new Vector3(muzzleOrientation.x + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleOrientation.y + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleOrientation.z);
			else	
				muzzleTransform.transform.localEulerAngles = new Vector3(muzzleTransform.transform.localEulerAngles.x + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleTransform.transform.localEulerAngles.y + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleTransform.transform.localEulerAngles.z);
			refireCounter = refireTime;
			GameObject bullet = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
			Bullet proj = bullet.GetComponent<Bullet>();
			if (proj != null)
				proj.owner = GameObject.FindGameObjectWithTag("Player");
			if (aimCorrection == AimCorrectionTypes.None)
				muzzleTransform.transform.localEulerAngles = muzzleOrientation;
			if (muzzleFlash != null) {
				GameObject flash;
				if (muzzleFlashSpawnTransform == null)
					flash = Instantiate(muzzleFlash, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
				else
					flash = Instantiate(muzzleFlash, muzzleFlashSpawnTransform.transform.position, muzzleFlashSpawnTransform.transform.rotation) as GameObject;
					
				if (muzzleFlashSpawnTransform == null)
					flash.transform.parent = muzzleTransform.transform;
				else
					flash.transform.parent = muzzleFlashSpawnTransform.transform;
					
				flash.transform.localEulerAngles = new Vector3(flash.transform.localEulerAngles.x, flash.transform.localEulerAngles.y, Random.Range(0.0f, 360.0f));
				if (flashDuration > 0.0f)
					Destroy(flash, flashDuration);
			}
			if (fireSound != null)
				audio.PlayOneShot(fireSound);
		}
	}
	
	public void Reload () {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		ClipInventory clipInv = player.GetComponent<ClipInventory>();
		if (clipInv.numClips[clipType] > 0) {
			clipInv.numClips[clipType]--;
			reloading = true;
			StartCoroutine(FinishReload(reloadTime));
		}
		
	}
	
	public IEnumerator FinishReload (float delay) {
		yield return new WaitForSeconds(delay);
		magazineCount = magazineMax;
		reloading = false;
	}
	
	public void OnDestroy () {
		if (saved)
			return;
		else {
			saved = true;
			Debug.Log("Saved " + magazineCount);
			PlayerPrefs.SetInt(gameObject.name + "magazineCount", magazineCount);
		}
	}
}
