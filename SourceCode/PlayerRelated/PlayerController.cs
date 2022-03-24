using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private Transform followingCamera;
	[SerializeField]
	private CharacterController controller;
	[SerializeField]
	private float speed = 6f;
	[SerializeField]
	private float gravity = 2f;
	[SerializeField]
	private float angularSmoothTime = 0.1f;

	private float yVelocity_ = 0;
	private float AngularSmoothVelocity_;

	// Signal the SceneHandler which is the current PlayerObject, used for Pause and Overlay
	void Start()
	{
		//Moves the player to the corresponding spawnPoint
		if (Interactable.playerNeedsReposition)
		{
			Debug.Log("Repositioned");
			transform.rotation = Interactable.nextRespawnRotation;

			Vector3 newPosition = Interactable.nextRespawnPosition;
			if (transform.position.y < 2)
				newPosition.y = 2;
			transform.position = newPosition;
			Interactable.playerNeedsReposition = false;
		}
		SceneHandler.SetPlayerComp(gameObject, followingCamera); //Sends information about the game object to the scene handler
	}

	void Update()
	{
		//Player movement
		Vector3 movInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

		if (movInput.magnitude >= 0.1f || !controller.isGrounded) 
		{
			float target = Mathf.Atan2(movInput.x, movInput.z) * Mathf.Rad2Deg + followingCamera.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref AngularSmoothVelocity_, angularSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

			Vector3 movement = Quaternion.Euler(0f, target, 0f) * Vector3.forward;

			if (!controller.isGrounded)
			{
				yVelocity_ -= gravity * Time.deltaTime;
				movement.y = yVelocity_;
			}
			else
			{
				yVelocity_ = 0;
			}

			controller.Move(movement.normalized * Time.deltaTime * speed);
		}
	}
}
