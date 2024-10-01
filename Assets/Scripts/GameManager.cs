using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core.Easing;
using UnityEngine;
using TwoDotGridManager;
using TwoDotsDotItem;
using TwoDotsRequirementBar;

namespace TwoDotsGameManager 
{
    public class GameManager : MonoBehaviour
    {
        public GridManager gridManager;
        public Requirementbar requirementBar;
        public int trackingMove;
        public bool winOrLoose;
        public List<DotItem> dotItems = new();
        void Start()
        {
            trackingMove = gridManager.movesLeft;
            dotItems = requirementBar.dotItmes;
        }

        // Update is called once per frame
        void Update()
        {
            winOrLoose = dotItems.All(win => win.trackingCondition);
        }
        public bool WinLooseTracking()
        {
            if (winOrLoose)
            {
                return true;
            }
            else if (trackingMove == 0 && !winOrLoose)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}

