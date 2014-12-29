using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class StandardScrollbar : Scrollbar, IButtonCondition {	
		#region implemented abstract members of Scrollbar

		public override void Scroll (int val)
		{
			CurrentPos += val;
			UpdateBoxes ();
		}

		#endregion
		
		#region IButtonCondition implementation
		ButtonState IButtonCondition.Test (params string[] info)
		{
			if (info[0] == "Up")
			{
				return (CurrentPos > 0 ? ButtonState.Active : ButtonState.Inactive);
			}
			
			if (info[0] == "Down")
			{
				return (CurrentPos < MaxIndex ? ButtonState.Active : ButtonState.Inactive);
			}
			return ButtonState.Active;
		}
		#endregion
	}
}