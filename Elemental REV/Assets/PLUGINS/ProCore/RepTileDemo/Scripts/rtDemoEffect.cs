using UnityEngine;
using System.Collections;

public static class rtDemoEffect
{
	private static GameObject explosionPrefab = Resources.Load("rtDemoExplosionEmmitter") as GameObject;
	private static ParticleSystem explosionSystem;
	private static ParticleSystem explosionSystem2;
	public static void Explode(Vector3 position)
	{
		if(explosionSystem == null || explosionSystem2 == null)
		{	GameObject newObject = (GameObject)GameObject.Instantiate(explosionPrefab,position,Quaternion.identity);
			explosionSystem = newObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
			explosionSystem2 = newObject.transform.GetChild(0).GetComponent(typeof(ParticleSystem)) as ParticleSystem;
		}
		
		explosionSystem.transform.position = position;
		explosionSystem.Emit(25);
		explosionSystem2.Emit(1);
	}
	
	private static GameObject crumblePrefab = Resources.Load("rtDemoCrumbleEmmitter") as GameObject;
	private static ParticleSystem crumbleSystem;
	public static void Crumble(Vector3 position)
	{
		if(crumbleSystem == null)
		{	GameObject newObject = (GameObject)GameObject.Instantiate(crumblePrefab,position,Quaternion.identity);
			crumbleSystem = newObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
		}
		
		crumbleSystem.transform.position = position;
		crumbleSystem.Emit(2);
	}
	
}
