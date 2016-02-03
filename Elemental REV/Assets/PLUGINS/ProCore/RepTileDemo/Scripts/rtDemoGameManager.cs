using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rtDemoGameManager : MonoBehaviour
{
	public static RepTile.Map gameMap;
	public static rtDemoCamera gameCam;

	//	To keep things simple for this example,
	//	the game manager is a MonoBehaviour that we manually place in a scene.
	//	It spawns the map and the camera on awake.
	void Awake()
	{
		SpawnMap(0);
		
		SpawnCamera();
	}

	public static void SpawnMap(int mapNumber)
	{
		for(int i = 0; i < debrisObjects.Count; i++)
			Destroy(debrisObjects[i]);
		
		debrisObjects = new List<GameObject>();
		
		if(gameMap != null)
			Destroy(gameMap.gameObject);
		
		GameObject mapInstance;
		GameObject mapPrefab = null;
		
		switch(mapNumber)
		{
			case 0:
				mapPrefab = Resources.Load("rtDemoMap") as GameObject;
			break;
			
			case 1:
				mapPrefab = Resources.Load("rtDemoMap2") as GameObject;
			break;
		}
		
		if(mapPrefab != null)
		{
			mapInstance = GameObject.Instantiate(mapPrefab) as GameObject;
			mapInstance.transform.position = Vector3.zero;
			gameMap = (RepTile.Map)mapInstance.GetComponent(typeof(RepTile.Map));
		}
		
		else
			Debug.Log("RepTile Demo: rtDemoMap asset is missing");
	}

	private static void SpawnCamera()
	{
		GameObject cam = new GameObject("rtDemoCamera");
		gameCam = (rtDemoCamera)cam.AddComponent(typeof(rtDemoCamera));
		gameCam.transform.position = new Vector3(10f,4f,-10f);
		gameCam.transform.LookAt(new Vector3(0f,3f,-0f));
	}
	
	public static void DestroyTile(RepTile.int3 targetIndex)
	{
		//	the RemoveAtIndex method returns the indices of successfully removed tiles (for multi-tiles, this includes all of the cells they occupy)
		List<RepTile.int3> destroyedIndices = RepTile.Public.RemoveAtIndex(gameMap,targetIndex);
		
		for(int i = 0; i < destroyedIndices.Count; i++)
		{
			DestroyTile(destroyedIndices[i]);
			
			rtDemoEffect.Crumble(RepTile.Public.CellIndexCenterToWorldPoint(gameMap,destroyedIndices[i]));
			
			RepTile.int3 indexAbove = destroyedIndices[i] + RepTile.int3.up;
			
			if(!destroyedIndices.Contains(indexAbove))
			{
				if(RepTile.Public.CellOccupied(gameMap,indexAbove))
				{
					DestroyTile(indexAbove);
				}
			}
			
			CheckAdjacentUnsupported(destroyedIndices[i]);
		}
	}
	
	//	this is the entry point to a simple recursive probe which follows the contiguous occupied indices adjacent to the given cell index
	//	to see if any of the adjacent tiles is no longer part of a tile structure that touches the zero y index (ground) 
	//	we want to detach any such disconnected regions into debris objects.
	private static void CheckAdjacentUnsupported(RepTile.int3 cellIndex)
	{
		for(int i = 0; i < 6; i++)
		{
			if(RepTile.Public.CellOccupied(gameMap,cellIndex + RepTile.int3.adjacentIndices[i]))
			{
				if(CheckSupportedRecursiveStart(cellIndex + RepTile.int3.adjacentIndices[i]) == false)
					CreateDebrisObject(cellIndex + RepTile.int3.adjacentIndices[i]);
			}
		}
	}
	
	public static List<GameObject> debrisObjects = new List<GameObject>();
	private static void CreateDebrisObject(RepTile.int3 startIndex)
	{
		List<RepTile.int3> contiguousRegion = RepTile.Public.GetContiguousIndices(gameMap,startIndex);
		rtDemoDebrisObject.Create(gameMap,contiguousRegion);
	}

	private static List<RepTile.int3> visitedIndices = new List<RepTile.int3>();
	private static bool CheckSupportedRecursiveStart(RepTile.int3 cellIndex)
	{
		visitedIndices = new List<RepTile.int3>();
		return CheckSupportedRecursive(cellIndex);
	}
	private static bool CheckSupportedRecursive(RepTile.int3 cellIndex)
	{
		visitedIndices.Add(cellIndex);
		if(cellIndex.y == 0)
			return true;
		
		for(int i = 0; i < 6; i++)
		{	
			if(visitedIndices.Contains(cellIndex + RepTile.int3.adjacentIndices[i]))
				continue;
					
			if(RepTile.Public.CellOccupied(gameMap,cellIndex + RepTile.int3.adjacentIndices[i]))
			{
				if(CheckSupportedRecursive(cellIndex + RepTile.int3.adjacentIndices[i]) == true)
					return true;
			}
		}	
		return false;
	}
	
}
