using Cinemachine;
using UnityEngine;
using Mirror;

public class TestMirror : NetworkBehaviour
{
   [SerializeField]
   private float movementSpeed = 5f;

   [SerializeField]
   private float gravity = 9.8f;
   [SerializeField]
   private float fallVelocity;
   [SerializeField]
   private float jumpForce;

   [SerializeField]
   private float rotationSpeed = 10f;
   [SerializeField]
   private Animator animator;
   [SerializeField]
   private Transform camera;




   private Vector3 movement;
   private bool isJump;



   private bool isWalking;

   [SerializeField]
   private CharacterController controller = null;

   private Vector2 previousInput;

   private PlayerInputs controls;
   private PlayerInputs Controls
   {
      get
      {
         if (controls != null) { return controls; }
         return controls = new PlayerInputs();
      }
   }


   private void OnEnable()
   {
      Controls.Enable();
      enabled = true;
      Controls.Player.Move.performed += ctx => SetPlayerMovement(ctx.ReadValue<Vector2>());
      Controls.Player.Move.canceled += ctx => ResetMovement();
      Controls.Player.Sprint.performed += ctx => Run();
      Controls.Player.Jump.performed += ctx => JumpController();
   }



   public override void OnStartClient()
   {
      CinemachineFreeLook freeLook = FindObjectOfType<CinemachineFreeLook>();
      camera = freeLook.transform;
   }

   void OnDisable()
   {
      Controls.Disable();
   }


   private void Update()
   {
      if (!isLocalPlayer) { return; }
      Move();
   }



   private void SetPlayerMovement(Vector2 movement)
   {
      Debug.Log(movement);
      previousInput = movement;
      isWalking = true;
   }


   private void ResetMovement()
   {
      previousInput = Vector2.zero;
      isWalking = false;
      animator.SetBool("IsWalking", false);
   }


   private void Move()
   {
      HandleAnimations();
      if (controller.isGrounded)
      {
         animator.SetBool("Grounded", true);
         animator.SetBool("Jump", false);
      }
      Vector3 right = controller.transform.right;
      Vector3 forward = controller.transform.forward;
      // right.y = 0f;
      // forward.y = 0f;

      movement = Quaternion.Euler(0, camera.transform.forward.y, 0) * new Vector3(previousInput.x, 0, previousInput.y);


      if (movement.normalized != Vector3.zero)
      {
         Quaternion rotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
      }

      SetGravity();
      Jump();

      movement = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * movement;
      //movement = Quaternion.AngleAxis(camera.rotation.eulerAngles.y, Vector3.up) * movement;
      //Debug.Log(movement.normalized);
      controller.Move(movement.normalized * movementSpeed * Time.deltaTime);
      //animator.SetFloat("Speed", controller.velocity.normalized);
   }

   private void SetGravity()
   {
      if (controller.isGrounded)
      {
         fallVelocity = -gravity * Time.deltaTime;
         movement.y = fallVelocity;
      }
      else
      {
         fallVelocity -= gravity * Time.deltaTime;
         movement.y = fallVelocity;
      }
   }


   private void Jump()
   {
      if (controller.isGrounded && isJump)
      {
         animator.SetBool("Jump", true);
         animator.SetBool("Grounded", false);
         fallVelocity = jumpForce;
         movement.y = fallVelocity;
         isJump = false;
      }
   }

   private void Run()
   {
      movement = movement * 20f;
   }

   private void JumpController()
   {
      if (controller.isGrounded && !isJump)
         isJump = true;
   }

   public void HandleAnimations()
   {
      bool isCurrentWalking = animator.GetBool("IsWalking");

      if (isWalking && !isCurrentWalking)
      {
         animator.SetBool("IsWalking", true);
      }
   }
}
