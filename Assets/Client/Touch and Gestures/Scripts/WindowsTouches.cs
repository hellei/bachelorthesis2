using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;


#if UNITY_STANDALONE_WIN
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class tTouchData
{
    public int m_x;
    public int m_y;
    public int m_ID;
    public int m_Time;
    public int m_Phase;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif

public class WindowsTouches : MonoBehaviour
{

    public string NameOfApplicationWindow;

#if UNITY_STANDALONE_WIN
    
    [HideInInspector] public bool m_Initialised;
    [DllImport("TouchOverlay", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int Initialise(string Str);

    [DllImport("TouchOverlay")]
    public static extern int GetTouchPointCount();
    [DllImport("TouchOverlay")]
    public static extern void GetTouchPoint(int i, tTouchData n);

    [DllImport("TouchOverlay")]
    public static extern void UpdateTouches();

    // Use this for initialization
    void Start()
    {
        if (Initialise(NameOfApplicationWindow) < 0)
        {
            UnityEngine.Debug.Log("Couldn't Initialize WindowsTouches");
        }
    }

    public List<MyTouch> GetWindowsTouches()
    {
        List<MyTouch> ReturnList = new List<MyTouch>();
        int NumTouch = GetTouchPointCount();

        for (int p = 0; p < NumTouch; p++)
        {
            MyTouch tmp = new MyTouch();
            tTouchData TouchData = new tTouchData();
            GetTouchPoint(p, TouchData);
            tmp.fingerId = TouchData.m_ID;
            tmp.position = new Vector2(TouchData.m_x / 100, Screen.height - TouchData.m_y / 100);
            if ((TouchData.m_Phase & 0x0001) > 0) tmp.phase = TouchPhase.Moved;
            else if ((TouchData.m_Phase & 0x0002) > 0) tmp.phase = TouchPhase.Began;
            else if ((TouchData.m_Phase & 0x0004) > 0) tmp.phase = TouchPhase.Ended;
            ReturnList.Add(tmp);
        }
        if (NumTouch > 0) UpdateTouches();

        return ReturnList;
    }


    void OnGUI()
    {
#if DEVELOPMENT_BUILD
        string Str;
        int NumTouch = 0;
        if (!m_Initialised)
        {
            if (Initialise(NameOfApplicationWindow) < 0)
            {
                // ERROR STATE
            }
            m_Initialised = true;
        }
		
        NumTouch = GetTouchPointCount ();
        Str = "Number of Touch Points: " + NumTouch.ToString();
        GUI.Label (new Rect (10,10,150,40), Str);
        for (int p=0; p<NumTouch; p++)
        {
            tTouchData TouchData = new tTouchData();
            GetTouchPoint (p, TouchData);
            if (TouchData.m_Phase == 10 || TouchData.m_Phase == 26) UnityEngine.Debug.Log("touch begin");
            else if (TouchData.m_Phase == 4 || TouchData.m_Phase == 20) UnityEngine.Debug.Log("touch stop");
            GUI.Label (new Rect (10,10 + (p+1) * 40, 200, 40), 
                "ID:" + TouchData.m_ID + 
                "Time:" + TouchData.m_Time.ToString() + 
                "(" + TouchData.m_x.ToString() + "," + TouchData.m_y.ToString() + ")"
                + " Phase: " + TouchData.m_Phase);
			
        }
        if (NumTouch > 0) UpdateTouches();
#endif
    }
#endif
}
