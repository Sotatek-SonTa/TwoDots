
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Dot : MonoBehaviour,IPointerEnterHandler,IPointerUpHandler,IPointerDownHandler,IPointerExitHandler
{
   public Color color;
   public DotType dotType;
   public int row;
   public int column;
   private Image spriteRenderer;
   [SerializeField] private GridManager gridManager;
   public void OnPointerDown(PointerEventData eventData){
    gridManager.OnSelectionStart(this);  
   }
   public void  OnPointerEnter(PointerEventData eventData){
    if(gridManager.selectedDots.Count >1 && gridManager.selectedDots[gridManager.selectedDots.Count -2]==this)  
    {
      gridManager.selectedDots.RemoveAt(gridManager.selectedDots.Count-1);
      gridManager.UpdateLineRenderer();
    }else{
        gridManager.OnDotSelected(this);
    }
   }
   public void OnPointerUp(PointerEventData eventData){
         gridManager.OnSelectionEnd();
   }
   public void OnPointerExit(PointerEventData eventData){
       // gridManager.OnDotExit(this);
   }
   private void Start() {

    spriteRenderer = GetComponent<Image>();
    gridManager = FindAnyObjectByType<GridManager>();
    //AssignRandomColor();
   }
   void AssignRandomColor(){
    dotType = GetRandomDotType();
    color = GetColorFromDotType(dotType);
    spriteRenderer.color = color;
   }
   DotType GetRandomDotType(){
     return (DotType)Random.Range(0,System.Enum.GetValues(typeof(DotType)).Length);
   }
   Color GetColorFromDotType(DotType type) {
    switch (type) {
        case DotType.Red:
            return Color.red;
        case DotType.Green:
            return Color.green;
        case DotType.Blue:
            return Color.blue;
        case DotType.Yellow:
            return Color.yellow;
        case DotType.Gray:
             return Color.gray;
        case DotType.Pink:
            return new Color(1f,0.1f,1f);
        case DotType.Orange:
            return new Color(1f,0.5f,0f);
        case DotType.PerrasinGreen:
             return new Color(0f,0.6f,0.6f);
        default:
            return Color.white; // Default case, if needed
    }
 }
 public DotType GetDotType()
{
    return dotType;
}

}

   public enum DotType{
    Red,
    Green,
    Blue,
    Yellow,
    Gray,
    Pink,
    Orange,
    PerrasinGreen,
    DiscoBallBlue,
    Chatreuse,
    Indigo,
    Raspberry,
    PhtaloBlue,
    CersizePink,
    DarkMagneta,
    Brown,
    TyrianPurple,
    OxfordBlue,
    CherryBlossomPink,
    Chocolate,
    BabyBlueEyes,
   }