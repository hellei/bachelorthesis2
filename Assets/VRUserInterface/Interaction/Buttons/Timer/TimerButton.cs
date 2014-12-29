using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Creates a button that is selected if you look at it for a specified time. A fill-up effect shows the progress.
	/// </summary>
	public class TimerButton : Button {



		Vector3 standardPosition;


	    void Start()
	    {
			SetStandardAndMovedForwardPosition ();
	    }

		


		void SetStandardAndMovedForwardPosition()
		{
			standardPosition = transform.localPosition;
		}

		void OnEnable()
		{
			SetStandardAndMovedForwardPosition ();
			Update ();
		}

	    float timeSelected = 0;

	    public float selectTimer = 2;

		bool isSelected;

		public bool useSound;
		public AudioClip hover, select;

		/// <summary>
		/// If the button is not selected, it should not immediately reset the progress bar to 0.
		/// If you set a value different to 0, the progress bar is reduced with the speed given in this value.
		/// </summary>
		public float fadeOutMultiplierIfNotSelected = 2;

	    public new void Update()
	    {
			GameObject sel = Selection.instance.GetWatchedSelectableObject ();
			if (sel == gameObject && objectSelectable)
	        {
	            if (timeSelected >= selectTimer)
	            {
					timeSelected = 0;
					CallEvent();
	            }
				//If the object hasnt been selected in the last frame, play sound
				if (!isSelected) PlaySound (hover);
	            isSelected = true;
	            timeSelected += Time.deltaTime;
	            timeSelected = Mathf.Min(selectTimer, timeSelected);
	        }
	        else
	        {
				if (fadeOutMultiplierIfNotSelected == 0) timeSelected = 0;
	            timeSelected -= Time.deltaTime * fadeOutMultiplierIfNotSelected;
				timeSelected = Mathf.Max(timeSelected, 0);
	            isSelected = false;
	        }
			//ShowTag (sel == this);
			base.Update ();
			ChangeIconShader ();
	        
	    }
		


		new GameObject tag;

		/*void ShowTag(bool value)
		{
			if (value)
			{
				if (!tag)
				{
					tag = (GameObject)Instantiate(Resources.Load("TagPrefab") as GameObject, Vector3.zero, Quaternion.identity);
				
					Vector3 tagPosition = MenuIcon.selectedItem.transform.position + new Vector3(0, 0.1f, 0);
					tag.GetComponent<TextMesh>().text = text;
					tag.transform.parent = transform;
					tag.transform.localPosition = new Vector3(0,0.7f,-0.01f);//the offset of -0.01 prevents z-fighting
					tag.transform.localRotation = Quaternion.identity;
					
					GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
					background.renderer.material = Resources.Load("EmptyButtonMat") as Material;
					background.renderer.material.SetFloat("_Transparency", 0.8f);
					background.renderer.material.SetFloat("_FilledTransparency", 0);
					background.renderer.material.color = Color.black;
					background.transform.parent = transform;
					background.transform.localPosition = new Vector3(0,0.7f,-0.01f);
					background.transform.localScale = new Vector3(1, 0.2f, 0.2f);
					background.transform.localRotation = Quaternion.identity;
				}

				//tag.transform.Rotate(new Vector3(0, 180, 0));
			}
			else
			{
				if (tag) Destroy(tag);
			}
		}*/

		void ChangeIconShader()
		{
			float factor = 0;

			float percentage = 0;
			if (selectTimer != 0) percentage = timeSelected / selectTimer;
			if (timeSelected > 0) {
					factor = MenuIcon.selectedFactor;
			}

			//Set shader values
			renderer.material.SetFloat ("_Transparency", MenuIcon.standardTransparency + factor);
			float filledTransparency = factor + MenuIcon.filledTransparency;
			renderer.material.SetFloat("_FilledTransparency", filledTransparency);
			renderer.material.SetFloat ("_PercentageFilled", percentage);

			//Set icon
			transform.localPosition = standardPosition;
			transform.Translate (new Vector3 (0, 0, -MenuIcon.moveForwardLength * transform.localScale.x * percentage));

			renderer.material.color = (objectSelectable ? Color.white : inactiveColor);
		}

		public Color inactiveColor;



		void PlaySound(AudioClip sound)
		{
			//Play sound
			if (useSound && sound)
			{
				AudioSource.PlayClipAtPoint(sound, transform.position);
			}
		}

		public override void SetIcon(Sprite icon)
		{
			renderer.material.mainTexture = icon.texture;
		}
	}
}