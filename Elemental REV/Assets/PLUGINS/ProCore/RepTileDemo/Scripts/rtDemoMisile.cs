using UnityEngine;
using System.Collections;

public class rtDemoMisile : MonoBehaviour
{
	private float totalDistance;
	private Vector3 startPosition;
	private RepTile.int3 targetIndex;
	private Vector3 targetPosition;
	private float startTime;
	private float totalTime;
	private Vector3 currentPosition;
	private float timeSinceStart;
	
	public void SetTarget(RepTile.Map targetMap,RepTile.int3 targetIndex)
	{
		this.targetIndex = targetIndex;
		targetPosition = RepTile.Public.CellIndexCenterToWorldPoint(rtDemoGameManager.gameMap,targetIndex);
		
		startPosition = transform.position;
		
		startTime = Time.time;
		totalDistance = Vector3.Distance(transform.position,targetPosition);
		totalTime = totalDistance / 20f;
	}
		
	private static float tAlongPath = 0f;
	void Update()
	{
		timeSinceStart = Time.time - startTime;
		tAlongPath = (1 - (totalTime - timeSinceStart) / totalTime);
	
		if(tAlongPath < 1f)
		{
			currentPosition = Vector3.Lerp(startPosition,targetPosition, tAlongPath);
		
			transform.position = currentPosition;
		}
		
		else
		{
			Detonate();
		}
	}
	
	void Detonate()
	{
		//spawn explosion effect
		//destroy target tile
		rtDemoGameManager.DestroyTile(targetIndex);
			
		rtDemoEffect.Explode(this.transform.position);
		GameObject.Destroy(this.gameObject);
		
	}
}
