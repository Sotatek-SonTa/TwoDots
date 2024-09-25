using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class GridManager : MonoBehaviour
{
   [SerializeField]public int columns;
   [SerializeField]public int rows;
   [SerializeField]public float titleSpacing=1f;

   [SerializeField] public GameObject dotPrefab;
   [SerializeField] public GameObject[,] dotMatrix;

   [SerializeField] public GameObject WinUI;
   [SerializeField] public GameObject LooseUI;

    [SerializeField] public List<Dot> selectedDots = new List<Dot>();

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LevelList levelList;
    [SerializeField] public LevelData levelData;
    [SerializeField] public Requirementbar requirementBar;


    [SerializeField] private SpriteAtlas dotSpriteAtlas;
    [SerializeField] private Dictionary<DotType, string> dotTypeToSpriteNameMap;

    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] public int movesLeft;
    [SerializeField] public int levelIndex;

    [SerializeField] public Sprite blockCell;
    
    private void Start() {
        IntilalizeDotTypeToSpriteMap();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        levelIndex = 0;
        dotMatrix = new GameObject[rows,columns];
        LoadLevel(levelIndex);
   }
   private void Update() {
     if(selectedDots.Count >0 ){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        lineRenderer.SetPosition(lineRenderer.positionCount -1, mousePosition);

     }
   }
   public void  LoadLevel(int levelIndex){
    if(levelIndex<0 || levelIndex >= levelList.levels.Length){
        return;
    }
     levelData = levelList.levels[levelIndex];
     rows = levelList.levels[levelIndex].rows;
     columns = levelList.levels[levelIndex].columns;
     dotMatrix = new GameObject[rows,columns]; 
     ApplyLevelData();
     requirementBar.SetRequirement(levelData);
     movesLeft = levelData.numberOfMoves;
   }
   private void ApplyLevelData(){
     AdjustDotSize();
     CreateGrid();
   }
   private void OnMoveDone(){
    if(movesLeft >0){
        movesLeft--;
        requirementBar.UpdateMoveLeft(movesLeft);
    }
   }
    private void CreateGrid(){
     RectTransform gridRectTransform = GetComponent<RectTransform>();
    RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();

     Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        float maxGridWidth = screenSize.x * 0.7f;
        float maxGridHeight = screenSize.y * 0.7f;

        float dotWidth = maxGridWidth / columns - titleSpacing;
        float dotHeigth = maxGridHeight / rows - titleSpacing;

        float dotSize = Mathf.Min(dotWidth, dotHeigth);
        dotRect.sizeDelta = new Vector2(dotSize, dotSize);
    

    float gridWidth = columns * (dotRect.sizeDelta.x + titleSpacing) - titleSpacing;
    float gridHeight = rows * (dotRect.sizeDelta.y + titleSpacing) - titleSpacing;

    
    gridRectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

    gridRectTransform.pivot = new Vector2(0.5f, 0.5f);
    gridRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    gridRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    gridRectTransform.anchoredPosition = Vector2.zero;

    Vector2 startPos = new Vector2(-gridWidth / 2 + dotRect.sizeDelta.x / 2, gridHeight / 2 - dotRect.sizeDelta.y / 2);
        for(int i =0;i<rows;i++){
            for(int j=0;j<columns;j++){
                if(!IsBlockedCell(i,j)){
                     SpawnDot(i,j,startPos);
                }else{
                    CreateBlockedCell(i,j,startPos);
                }
               
            }
        }
    }
    private void SpawnDot(int row, int column,Vector2 startPos){
          GameObject newDot = Instantiate(dotPrefab,transform);
                dotMatrix[row,column] = newDot;
                RectTransform rectTransform = newDot.GetComponent<RectTransform>();
                Vector2 targetPosition = new Vector2(startPos.x+column*(rectTransform.sizeDelta.x + titleSpacing),startPos.y-row*(rectTransform.sizeDelta.y+titleSpacing));
                rectTransform.anchoredPosition = new Vector2(targetPosition.x,startPos.y + (rectTransform.sizeDelta.y + titleSpacing)*rows);
                rectTransform.DOAnchorPos(targetPosition,fallDuration).SetEase(Ease.OutBounce);
                Dot dotComponent = newDot.GetComponent<Dot>();
                dotComponent.row = row;
                dotComponent.column = column;
                AssignDotColor(dotComponent);
    }
    private bool CheckingColour(Dot dot){
       if(selectedDots.Count ==0){
        return true;
       }
        DotType firstDotColor = selectedDots[0].dotType;
        DotType newDotColor = dot.dotType;
        return firstDotColor == newDotColor;
    }
    private bool CheckingNearby(Dot dot){
        if(selectedDots.Count ==0){
        return true;
        }
            Dot  lastSeletDot = selectedDots[selectedDots.Count -1];
            int deltaX = Mathf.Abs(dot.row - lastSeletDot.row);
            int deltaY = Mathf.Abs(dot.column - lastSeletDot.column);
            return deltaX <=1 && deltaY <=1 && !(deltaX==1 && deltaY ==1);
    }
    public void OnDotSelected(Dot dot){
            if(CheckingColour(dot) && CheckingNearby(dot))
            {
                selectedDots.Add(dot);
                 UpdateLineRenderer();
            }  
    }
    public void OnDotExit(Dot dot){
        if(selectedDots.Count >1 && selectedDots[selectedDots.Count -2] == dot){
         selectedDots.RemoveAt(selectedDots.Count-1);
         UpdateLineRenderer();
        }
    }
    public void OnSelectionStart(Dot startDot){
        selectedDots.Clear();
        OnDotSelected(startDot);
    }
    public void OnSelectionEnd(){
        if(selectedDots.Count>=2){
            if(IsClosedLoop()){
                ClearAllDotsOfColor(selectedDots[0].dotType);
            }else{
               HandleSelectedDots();
            }
            OnMoveDone();
        }
        selectedDots.Clear();
        lineRenderer.positionCount = 0;
        if(movesLeft==0 && !requirementBar.trackingCondition){
            LooseUI.SetActive(true);
        }
        if(requirementBar.trackingCondition){
            WinUI.SetActive(true);
        }
    }
    public void OnWinClick(){
        ClearBlockedCells();
        ClearAllDotMatrix();
        levelIndex++;
        WinUI.SetActive(false);
        LoadLevel(levelIndex);
        selectedDots.Clear();
    }
    public void OnLooseClick(){
        ClearBlockedCells();
        ClearAllDotMatrix();
        LooseUI.SetActive(false);
        LoadLevel(levelIndex);
        selectedDots.Clear();
    }
    public void OnClickNext(){
        ClearBlockedCells();
        ClearAllDotMatrix();
        levelIndex++;
        WinUI.SetActive(false);
        LoadLevel(levelIndex);
        selectedDots.Clear();
    }
    public void OnClickPrevious(){
          ClearBlockedCells();
        ClearAllDotMatrix();
        levelIndex--;
        WinUI.SetActive(false);
        LoadLevel(levelIndex);
        selectedDots.Clear();
    }
    public void ClearAllDotMatrix(){
        for(int i =0;i<rows;i++){
            for(int j=0;j<columns;j++){
                 Destroy(dotMatrix[i,j]);
                dotMatrix[i,j]=null;
            }
        }
    }
    private void HandleSelectedDots(){
        requirementBar.UpdateCollectedDots(selectedDots[0].dotType,selectedDots.Count);
        foreach(var dot in selectedDots){
        dotMatrix[dot.row,dot.column] = null;
        Destroy(dot.gameObject);
        }
        FillEmptySpace();   
    }
    private void HandeldLoopSelectedDot(){
        requirementBar.UpdateCollectedDots(selectedDots[0].dotType,selectedDots.Count-1);
         foreach(var dot in selectedDots){
        dotMatrix[dot.row,dot.column] = null;
        Destroy(dot.gameObject);
        }
        FillEmptySpace();   
    }
    public void UpdateLineRenderer(){
        lineRenderer.positionCount = selectedDots.Count+1;
        for(int i =0;i<selectedDots.Count;i++){
            Vector3 dotPosition = selectedDots[i].transform.position;
            dotPosition.z =0;
            lineRenderer.SetPosition(i,dotPosition);
            lineRenderer.startColor = selectedDots[0].color;
            lineRenderer.endColor = selectedDots[0].color;
            lineRenderer.numCapVertices = 2; 
            lineRenderer.numCornerVertices = 5;
        }
        if(selectedDots.Count >0){
          Vector3 lastDotPosition = selectedDots[selectedDots.Count-1].transform.position;
          lastDotPosition.z =0;
          lineRenderer.SetPosition(selectedDots.Count,lastDotPosition);
        } 
}
    private void FillEmptySpace(){
    RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();

    float gridWidth = columns * (dotRect.sizeDelta.x + titleSpacing) - titleSpacing;
    float gridHeight = rows * (dotRect.sizeDelta.y + titleSpacing) - titleSpacing;
    Vector2 startPos = new Vector2(-gridWidth / 2 + dotRect.sizeDelta.x / 2, gridHeight / 2 - dotRect.sizeDelta.y / 2);

    for (int column = 0; column < columns; column++) {
        for (int row = rows - 1; row >= 0; row--) {
            if (dotMatrix[row, column] == null && !IsBlockedCell(row, column)) {
                for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--) {
                    if (dotMatrix[aboveRow, column] != null && !IsBlockedCell(aboveRow, column)) {
                        dotMatrix[row, column] = dotMatrix[aboveRow, column];
                        dotMatrix[aboveRow, column] = null;

                        Dot dotComponent = dotMatrix[row, column].GetComponent<Dot>();
                        dotComponent.row = row;

                        
                        RectTransform dotRectTransform = dotMatrix[row, column].GetComponent<RectTransform>();
                        Vector2 newPosition = new Vector2(
                            startPos.x + column * (dotRect.sizeDelta.x + titleSpacing),
                            startPos.y - row * (dotRect.sizeDelta.y + titleSpacing)
                        );
                        dotRectTransform.DOAnchorPos(newPosition, fallDuration).SetEase(Ease.OutBounce);
                        break;
                    }
                }
            }
        }
        for (int row = 0; row < rows; row++) {
            if (dotMatrix[row, column] == null &&!IsBlockedCell(row, column)) {
                Vector2 newPosition = new Vector2(
                    startPos.x + column * (dotRect.sizeDelta.x + titleSpacing),
                    startPos.y - row * (dotRect.sizeDelta.y + titleSpacing)
                );

                GameObject newDot = Instantiate(dotPrefab, transform);
                RectTransform dotRectTransform = newDot.GetComponent<RectTransform>();

                dotRectTransform.anchoredPosition = new Vector2(newPosition.x, startPos.y + (dotRect.sizeDelta.y + titleSpacing) * rows);

                dotRectTransform.DOAnchorPos(newPosition, 0.5f).SetEase(Ease.OutBounce);

                dotMatrix[row, column] = newDot;
                Dot dotComponent = newDot.GetComponent<Dot>();
                dotComponent.row = row;
                dotComponent.column = column;
                AssignDotColor(dotComponent);
            }
        }
    }
  }
  private bool IsBlockedCell(int row, int column)
{
    foreach (var blockedCell in levelData.blockedCells)
    {
        if (blockedCell.x == row && blockedCell.y == column)
        {
            return true;
        }
    }
    return false;
}

