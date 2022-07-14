using Cinemachine;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
   public Transform lookAt;

   [SerializeField]
   private Vector2 maxFollowOffset = new Vector2(-1f, 6f);
   [SerializeField]
   private Vector2 cameraVelocity = new Vector2(4f, 0.25f);
   [SerializeField]
   private Transform playerRoot = null;
   [SerializeField]

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

      CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>(); //Find main camera which is part of the scene instead of the prefab
      cam.LookAt = lookAt;
      cam.Follow = playerRoot;

      Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
   }

   [ClientCallback]
   private void OnEnable()
   {
      Controls.Enable();
   }

   [ClientCallback]
   private void OnDisable()
   {
      Controls.Disable();
   }


   private void Look(Vector2 lookAxis)
   {
      // float deltaTime = Time.deltaTime;

      // float followOffset = Mathf.Clamp(
      //     transposer.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * deltaTime),
      //     maxFollowOffset.x,
      //     maxFollowOffset.y
      // );
      // transposer.m_FollowOffset.y = followOffset;

      // playerRoot.Rotate(0f, lookAxis.x * cameraVelocity.x * deltaTime, 0f);
   }
}
