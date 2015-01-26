using UnityEngine;
using System.Collections;

public class Config : XMLSaveAndLoad<Config> {
    //Position and size are given in meters

    /// <summary>
    /// The position of the tablet in relation to the rift position tracker. (The referenced position of the tablet is: x = Left table(t) edge, y = surface of tablet, z = lower table(t) edge).
    /// </summary>
    public Vector3 relativeTabletPosition = Vector3.zero;
    public Vector3 tabletSize = Vector3.zero;

	/// <summary>
	/// The z distance from the left bottom tablet corner to the table border.
	/// </summary>
	public float zOffsetToTableBorder = 0.04f;

    /// <summary>
    /// The position of the seat of the chair, relative to the neck.
    /// </summary>
    //public Vector3 relativeSeatPosition = Vector3.zero;

    /// <summary>
    /// Determines how high above the ground the table surface is positioned.
    /// </summary>
    public float tableSurfaceHeight;

    public static Config instance;

    /// <summary>
    /// Defines how tall a person is when sitting
    /// </summary>
    public float personSitHeight = 1.05f;

    public Config()
    {
        if (instance != null)
        {
            Debug.LogError("There must be only one instance of the config file");
        }
        else
        {
            instance = this;
        }
    }

	public int numberOfInitialHandCards = 4;

	public string displayType = "";

	/// <summary>
	/// If this flag is set, the game runs at low graphics settings.
	/// </summary>
	public bool lowGraphics = false;

	/// <summary>
	/// You can create a deck folder to create a start deck.
	/// Set this flag to false, if you dont want it to be loaded on startup.
	/// </summary>
	public bool loadDeck = true;
}

