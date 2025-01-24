using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwoDotsRequirementBar;

namespace TwodDotUIManager 
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] public GameObject WinUI;
        [SerializeField] public GameObject LooseUI;
        [SerializeField] public Requirementbar requirementbar;
        [SerializeField] public int moveLeft;
        [SerializeField] public int levelIndex;
        

        public void ShowPopUpWin()
        {
            WinUI.SetActive(true);
        }
        public void ShowPopUpLoose()
        {
            LooseUI.SetActive(true);
        }
        public void ClosePopUp()
        {
            WinUI.SetActive(false);
            LooseUI.SetActive(false);
        }
        public void OnMoveDone()
        {
            if (moveLeft > 0)
            {
                moveLeft--;
                requirementbar.UpdateMoveLeft(moveLeft);
            }
        }
    }

}
