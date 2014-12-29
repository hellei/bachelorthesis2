using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour
{
    protected List<Gesture> gestures = new List<Gesture>();

    /// <summary>
    /// Checks triggering of all registered gestures of this object.
    /// </summary>
    /// <param name="touchlist">A list of touches that currently hit this object.</param>
    public virtual void handleTouches(List<TouchInfo> touchlist)
    {
        foreach (Gesture g in gestures)
        {
            g.check(touchlist);
        }
    }
}