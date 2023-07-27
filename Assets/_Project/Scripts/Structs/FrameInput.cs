#region

using UnityEngine;

#endregion

namespace _Project.Scripts.Structs
{

	public struct FrameInput
	{
		#region Constants and Fields

		public bool BoostDown;
		public bool JumpDown;
		public bool JumpPressed;
		public bool JumpReleased;
		public float X;
		public float Z;

		#endregion

		#region Properties

		public Vector2 InputVector
		{
			get => Vector2.ClampMagnitude(new Vector2(X, Z), 1f);
		}

		#endregion
	}

}