private void CreateBlockedCell(int row, int column, Vector2 startPos)
{
    GameObject newBlock = new GameObject("BlockedCell");
    newBlock.transform.SetParent(transform,false);
    RectTransform rectTransform = newBlock.AddComponent<RectTransform>();
    rectTransform.sizeDelta = dotPrefab.GetComponent<RectTransform>().sizeDelta;
    rectTransform.anchoredPosition = new Vector2(startPos.x + column * (rectTransform.sizeDelta.x + titleSpacing),
                                                 startPos.y - row * (rectTransform.sizeDelta.y + titleSpacing));

    Image blockImage = newBlock.AddComponent<Image>();
    blockImage.sprite = blockCell; // Set the image color to white
}
public void ClearBlockedCells(){
    foreach(var blockedCell in levelData.blockedCells){
        int row = blockedCell.x;
        int column = blockedCell.y;
    foreach (Transform child in transform)
    {
    if(child.name =="BlockedCell"){ 
    RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();
    float gridWidth = columns * (dotRect.sizeDelta.x + titleSpacing) - titleSpacing;
    float gridHeight = rows * (dotRect.sizeDelta.y + titleSpacing) - titleSpacing;
    Vector2 startPos = new Vector2(-gridWidth / 2 + dotRect.sizeDelta.x / 2, gridHeight / 2 - dotRect.sizeDelta.y / 2);

            RectTransform rectTransform = child.GetComponent<RectTransform>();
                Vector2 position = new Vector2(
                    startPos.x + column * (rectTransform.sizeDelta.x + titleSpacing),
                    startPos.y - row * (rectTransform.sizeDelta.y + titleSpacing)
                );

                if (rectTransform.anchoredPosition == position)
                {
                    Destroy(child.gameObject);
                    break;
                }
        }
     }
    }
}
 private bool IsClosedLoop()
{
    Dot lastDot = selectedDots[selectedDots.Count - 1];
    // Check if lastDot is the same as the first or any earlier dot
    return selectedDots.IndexOf(lastDot) != selectedDots.Count - 1 && selectedDots.Count >= 4;
}
private void ClearAllDotsOfColor(DotType dotType)
{
    for (int row = 0; row < rows; row++)
    {
        for (int column = 0; column < columns; column++)
        {
            Dot dot = dotMatrix[row, column]?.GetComponent<Dot>();
            if (dot != null && dot.dotType == dotType)
            {
                if (!selectedDots.Contains(dot))
                {
                    selectedDots.Add(dot); 
                }
            }
        }
    }
    HandeldLoopSelectedDot();
}
  private void AssignDotColor(Dot dot){
    DotType randomDotType = levelData.spawnableDotTypes[Random.Range(0,levelData.spawnableDotTypes.Length)];
    dot.dotType =randomDotType;
    string spriteName;
    if(dotTypeToSpriteNameMap.TryGetValue(randomDotType, out spriteName))
        {
            Sprite dotSprite = dotSpriteAtlas.GetSprite(spriteName);
            dot.GetComponent<Image>().sprite = dotSprite;
        }
  }
    private void AdjustDotSize()
    {
        RectTransform gridRectTransform = GetComponent<RectTransform>();
        RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();

        
        float gridWidth = gridRectTransform.rect.width;
        float gridHeight = gridRectTransform.rect.height;

       
        float dotWidth = (gridWidth - (columns - 1) * titleSpacing) / columns;
        float dotHeight = (gridHeight - (rows - 1) * titleSpacing) / rows;

       
        float newDotSize = Mathf.Min(dotWidth, dotHeight);

       
        dotRect.sizeDelta = new Vector2(newDotSize, newDotSize);
    }
    private void IntilalizeDotTypeToSpriteMap()
    {
        dotTypeToSpriteNameMap = new Dictionary<DotType, string>
         {
            {DotType.Red,"egg_1" },
            {DotType.Blue,"egg_2" },
            {DotType.Yellow,"egg_3" },
            {DotType.Pink, "egg_4" },
        };
    }
}