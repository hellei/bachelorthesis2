using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public static class GameObjectExtensions {
	    /// <summary>
	    /// Go over all meshes in the object and reposition them, so that the gameObject is the center of the surrounding axis aligned bounding box.
	    /// </summary>
	    /// <param name="obj"></param>
	    public static void CenterWeight(this GameObject obj)
	    {
			Vector3 center = obj.GetCenter ();
	        foreach (Transform t in obj.transform)
	        {
	            t.transform.position -= center;
	        }
	    }

		/// <summary>
		/// Tests if the object is or belongs to a button. This is important as sometimes a button belongs to an object
		/// and you do not want to deselect the object when you look at the button
		/// </summary>
		/// <returns><c>true</c> if is button the specified obj; otherwise, <c>false</c>.</returns>
		/// <param name="obj">Object.</param>
		public static bool IsButton(this GameObject obj)
		{
			if (obj.GetComponent<Button>() || obj.tag == Tags.buttonComponent) return true;
			return false;
		}

		
		/// <summary>
		/// Returns the center of the bounding box surrounding the object
		/// </summary>
		public static Vector3 GetCenter(this GameObject obj)
		{
			Vector3 min;
			Vector3 max;
			min = max = GameObjectExtensions.initializationVector;
			obj.GetBounds(ref min, ref max);
			Vector3 center = (max + min) * 0.5f;
			return center;
		}

		/// <summary>
		/// Adds a looking glass effect component to an object and copies the values defined in the settings
		/// </summary>
		public static void AddLookingGlassEffect(this GameObject obj, LookingGlassSettings lgs)
		{
			if (!lgs.useLookingGlassEffect) return;
			LookingGlassEffect lge = obj.AddComponent<LookingGlassEffect>();
			lge.sizeCurve = lgs.lookingGlassCurve;
			lge.magnifyingFactor = lgs.magnifyingFactor;
			lge.moveForwardValue = lgs.moveForwardValue;
			lge.popupDuration = lgs.popupDuration;
			lge.minimizeSpeedMultiplication = lgs.minimizeSpeedMultiplication;
		}

		public static void SetTransparency(this GameObject obj, float factor)
		{
			Color c = obj.renderer.material.color;
			c.a = factor;
			obj.renderer.material.color = c;
		}

		/// <summary>
		/// Tests all bounding boxes to get the size of the object. If the size is larger or smaller than the desired height, rescale the object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="maxWidth"></param>
		public static void SetWidth(this GameObject obj, float width)
		{
			Vector3 min;
			Vector3 max; 
			min = max = GameObjectExtensions.initializationVector;
			obj.GetBounds(ref min, ref max);
			float currentWidth = max.x - min.x;
			if (currentWidth == 0)
			{
				Debug.LogWarning("Could not change width. Width is set to zero.");
				return;
			}
			if (currentWidth != width)
			{
				float scaleFactor = width / currentWidth;
				obj.transform.localScale = obj.transform.localScale * scaleFactor;
			}
		}

		/// <summary>
		/// Tests all bounding boxes to get the size of the object. If the size is larger or smaller than the desired height, rescale the object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="maxHeight"></param>
		public static void SetHeight(this GameObject obj, float height)
		{
			Vector3 min;
			Vector3 max; 
			min = max = GameObjectExtensions.initializationVector;
			obj.GetBounds(ref min, ref max);
			float currentHeight = max.y - min.y;
			if (currentHeight != height)
			{
				float scaleFactor = height / currentHeight;
				obj.transform.localScale = obj.transform.localScale * scaleFactor;
			}
		}

	    /// <summary>
	    /// Tests all bounding boxes to get the size of the object. If the size is larger than the maximum height, rescale the object
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <param name="maxHeight"></param>
	    public static void SetMaxHeight(this GameObject obj, float maxHeight)
	    {
	        Vector3 min;
	        Vector3 max; 
	        min = max = GameObjectExtensions.initializationVector;
	        obj.GetBounds(ref min, ref max);
	        float height = max.y - min.y;
	        if (height > maxHeight)
	        {
	            float scaleFactor = maxHeight / height;
	            obj.transform.localScale = obj.transform.localScale * scaleFactor;
	        }
	    }

	    /// <summary>
	    /// Tests all bounding boxes to get the size of the object. If the size is larger than the maximum width, rescale the object
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <param name="maxHeight"></param>
	    public static void SetMaxWidth(this GameObject obj, float maxWidth)
	    {
	        Vector3 min;
	        Vector3 max;
	        min = max = GameObjectExtensions.initializationVector;
	        obj.GetBounds(ref min, ref max);
	        float width = max.x - min.x;
	        if (width > maxWidth)
	        {
	            float scaleFactor = maxWidth / width;
	            obj.transform.localScale = obj.transform.localScale * scaleFactor;
	        }
	    }

	    public static Vector3 initializationVector = new Vector3(-1000, -900, -800);

	    /// <summary>
	    /// Tests all axis-aligned bounding boxes to get a bounding box surrounding the whole object.
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <param name="parentObj">This should always be set to null.</param>
	    /// <returns></returns>
	    public static void GetBounds(this GameObject obj, ref Vector3 min, ref Vector3 max, GameObject parentObj = null)
	    {
	        bool initialCall = false;
	        if (parentObj == null)
	        {
	            parentObj = obj;
	            initialCall = true;
	        }
			if (!obj.activeSelf) return;
			OverrideBounds ob = obj.GetComponent<OverrideBounds> ();
			if (ob)
			{
				min = ob.cube.renderer.bounds.min - parentObj.transform.position;
				max = ob.cube.renderer.bounds.max - parentObj.transform.position;
				if (ob.boundContainsAllChildObjects) return;
			}
	        else if (obj.renderer)
	        {
	            min = obj.renderer.bounds.min - parentObj.transform.position;
	            max = obj.renderer.bounds.max - parentObj.transform.position;
	        }
	        foreach (Transform t in obj.transform)
	        {
	            Vector3 childMin = min, childMax = max;
	            t.gameObject.GetBounds(ref childMin, ref childMax, parentObj);
	            if (min == initializationVector && childMin != initializationVector)
	            {
	                min = childMin;
	                max = childMax;
	            }
	            else
	            {
	                min = Vector3.Min(childMin, min);
	                max = Vector3.Max(childMax, max);
	            }
	        }
	        //If there are no bounds at all, return 0,0,0
	        if (initialCall && min == initializationVector)
	        {
	            min = max = Vector3.zero;
	        }
	    }

		/// <summary>
		/// Somehow, Unity always rotates my beloved objects away instead of looking towards.
		/// This function rotates them 180 degrees automatically after looking "away" from the player
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="position">Position.</param>
		public static void LookAtAndRotate180Degrees(this Transform t, Vector3 position)
		{
			t.LookAt (position);
			t.Rotate (new Vector3 (0, 180, 0));
		}

	    /// <summary>
	    /// Returns the size of an axis-aligned bounding boxes surrounding the whole object. This function enhances renderer.bounds.size
	    /// because it also takes children into account.
	    /// </summary>
	    /// <returns></returns>
	    public static Vector3 GetBoundsSize(this GameObject obj)
	    {
	        Vector3 min;
	        Vector3 max;
	        min = max = GameObjectExtensions.initializationVector;
	        obj.GetBounds(ref min, ref max);
	        return max - min;
	    }

		/// <summary>
		/// Sets the shader of the specified renderer to unlit.
		/// </summary>
		/// <param name="renderer">Renderer.</param>
		public static void SetShaderToUnlit(this Renderer renderer)
		{
			foreach (Material m in renderer.materials)
			{
				if (m.shader.name.Contains ("Diffuse"))
				{
					m.shader = Shader.Find ("Unlit/Diffuse");
				}
				else if (m.shader.name.Contains ("Transparent"))
				{
					m.shader = Shader.Find ("Unlit/Transparent");
				}
			}
		}
	}
}
