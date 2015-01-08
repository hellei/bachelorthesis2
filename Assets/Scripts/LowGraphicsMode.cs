using UnityEngine;
using System.Collections;

/// <summary>
/// If set in the config, the game runs at the minimum quality settings to ensure stable performance
/// </summary>
public class LowGraphicsMode : MonoBehaviour {
	public Terrain terrain;
	
	// Use this for initialization
	void Start () {
		if (Config.instance.lowGraphics)
		{
			SetLowGraphics();
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F))
						SetLowGraphics ();
	}

	void SetLowGraphics()
	{
		terrain.detailObjectDistance = 5;
		terrain.treeBillboardDistance = 5;
		QualitySettings.antiAliasing = 0;
	}
}
