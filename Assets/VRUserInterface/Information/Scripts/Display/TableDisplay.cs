using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRUserInterface
{
	/// <summary>
	/// Shows the information on a specified position
	/// </summary>
	public class TableDisplay : InformationDisplay {

		public Vector3 displayPosition = new Vector3(0,1,0);
		public TextGenerator textGenerator;
		public ButtonGenerator closeButton, scrollUpButton, scrollDownButton;
	    public float textItemMargin = 0.02f;
	    public float horizontalMargin = 0.04f;

	    //The maximum height and width of the displayed info
	    public float maxHeight = 1.0f;
	    public float maxWidth = 1.0f;

	    /// <summary>
	    /// The currently selected object.
	    /// </summary>
		protected GameObject activeObject {
			get;
			private set;
		}

	    
	    /// <summary>
	    /// The main function. Creates the info depending on the currently selected object.
	    /// </summary>
	    /// <param name="obj"></param>
		public override void SetActiveObject(InformationObject obj){
			if (activeObject) DiscardActiveObject();
			//Create a new instantiation of the active object and show it in larger
			InformationObject infoObjectScript = obj.GetComponent<InformationObject>();
			activeObject = CreateObjectFromInfo(infoObjectScript);
		}
		public override void DiscardActiveObject(){
			Destroy (activeObject);
		}

	    /// <summary>
	    /// Defines if the selected item should be shown in large
	    /// </summary>
	    public bool showDisplayPrefab = true;

	    /// <summary>
	    /// Defines if the info text should be shown
	    /// </summary>
	    public bool showInfoText = true;

		/// <summary>
		/// Returns the reference to the object whose info is shown
		/// </summary>
		/// <value>The reference.</value>
		protected GameObject Reference {
					get;
			        set;
		}

		/// <summary>
		/// The display prefab that is shown in large can have a looking glass effect as well
		/// </summary>
		public LookingGlassSettings objectLookingGlassSettings;

		/// <summary>
		/// If set to true the text is positioned at the center
		/// </summary>
		public bool centerAtText = false;


	    /// <summary>
	    /// Retrieves the information from the information object and creates the info display based on the given values
	    /// </summary>
	    /// <param name="infoObjectScript"></param>
	    /// <returns></returns>
		public virtual GameObject CreateObjectFromInfo(InformationObject infoObjectScript){
			Reference = infoObjectScript.gameObject;
	        GameObject infoObject = null;

			//Create an empty container for the information
			GameObject container = new GameObject("InformationView");
	        container.transform.parent = EnvironmentPositioner.instance.table.transform;
			container.transform.localPosition = displayPosition;

			Vector3 min;
			Vector3 max;

	        //Create a list of UI elements that have to be shown.
			List<GameObject> uiElements = new List<GameObject>();
			
	        //If there is an info object to be shown, show it.
			if (infoObjectScript.displayPrefab && showDisplayPrefab){
				infoObject = (GameObject)Instantiate(infoObjectScript.displayPrefab, Vector3.zero, Quaternion.identity);
				infoObject.transform.parent = container.transform;
	            infoObject.CenterWeight();
	            infoObject.SetMaxHeight(maxHeight);
	            infoObject.SetMaxWidth(maxWidth);
				infoObject.AddLookingGlassEffect(objectLookingGlassSettings);

				//Hook to the information object to allow last-minute changes
				infoObjectScript.OnDisplayPrefabInstantiated(infoObject);

				uiElements.Add (infoObject);
			}
			//If there is a text to display, show it. The way it is shown depends on the text generator used
			//If no text generator is defined, do not show the text
			if (infoObjectScript.infoText != null && textGenerator != null && showInfoText){
				textContainer = new GameObject("textContainer");
	            textContainer.transform.parent = container.transform;
	            //Create all text items
	            CreateTextItems(textContainer, infoObjectScript);
				ScaleAndPositionTextContainer(maxHeight, maxWidth);
	            //Add the text item container to the list of objects shown
	            uiElements.Add(textContainer);
			}

			//Go over all objects and position them
	        float offset = 0;
			for (int i = 0; i < uiElements.Count; i++){
	            min = max = GameObjectExtensions.initializationVector;
	            uiElements[i].GetBounds(ref min, ref max);
	            offset -= min.x;
				uiElements[i].transform.localPosition = new Vector3(offset, 0, 0);
	            offset += (max.x + horizontalMargin);
			}

			//Center the objects
			container.CenterWeight();

			if (centerAtText)
			{
				Vector3 distanceToCenter = uiElements[uiElements.Count-1].transform.localPosition;
				foreach (Transform t in container.transform)
				{
					t.position -= distanceToCenter;
				}
			}

			container.SetMaxHeight(maxHeight);
			container.SetMaxWidth(maxWidth);

			//Add all buttons

			//Get container bounds
			min = max = GameObjectExtensions.initializationVector;
			container.GetBounds(ref min, ref max);
			float containerHeight = max.y;
			float containerWidth = max.x;


	        //Add back button
	        if (closeButton && showCloseButton)
	        {
				GameObject ba = closeButton.Instantiate();
	            ba.transform.localScale *= buttonScale;

				//Get button height
				min = max = GameObjectExtensions.initializationVector;
				ba.GetBounds(ref min, ref max);
				float buttonHeight = -min.y;

				SetButtonParentAndPosition(ba, container, new Vector3(containerWidth, containerHeight + buttonHeight), CloseButtonPressed);
	        }

			//Add scroll button
			if (scrollUpButton && scrollDownButton && useScrollbar)
			{	
				GameObject scrollUp = scrollUpButton.Instantiate();

				//Add the condition whether it is possible to scroll up or not
				scrollUp.GetComponent<Button>().condition = (IButtonCondition)scrollBar;
				scrollUp.GetComponent<Button>().conditionInfo = new string[]{"Up"};
				scrollUp.transform.localScale *= buttonScale;

				//Get button width and height
				min = max = GameObjectExtensions.initializationVector;
				scrollUp.GetBounds(ref min, ref max);
				float buttonHeight = -min.y;
				float buttonWidth = -min.x;

				GameObject scrollDown = scrollDownButton.Instantiate();
				//Add the condition whether it is possible to scroll up or not
				scrollDown.GetComponent<Button>().condition = (IButtonCondition)scrollBar;
				scrollDown.GetComponent<Button>().conditionInfo = new string[]{"Down"};

				scrollDown.transform.localScale *= buttonScale;

				SetButtonParentAndPosition(scrollUp, container, new Vector3(containerWidth - buttonWidth * 2 - 0.05f, containerHeight + buttonHeight), ScrollUp);		
				SetButtonParentAndPosition(scrollDown, container, new Vector3(containerWidth - buttonWidth * 2 - 0.05f, -containerHeight - buttonHeight), ScrollDown);
			}

			//Add a back button on the display prefab if wanted
			if (infoObjectScript.showCloseButtonOnDisplayPrefab)
			{
				GameObject dba = closeButton.Instantiate();
				SetButtonParentAndPosition(dba, infoObject, infoObjectScript.displayCloseButton.transform.localPosition, CloseButtonPressed);
				dba.transform.Rotate(new Vector3(180,0,0));
				dba.transform.localScale = dba.transform.localScale.ComponentProduct(infoObjectScript.displayCloseButton.transform.localScale);
			}
			
			/*container.CenterWeight();
			container.SetMaxHeight(maxHeight);
			container.SetMaxWidth(maxWidth);*/

	        //Create a connection between the info object and the displayed object
	        if (connectionPrefab && infoObject)
	        {
	            GameObject connection = CreateConnection(infoObject, infoObjectScript.gameObject);
	            connection.transform.parent = container.transform;//This object has to be parented at last because it shall not contribute to center weight etc.
	        }

			if (closeWhenLookingAway)
			{
				LookAwayCallback lac = container.AddComponent<LookAwayCallback>();
				lac.callback = CloseButtonPressed;
				lac.lookAwayThreshold = lookAwayThreshold;
			}
			return container;
		}

		/// <summary>
		/// If this flag is set, a back button is shown. Otherwise, you still have the possibility to close the display when you look away.
		/// </summary>
		public bool showCloseButton = true;

		/// <summary>
		/// If this flag is set, the info display closes as soon as you look away
		/// </summary>
		public bool closeWhenLookingAway = true;
		/// <summary>
		/// How far do you have to look away from the container center to close the view? (x = horizontal angle, y = vertical angle)
		/// </summary>
		public Vector2 lookAwayThreshold = new Vector2(40,30);

		/// <summary>
		/// The game object that contains all the labels with text in it.
		/// </summary>
		GameObject textContainer;

		/// <summary>
		/// The text container should fit the size of the table display. It should show the text labels centered and
		/// in the correct size.
		/// </summary>
		void ScaleAndPositionTextContainer(float h, float maxW)
		{
			textContainer.CenterWeight();
			Vector3 min, max;
			min = max = GameObjectExtensions.initializationVector;
			textContainer.GetBounds(ref min, ref max);
			textContainer.SetHeight(h);
			textContainer.SetMaxWidth(maxW);
		}

		void ScrollUp()
		{
			Scroll (-1);
		}

		void ScrollDown()
		{
			Scroll (1);
		}

		Vector3 initialTextBoxSize = Vector3.zero;

		void Scroll(int value)
		{
			if (initialTextBoxSize == Vector3.zero)
			{
				initialTextBoxSize = textContainer.GetBoundsSize ();
			}
			scrollBar.Scroll (value);
			ScaleAndPositionTextContainer (initialTextBoxSize.y, initialTextBoxSize.x);
		}

		/// <summary>
		/// Sets a button to a specific local position and attaches it to a parent object
		/// </summary>
		void SetButtonParentAndPosition(GameObject obj, GameObject parent, Vector3 pos, Button.PressEvent callback)
		{
			obj.transform.parent = parent.transform;
			
			//Position button
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localPosition = pos;
			
			Button b = obj.GetComponent<Button>();
			if(b)
			{
				b.LookAtPlayer = false; //Prevent the button from looking at the player
				b.OnButtonPressed += callback;
			}
		}

	    /// <summary>
	    /// Multiplies the back button scale. Useful if you want the button scale to be dependent of an external factor, 
	    /// </summary>
	    protected float buttonScale = 1;

	    public Connection connectionPrefab;

	    GameObject CreateConnection(GameObject a, GameObject b)
	    {
	        GameObject connection = (GameObject)Instantiate(connectionPrefab.gameObject, Vector3.zero, Quaternion.identity);
	        connection.GetComponent<Connection>().SetObjects(a, b);
	        return connection;
	    }

		/// <summary>
		/// The maximum number of info box that can be shown at a time. If there are more info boxes,
		/// a scrollbar is added.
		/// </summary>
		public int numberOfInfoBoxes = 3;

		bool useScrollbar;

	    void CreateTextItems(GameObject parent, InformationObject infoObjectScript)
	    {
	        float textContainerHeight = 0;
	        //Create all text items
			int i = 0;
			useScrollbar = false;
	        foreach (InformationObject.InfoBox info in infoObjectScript.infoText)
	        {
				//Create the actual object
	            GameObject textObject = textGenerator.CreateTextObject(info);
	            textObject.SetMaxWidth(maxWidth);
	            textObject.transform.parent = parent.transform;

				//Get the box height and prepare the offset for the next box
	            float objectHeight = textObject.GetBoundsSize().y;
	            textObject.transform.localPosition = new Vector3(0, -objectHeight / 2.0f - textContainerHeight, 0);
	            textContainerHeight += objectHeight + textItemMargin;

				//If there are too many info boxes, hide the later ones and use a scrollbar to navigate through
				if (i >= numberOfInfoBoxes)
				{
					textObject.SetActive(false);
					useScrollbar = true;
				}
				i++;
	        }
			if (useScrollbar)
			{
				AddScrollbar(parent);
			}
	    }

		Scrollbar scrollBar = null;

		/// <summary>
		/// Adds the scroll functionality to the specified container
		/// </summary>
		/// <param name="obj">Object.</param>
		void AddScrollbar(GameObject obj)
		{
			scrollBar = obj.AddComponent<StandardScrollbar>();

		}

	    void CloseButtonPressed()
	    {
	        InformationController.instance.ViewClosed();
	        //Deselect the information objects
	        InformationObject.Deselect();
	    }
	}
}
