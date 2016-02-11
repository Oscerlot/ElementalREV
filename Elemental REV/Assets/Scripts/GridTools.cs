using System.Linq;
using UnityEngine;

public class GridTools : MonoBehaviour
{

    private static GridTools GT_Singleton = null;


    public static GridTools Instance
    {
        get
        {

            if (GT_Singleton == null)
            {
                GT_Singleton = new GameObject("GridTools").AddComponent<GridTools>();

            }
            return GT_Singleton;

        }
    }


    void Awake()
    {
        if (GT_Singleton)
        {
            DestroyImmediate(gameObject); // delete duplicates (if any)
        }
        else {
            GT_Singleton = this;
            DontDestroyOnLoad(gameObject); //in order to preserve this object in new screen loads.
        }
    }



    private readonly Vector3 GridOffset = new Vector3(.5f,.5f,.5f);


    public bool PositionIsAccessible(Vector3 position, GameObject [] gameObjectsToIgnore)
    {
        Vector3 newPos = SnapVectorToGrid(position);
        Collider[] collidingObjects = Physics.OverlapBox(newPos, new Vector3(.4f, .4f, .4f), Quaternion.identity);
        bool positionIsAccessible = false;

        foreach (var go in gameObjectsToIgnore)
        {
            positionIsAccessible = !(collidingObjects.Length > 0 && collidingObjects.Any(collidingObject => !collidingObject.gameObject.Equals(go)));
        }

        return positionIsAccessible;

    }


    public Vector3 SnapVectorToGrid(Vector3 position)
    {
        position -= GridOffset;
        Vector3 newVector = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
        newVector += GridOffset;
        
        return newVector;
    }

	
}
