using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class VRCursor : MonoBehaviour {


	    GameObject cursor = null;

	    public float cursorDistance = 2f;

		public static VRCursor instance;
		



		void Start()
		{
			instance = this;
			if (!cursor) CreateCursor ();
		}

		/// <summary>
		/// Attaches an icon to the cursor.
		/// </summary>
		/// <param name="prefab">The icon prefab</param>
		public void AttachIcon(GameObject prefab)
		{
			GameObject attachment = (GameObject)Instantiate (prefab);
			attachment.transform.parent = cursor.transform;
			attachment.transform.localPosition = Vector3.zero;
			attachment.transform.localRotation = Quaternion.identity;
		}

	    Quaternion initialRotation;

	    public float cursorSize = 0.05f;
	    /// <summary>
	    /// Creates a cursor for the camera
	    /// </summary>
	    void CreateCursor()
	    {
			cursor = (GameObject)Instantiate (cursorPrefab);//GameObject.CreatePrimitive(PrimitiveType.Sphere);
	        initialRotation = cursorPrefab.transform.rotation;
	        cursor.name = "Cursor";
	        cursor.transform.localScale = new Vector3(cursorSize, cursorSize, cursorSize);
	        cursor.transform.parent = transform;
	        if (cursor.GetComponent<Collider>())
	        {
	            Destroy(cursor.GetComponent<Collider>());
	        }
	        UIController.Instance.reference = gameObject;
	    }

		
		// Update is called once per frame
		void Update () {
	        if (!cursor) CreateCursor();
	        PositionCursor();
	        CheckCollision();
		
	    }

	    /// <summary>
	    /// If the distance to the hit is above this threshold, hide the cursor.
	    /// This is useful e.g. to prevent the cursor from popping up on environment objects far away
	    /// </summary>
	    public float distanceThreshold = 3.0f;

	    float distanceToHit = 0;

	    /// <summary>
	    /// Positions the cursor in the center of the left and right camera
	    /// </summary>
	    void PositionCursor(){
	        RaycastHit hit;
	        if (Physics.Raycast(transform.position, transform.forward, out hit))
	        {
	            cursor.transform.position = hit.point;
	            cursor.transform.rotation = Quaternion.LookRotation(hit.normal);
	            cursor.transform.Translate(new Vector3(0, 0, 0.01f));
	            cursor.transform.rotation *= initialRotation;
	            distanceToHit = Vector3.Distance(transform.position, hit.point);

	            if (distanceToHit > distanceThreshold) distanceToHit = 0;
	            cursor.transform.localScale = distanceToHit * new Vector3(cursorSize, cursorSize, cursorSize);
	        }
	        else
	        {
	            //Bury the cursor below the ground if there is no raycast hit
	            cursor.transform.localPosition = new Vector3(0,-100,0);
	        }
	    }

	    void OnGUI()
	    {
	        //GUI.Label(new Rect(300, 200, 200, 50), "d: " + distanceToHit);
	    }

		public GameObject cursorPrefab;

		void ShowCursor(bool value)
		{
			cursor.renderer.enabled = value;
		}

		float selectionTimer = 0;

		SelectableObject lastSelectedSo = null;

	    void CheckCollision()
	    {
			ShowCursor (true);
	        RaycastHit hitInfo;
	        MenuIcon.selectedItem = null;
	        if (Physics.Raycast(transform.position, cursor.transform.position - transform.position, out hitInfo)){
				if (hitInfo.collider.gameObject.tag == "Screen"){
					ShowCursor(false);
				}
	            //Test if the collided object is selectable
	            SelectableObject so = hitInfo.collider.gameObject.GetComponent<SelectableObject>();
				if (so == null && lastSelectedSo != null && lastSelectedSo.progressBar)
				{
					lastSelectedSo.progressBar.SetActive(false);
				}

	            if (so && !UIController.Instance.menu)
	            {
					selectionTimer += Time.deltaTime;
					if (so.selectionTime != 0)
					{
						so.SetProgress(selectionTimer / so.selectionTime);
					}
					if (selectionTimer >= so.selectionTime)
					{
						so.SelectionEvent();
						if (so.selectionTime != 0 && so.progressBar) so.progressBar.SetActive(false);
	                	//Inform the ui controller that a menu has been selected
	                	UIController.Instance.SetMenu(so.menuPrefab.gameObject);
					}
	            }
	            //If it is not a selectable object, test if it is a menu icon
	            else
	            {
					selectionTimer = 0;
	                MenuIcon mi = hitInfo.collider.gameObject.GetComponent<MenuIcon>();
	                if (mi)
	                {
	                    MenuIcon.selectedItem = mi;
	                }
	            }
				lastSelectedSo = so;
	        }
			else if (lastSelectedSo)
			{
				selectionTimer = 0;
				if (lastSelectedSo.progressBar) lastSelectedSo.progressBar.SetActive(false);
			}
	    }
	}
}