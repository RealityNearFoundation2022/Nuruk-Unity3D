using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class SelectionCharacter : MonoBehaviour
{

   [Header("Men")]
   #region Extra control Men
   public GameObject[] extrasObjectsMen;
   public string currentExtraMen;
   int indexCurrentExtraMen = 0;
   #endregion

   #region Head control Men
   public GameObject[] headObjectsMen;
   public string currentHeadMen;
   int indexCurrentHeadMen = 0;
   #endregion

   #region Shirt control Men
   public GameObject[] shirtObjectsMen;
   public string currentShirtMen;
   int indexCurrentShirtMen = 0;
   #endregion

   #region Pants control Men

   public GameObject[] pantsObjectsMen;
   public string currentPantsMen;
   int indexCurrentPantsMen = 0;

   #endregion

   #region Shoes control
   public GameObject[] shoesObjectsMen;
   public string currentShoesMen;
   int indexCurrentShoesMen = 0;
   #endregion

   [Header("Women")]
   #region Extra control Women
   public GameObject[] extrasObjectsWomen;
   public string currentExtraWomen;
   int indexCurrentExtraWomen = 0;
   #endregion

   #region Head control Women
   public GameObject[] headObjectsWomen;
   public string currentHeadWomen;
   int indexCurrentHeadWomen = 0;
   #endregion

   #region Shirt control Men
   public GameObject[] shirtObjectsWomen;
   public string currentShirtWomen;
   int indexCurrentShirtWomen = 0;
   #endregion

   #region Pants control Men

   public GameObject[] pantsObjectsWomen;
   public string currentPantsWomen;
   int indexCurrentPantsWomen = 0;

   #endregion

   #region Shoes control
   public GameObject[] shoesObjectsWomen;
   public string currentShoesWomen;
   int indexCurrentShoesWomen = 0;
   #endregion

   [Header("Monster")]
   #region Extra control Monster
   public GameObject[] extrasObjectsMonster;
   public string currentExtraMonster;
   int indexCurrentExtraMonster = 0;
   #endregion

   #region Head control Monster
   public GameObject[] headObjectsMonster;
   public string currentHeadMonster;
   int indexCurrentHeadMonster = 0;
   #endregion

   #region Shirt control Monster
   public GameObject[] shirtObjectsMonster;
   public string currentShirtMonster;
   int indexCurrentShirtMonster = 0;
   #endregion

   #region Pants control Monster
   public GameObject[] pantsObjectsMonster;
   public string currentPantsMonster;
   int indexCurrentPantsMonster = 0;

   #endregion

   #region Shoes control
   public GameObject[] shoesObjectsMonster;
   public string currentShoesMonster;
   int indexCurrentShoesMonster = 0;
   #endregion


   #region Gender

   public string currentGender = "Male";
   #endregion

   #region Panels
   public GameObject panelMen;
   public GameObject panelWomen;
   public GameObject panelMonster;

   #endregion

   #region Characters
   public GameObject Women;
   public GameObject Men;
   public GameObject Monster;

   public Material materialSkin;


   private string colorSelect;
   #endregion

   public void ChangeExtra(int positionExtra)
   {
      extrasObjectsMen[indexCurrentExtraMen].SetActive(false);
      extrasObjectsMen[positionExtra].SetActive(true);
      indexCurrentExtraMen = positionExtra;
      currentExtraMen = extrasObjectsMen[indexCurrentExtraMen].name;
   }

   public void ChangeHead(int positionHead)
   {
      headObjectsMen[indexCurrentHeadMen].SetActive(false);
      headObjectsMen[positionHead].SetActive(true);
      indexCurrentHeadMen = positionHead;
      currentHeadMen = headObjectsMen[indexCurrentHeadMen].name;
   }

   public void ChangeShirt(int positionShirt)
   {
      shirtObjectsMen[indexCurrentShirtMen].SetActive(false);
      shirtObjectsMen[positionShirt].SetActive(true);
      indexCurrentShirtMen = positionShirt;
      currentShirtMen = shirtObjectsMen[indexCurrentShirtMen].name;
   }

   public void ChangePants(int positionPants)
   {
      pantsObjectsMen[indexCurrentPantsMen].SetActive(false);
      pantsObjectsMen[positionPants].SetActive(true);
      indexCurrentPantsMen = positionPants;
      currentPantsMen = pantsObjectsMen[indexCurrentPantsMen].name;
   }

   public void ChangeShoes(int positionShoes)
   {
      shoesObjectsMen[indexCurrentShoesMen].SetActive(false);
      shoesObjectsMen[positionShoes].SetActive(true);
      indexCurrentShoesMen = positionShoes;
      currentShoesMen = shoesObjectsMen[indexCurrentShoesMen].name;
   }


   public void ChangeExtraWomen(int positionExtra)
   {
      extrasObjectsWomen[indexCurrentExtraWomen].SetActive(false);
      extrasObjectsWomen[positionExtra].SetActive(true);
      indexCurrentExtraWomen = positionExtra;
      currentExtraWomen = extrasObjectsWomen[indexCurrentExtraWomen].name;
   }

   public void ChangeHeadWomen(int positionHead)
   {
      headObjectsWomen[indexCurrentHeadWomen].SetActive(false);
      headObjectsWomen[positionHead].SetActive(true);
      indexCurrentHeadWomen = positionHead;
      currentHeadWomen = headObjectsWomen[indexCurrentHeadWomen].name;
   }

   public void ChangeShirtWomen(int positionShirt)
   {
      shirtObjectsWomen[indexCurrentShirtWomen].SetActive(false);
      shirtObjectsWomen[positionShirt].SetActive(true);
      indexCurrentShirtWomen = positionShirt;
      currentShirtWomen = shirtObjectsWomen[indexCurrentShirtWomen].name;
   }

   public void ChangePantsWomen(int positionPants)
   {
      pantsObjectsWomen[indexCurrentPantsWomen].SetActive(false);
      pantsObjectsWomen[positionPants].SetActive(true);
      indexCurrentPantsWomen = positionPants;
      currentPantsWomen = pantsObjectsWomen[indexCurrentPantsWomen].name;
   }

   public void ChangeShoesWomen(int positionShoes)
   {
      shoesObjectsWomen[indexCurrentShoesWomen].SetActive(false);
      shoesObjectsWomen[positionShoes].SetActive(true);
      indexCurrentShoesWomen = positionShoes;
      currentShoesWomen = shoesObjectsWomen[indexCurrentShoesWomen].name;
   }

   public void ChangeExtraMonster(int positionExtra)
   {
      extrasObjectsMonster[indexCurrentExtraMonster].SetActive(false);
      extrasObjectsMonster[positionExtra].SetActive(true);
      indexCurrentExtraMonster = positionExtra;
      currentExtraMonster = extrasObjectsMonster[indexCurrentExtraMonster].name;
   }

   public void ChangeHeadMonster(int positionHead)
   {
      headObjectsMonster[indexCurrentHeadMonster].SetActive(false);
      headObjectsMonster[positionHead].SetActive(true);
      indexCurrentHeadMonster = positionHead;
      currentHeadMonster = headObjectsMonster[indexCurrentHeadMonster].name;
   }

   public void ChangeShirtMonster(int positionShirt)
   {
      shirtObjectsMonster[indexCurrentShirtMonster].SetActive(false);
      shirtObjectsMonster[positionShirt].SetActive(true);
      indexCurrentShirtMonster = positionShirt;
      currentShirtMonster = shirtObjectsMonster[indexCurrentShirtMonster].name;
   }

   public void ChangePantsMonster(int positionPants)
   {
      pantsObjectsMonster[indexCurrentPantsMonster].SetActive(false);
      pantsObjectsMonster[positionPants].SetActive(true);
      indexCurrentPantsMonster = positionPants;
      currentPantsMonster = pantsObjectsMonster[indexCurrentPantsMonster].name;
   }

   public void ChangeShoesMonster(int positionShoes)
   {
      shoesObjectsMonster[indexCurrentShoesMonster].SetActive(false);
      shoesObjectsMonster[positionShoes].SetActive(true);
      indexCurrentShoesMonster = positionShoes;
      currentShoesMonster = shoesObjectsMonster[indexCurrentShoesMonster].name;
   }


   public void ChangeGender(string gender)
   {
      switch (gender)
      {
         case "Male":
            panelMen.SetActive(true);
            panelWomen.SetActive(false);
            panelMonster.SetActive(false);
            Men.SetActive(true);
            Women.SetActive(false);
            Monster.SetActive(false);
            break;

         case "Female":
            panelMen.SetActive(false);
            panelWomen.SetActive(true);
            panelMonster.SetActive(false);
            Men.SetActive(false);
            Women.SetActive(true);
            Monster.SetActive(false);
            break;

         case "Monster":
            panelMen.SetActive(false);
            panelWomen.SetActive(false);
            panelMonster.SetActive(true);
            Men.SetActive(false);
            Women.SetActive(false);
            Monster.SetActive(true);
            break;


      }
   }

   public void ChangeColor(string hexColor)
   {
      Color color;
      ColorUtility.TryParseHtmlString(hexColor, out color);
      materialSkin.color = color;
      colorSelect = hexColor;
   }
}
