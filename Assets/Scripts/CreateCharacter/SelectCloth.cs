using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCloth : MonoBehaviour
{
   public SkinnedMeshRenderer partBody;

   public Mesh mesh;


   public void SetCloth()
   {

      partBody.sharedMesh = mesh;


   }
}
