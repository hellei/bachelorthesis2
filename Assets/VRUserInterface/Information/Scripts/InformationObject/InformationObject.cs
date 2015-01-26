using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public enum TextStyle { H1, H2, P }

	public enum SelectionType { AutoSelect, Button }

	/// <summary>
	/// This script has no real functionality. It serves mainly as a tag, indicating that this object contains information.
	/// It can provide details on how to display the information as well, though.
	/// </summary>
	public class InformationObject : MonoBehaviour {

	    [System.Serializable]
	    public class InfoBox
	    {
	        public string text;
	        public TextStyle textStyle;
	    }

		string standardShader;

		/// <summary>
		/// You can highlight an object if you want to shift the focus towards it.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void Highlight(bool value)
		{
			if (value)
			{
				renderer.material.shader = Shader.Find ("Unlit/Diffuse-Shining");
			}
			else
			{
				renderer.material.shader = Shader.Find (standardShader);
			}
		}

	    /// <summary>
	    /// Only one object can be watched at a time
	    /// </summary>
	    public static InformationObject watchedObj;

	    /// <summary>
	    /// Only one object can be selected at a time
	    /// </summary>
	    public static InformationObject selectedObj;

	    public static void Deselect()
	    {
	        watchedObj = null;
	        selectedObj = null;
	    }

		//public Quaternion displayRotation;
		public GameObject displayPrefab;//The object that will be shown in large
	    public Vector3 displayPrefabRotation = new Vector3(180, 0, 0);
	    public float displayPrefabScale = 1.5f;

		/// <summary>
		/// At what position on the object should the close button be shown (Should be a child of the display prefab)
		/// </summary>
		public GameObject displayCloseButton;
		/// <summary>
		/// Defines if a close button is shown directly on the prefab
		/// </summary>
		public bool showCloseButtonOnDisplayPrefab;

		public InfoBox[] infoText;

		/// <summary>
		/// If this flag is set, the display prefab shader is set to unlit
		/// </summary>
		public bool ignoreLightConditionsOnDisplayPrefab = true;

	    public SelectionType selectionType;
	    public ButtonGenerator button;
	    protected Button selectionButton;
	    
	    protected virtual void Start()
	    {
	        if (!displayPrefab)
	        {
	            displayPrefab = gameObject;
	        }
	    }

		protected virtual void Awake()
		{
			if (renderer)
			{
				standardShader = renderer.material.shader.name;
			}
		}

		bool buttonActive = false;


		protected virtual void Update()
	    {
	        //Check the different selection types
	        
	        //Auto select always selects the object as soon as it is watched
	        if (selectionType == SelectionType.AutoSelect)
	        {
	            if (watchedObj == this)
	            {
	                selectedObj = this;
	            }
	        }
	        //Button shows a selection button as soon as it is watched
	        else if (selectionType == SelectionType.Button)
	        {
	            if (!selectionButton) CreateSelectionButton();
				//If the player looks at a button, the card state should not be hidden
				if (!ViewSelection.instance.WatchedObject || !ViewSelection.instance.WatchedObject.IsButton())
				{
					buttonActive = Selection.instance.WatchedObject == gameObject && selectedObj != this;
				}
				bool lastButtonActive = selectionButton.gameObject.activeSelf;
	            selectionButton.gameObject.SetActive(buttonActive && !selectedObj == this);
				//If the button has just been set active, disable all other buttons
				if (!lastButtonActive && buttonActive)
				{
					foreach (GameObject obj in GameObject.FindGameObjectsWithTag(Tags.buttonComponent))
					{
						//Disable all other buttons except the current one
						if (obj != selectionButton.gameObject && obj.GetComponent<Button>() && obj.GetComponent<Button>().creator)
						{
							obj.GetComponent<Button>().creator.DisableButton();
						}
					}
				}
	        }

	    }

		void LateUpdate()
		{
			if (recreateButtons)
			{
				RecreateButton();
			}
		}

		public void RecreateButton()
		{
			if (!selectionButton) return;
			Destroy(selectionButton.gameObject);
			selectionButton = null;
		}


		/// <summary>
		/// Set to true to recreate all buttons of the info objects.
		/// </summary>
		public static bool recreateButtons;

		public void DisableButton()
		{
			buttonActive = false;
			selectionButton.gameObject.SetActive (false);
		}

	    protected void OnButtonSelection()
	    {
			selectionButton.gameObject.SetActive (false);
	        selectedObj = this;
	    }

	    public Vector3 buttonOffset = new Vector3(0, 0, 0.05f);

		/// <summary>
		/// The local position of the button
		/// </summary>
		protected virtual void PositionInfoButton(GameObject obj)
		{
	        obj.transform.parent = transform;
	        obj.transform.localPosition = buttonOffset;
	        selectionButton = obj.GetComponent<Button>();
	        selectionButton.OnButtonPressed += OnButtonSelection;
	        selectionButton.text = "Info";
			//TODO: Put to magic information object
			//selectionButton.condition = Tablet.instance;
		}


	    protected virtual void CreateSelectionButton()
	    {
	        GameObject obj = button.Instantiate();
			obj.GetComponent<Button> ().creator = this;
	        PositionInfoButton(obj);
	    }

		/// <summary>
		/// This function is called when the table display creates the info box.
		/// </summary>
		/// <param name="display">Display.</param>
		public virtual void OnDisplayPrefabInstantiated(GameObject display)
		{
			//Change shader if set to unlit
			if (ignoreLightConditionsOnDisplayPrefab)
			{
				display.renderer.SetShaderToUnlit();
			}
			
			//Delete the information script
			if (display.GetComponent<InformationObject>()) Destroy(display.GetComponent<InformationObject>());

			//Rotate and scale the display correctly
			display.transform.Rotate(displayPrefabRotation);
			display.transform.localScale *= displayPrefabScale;
		}
	}
}