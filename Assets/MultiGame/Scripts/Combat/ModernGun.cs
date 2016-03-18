using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Modern Gun")]
	[RequireComponent (typeof(AudioSource))]
	public class ModernGun : MultiModule {

		[Tooltip("Should we use Unity's built-in fire button? If false, send 'Fire' each frame.")]
		public bool useFireButton = true;
		public string fireButton = "Fire1";
		[Tooltip("The model of the weapon, if unassigned and no Animator found, no animations will be sent!")]
		public GameObject image;
		public enum AimCorrectionTypes {None, Raycast, DistantPoint};
		[Tooltip("What type of aim correction should we use? Raycast aims directly at what we point at, distant point aims at a point in the far distance representing our crosshair location (recommended for FPS games)")]
		public AimCorrectionTypes aimCorrection = AimCorrectionTypes.None;
		[Tooltip("Show how much ammo we currently have with a legacy Unity GUI?")]
		public bool showAmmoGUI = true;
		[Tooltip("Normalized viewport rectangle representing the area used by the legacy GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.01f, 0.9f, 0.2f, 0.09f);
		public GUISkin guiSkin;
		[Tooltip("What do we spawn from the muzzle of the gun?")]
		public GameObject projectile;
		[Tooltip("Multiplied by the velocity before it's transferred to the projectile")]
		public float inheritVelocityScale = 1f;
		[HideInInspector]
		public GameObject reloadMessageReceiver;
		[Tooltip("How many shots per mag?")]
		public int magazineMax = 32;
		[HideInInspector]
		public int magazineCount;
		[HideInInspector]
		public bool reloading = false;
		[Tooltip("Which clip type from Clip Inventory do we use?")]
		public int clipType = 0;
		public KeyCode reload = KeyCode.R;
		[Tooltip("How long does it take us to reload?")]
		public float reloadTime = 2.2f;
		[Tooltip("A game object representing the exit point of the projectile")]
		public GameObject muzzleTransform;
		[Tooltip("An object spawned at the muzzle transform representing visual flash effects")]
		public GameObject muzzleFlash;
		[Tooltip("An optionally different flash spawn transform")]
		public GameObject muzzleFlashSpawnTransform;
		[Tooltip("How long should the flash object be alive?")]
		public float flashDuration = 0.125f;

		public AudioClip fireSound;
		[Tooltip("A Mecanim trigger that will be sent to the Image")]
		public string mecanimFireTrigger;
		[Tooltip("Message sent on successfully firing a round")]
		public MessageManager.ManagedMessage fireMessage;

		public AudioClip reloadSound;
		[Tooltip("A Mecanim trigger that will be sent to the Image")]
		public string mecanimReloadTrigger;
		[Tooltip("Message sent when starting to reload")]
		public MessageManager.ManagedMessage reloadingMessage;

		[Tooltip("The worst sound you can hear in a firefight")]
		public AudioClip ammoExhaustedClick;
		[Tooltip("A Mecanim trigger that will be sent to the Image")]
		public string mecanimAmmoExhaustedTrigger;
		[Tooltip("Message sent when we have run out of ammo")]
		public MessageManager.ManagedMessage ammoExhaustedMessage;

		[Tooltip("How long, in seconds, between each bullet?")]
		public float refireTime = 0.4f;
		[Tooltip("Minimum variation of a bullet from center")]
		public float muzzleSpreadMin = 2.0f;
		[Tooltip("Maximum variation of a bullet from center")]
		public float muzzleSpreadMax = 10.0f;
		[Tooltip("How much the spread increases each time a projectile is discharged")]
		public float roundSpreadCost = 1.4f;
		[Tooltip("How fast the spread decreases")]
		public float refocusRate = 1.2f;
		private float currentSpread;
		private float refireCounter;
		private Vector3 muzzleOrientation;
		[Tooltip("A nice crosshair texture")]
		public Texture2D crosshairs;
		[Tooltip("How much to increase the size of the crosshairs when indicating spread")]
		public float crossSpreadScalar = 1.0f;
		
		private bool saved = false;

		public HelpInfo help = new HelpInfo("Althoug originally developed for AK-47s and the like, this can be used for anything from a crossbow to a plasma rifle. It's a " +
			"multipurpose solution to get some decent FPS action going on. You will need to also set up a projectile prefab and some ammo handling using the included 'ClipInventory'" +
			" and related functionality. Clips are discarded on reload.");
		
		void Start () {
			if (image == null) {
				Animator _anim = GetComponentInChildren<Animator>();
				if (_anim != null)
					image = _anim.gameObject;
			}
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

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref fireMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref reloadingMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref ammoExhaustedMessage, gameObject);
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
			if (Cursor.lockState == CursorLockMode.Locked) {
				if (useFireButton && Input.GetButton(fireButton))
					Fire ();
			}
		}
		
		public void Fire() {
			if (refireCounter > 0)
				return;
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
				if (image != null && !string.IsNullOrEmpty(mecanimAmmoExhaustedTrigger))
					image.GetComponent<Animator>().SetTrigger(mecanimAmmoExhaustedTrigger);

				if (ammoExhaustedClick != null)
					GetComponent<AudioSource>().PlayOneShot(ammoExhaustedClick);
				if (!string.IsNullOrEmpty(ammoExhaustedMessage.message))
					MessageManager.Send(ammoExhaustedMessage);
				if (reloadMessageReceiver != null)
					reloadMessageReceiver.SendMessage("Reload",SendMessageOptions.DontRequireReceiver);
				if (reloadMessageReceiver != gameObject)
					Reload();
				return;
			}
			if (magazineCount > 0 && !reloading) {

				if (image != null && !string.IsNullOrEmpty(mecanimFireTrigger))
					image.GetComponent<Animator>().SetTrigger(mecanimFireTrigger);

				magazineCount -= 1;
				currentSpread += roundSpreadCost;
				if (currentSpread > muzzleSpreadMax)
					currentSpread = muzzleSpreadMax;
				if (aimCorrection == AimCorrectionTypes.None)
					muzzleTransform.transform.localEulerAngles = new Vector3(muzzleOrientation.x + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleOrientation.y + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleOrientation.z);
				else	
					muzzleTransform.transform.localEulerAngles = new Vector3(muzzleTransform.transform.localEulerAngles.x + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleTransform.transform.localEulerAngles.y + Random.Range(-(currentSpread + muzzleSpreadMin), currentSpread + muzzleSpreadMin), muzzleTransform.transform.localEulerAngles.z);
				refireCounter = refireTime;

				GameObject bullet;
				if (projectile != null) {
					bullet = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
					Rigidbody _rigid = bullet.GetComponent<Rigidbody>();
					Rigidbody _myRigid = transform.root.GetComponentInChildren<Rigidbody>();
					if (_rigid != null && _myRigid)
						_rigid.velocity = _myRigid.velocity * inheritVelocityScale;
					Bullet proj = bullet.GetComponent<Bullet>();
					if (proj != null){
						proj.SendMessage("SetOwner", transform.root.gameObject, SendMessageOptions.DontRequireReceiver);
					}
				}
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
					GetComponent<AudioSource>().PlayOneShot(fireSound);
				if (!string.IsNullOrEmpty(fireMessage.message))
					MessageManager.Send(fireMessage);
			}
		}
		
		public void Reload () {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player == null)
				player = gameObject.transform.root.gameObject;
			ClipInventory clipInv = player.GetComponent<ClipInventory>();
			if (clipInv == null) {
				Debug.LogError("Clip Inventory component not found on the player! Please add one, and set up some ammo clip types.");
				enabled = false;
				return;
			}
			if (clipInv.numClips[clipType] > 0) {
				clipInv.numClips[clipType]--;
				if (image != null && !string.IsNullOrEmpty(mecanimReloadTrigger))
					image.GetComponent<Animator>().SetTrigger(mecanimReloadTrigger);
				if (!string.IsNullOrEmpty(reloadingMessage.message))
					MessageManager.Send(reloadingMessage);
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
}