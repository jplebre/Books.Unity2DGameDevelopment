using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public PlayerStateController.playerStates currentPlayerState = PlayerStateController.playerStates.idle;

	public GameObject playerObject = null;
	public float cameraTrackingSpeed = 0.2f;

	private Vector3 lastTargetPosition = Vector3.zero;
	private Vector3 currTargetPosition = Vector3.zero;

	private float currLerpDistance = 0.0f;

	// Use this for initialization
	void Start () 
	{
		//initial camera position:
		Vector3 playerPos = playerObject.transform.position;
		Vector3 cameraPos = transform.position;
		Vector3 startTargPos = playerPos;

		//set the "Z" of camera to be the same "Z" as the player
		startTargPos.z = cameraPos.z;
		lastTargetPosition = startTargPos;
		currTargetPosition = startTargPos;
		currLerpDistance = 1.0f;
	}

	void OnEnable()
	{
		PlayerStateController.onStateChange += OnPlayerStateChange;
	}

	void OnDisable()
	{
		PlayerStateController.onStateChange -= OnPlayerStateChange;
	}

	void OnPlayerStateChange(PlayerStateController.playerStates newState)
	{
		currentPlayerState = newState;
	}

	void LateUpdate()
	{
		//Update based on our current state
		OnStateCycle();

		//continue moving to the targe position
		currLerpDistance += cameraTrackingSpeed;
		transform.position = Vector3.Lerp (lastTargetPosition, currTargetPosition, currLerpDistance);
	}

	void OnStateCycle()
	{
		/* Use the player state to determine the current action that the camera should take.
		 * In most cases, we'll be tracking the player.
		 * we don't want to track the player when player is dead or ressurecting
		 */
		switch (currentPlayerState)
		{
		case PlayerStateController.playerStates.idle:
			TrackPlayer();
			break;
		case PlayerStateController.playerStates.left:
			TrackPlayer();
			break;
		case PlayerStateController.playerStates.right:
			TrackPlayer();
			break;
		}
	}

	void TrackPlayer()
	{
		//Store current camera and player position in world coordinates
		Vector3 currCamPos = transform.position;
		Vector3 currPlayerPos = playerObject.transform.position;

		if (currCamPos.x == currPlayerPos.x && currCamPos.y == currPlayerPos.y)
		{
			currLerpDistance = 1f;
			lastTargetPosition = currCamPos;
			currTargetPosition = currCamPos;
			return;
		}

		//reset the travel distance for the Lerp
		currLerpDistance = 0f;

		//Store target position so we can lerp from it
		lastTargetPosition = currCamPos;

		//Store the new target position
		currTargetPosition = currPlayerPos;

		//Store the current target position so we can lerp from it
		currTargetPosition.z = currCamPos.z;
	}

	void StopTrackingPlayer()
	{
		//Store the current target position so we can lerp from it
		Vector3 currCamPos = transform.position;
		currTargetPosition = currCamPos;
		lastTargetPosition = currCamPos;

		//camera will lerp to it's current spot and stop there
		currLerpDistance = 1.0f;
	} 

	// Update is called once per frame
	void Update () {
	
	}
}
