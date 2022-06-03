using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlManagerCharacter : MonoBehaviour
{
   public ScrollRect scrollReact;
   public SelectionCharacter selectionCharacter;

   #region Men

   public GameObject headMen;
   public GameObject bodyMen;
   public GameObject pantsMen;
   public GameObject shoesMen;
   #endregion



   #region Women
   public GameObject headWomen;
   public GameObject bodyWomen;
   public GameObject pantsWomen;
   public GameObject shoesWomen;
   #endregion


   private void Start()
   {
      headMen.SetActive(true);
   }
   public void ChangeSection(int type)
   {
      if (!selectionCharacter.flagChange)
      {
         switch (type)
         {
            case 1:
               headMen.SetActive(true);
               bodyMen.SetActive(false);
               pantsMen.SetActive(false);
               shoesMen.SetActive(false);
               scrollReact.content = headMen.GetComponent<RectTransform>();
               break;
            case 2:
               headMen.SetActive(false);
               bodyMen.SetActive(true);
               pantsMen.SetActive(false);
               shoesMen.SetActive(false);
               scrollReact.content = bodyMen.GetComponent<RectTransform>();
               break;
            case 3:
               headMen.SetActive(false);
               bodyMen.SetActive(false);
               pantsMen.SetActive(true);
               shoesMen.SetActive(false);
               scrollReact.content = pantsMen.GetComponent<RectTransform>();
               break;
            case 4:
               headMen.SetActive(false);
               bodyMen.SetActive(false);
               pantsMen.SetActive(false);
               shoesMen.SetActive(true);
               scrollReact.content = shoesMen.GetComponent<RectTransform>();
               break;
         }
      }
      else
      {
         switch (type)
         {
            case 1:
               headWomen.SetActive(true);
               bodyWomen.SetActive(false);
               pantsWomen.SetActive(false);
               shoesWomen.SetActive(false);
               scrollReact.content = headMen.GetComponent<RectTransform>();
               break;
            case 2:
               headWomen.SetActive(false);
               bodyWomen.SetActive(true);
               pantsWomen.SetActive(false);
               shoesWomen.SetActive(false);
               scrollReact.content = bodyMen.GetComponent<RectTransform>();
               break;
            case 3:
               headWomen.SetActive(false);
               bodyWomen.SetActive(false);
               pantsWomen.SetActive(true);
               shoesWomen.SetActive(false);
               scrollReact.content = pantsMen.GetComponent<RectTransform>();
               break;
            case 4:
               headWomen.SetActive(false);
               bodyWomen.SetActive(false);
               pantsWomen.SetActive(false);
               shoesWomen.SetActive(true);
               scrollReact.content = shoesMen.GetComponent<RectTransform>();
               break;
         }
      }
   }

   public void DisableMen()
   {
      headMen.SetActive(false);
      bodyMen.SetActive(false);
      pantsMen.SetActive(false);
      shoesMen.SetActive(false);
   }

   public void DisableWomen()
   {
      headWomen.SetActive(false);
      bodyWomen.SetActive(false);
      pantsWomen.SetActive(false);
      shoesWomen.SetActive(false);
   }

   public void InitMen()
   {
      headMen.SetActive(true);
   }

   public void InitWomen()
   {
      headWomen.SetActive(true);
   }
}
