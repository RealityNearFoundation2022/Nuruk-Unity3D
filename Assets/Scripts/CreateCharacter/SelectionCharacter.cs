using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class SelectionCharacter : MonoBehaviour
{


   [Header("Men")]
   #region Head
   public SkinnedMeshRenderer head;
   public Mesh[] headMeshes;

   int currentHead = 0;
   #endregion

   #region Shirt
   public SkinnedMeshRenderer shirt;
   public Mesh[] shirtMeshes;

   int currentShirt = 0;

   #endregion

   #region Shoes
   public SkinnedMeshRenderer shoe;
   public Mesh[] shoeMeshes;

   int currentShoe = 0;

   #endregion

   #region Eyes
   public SkinnedMeshRenderer eye;
   public Mesh[] eyeMeshes;

   int currentEye = 0;
   #endregion

   #region Pants
   public SkinnedMeshRenderer pants;
   public Mesh[] pantsMeshes;

   int currentPants = 0;
   #endregion



   [Header("Women")]
   #region HeadWomen
   public SkinnedMeshRenderer headWomen;
   public Mesh[] headWomenMeshes;

   int currentHeadWomen = 0;
   #endregion

   #region ShirtWomen
   public SkinnedMeshRenderer shirtWomen;
   public Mesh[] shirtWomenMeshes;

   int currentShirtWomen = 0;

   #endregion

   #region ShoesWomen
   public SkinnedMeshRenderer shoeWomen;
   public Mesh[] shoeWomenMeshes;

   int currentShoeWomen = 0;

   #endregion

   #region EyesWomen
   public SkinnedMeshRenderer eyeWomen;
   public Mesh[] eyeWomenMeshes;

   int currentEyeWomen = 0;
   #endregion

   #region PantsWomen
   public SkinnedMeshRenderer pantsWomen;
   public Mesh[] pantsWomenMeshes;

   int currentPantsWomen = 0;
   #endregion

   #region ChangeGender
   public bool flagChange = false;
   #endregion

   [Header("General")]
   #region General
   public GameObject male;
   public GameObject female;

   public GameObject panelMen;
   public GameObject panelWomen;
   #endregion

   public ControlManagerCharacter controlManagerCharacter;

   public Material materialSkin;
   public int currentCharacter = 1;

   public void ChangeGender()
   {

      if (!flagChange)
      {
         flagChange = true;
         male.SetActive(false);
         female.SetActive(true);
         panelWomen.SetActive(true);
         panelMen.SetActive(false);
         controlManagerCharacter.DisableMen();
         controlManagerCharacter.InitWomen();
      }
      else
      {
         flagChange = false;
         male.SetActive(true);
         female.SetActive(false);
         panelWomen.SetActive(false);
         panelMen.SetActive(true);
         controlManagerCharacter.DisableWomen();
         controlManagerCharacter.InitMen();
      }
   }


   public void ChangeHead(int index)
   {
      if (!flagChange)
      {
         head.sharedMesh = headMeshes[index];
         currentHead = index;
      }
      else
      {
         headWomen.sharedMesh = headWomenMeshes[index];
         currentHeadWomen = index;
      }

   }

   public void ChangeShirt(int index)
   {
      if (!flagChange)
      {
         shirt.sharedMesh = shirtMeshes[index];
         currentShirt = index;
      }
      else
      {
         shirtWomen.sharedMesh = shirtWomenMeshes[index];
         currentShirtWomen = index;
      }


   }

   public void ChangeShoe(int index)
   {
      if (!flagChange)
      {
         shoe.sharedMesh = shoeMeshes[index];
         currentShoe = index;
      }
      else
      {
         shoeWomen.sharedMesh = shoeWomenMeshes[index];
         currentShoeWomen = index;
      }

   }

   public void ChangePant(int index)
   {
      if (!flagChange)
      {
         pants.sharedMesh = pantsMeshes[index];
         currentPants = index;
      }
      else
      {
         pantsWomen.sharedMesh = pantsWomenMeshes[index];
         currentPantsWomen = index;
      }

   }

   public void ChangeColor(string colorHex)
   {
      Color color;
      if (ColorUtility.TryParseHtmlString(colorHex, out color))
      {
         materialSkin.color = color;
      }

   }
}
