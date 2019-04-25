using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Modern Gun")]
	[RequireComponent (typeof(AudioSource))]
	public class ModernGun : MultiModule {

		public enum AimCorrectionTypes {None, Raycast, DistantPoint};

		[Header("IMGUI Settings")]
		[Tooltip("Show how much ammo we currently have with a legacy Unity GUI? Not suitable for mobile devices.")]
		public bool showAmmoGUI = true;
		[Tooltip("Normalized viewport rectangle representing the area used by the legacy GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.01f, 0.9f, 0.2f, 0.09f);
		public GUISkin guiSkin;
		[RequiredFieldAttribute("A nice crosshair texture",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public Texture2D crosshairs;
		[RequiredFieldAttribute("How much to increase the size of the crosshairs when indicating spread")]
		public float crossSpreadScalar = 1.0f;

		[Header("Input Settings")]
		[Tooltip("Should we use Unity's built-in input for fire button handling? If false, send 'Fire' each frame.")]
		public bool useFireButton = true;
		[RequiredFieldAttribute("The button in the Input Manager associated with shooting things.",RequiredFieldAttribute.RequirementLevels.Required)]
		public string fireButton = "Fire1";
		public KeyCode reload = KeyCode.R;

		[Header("Action Settings")]
		[Tooltip("What type of aim correction should we use? Raycast aims directly at what we point at, distant point aims at a point in the far distance representing our crosshair location (recommended for FPS games)")]
		public AimCorrectionTypes aimCorrection = AimCorrectionTypes.None;
		[RequiredFieldAttribute("What do we spawn from the muzzle of the gun?")]
		public GameObject projectile;
		[Tooltip("Should the projectiles be added to a pool the first time they're spawned so that they can be re-used from the pool later?")]
		public bool poolProjectiles = false;
		[Tooltip("Multiplied by the velocity before it's transferred to the projectile")]
		public float inheritVelocityScale = 1f;
		[RequiredFieldAttribute("How many shots per mag?")]
		public int magazineMax = 32;
		[Tooltip("Which clip type from Clip Inventory do we use? First clip type is 0, second is 1, and so forth.")]
		public int magazineType = 0;
		[RequiredFieldAttribute("How long does it take us to reload?")]
		public float reloadTime = 2.2f;
		[RequiredFieldAttribute("A game object representing the exit point of the projectile")]
		public GameObject muzzleTransform;
		[RequiredFieldAttribute("How long, in seconds, between each bullet?")]
		public float refireTime = 0.4f;
		[RequiredFieldAttribute("Minimum variation of a bullet from center")]
		public float muzzleSpreadMin = .2f;
		[RequiredFieldAttribute("Maximum variation of a bullet from center")]
		public float muzzleSpreadMax = 2.0f;
		[RequiredFieldAttribute("How much the spread increases each time a projectile is discharged")]
		public float roundSpreadCost = .14f;
		[RequiredFieldAttribute("How fast the spread decreases")]
		public float refocusRate = .16f;

		[Header("Audio Settings")]
		[RequiredFieldAttribute("'BANG!'", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioClip fireSound;
		[RequiredFieldAttribute("Sound made when reloading", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioClip reloadSound;
		[RequiredFieldAttribute("The worst sound you can hear in a firefight",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioClip ammoExhaustedClick;

		[Header("Visual Settings")]
		[RequiredFieldAttribute("The model of the weapon, if unassigned and no Animator found, no animations will be sent!", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject image;
		[RequiredFieldAttribute("An object spawned at the muzzle transform representing visual flash effects", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject muzzleFlash;
		[Tooltip("An optionally different flash spawn transform")]
		public GameObject muzzleFlashSpawnTransform;
		[Tooltip("How long should the flash object be alive?")]
		public float flashDuration = 0.125f;
		[RequiredFieldAttribute("A Mecanim trigger that will be sent to the Image",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string mecanimFireTrigger;
		[RequiredFieldAttribute("A Mecanim trigger that will be sent to the Image",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string mecanimReloadTrigger;
		[RequiredFieldAttribute("A Mecanim trigger that will be sent to the Image",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string mecanimAmmoExhaustedTrigger;

		[Header("Message Senders")]
		[Tooltip("Message sent on successfully firing a round")]
		public MessageManager.ManagedMessage fireMessage;
		[Tooltip("Message sent when starting to reload")]
		public MessageManager.ManagedMessage reloadingMessage;
		[Tooltip("Message sent when we have run out of ammo")]
		public MessageManager.ManagedMessage ammoExhaustedMessage;

		[HideInInspector]
		public GameObject reloadMessageReceiver;
		[HideInInspector]
		public int magazineCount;
		[HideInInspector]
		public bool reloading = false;
		private float currentSpread;
		private float refireCounter;
		private Vector3 muzzleOrientation;
		private bool saved = false;
		GameObject distantPoint;
		GameObject pointCaster;
		public bool debug = false;
		GameObject bullet;
		private List<GameObject> objectPool = new List<GameObject>();

		public HelpInfo help = new HelpInfo("Althoug originally developed for AK-47s and the like, this can be used for anything from a crossbow to a plasma rifle. It's a " +
			"multipurpose solution to get some decent FPS action going on. You will need to also set up a projectile prefab and some ammo handling using the included 'ClipInventory'" +
			" and related functionality. Clips are discarded on reload. \n\n" +
			"" +
			"Pooling should only be used on mobile, to increase the stability of the game. It does this by delaying invocation of the garbage collector to clean up destroyed objects " +
			"until the scene is unloaded.");
		
		void Start () {
			//set up aim correction
			distantPoint = new GameObject("DistantPoint");
			pointCaster = new GameObject("PointCaster");
			pointCaster.transform.parent = Camera.main.transform;
			pointCaster.transform.localPosition = Vector3.zero;
			pointCaster.transform.localRotation = Quaternion.identity;
			distantPoint.transform.parent = pointCaster.transform;
			distantPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 1500.0f);

			if (image == null) {
				Animator _anim = GetComponentInChildren<Animator>();
				if (_anim != null)
					image = _anim.gameObject;
			}

			if (PlayerPrefs.HasKey(gameObject.name + "magazineCount")) {
				magazineCount = PlayerPrefs.GetInt(gameObject.name + "magazineCount");
				Debug.Log("Loaded " + magazineCount);
				
			}
			currentSpread = (muzzleSpreadMin + muzzleSpreadMax)*.5f;
			refireCounter = refireTime;
			if (muzzleTransform == null) {
				Debug.LogError("ModernGun requires a muzzle transform! Please assign one in the inspector, it's an empty Game Object at the end of the barrel where the bullets come out!");
				muzzleTransform = gameObject;
			}
				muzzleOrientation = muzzleTransform.transform.localEulerAngles;
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref fireMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref reloadingMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref ammoExhaustedMessage, gameObject);
		}
		
		void OnGUI () {
			if (showAmmoGUI) {
				GUILayout.Window( 10000, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), AmmoWindow, "");
			}

			if (crosshairs == null)
				return;
			float adjustedWidth = crosshairs.width * crossSpreadScalar;
			float adjustedHeight = crosshairs.height * crossSpreadScalar;
			GUI.DrawTexture(new Rect(((Screen.width * .5f) - ((currentSpread * adjustedWidth) * .5f)), (Screen.height * .5f) - ((currentSpread * adjustedHeight) * .5f), (adjustedWidth * currentSpread), (adjustedHeight * currentSpread)), crosshairs, ScaleMode.ScaleToFit);
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
					Fire();
			}
			else if (debug) {
				Debug.Log("Modern Gun " + gameObject.name + " cannot fire because the cursor is not locked. Try adding a CursorLock component to the game!");
			}
		}

		[Header("Available Messages")]
		public MessageHelp fireHelp = new MessageHelp("Fire", "Causes the ranged weapon to emit a projectile, respecting all firing rules.");
		public void Fire() {
			if (!gameObject.activeInHierarchy)
				return;
			if (refireCounter > 0)
				return;
			RaycastHit hinfo;
			if (aimCorrection == AimCorrectionTypes.Raycast) {
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width*.5f,Screen.height*.5f,0.0f));
				bool didHit = Physics.Raycast(ray, out hinfo);
				if (didHit) {
					muzzleTransform.transform.LookAt(hinfo.point);
				}
				if (!didHit) {
					muzzleTransform.transform.LookAt(distantPoint.transform.position);
				}
			}
			
			if (aimCorrection == AimCorrectionTypes.DistantPoint) {
				muzzleTransform.transform.LookAt(distantPoint.transform.position);
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

				if (projectile != null) {
					bullet = SpawnBullet();//Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
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

		private GameObject SpawnBullet() {
			GameObject _bullet = null;

			if (!poolProjectiles)
				_bullet = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
			else {
				_bullet = FindPooledObject();
				if (_bullet == null) {
					_bullet = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
					if (_bullet.GetComponent<CloneFlagRemover>() == null)
						_bullet.AddComponent<CloneFlagRemover>();
					objectPool.Add(_bullet);
				}
				else {
					SpawnFromPool(_bullet);
				}
			}

			return _bullet;
		}

		private void SpawnFromPool(GameObject obj) {
			obj.transform.position = muzzleTransform.transform.position;
			obj.transform.rotation = muzzleTransform.transform.rotation;
			obj.SetActive(true);
			obj.BroadcastMessage("ReturnFromPool", SendMessageOptions.DontRequireReceiver);
		}

		/// <summary>
		/// Searches the heirarchy for a pooled (disabled) object
		/// </summary>
		/// <returns></returns>
		private GameObject FindPooledObject() {
			GameObject ret = null;

			foreach (GameObject obj in objectPool) {
				if (!obj.activeInHierarchy) {
					ret = obj;
					break;
				}
			}

			return ret;
		}

		public MessageHelp reloadHelp = new MessageHelp("Reload","Initiates a reloading sequence for the weapon.");
		public void Reload () {
			if (!gameObject.activeInHierarchy)
				return;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player == null)
				player = gameObject.transform.root.gameObject;
			ClipInventory clipInv = player.GetComponentInChildren<ClipInventory>();
			if (clipInv == null) {
				/*Debug.LogError("Clip Inventory component not found on the player! Please add one to the object tagged 'Player', and set up some ammo clip types.");
				enabled = false;
				return;*/
				StartCoroutine(FinishReload(reloadTime));
			} else {
				if (clipInv.numClips[magazineType] > 0) {
					if (debug)
						Debug.Log("Clip found for weapon, reloading...");
					clipInv.numClips[magazineType]--;
					if (image != null && !string.IsNullOrEmpty(mecanimReloadTrigger))
						image.GetComponent<Animator>().SetTrigger(mecanimReloadTrigger);
					if (!string.IsNullOrEmpty(reloadingMessage.message))
						MessageManager.Send(reloadingMessage);
					reloading = true;
					StartCoroutine(FinishReload(reloadTime));
				}  else {
					if (debug)
						Debug.Log("Not enough clips left for this weapon type");
				}
			}
		}
		
		public IEnumerator FinishReload (float delay) {
			yield return new WaitForSeconds(delay);
			magazineCount = magazineMax;
			reloading = false;
		}
		
		void OnDestroy () {
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