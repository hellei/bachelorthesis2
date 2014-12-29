using UnityEngine;
using System.Collections;

public class OnTriggerEnter2DEventArgs
{
    public Collider2D other;
    public OnTriggerEnter2DEventArgs(Collider2D other){
        this.other = other;
    }
}

/// <summary>
/// In Unity it is not possible to get an oncollision callback on a script on another object
/// This class handles with that. Put it on an object. Everytime the oncollision function is called,
/// it will trigger an event that can be accessed by other script
/// </summary>
public class OnCollisionRemoteCallback : MonoBehaviour {

    public delegate void OnTriggerEnter2DInfo(object sender, OnTriggerEnter2DEventArgs e);

    public event OnTriggerEnter2DInfo OnTriggerEnter2DCallback;

	void OnTriggerEnter2D(Collider2D other)
    {
        var handler = OnTriggerEnter2DCallback;
        if (handler != null)
        {
            handler(this, new OnTriggerEnter2DEventArgs(other));
        }
    }
}
