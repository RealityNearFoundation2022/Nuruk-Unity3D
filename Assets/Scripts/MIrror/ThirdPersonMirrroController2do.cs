using UnityEngine;
using Mirror;

public class ThirdPersonMirrroController2do : NetworkBehaviour
{
   [SerializeField]
   private float movementSpeed = 5f;

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

   public override void OnStartAuthority()
   {
      enabled = true;
      Controls.Player.Move.performed += ctx => SetPlayerMovement(ctx.ReadValue<Vector2>());
      Controls.Player.Move.canceled += ctx => ResetMovement();
   }

   [ClientCallback]
   private void OnEnable()
   {
      Controls.Enable();
   }

   [ClientCallback]
   void OnDisable()
   {
      Controls.Disable();
   }

   [ClientCallback]
   private void Update()
   {
      Move();
   }


   [Client]
   private void SetPlayerMovement(Vector2 movement)
   {
      previousInput = movement;
   }

   [Client]
   private void ResetMovement()
   {
      previousInput = Vector2.zero;
   }

   [Client]
   private void Move()
   {
      Vector3 right = controller.transform.right;
      Vector3 forward = controller.transform.forward;
      right.y = 0f;
      forward.y = 0f;

      Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y;
      controller.Move(movement * movementSpeed * Time.deltaTime);
   }
}
