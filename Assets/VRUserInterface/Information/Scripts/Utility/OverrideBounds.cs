using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Unity sometimes has problems with bounds (e.g. when using sprite renderers).
	/// If you attach this script to an object, you can define custom bounds.
	/// The size is relative to a local scale of 1,1,1 of the object.
	/// </summary>
	public class OverrideBounds : MonoBehaviour {
		public Vector3 min, max;//The minimum and maximum bounds position

		///If this flag is set to true, the script will not search for larger bounds in the child objects
		public bool boundContainsAllChildObjects = true;

		public GameObject cube {
			get;
			private set;
		}
		void Awake()
		{
			//On Start an invisible cube is created that has the correct bounds
			GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			Destroy (obj.GetComponent<Collider> ());
			obj.renderer.enabled = false;

			obj.transform.parent = transform;
			obj.transform.localScale = max - min;
			obj.transform.localPosition = (min + max) * 0.5f;
			cube = obj;
		}
	}
}