using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// The main ui class. The information controller deals with the logic of receiving a callback from the selection and
	/// opening info displays.
	/// </summary>
	public class InformationController : MonoBehaviour {

		public static InformationController instance;

		void Start(){
			//Check the config for parameter overwrites
			if (Config.instance.displayType != "" && Config.instance.displayType != null)
			{
				//Search for the game object
				GameObject displayObj = GameObject.Find (Config.instance.displayType);
				if (!displayObj)
				{
					Debug.LogError("The display type defined in the config could not be found");
				}
				else
				{
					InformationDisplay displayScript = displayObj.GetComponent<InformationDisplay>();
					if (!displayScript)
					{
						Debug.LogError("The display type defined in the config has a game object without the correct script attached.");
					}
					else
					{
						infoDisplay = displayScript;
					}
				}
			}

			if (!instance){
				instance = this;
			}
			else {
				Debug.LogError("You may only have one instance of information controller in your scene.");
			}
		}
		/// <summary>
		/// The info display currently used to show information
		/// </summary>
		public InformationDisplay infoDisplay;

		/// <summary>
		/// T
		/// </summary>
		public Selection selection;

	    public bool debugKeys = true;
		/// <summary>
		/// Link to the info button prefab, only used for debug purposes.
		/// </summary>
		public ButtonGenerator infoButton;

		void Update(){
			GameObject selectedObject = null;
			if (InformationObject.selectedObj)
			{
				selectedObject = InformationObject.selectedObj.gameObject;
			}
			SetActiveObject(selectedObject);

			#region DEBUG
	        if (debugKeys)
	        {
	            //Enable / Disable Looking Glass Effect
	            if (Input.GetKey(KeyCode.Q))
	            {
	                infoDisplay.GetComponent<TableDisplay>().textGenerator.GetComponent<TextBox>().magnifyingGlassSettings.useMagnifyingGlassEffect = false;
	            }
	            if (Input.GetKey(KeyCode.W))
	            {
					infoDisplay.GetComponent<TableDisplay>().textGenerator.GetComponent<TextBox>().magnifyingGlassSettings.useMagnifyingGlassEffect = true;
	            }
	            if (Input.GetKeyDown(KeyCode.E))
	            {
	                infoI--;
					infoI = Mathf.Max(infoI, 0);
	                infoDisplay = allInfoDisplays[(infoI + allInfoDisplays.Length) % allInfoDisplays.Length];
	                Debug.Log(infoDisplay.name);
	            }
	            if (Input.GetKeyDown(KeyCode.R))
	            {
	                infoI++;
					infoI = Mathf.Min(infoI, allInfoDisplays.Length-1);
	                infoDisplay = allInfoDisplays[infoI % allInfoDisplays.Length];
	                Debug.Log(infoDisplay.name);
	            }

				if (Input.GetKeyDown(KeyCode.T))
				{
					infoButton.buttonType = (ButtonType)Mathf.Max(0, (int)infoButton.buttonType - 1);
					Debug.Log(infoButton.buttonType);
					InformationObject.recreateButtons = true;
				}
				if (Input.GetKeyDown(KeyCode.Z))
				{
					infoButton.buttonType = (ButtonType)Mathf.Min(System.Enum.GetNames(typeof(ButtonType)).Length - 1, (int)infoButton.buttonType + 1);
					Debug.Log(infoButton.buttonType);
					InformationObject.recreateButtons = true;
				}

				//Start evaluation for buttons on table
				if (Input.GetKeyDown(KeyCode.U))
				{
					handsEvaluation.SetActive(false);
					tableEvaluation.SetActive(true);
					displayEvaluation.SetActive(false);
					playground.SetActive(false);
				}

				//Start evaluation for buttons on hand
				if (Input.GetKeyDown(KeyCode.I))
				{
					handsEvaluation.SetActive(true);
					tableEvaluation.SetActive(false);
					displayEvaluation.SetActive(false);
					playground.SetActive(false);
				}

				//Start evaluation for displays
				if (Input.GetKeyDown(KeyCode.O))
				{
					handsEvaluation.SetActive(false);
					tableEvaluation.SetActive(false);
					displayEvaluation.SetActive(true);
					playground.SetActive(false);
				}

				//Start evaluation for displays
				if (Input.GetKeyDown(KeyCode.P))
				{
					handsEvaluation.SetActive(false);
					tableEvaluation.SetActive(false);
					displayEvaluation.SetActive(false);
					playground.SetActive(true);
				}
	        }
			#endregion
		}

		public GameObject tableEvaluation, handsEvaluation, displayEvaluation, playground;

	    int infoI = 3;

	    public InformationDisplay[] allInfoDisplays;

		public GameObject activeObject
		{
			get;
			private set;
		}

		//If the active object is not selected anymore, but no other information object is selected neither, should nothing be shown or should the last active object be shown?
		public bool showLastActiveObjectIfNothingSelected = true;

		/// <summary>
		/// The currently selected object
		/// </summary>
		/// <value>The active object.</value>
		void SetActiveObject(GameObject value) {
			//Test if the value differs from the current value
			if (value != activeObject){
				//If the active object is set null, discard the active object (except if the last active object should be kept
				if (value == null){
					if (showLastActiveObjectIfNothingSelected && !customViewClose){
						//Do nothing
					}
					else {
						//Trigger the callback
						if (informationObjectClosed != null) informationObjectClosed();
						//
						infoDisplay.DiscardActiveObject();
						activeObject = null;
                        customViewClose = false;
					}
				}
				//otherwise update the active object
				else if (value != activeObject && value.GetComponent<InformationObject>()){
					activeObject = value;
					if (newInformationObjectSelectedCallback != null)
					{
						newInformationObjectSelectedCallback();
					}
					infoDisplay.SetActiveObject(activeObject.GetComponent<InformationObject>());
				}
			}
		}

		public delegate void Callback();

		/// <summary>
		/// This event is always triggered when a new information object has been selected.
		/// </summary>
		public event Callback newInformationObjectSelectedCallback;

		public event Callback informationObjectClosed;

	    bool customViewClose = false;

	    /// <summary>
	    /// Sometimes the player can manually close a menu. This function deals with that.
	    /// </summary>
	    public void ViewClosed()
	    {
	        customViewClose = true;
	        SetActiveObject(null);
	    }
	}
}
