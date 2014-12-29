using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleInteractableObject : Interactable
{
    private Camera tableCamera;

    public void Start()
    {
        Tap gtap = new Tap(this.gameObject);
        gtap.OnTriggering += tap;
        gestures.Add(gtap);

        Move gmove = new Move(this.gameObject, 1);
        gmove.OnTriggering += move;
        gestures.Add(gmove);

        Release grelease = new Release(this.gameObject);
        grelease.OnTriggering += release;
        gestures.Add(grelease);

        Pickup gpickup = new Pickup(this.gameObject, true);
        gpickup.OnTriggering += pickup;
        gestures.Add(gpickup);

        Lift glift = new Lift(this.gameObject);
        glift.OnTriggering += lift;
        gestures.Add(glift);

        tableCamera = ServerTouchProcessing.instance.GetTableCamera();

    }

    

    public void tap(List<TouchInfo> touchlist)
    {
        transform.Rotate(new Vector3(20, 0, 0));        
    }

    public void move(List<TouchInfo> touchlist)
    {
        Vector3 pos = Vector3.zero;
        foreach (TouchInfo tinfo in touchlist)
            pos += (Vector3)(tinfo.currentPosition - tinfo.objectOffsets[this.gameObject]);
        pos /= touchlist.Count;
        Vector3 worldPosition = Tablet.instance.TabletToWorldSpace(new Vector2(pos.x / Screen.width, pos.y / Screen.height));
        transform.position = worldPosition;//new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        gameObject.renderer.material.color = Color.cyan;        
    }

    public void release(List<TouchInfo> touchlist)
    {
        gameObject.renderer.material.color = Color.red;

    }

    public void pickup(List<TouchInfo> tinfo)
    {
        transform.Rotate(new Vector3(0, 20, 0));
        gameObject.renderer.material.color = Color.blue;

        
    }

    public void lift(List<TouchInfo> tinfo)
    {
        transform.Rotate(new Vector3(0, 0, 20));
        gameObject.renderer.material.color = Color.yellow;

        
    }
}
