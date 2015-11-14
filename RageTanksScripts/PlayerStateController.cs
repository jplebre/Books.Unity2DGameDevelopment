using UnityEngine;
using System.Collections;

public class PlayerStateController : MonoBehaviour 
{
	public enum playerStates
	{
		idle = 0,
		left,
		right,
		jump,
		landing,
		falling,
		kill,
		resurrect,
		firingWeapon,
		_stateCount,
	}

	public static float[] stateDelayTimer = new float[(int)playerStates._stateCount];

	public delegate void playerStateHandler(PlayerStateController.playerStates NewState);

	public static event playerStateHandler onStateChange;

	void LateUpdate()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		if(horizontal != 0)
		{
			if (horizontal < 0)
			{
				if (onStateChange != null) onStateChange(PlayerStateController.playerStates.left);
			}
				
			else if (horizontal > 0)
			{
				if(onStateChange != null) onStateChange(PlayerStateController.playerStates.right);
			}
		}
		else
			if (onStateChange != null) onStateChange(PlayerStateController.playerStates.idle);


		float jump = Input.GetAxis ("Jump");
		if (jump > 0.0f)
		{
			if (onStateChange != null)
				onStateChange(PlayerStateController.playerStates.jump);
		}
	}
}
