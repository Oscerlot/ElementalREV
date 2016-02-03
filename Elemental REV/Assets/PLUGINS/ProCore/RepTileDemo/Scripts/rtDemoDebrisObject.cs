using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rtDemoDebrisObject : MonoBehaviour 
{
	public RepTile.Map debrisMap;
	public Rigidbody physicsBody;
	
	private static float tileMass = 0.10f;
	public static void Create(RepTile.Map sourceMap, List<RepTile.int3> indexList)
	{
		RepTile.Map newMap = RepTile.Public.NewMapFromIndexList(sourceMap,indexList);
		
		if(newMap != null)
		{
			newMap.gameObject.name = "DebrisMap";
			
			rtDemoDebrisObject debrisObjectScript = newMap.gameObject.AddComponent(typeof(rtDemoDebrisObject)) as rtDemoDebrisObject;
			debrisObjectScript.debrisMap = newMap;
			debrisObjectScript.physicsBody = newMap.gameObject.AddComponent(typeof(UnityEngine.Rigidbody)) as Rigidbody;
			rtDemoGameManager.debrisObjects.Add(newMap.gameObject);
			
			debrisObjectScript.UpdateMass();
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(debrisMap == null)
			CleanUp();
		
		List<RepTile.int3> impacted = new List<RepTile.int3>();
		RepTile.int3 index;
		
		foreach(ContactPoint cp in collision.contacts)
		{
			index = RepTile.Public.WorldPointToCellIndex(debrisMap,cp.point);
			
			if(!impacted.Contains(index))
				impacted.Add(index);
		}
		
		for(int i = 0; i < impacted.Count; i++)
		{
			RepTile.Public.RemoveAtIndex(debrisMap,impacted[i]);
			rtDemoEffect.Crumble(RepTile.Public.CellIndexCenterToWorldPoint(debrisMap,impacted[i]));
		}
		
		if(RepTile.Public.MapIsEmpty(debrisMap) == true)
			CleanUp();
		
		List<List<RepTile.int3>> contiguousRegions = RepTile.Public.GetContiguousRegions(debrisMap);
		
		if(contiguousRegions.Count > 1)
		{
			for(int i = 0; i < contiguousRegions.Count; i++)
			{
				if(contiguousRegions[i].Count == 0)
					continue;
					
				Create(debrisMap,contiguousRegions[i]);
			}
			
			CleanUp();
		}
		
		else
		{
			if(RepTile.Public.MapIsEmpty(debrisMap) == true)
				CleanUp();
			
			else
			{
				UpdateMass();
			}
		}
	}
	
	
	private void UpdateMass()
	{
		int tileCount = RepTile.Public.GetTotalTileCount(debrisMap,false);
		if(tileCount > 0)
			physicsBody.mass = tileCount * tileMass;
	
		physicsBody.centerOfMass = transform.InverseTransformPoint(RepTile.Public.FindCenterOfMass(debrisMap));
	}
	
	private void CleanUp()
	{
		rtDemoGameManager.debrisObjects.Remove(this.gameObject); 
		Object.Destroy(this.gameObject);
	}
}
