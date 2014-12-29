using UnityEngine;
using System.Collections;
using VRUserInterface;

[RequireComponent(typeof(MoveModel))]
public class MoveModelCondition : MenuCondition {
	#region implemented abstract members of MenuCondition
	public override MenuState TestCondition (params string[] info)
	{
		if (mm.IsVisible) return MenuState.Show;
		else return MenuState.ShowSecondState;
	}
	#endregion

	MoveModel mm;

	void Start()
	{
		mm = GetComponent<MoveModel> ();
	}
}
