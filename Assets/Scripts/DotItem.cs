using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DotItem : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    public TextMeshProUGUI moves;
    public bool trackingCondition;
   [SerializeField] public int numberOfCollected;
   [SerializeField] public int requirementDots;
    
    private DotType dotType;
    public void InitDot(DotType dot,int requirmentQuantity){
        moves.enabled = false;
        image.color = GetDotColor(dot);
        text.text = $"0/{requirmentQuantity}";
        numberOfCollected = 0;
        requirementDots = requirmentQuantity;
        trackingCondition=false;
    }
    public DotType GetDotType()
    {
        return dotType;
    }
     public void UpdateCollectedQuantity(int collectedQuantity)
    {
        // Assuming the format is "collected/required"
        numberOfCollected+=collectedQuantity;
        text.text = $"{numberOfCollected}/{ requirementDots}";
        if(numberOfCollected >= requirementDots){
            trackingCondition = true;
            text.text = $"{requirementDots}/{ requirementDots}";
        }
    }
    public Color GetDotColor(DotType dot){
      switch (dot) {
        case DotType.Red:
            return Color.red;
        case DotType.Green:
            return Color.green;
        case DotType.Blue:
            return Color.blue;
        case DotType.Yellow:
            return Color.yellow;
        case DotType.Gray: return Color.gray;
        case DotType.Pink: return new Color(1f,0.1f,1f);
        case DotType.Orange: return new Color(1f,0.5f,0f);
        case DotType.PerrasinGreen: return new Color(0f,0.6f,0.6f);
        case DotType.DiscoBallBlue: return new Color(0.1f,0.85f,1f);
        case DotType.Chatreuse: return new Color(0.5f,1f,0f);
        case DotType.Indigo: return new Color(0f,0.27f,0.4f);
        case DotType.Raspberry: return new Color(0.9f,0f,0.45f);
        case DotType.PhtaloBlue: return new Color(0f,0.08f,0.5f);
        case DotType.CersizePink: return new Color(1f,0.2f,0.47f);
        case DotType.DarkMagneta: return new Color(0.5f,0f,0.6f);
        case DotType.Brown: return  new Color(0.6f,0.2f,0f);
        default:
            return Color.white; // Default case, if needed
    }
    }
}
