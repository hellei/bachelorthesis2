using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// The easiest connection type. Creates a line between object a and b
	/// </summary>
	public class LineConnection : Connection {

	    public GameObject endPointsPrefab;
	    public GameObject linePrefab;
	    public float linePrefabLength = 0.01f;

	    bool objectsSet = false;

	    public override void SetObjects(GameObject a, GameObject b)
	    {
	        if (!objectsSet)
	        {
	            objectsSet = true;

	            Vector3 minA = Vector3.zero, maxA = Vector3.zero;
	            a.GetBounds(ref minA, ref maxA);
	            
	            Vector3 minB = Vector3.zero, maxB = Vector3.zero;
	            b.GetBounds(ref minB, ref maxB);

	            minA.x = 0;
	            minB.x = 0;

	            Vector3 posA = a.transform.position + minA;
	            Vector3 posB = b.transform.position + minB;

	            GameObject startPoint = (GameObject)Instantiate(endPointsPrefab, posA, Quaternion.identity);
	            GameObject endPoint = (GameObject)Instantiate(endPointsPrefab, posB, Quaternion.identity);

	            /*startPoint.transform.LookAt(b.transform);
	            endPoint.transform.LookAt(a.transform);*/
	            Vector3 center = (posA + posB) * 0.5f;
	            GameObject line = (GameObject)Instantiate(linePrefab, center, Quaternion.identity);
	            line.transform.LookAt(posB);

	            float distance = (posB - posA).magnitude;
	            line.transform.localScale = new Vector3(line.transform.localScale.x, line.transform.localScale.y, line.transform.localScale.z * distance / linePrefabLength);

	            transform.position = center;
	            startPoint.transform.parent = endPoint.transform.parent = line.transform.parent = transform;
	        }
	    }
	}
}
