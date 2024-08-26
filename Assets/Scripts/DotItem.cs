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
        default:
            return Color.white; // Default case, if needed
    }
    }
}
