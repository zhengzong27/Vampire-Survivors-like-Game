using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
   public GameObject[] titles;
   public GameObject continueButton;
   public GameObject retryButton;

   public void Lose()
   {
        titles[0].SetActive(true);
        titles[1].SetActive(false);
        continueButton.SetActive(false);
        retryButton.SetActive(true);
   }

   public void Win()
   {
        titles[0].SetActive(false);
        titles[1].SetActive(true);
        continueButton.SetActive(false);
        retryButton.SetActive(true);
   }

   public void ShowContinue()
   {
        titles[0].SetActive(false);
        titles[1].SetActive(true);
        continueButton.SetActive(true);
        retryButton.SetActive(false);
   }

   public void ShowRetry()
   {
        titles[0].SetActive(false);
        titles[1].SetActive(true);
        continueButton.SetActive(false);
        retryButton.SetActive(true);
   }
}
