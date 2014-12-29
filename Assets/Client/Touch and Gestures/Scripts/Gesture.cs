using UnityEngine;
using System.Collections.Generic;

public abstract class Gesture
{
    public abstract void check(List<TouchInfo> tinfo);
    public event System.Action<List<TouchInfo>> OnTriggering;
    protected GameObject MyGameObject;
    protected bool onlyOncePerTouch = false;

    /// <summary>
    /// Throws the event "OnTriggering" if possible.
    /// </summary>
    protected void RaiseTrigger(List<TouchInfo> touchlist)
    {
        if (onlyOncePerTouch && CheckAlreadyTriggered(touchlist)) return;

        foreach (TouchInfo ti in touchlist)
            if (!ti.touchedObjects[MyGameObject].Contains(this))
                ti.touchedObjects[MyGameObject].Add(this);

        if (OnTriggering != null) OnTriggering(touchlist);
    }

    /// <summary>
    /// Checks if the gesture was already triggered by ALL touches in the touchlist.
    /// </summary>
    protected bool CheckAlreadyTriggered(List<TouchInfo> touchlist)
    {
        foreach (TouchInfo ti in touchlist)
        {
            if (!ti.touchedObjects[MyGameObject].Contains(this))
                return false;
        }
        return true;
    }
}

public class Tap : Gesture
{
    private float maxTapOffset = 0.03f * Config.instance.tabletSize.z; //maximum distance a touch can move with still being detected as "tap"

    public Tap(GameObject obj, bool OnlyOncePerTouch = false) { MyGameObject = obj; onlyOncePerTouch = OnlyOncePerTouch; }

    public override void check(List<TouchInfo> touchlist)
    {
        if (touchlist.Count == 1 && touchlist[0].phase == TouchPhase.Ended)
        {
            if ((Time.time < touchlist[0].startTime + 0.3f) && (Mathf.Abs(Vector3.Distance(touchlist[0].startPosition, touchlist[0].currentPosition)) < maxTapOffset))
                RaiseTrigger(touchlist);
        }
    }
}

public class Move : Gesture
{
    private int exactNumberOfTouches;

    public Move(GameObject obj, int ExactNumberOfTouches = 0, bool OnlyOncePerTouch = false) { MyGameObject = obj; exactNumberOfTouches = ExactNumberOfTouches; onlyOncePerTouch = OnlyOncePerTouch; }

    public override void check(List<TouchInfo> touchlist)
    {
        if (exactNumberOfTouches > 0 && touchlist.Count != exactNumberOfTouches)
            return;
        foreach (TouchInfo ti in touchlist)
        {
            if (ti.phase != TouchPhase.Moved)
                return;
        }
        RaiseTrigger(touchlist);
    }
}

public class Release : Gesture
{
    public Release(GameObject obj, bool OnlyOncePerTouch = false) { MyGameObject = obj; onlyOncePerTouch = OnlyOncePerTouch; }

    public override void check(List<TouchInfo> touchlist)
    {
        foreach (TouchInfo ti in touchlist)
        {
            if (ti.phase != TouchPhase.Ended)
                return;
        }
        RaiseTrigger(touchlist);
    }
}

public class Pickup : Gesture
{
    public Pickup(GameObject obj, bool OnlyOncePerTouch = false) { MyGameObject = obj; onlyOncePerTouch = OnlyOncePerTouch; }

    public override void check(List<TouchInfo> touchlist)
    {
        if (touchlist.Count >= 2)
            foreach (TouchInfo ti in touchlist)
                if (ti.phase == TouchPhase.Moved)
                    foreach (TouchInfo ti2 in touchlist)
                        if (ti2 != ti && ti2.phase == TouchPhase.Moved)
                            if (ti.startObjects.Contains(MyGameObject) ^ ti2.startObjects.Contains(MyGameObject))
                                RaiseTrigger(touchlist);
    }
}

public class Lift : Gesture
{
    public Lift(GameObject obj, bool OnlyOncePerTouch = false) { MyGameObject = obj; onlyOncePerTouch = OnlyOncePerTouch; }

    public override void check(List<TouchInfo> touchlist)
    {
        if (touchlist.Count >= 2)
            foreach (TouchInfo ti in touchlist)
                if (ti.phase == TouchPhase.Moved)
                    foreach (TouchInfo ti2 in touchlist)
                        if (ti2 != ti && ti2.phase == TouchPhase.Moved)
                            if (!ti.startObjects.Contains(MyGameObject) && !ti2.startObjects.Contains(MyGameObject))
                            {
                                Vector2 pos = new Vector2(MyGameObject.transform.position.x, MyGameObject.transform.position.y);
                                float angle = Vector2.Angle((Vector2)Tablet.instance.TabletToWorldSpace(ti.currentPosition) - pos, (Vector2)Tablet.instance.TabletToWorldSpace(ti2.currentPosition) - pos);
                                if (angle > 120)
                                    RaiseTrigger(touchlist);
                            }
    }
}