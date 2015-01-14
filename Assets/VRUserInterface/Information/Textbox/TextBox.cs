using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class TextBox : TextGenerator {

	    [System.Serializable]
	    public class TextAsset
	    {
	        public GameObject prefab;
	        public TextStyle textStyle;
	        public Color textColor = Color.white;
	        public Color backgroundColor = Color.black;
	    }

		public TextAsset[] textAssets;
		public GameObject background;

	    public float borderLeft = 0.05f, borderTop = 0.05f;

		const float zOffset = 0.005f;//z-offset used to prevent z fighting



	    /// <summary>
	    /// Creates a text object based on the string.
	    /// </summary>
	    /// <param name="text">The text string. '//' is used for new line (unfortunately the inspector does not support \n)</param>
	    /// <returns></returns>
		public override GameObject CreateTextObject(InformationObject.InfoBox infoBox){
	        //Replace the newline characters
	        infoBox.text = infoBox.text.Replace("//", "\n");
			//Create container
			GameObject obj = new GameObject("TextContainer");
			//Create text asset
	        GameObject ta = CreateTextAsset(obj, infoBox);
			//Create background
			OverrideTextInfo oti = ta.GetComponent<OverrideTextInfo> ();
			if (oti)
			{
				oti.SetBackgroundColor(GetTextAssetInstance(infoBox.textStyle).backgroundColor);
			}
			else if (ta.GetComponent<TextMesh>())
			{
	        	CreateTextBackground(obj, ta.GetComponent<TextMesh>(), GetTextAssetInstance(infoBox.textStyle).backgroundColor);
			}

			//Add the looking glass effect
			if (magnifyingGlassSettings.useMagnifyingGlassEffect)
			{
				obj.AddMagnifyingGlassEffect(magnifyingGlassSettings);
			}
			//Always add a collider to show the cursor
			Vector3 size = obj.GetBoundsSize();
			BoxCollider collider = obj.AddComponent<BoxCollider>();
			collider.size = size;
			return obj;
		}



		public MagnifyingGlassSettings magnifyingGlassSettings;


	    /// <summary>
	    /// Returns the prefab saved in the text assets.
	    /// </summary>
	    /// <param name="textStyle"></param>
	    /// <returns></returns>
	    GameObject GetTextAsset(TextStyle textStyle)
	    {
	        foreach (TextAsset ta in textAssets)
	        {
	            if (ta.textStyle == textStyle) return ta.prefab;
	        }
	        Debug.LogError("Text Generator " + name + " does not include text assets for every style");
	        return null;
	    }

	    TextAsset GetTextAssetInstance(TextStyle textStyle)
	    {
	        foreach (TextAsset ta in textAssets)
	        {
	            if (ta.textStyle == textStyle) return ta;
	        }
	        Debug.LogError("Text Generator " + name + " does not include text assets for every style");
	        return null;
	    }

	    /// <summary>
	    /// Creates a text mesh
	    /// </summary>
	    /// <param name="parent">The parent of the object</param>
	    /// <param name="text">The text displayed in the text mesh</param>
	    /// <returns></returns>
	    GameObject CreateTextAsset(GameObject parent, InformationObject.InfoBox infoBox)
	    {
	        GameObject ta = (GameObject)Instantiate(GetTextAsset(infoBox.textStyle));
	        ta.transform.parent = parent.transform;
	        ta.transform.localPosition = new Vector3(0, 0, -zOffset);
	        ta.transform.localRotation = Quaternion.identity;

			//Look for an info component that handles the text
			OverrideTextInfo oti = ta.GetComponent<OverrideTextInfo> ();
			if (oti)
			{
				oti.SetText(infoBox.text);
				oti.SetTextColor(GetTextAssetInstance(infoBox.textStyle).textColor);
			}
			else
			{
		        TextMesh tm = ta.GetComponent<TextMesh>();
		        if (!tm)
		        {
		            Debug.LogWarning("The text asset prefab should have an override text info or textMesh component");
		        }
		        else
		        {
		            tm.text = infoBox.text;
		        }
				if (ta.renderer)
				{
		        	ta.renderer.material.color = GetTextAssetInstance(infoBox.textStyle).textColor;
				}
			}
	        return ta;
	    }

	    /// <summary>
	    /// Creates the background for a text
	    /// </summary>
	    /// <param name="parent">The parent of the object</param>
	    /// <param name="tm">The textmesh infront of the background, needed for the correct dimensions of the background.</param>
	    /// <returns></returns>
	    GameObject CreateTextBackground(GameObject parent, TextMesh tm, Color color)
	    {
	        GameObject ba = (GameObject)Instantiate(background, Vector3.zero, Quaternion.identity);
	        ba.transform.parent = parent.transform;
	        ba.transform.position = Vector3.zero;
	        //Scale Background according to the dimensions of the text
	        float width = tm.renderer.bounds.max.x;
	        float height = tm.renderer.bounds.max.y;
	        ba.transform.localScale = new Vector2((width + borderLeft) * 2, (height + borderTop) * 2);
	        ba.SetColorRecursively(color);
	        return ba;
	    }
	}

	[System.Serializable]
	public class MagnifyingGlassSettings
	{
		/// <summary>
		/// The looking glass effect magnifies the object when you look at it
		/// </summary>
		public bool useMagnifyingGlassEffect = true;
		/// <summary>
		/// Determines in which curve the size increase takes place
		/// </summary>
		public AnimationCurve magnifyingGlassCurve;
		
		public float magnifyingFactor = 1.5f;
		/// <summary>
		/// Defines how far the object moves forward on select
		/// </summary>
		public float moveForwardValue = 0.1f;
		/// <summary>
		/// How long does the popup effect take
		/// </summary>
		public float popupDuration = 0.7f;
		/// <summary>
		/// The object should often decrease to its original size faster than it increases to the magnified size.
		/// This can be achieved by setting this multiplication factor.
		/// </summary>
		public float minimizeSpeedMultiplication = 2;
	}
}