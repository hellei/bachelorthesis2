using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// The  selection script deals with all issues related to selecting an object, e.g. defining the object that is currently looked at,
	/// interacted with or selected.
	/// </summary>
	public abstract class Selection : MonoBehaviour {
	    public static Selection instance;

	    protected void Start()
	    {
	        instance = this;
	    }

		/// <summary>
		/// Returns the currently selected object. Returns null if no object is selected.
		/// </summary>
		/// <returns>The selected object.</returns>
		public abstract GameObject GetSelectedObject();

	    /// <summary>
	    /// Returns the currently watched object. Returns null if no object is selected. Should be kept independent of tags which are handled in the information controller
	    /// An object that is watched is not necessarily selected. Only information objects can be selected.
	    /// </summary>
	    /// <returns>The selected object.</returns>
	    public abstract GameObject GetWatchedSelectableObject();


		/// <summary>
		/// Returns the currently watched object, independent of whether it is an information object or not.
		/// </summary>
		/// <value>The watched object.</value>
		public GameObject WatchedObject
		{
			get;
			private set;
		}

	    protected InformationObject currentlyWatchedInformationObject;//The current information object

	    /// <summary>
	    /// Informs the information object script that this object is watched
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <returns></returns>
	    protected void SetWatchedObject(GameObject obj)
	    {

			WatchedObject = obj;
			if (!obj)
			{
				return;
			}
	        InformationObject io = obj.GetComponent<InformationObject>();
	        if (io)
	        {
	            InformationObject.watchedObj = io;
	            currentlyWatchedInformationObject = io;
	        }
	    }
	}
}
