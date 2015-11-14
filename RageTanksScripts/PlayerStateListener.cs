using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{         
	public float playerWalkSpeed = 3f;

	public GameObject playerRespawnPoint = null;

	public float playerJumpForceVertical = 500f;
	public float playerJumpForceHorizontal = 250f;
	private bool playerHasLanded = true;

	
	private Animator playerAnimator = null;
	private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;
	
	void OnEnable()
	{
		PlayerStateController.onStateChange += OnStateChange;
	}
	
	void OnDisable()
	{
		PlayerStateController.onStateChange -= OnStateChange;
	}
	
	void Start()
	{
		playerAnimator = GetComponent<Animator>();
	}
	
	void LateUpdate()
	{
		OnStateCycle();
	}

	void OnStateCycle()
	{
		//the scale is actually going to be used to turn the player left or right
		Vector3 localScale = transform.localScale;

		switch(currentState)
		{
		case PlayerStateController.playerStates.idle:
			playerAnimator.SetBool ("Walking", false);
			break;

		case PlayerStateController.playerStates.left:
			playerAnimator.SetBool ("Walking", true);
			transform.Translate(new Vector3((playerWalkSpeed * -0.1f)*Time.deltaTime, 0.0f, 0.0f));
			if (localScale.x >0.0f)
			{
				localScale.x *= -1.0f;
				transform.localScale = localScale;
			}
			break;

		case PlayerStateController.playerStates.right:
			playerAnimator.SetBool ("Walking", true);
			transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime, 0.0f, 0.0f));
			if (localScale.x < 0.0f)
			{
				localScale.x *= -1.0f;
				transform.localScale = localScale;
			}
			break;

		case PlayerStateController.playerStates.jump:
			break;
			
		case PlayerStateController.playerStates.landing:
			break;
			
		case PlayerStateController.playerStates.falling:
			break;              
			
		case PlayerStateController.playerStates.kill:
			OnStateChange (PlayerStateController.playerStates.resurrect);
			break;         
			
		case PlayerStateController.playerStates.resurrect:
			OnStateChange(PlayerStateController.playerStates.idle);
			break;
		}
	}

	//onStateChange is called when we need to change player's state
	public void OnStateChange(PlayerStateController.playerStates newState)
	{
		//if the current state and the new state are the same, do nothing
		if (newState == currentState)
			return;

		//can current state transition into a new state?
		if (!CheckForValidStatePair(newState))
			return;

		//if it passes the above check:
		switch(newState)
		{
		case PlayerStateController.playerStates.idle:
			break;
		case PlayerStateController.playerStates.left:
			break;
		case PlayerStateController.playerStates.right:
			break;
		case PlayerStateController.playerStates.kill:
			break;
		case PlayerStateController.playerStates.resurrect:
			transform.position = playerRespawnPoint.transform.position;
			transform.rotation = Quaternion.identity;
			rigidbody2D.velocity = Vector2.zero;
			break;
		case PlayerStateController.playerStates.jump:
			if(playerHasLanded)
			{
				float JumpDirection = 0.0f;
				if(currentState == PlayerStateController.playerStates.left)
					JumpDirection = -1.0f;
				else if (currentState == PlayerStateController.playerStates.right)
					JumpDirection = 1.0f;
				else
					JumpDirection = 0.0f;

				//Apply jump force
				rigidbody2D.AddForce(new Vector2(JumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));

				playerHasLanded = false;
				PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.jump] = 0f;
			}
			break;
		}

		//update the state check variable
		currentState = newState;
	}


	//this compares the current state with the desired new state
	//checks if they are compatible and outputs a bool
	bool CheckForValidStatePair(PlayerStateController.playerStates newState)
	{
		bool returnVal = false;

		//Compare the current state against new desirable state
		switch(currentState)
		{
		case PlayerStateController.playerStates.idle:
			returnVal = true; //any state can take over from idle
			break;
		case PlayerStateController.playerStates.left:
			returnVal = true; //any state can take over from left
			break;
		case PlayerStateController.playerStates.right:
			returnVal = true; //any state can take over from right
			break;
		case PlayerStateController.playerStates.kill:
			if(newState == PlayerStateController.playerStates.resurrect) //only ressurect can take over from kill
				returnVal = true;
			else
				returnVal = false;
			break;
		case PlayerStateController.playerStates.resurrect:
			if(newState == PlayerStateController.playerStates.idle)
				returnVal = true;
			else
				returnVal = false;
			break;
		case PlayerStateController.playerStates.jump:
			if(newState == PlayerStateController.playerStates.landing || newState == PlayerStateController.playerStates.kill ||
			   newState == PlayerStateController.playerStates.firingWeapon)
				returnVal = true;
			else
				returnVal = false;
			break;
		}

		return returnVal;
	}


	public void HitDeathTrigger()
	{
		OnStateChange (PlayerStateController.playerStates.kill);
	}
}