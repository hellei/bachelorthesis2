using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ConfigLoader : MonoBehaviour {

    Config config;

	// Use this for initialization
	void Start () {
        LoadOrCreateConfigFile();
		LoadSavedDeck ();
	}

    void LoadOrCreateConfigFile()
    {
        string path = Application.dataPath + "/../config.xml";
        if (File.Exists(path))
        {
            config = Config.Load(path);
            Debug.Log("Load config");
        }
        else
        {
            config = new Config();
            config.Save(path);
            Debug.Log("Config not existing. Creating new config file.");
        }
    }

	public static SavedDeck savedDeck;

	void LoadSavedDeck()
	{
		if (!Config.instance.loadDeck) return;
		/*SavedDeck sd = new SavedDeck ();
		sd.cards = new List<string> ();
		sd.cards.Add ("Cards!");
		sd.Save (path + "main.xml");*/
		string path = Application.dataPath + "/../Decks/";

		if (Directory.Exists(path))
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			Debug.Log("Found deck directory. Loading all saved decks.");
			foreach(FileInfo file in dir.GetFiles())
			{
				Debug.Log("Deck: "+file.Name);
				//Load first deck
				savedDeck = SavedDeck.Load(path+file.Name);
				break;
			}
		}
	}
}
