using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using TwoDotGridManager;
using TwoDotsDotItem;
using TwoDotsLevelData;

namespace TwoDotsRequirementBar
{
    public class Requirementbar : MonoBehaviour
    {
        public DotItem dotItem;
        public GridManager gridManager;
        public RectTransform requiremnetHolder;
        public TextMeshProUGUI moves;
        public TextMeshProUGUI movesLeft;

        public TextMeshProUGUI level;
        [SerializeField] public List<DotItem> dotItmes = new();
        private Dictionary<DotType, DotItem> collectedDots = new Dictionary<DotType, DotItem>();
        public bool trackingCondition;
        void Start()
        {

        }
        public void SetRequirement(LevelData levelData)
        {
            DeleteLevelRequierment();
            dotItmes.Clear();
            collectedDots.Clear();
            foreach (var dot in levelData.dotTypeRequirements)
            {
                DotItem newItem = Instantiate(dotItem, requiremnetHolder);
                newItem.InitDot(dot.dotType, dot.quantity);
                RectTransform dotRect = newItem.GetComponent<RectTransform>();
                dotRect.SetParent(requiremnetHolder, false);
                collectedDots.Add(dot.dotType, newItem);
                dotItmes.Add(newItem);
            }
            moves.text = "Moves";
            movesLeft.text = $"{levelData.numberOfMoves}";
            level.text = $"{levelData.level}";

        }
        public void UpdateMoveLeft(int moveLeft)
        {
            movesLeft.text = $"{moveLeft}";
            trackingCondition = dotItmes.All(condition => condition.trackingCondition);
        }
        public void DeleteLevelRequierment()
        {
            trackingCondition = false;
            for (int i = 0; i < dotItmes.Count; i++)
            {
                Destroy(dotItmes[i].gameObject);
            }
        }
        public void UpdateCollectedDots(DotType dotType, int quantity)
        {
            if (collectedDots.ContainsKey(dotType))
            {
                collectedDots[dotType].UpdateCollectedQuantity(quantity);
            }
        }
        void Update()
        {

        }
    }
}

