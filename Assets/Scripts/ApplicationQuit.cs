using UnityEngine;
using System.Collections;
using VRUserInterface;
public class ApplicationQuit : MenuCallback {
	#region implemented abstract members of MenuCallback
	public override void ReceiveMenuCallback (params string[] info)
	{
		StartCoroutine (DoQuitApplication ());
	}
	#endregion

	public float waitTimeUntilQuit = 4.0f;

	public GameObject environment;
	public GameObject quitPrefab;

	IEnumerator DoQuitApplication()
	{
		Instantiate (quitPrefab, VRCameraEnable.instance.GetCameraCenter (), Quaternion.identity);
		environment.SetActive (false);
		yield return new WaitForSeconds(waitTimeUntilQuit);
		Application.Quit ();
	}
}
