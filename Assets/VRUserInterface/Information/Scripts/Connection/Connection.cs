using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a connection between two gameobjects
/// </summary>
public abstract class Connection : MonoBehaviour {
    public abstract void SetObjects(GameObject a, GameObject b);
}
