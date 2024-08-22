
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
        default:
            return Color.white; // Default case, if needed
    }
 }
}

   public enum DotType{
    Red,
    Green,
    Blue,
    Yellow,
   }