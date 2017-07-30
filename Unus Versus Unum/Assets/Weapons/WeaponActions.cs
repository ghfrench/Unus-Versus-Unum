using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponActions : MonoBehaviour {

	public Weapon weapon;
	public GameObject[] weapons = new GameObject[3];
	public Text weaponUI;
	private bool isSwitching0, isSwitching1, isSwitching2 = false;
	private float switchSeconds = 1;
	private float switchTimer = 0;
	public Vector3 equippedPosition0, equippedPosition1, equippedPosition2, holsterPosition;
	public Quaternion equippedRotation0, equippedRotation1, equippedRotation2, holsterRotation;

	void Start () {}//Start

	void Update () {
		Vector3 forward = weapon.rayCaster.transform.TransformDirection (Vector3.forward) * weapon.range;
		RaycastHit hit;
		Recoil ();

		if (Input.GetKeyDown (KeyCode.C)) {
			if (weapons [0].activeSelf == true) {
				startSwitch0 ();
			}
			else if (weapons [1].activeSelf == true) {
				startSwitch1 ();
			}
			else {
				startSwitch2 ();
			}
		}//weapon switch checker

		if (isSwitching0) {
			switchTimer += Time.deltaTime;
			if (switchTimer > switchSeconds) {
				stopSwitch0 ();
			}
			else {
				float ratio = switchTimer / switchSeconds;
				weapons [0].transform.localPosition = Vector3.Lerp(equippedPosition0, holsterPosition, ratio);
				weapons [0].transform.localRotation = Quaternion.Slerp(equippedRotation0, holsterRotation, ratio);
				weapons [1].transform.localPosition = Vector3.Lerp(holsterPosition, equippedPosition1, ratio);
				weapons [1].transform.localRotation = Quaternion.Slerp(holsterRotation, equippedRotation1, ratio);
			}
		}//weapon switch 0

		if (isSwitching1) {
			switchTimer += Time.deltaTime;
			if (switchTimer > switchSeconds) {
				stopSwitch1 ();
			}
			else {
				float ratio = switchTimer / switchSeconds;
				weapons [1].transform.localPosition = Vector3.Lerp(equippedPosition1, holsterPosition, ratio);
				weapons [1].transform.localRotation = Quaternion.Slerp(equippedRotation1, holsterRotation, ratio);
				weapons [2].transform.localPosition = Vector3.Lerp(holsterPosition, equippedPosition2, ratio);
				weapons [2].transform.localRotation = Quaternion.Slerp(holsterRotation, equippedRotation2, ratio);
			}
		}//weapon switch 1

		if (isSwitching2) {
			switchTimer += Time.deltaTime;
			if (switchTimer > switchSeconds) {
				stopSwitch2 ();
			}
			else {
				float ratio = switchTimer / switchSeconds;
				weapons [2].transform.localPosition = Vector3.Lerp(equippedPosition2, holsterPosition, ratio);
				weapons [2].transform.localRotation = Quaternion.Slerp(equippedRotation2, holsterRotation, ratio);
				weapons [0].transform.localPosition = Vector3.Lerp(holsterPosition, equippedPosition0, ratio);
				weapons [0].transform.localRotation = Quaternion.Slerp(holsterRotation, equippedRotation0, ratio);
			}
		}//weapon switch 2

		if (Input.GetKeyDown (KeyCode.R) || Input.GetMouseButtonDown (0)) {
			StopCoroutine ("Reload");
		}//stop reloading

		if (Input.GetKeyDown (KeyCode.R) && weapon.ammoReserve >= 1 && weapon.ammoInClip < weapon.clipSize) {
			StartCoroutine ("Reload");
		}//reloading

		if (weapon.mode == Weapon.fireMode.semi && Input.GetMouseButtonDown (0) && weapon.ammoInClip >= 1) {
			weapon.ammoInClip --;
			weapon.muzzleFlash.Stop ();
			weapon.fireSound.Stop ();
			weapon.muzzleFlash.Play ();
			weapon.fireSound.Play ();
			if (Physics.Raycast (weapon.rayCaster.transform.position, forward, out hit, weapon.range)) {
				if (hit.transform.tag == "Enemy") {
					weapon.bloodSplatter.transform.position = hit.point;
					weapon.bloodSplatter.Play ();
					/*
					EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth> ();
					if (enemyHealth != null) {
						enemyHealth.TakeDamage (weapon.DPB);
					}
					*/
				}
				else {
					Instantiate (weapon.bullethole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
				}
			}
			weapon.recoil += Time.deltaTime/(60/weapon.RPM);
		}//firing semi

		if (weapon.mode == Weapon.fireMode.auto && Input.GetMouseButton (0) && weapon.ammoInClip >= 1) {
			int aIC = (int)weapon.ammoInClip;
			weapon.ammoInClip -= Time.deltaTime/(60/weapon.RPM);
			if (aIC == (int)weapon.ammoInClip + 1) {
				weapon.muzzleFlash.Stop ();
				weapon.fireSound.Stop ();
				weapon.muzzleFlash.Play ();
				weapon.fireSound.Play ();
				if (Physics.Raycast (weapon.rayCaster.transform.position, forward, out hit, weapon.range)) {
					if (hit.transform.tag == "Enemy") {
						weapon.bloodSplatter.transform.position = hit.point;
						weapon.bloodSplatter.Play ();
						/*EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth> ();
						if (enemyHealth != null) {
							enemyHealth.TakeDamage (weapon.DPB);
						}
						*/
					}
					else {
						Instantiate (weapon.bullethole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
					}
				}
				weapon.recoil += Time.deltaTime/(60/weapon.RPM);
			}
		}//firing auto

		/*
		weaponUI.text = "\t" + "Weapon : " + weapon.name + "\n"
					  + "\t" + "Ammo in Clip : " + (int)weapon.ammoInClip + "\n";
		if (weapon.type == Weapon.weaponType.pistol) {
			weaponUI.text += "\t" + "Ammo in Reserve : Infinity";
		}
		else {
			weaponUI.text += "\t" + "Ammo in Reserve : " + (int)weapon.ammoReserve;
		}
		*/
	}//Update

	void startSwitch0() {
		isSwitching0 = true;
		switchTimer = 0;
		weapons [1].transform.localPosition = holsterPosition;
		weapons [1].transform.localRotation = holsterRotation;
		weapons [1].SetActive (true);

	}//startSwitch0

	void stopSwitch0() {
		isSwitching0 = false;
		weapons [0].transform.localPosition = holsterPosition;
		weapons [0].transform.localRotation = holsterRotation;
		weapons [1].transform.localPosition = equippedPosition1;
		weapons [1].transform.localRotation = equippedRotation1;
		weapons [0].SetActive (false);
	}//stopSwitch0

	void startSwitch1() {
		isSwitching1 = true;
		switchTimer = 0;
		weapons [2].transform.localPosition = holsterPosition;
		weapons [2].transform.localRotation = holsterRotation;
		weapons [2].SetActive (true);
	}//startSwitch1

	void stopSwitch1() {
		isSwitching1 = false;
		weapons [1].transform.localPosition = holsterPosition;
		weapons [1].transform.localRotation = holsterRotation;
		weapons [2].transform.localPosition = equippedPosition2;
		weapons [2].transform.localRotation = equippedRotation2;
		weapons [1].SetActive (false);
	}//stopSwitch1

	void startSwitch2() {
		isSwitching2 = true;
		switchTimer = 0;
		weapons [0].transform.localPosition = holsterPosition;
		weapons [0].transform.localRotation = holsterRotation;
		weapons [0].SetActive (true);
	}//startSwitch2

	void stopSwitch2() {
		isSwitching2 = false;
		weapons [2].transform.localPosition = holsterPosition;
		weapons [2].transform.localRotation = holsterRotation;
		weapons [0].transform.localPosition = equippedPosition0;
		weapons [0].transform.localRotation = equippedRotation0;
		weapons [2].SetActive (false);
	}//stopSwitch2
	
	IEnumerator Reload() {
		if (weapon.type == Weapon.weaponType.shotgun) {
			for (int i = 0; weapon.ammoInClip != weapon.clipSize; i++) {
				weapon.reloadSound.Play ();
				yield return new WaitForSeconds (weapon.reloadTime);
				weapon.ammoReserve--;
				weapon.ammoInClip++;
			}
		}
		else {
			weapon.reloadSound.Play ();
			yield return new WaitForSeconds (weapon.reloadTime);
			if (weapon.ammoReserve >= weapon.clipSize) {
				weapon.ammoReserve -= (weapon.clipSize - weapon.ammoInClip);
				weapon.ammoInClip = weapon.clipSize;
			}
			else if (weapon.ammoReserve + weapon.ammoInClip > weapon.clipSize) {
				weapon.ammoReserve -= (weapon.clipSize - weapon.ammoInClip);
				weapon.ammoInClip = weapon.clipSize;
			}
			else {
				weapon.ammoInClip += weapon.ammoReserve;
				weapon.ammoReserve = 0;
			}
		}
	}//Reload

	void Recoil() {
		if (weapon.recoil > 0) {
			var maxRecoil = Quaternion.Euler(weapon.maxRecoilX+Random.Range( -5, 5), weapon.maxRecoilY+Random.Range(-5, 5), 0);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime*weapon.recoilRate);
			weapon.recoil -= Time.deltaTime/(60/weapon.RPM);
		}
		else
		{
			weapon.recoil = 0;
			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime*weapon.recoilRate);
		}
	}//Recoil

}//WeaponActions

[System.Serializable]
public class Weapon {
	public string name;
	public AudioSource fireSound;
	public AudioSource reloadSound;
	public ParticleSystem muzzleFlash;
	public ParticleSystem bloodSplatter;
	public GameObject rayCaster;
	public GameObject bullethole;
	public enum weaponType {pistol = 0, shotgun = 1, autorifle = 2};
	public weaponType type;
	public enum fireMode {semi = 0, auto = 1};
	public fireMode mode;
	public int DPB = 0;
	public float RPM = 0;
	public float clipSize = 0;
	public float ammoInClip = 0;
	public float reloadTime = 0;
	public float range = 0;
	public float recoil = 0;
	public float recoilRate = 0;
	public float maxRecoilX = 0;
	public float maxRecoilY = 0;
	public float ammoReserve = 0;
}//Weapon