using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCharacter : MonoBehaviour
{
   public int currentCharacter = 1;

   public void ChangeCharacter(int position)
   {
      Debug.Log(position);
   }
}
