using Mirror;
using UnityEngine;

public class MirrorManager : NetworkManager
{
   public Transform spawner;
   GameObject player;
   public bool isServer = false;


   public override void OnServerAddPlayer(NetworkConnectionToClient conn)
   {
      // add player at correct spawn position
      if (!isServer)
      {
         GameObject player = Instantiate(playerPrefab, spawner.position, spawner.rotation);
         NetworkServer.AddPlayerForConnection(conn, player);
      }
   }

   public override void OnServerDisconnect(NetworkConnectionToClient conn)
   {
      // destroy ball
      if (player != null)
         NetworkServer.Destroy(player);

      // call base functionality (actually destroys the player)
      base.OnServerDisconnect(conn);
   }
}
