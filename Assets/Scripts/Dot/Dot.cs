using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
public class Dot : MonoBehaviour,IPointerEnterHandler,IPointerUpHandler,IPointerDownHandler,IPointerExitHandler
{
   public Color color;
   public DotType dotType;
   public int row;
   public int column;
   private Image spriteRenderer;

   [SerializeField] private GridManager gridManager;
   [SerializeField] private SpriteAtlas spriteAtlas;
    private Dictionary<DotType, string> dotTypeToSpriteName;
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
    private void Awake()
    {
        dotTypeToSpriteName = new Dictionary<DotType, string>
        {
            {DotType.Red,"egg_1" },
            {DotType.Green,"egg_2" },
            {DotType.Yellow,"egg_3" },
            {DotType.Pink, "egg_4" },
        };
    }
    private void Start() {

    spriteRenderer = GetComponent<Image>();
    gridManager = FindAnyObjectByType<GridManager>();
   }
   DotType GetRandomDotType(){
     return (DotType)Random.Range(0,System.Enum.GetValues(typeof(DotType)).Length);
   }
    void AssignSpriteFromAtlas(DotType type)
    {
        if (dotTypeToSpriteName.ContainsKey(type))
        {
            Sprite dotSprite = spriteAtlas.GetSprite(dotTypeToSpriteName[type]);
            if(dotSprite != null) 
            {
                spriteRenderer.sprite = dotSprite;
            }
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