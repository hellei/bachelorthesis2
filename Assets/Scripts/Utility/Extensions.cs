using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {
    //This function returns a point which is a projection from a point to a plane.
    public static Vector3 ProjectPointOnPlane(this Plane plane, Vector3 planePoint, Vector3 point, out float sign)
    {

        float distance;
        Vector3 translationVector;

        //First calculate the distance from the point to the plane:
        distance = SignedDistancePlanePoint(plane.normal, planePoint, point);

        sign = distance > 0 ? 1 : -1;

        //Reverse the sign of the distance
        distance *= -1;

        //Get a translation vector
        translationVector = SetVectorLength(plane.normal, distance);

        //Translate the point to form a projection
        return point + translationVector;
    }

	public static void Shuffle<T>(this List<T> list)  
	{  
		System.Random rng = new System.Random(System.Guid.NewGuid().GetHashCode());  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];
			list[n] = value;  
		}
	}

    //Get the shortest distance between a point and a plane. The output is signed so it holds information
    //as to which side of the plane normal the point is.
    public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        return Vector3.Dot(planeNormal, (point - planePoint));
    }

    //create a vector of direction "vector" with length "size"
    public static Vector3 SetVectorLength(Vector3 vector, float size)
    {

        //normalize the vector
        Vector3 vectorNormalized = Vector3.Normalize(vector);

        //scale the vector
        return vectorNormalized *= size;
    }

	/// <summary>
	/// Returns a vector that contains (a.x * b.x, a.y * b.y, a.z * b.z)
	/// </summary>
	/// <returns>The product.</returns>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	public static Vector3 ComponentProduct(this Vector3 a, Vector3 b)
	{
		return new Vector3 (a.x * b.x, a.y * b.y, a.z * b.z);
	}

    public static void SetColorRecursively(this GameObject obj, Color color)
    {
        if (obj.renderer)
        {
            obj.renderer.material.color = color;
        }
        foreach (Transform t in obj.transform)
        {
            t.gameObject.SetColorRecursively(color);
        }
    }
}
