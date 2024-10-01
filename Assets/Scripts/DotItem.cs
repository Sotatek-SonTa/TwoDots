using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace TwoDotsDotItem
{
    public class DotItem : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI text;
        public TextMeshProUGUI moves;
        public bool trackingCondition;
        [SerializeField] public int numberOfCollected;
        [SerializeField] public int requirementDots;
        public SpriteAtlas spriteAtlas;
        private DotType dotType;
        public void InitDot(DotType dot, int requirmentQuantity)
        {
            moves.enabled = false;
            image.sprite = GetDotSprite(dot);
            text.text = $"0/{requirmentQuantity}";
            dotType = dot;
            numberOfCollected = 0;
            requirementDots = requirmentQuantity;
            trackingCondition = false;
        }
        public DotType GetDotType()
        {
            return dotType;
        }
        public void UpdateCollectedQuantity(int collectedQuantity)
        {
            // Assuming the format is "collected/required"
            numberOfCollected += collectedQuantity;
            text.text = $"{numberOfCollected}/{ requirementDots}";
            if (numberOfCollected >= requirementDots)
            {
                trackingCondition = true;
                text.text = $"{requirementDots}/{ requirementDots}";
            }
        }
        public Color GetDotColor(DotType dot)
        {
            switch (dot)
            {
                case DotType.Red:
                    return Color.red;
                case DotType.Green:
                    return Color.green;
                case DotType.Blue:
                    return Color.blue;
                case DotType.Yellow:
                    return Color.yellow;
                case DotType.Gray: return Color.gray;
                case DotType.Pink: return new Color(1f, 0.1f, 1f);
                case DotType.Orange: return new Color(1f, 0.5f, 0f);
                case DotType.PerrasinGreen: return new Color(0f, 0.6f, 0.6f);
                case DotType.DiscoBallBlue: return new Color(0.1f, 0.85f, 1f);
                case DotType.Chatreuse: return new Color(0.5f, 1f, 0f);
                case DotType.Indigo: return new Color(0f, 0.27f, 0.4f);
                case DotType.Raspberry: return new Color(0.9f, 0f, 0.45f);
                case DotType.PhtaloBlue: return new Color(0f, 0.08f, 0.5f);
                case DotType.CersizePink: return new Color(1f, 0.2f, 0.47f);
                case DotType.DarkMagneta: return new Color(0.5f, 0f, 0.6f);
                case DotType.Brown: return new Color(0.6f, 0.2f, 0f);
                case DotType.TyrianPurple: return new Color(0.5f, 0f, 0.25f);
                case DotType.OxfordBlue: return new Color(0f, 0.1f, 0.2f);
                case DotType.CherryBlossomPink: return new Color(1f, 0.7f, 0.8f);
                case DotType.Chocolate: return new Color(0.5f, 0.33f, 0f);
                case DotType.BabyBlueEyes: return new Color(0.6f, 0.67f, 1f);
                default:
                    return Color.white; // Default case, if needed
            }
        }
        public Sprite GetDotSprite(DotType dot)
        {
            switch (dot)
            {
                case DotType.Red: return spriteAtlas.GetSprite("egg_1");
                case DotType.Blue: return spriteAtlas.GetSprite("egg_2");
                case DotType.Yellow: return spriteAtlas.GetSprite("egg_3");
                case DotType.Pink: return spriteAtlas.GetSprite("egg_4");
                case DotType.Green: return spriteAtlas.GetSprite("green");
                case DotType.BabyBlueEyes: return spriteAtlas.GetSprite("baby blue eyes");
                case DotType.Brown: return spriteAtlas.GetSprite("cersize pink");
                case DotType.CersizePink: return spriteAtlas.GetSprite("cersize pink");
                case DotType.Chatreuse: return spriteAtlas.GetSprite("chartreuse");
                case DotType.CherryBlossomPink: return spriteAtlas.GetSprite("cherry blossom pink");
                case DotType.Chocolate: return spriteAtlas.GetSprite("chocolate");
                case DotType.DarkMagneta: return spriteAtlas.GetSprite("dark magneta");
                case DotType.DiscoBallBlue: return spriteAtlas.GetSprite("disco");
                case DotType.Gray: return spriteAtlas.GetSprite("gray");
                case DotType.Indigo: return spriteAtlas.GetSprite("idingo");
                case DotType.Orange: return spriteAtlas.GetSprite("orange");
                case DotType.OxfordBlue: return spriteAtlas.GetSprite("oxford blue");
                case DotType.PerrasinGreen: return spriteAtlas.GetSprite("perrasin");
                case DotType.PhtaloBlue: return spriteAtlas.GetSprite("phtalo blue");
                case DotType.Raspberry: return spriteAtlas.GetSprite("raspberry");
                case DotType.TyrianPurple: return spriteAtlas.GetSprite("tyrian purple");
                default: return spriteAtlas.GetSprite("egg_1");
            }
        }
    }
}

