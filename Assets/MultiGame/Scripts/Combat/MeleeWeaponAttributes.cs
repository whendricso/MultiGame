using UnityEngine;
using System.Collections;

public class MeleeWeaponAttributes : MonoBehaviour {
	
	public float damage = 10.0f;
	public float swingTime = 1.0f;
	public enum ElementalTypes {None, Life, Light, Lightning, Wind, Water, Earth, Fire, Darkness, Death};
	public ElementalTypes elementalType = ElementalTypes.None;
	public bool canBlock = false;
	
	
}
