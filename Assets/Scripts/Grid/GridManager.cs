using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
   [SerializeField]public int columns = 6;
   [SerializeField]public int rows = 6;
   [SerializeField]public float titleSpacing=10f;

   [SerializeField] public GameObject dotPrefab;
   [SerializeField] public GameObject[,] dotMatrix;

   [SerializeField] public GameObject WinUI;
   [SerializeField] public GameObject LooseUI;

    [SerializeField] public List<Dot> selectedDots = new List<Dot>();

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LevelList levelList;
    [SerializeField] public LevelData levelData;
    [SerializeField] public Requirementbar requirementBar;
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] public int movesLeft;
    [SerializeField] public int levelIndex;
    
    private void Start() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
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
     ApplyLevelData();
     requirementBar.SetRequirement(levelData);
     movesLeft = levelData.numberOfMoves;
   }
   private void ApplyLevelData(){
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
                SpawnDot(i,j,startPos);
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
               // dotComponent.dotType
                AssignDotColor(dotComponent);
    }
    private bool CheckingColour(Dot dot){
       if(selectedDots.Count ==0){
        return true;
       }
        Color firstDotColor = selectedDots[0].color;
        Color newDotColor = dot.color;
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
        if(!selectedDots.Contains(dot)){
            if(CheckingColour(dot) && CheckingNearby(dot))
            {
                selectedDots.Add(dot);
                 UpdateLineRenderer();
            }  
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
            HandleSelectedDots();
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
         ClearAllDotMatrix();
        levelIndex++;
        WinUI.SetActive(false);
        LoadLevel(levelIndex);
        selectedDots.Clear();
    }
    public void OnLooseClick(){
        ClearAllDotMatrix();
        LooseUI.SetActive(false);
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
            if (dotMatrix[row, column] == null) {
                for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--) {
                    if (dotMatrix[aboveRow, column] != null) {
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
            if (dotMatrix[row, column] == null) {
                Vector2 newPosition = new Vector2(
                    startPos.x + column * (dotRect.sizeDelta.x + titleSpacing),
                    startPos.y - row * (dotRect.sizeDelta.y + titleSpacing)
                );

                GameObject newDot = Instantiate(dotPrefab, transform);
                RectTransform dotRectTransform = newDot.GetComponent<RectTransform>();

                // Set the initial position (off-screen or above the grid)
                dotRectTransform.anchoredPosition = new Vector2(newPosition.x, startPos.y + (dotRect.sizeDelta.y + titleSpacing) * rows);

                // Animate the dot to its target position
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
  private void AssignDotColor(Dot dot){
    DotType randomDotType = levelData.spawnableDotTypes[Random.Range(0,levelData.spawnableDotTypes.Length)];
    Debug.Log(randomDotType);
    dot.color = DotTypeColor(randomDotType);
    dot.dotType =randomDotType;
    dot.GetComponent<Image>().color = dot.color;
  }
  private Color DotTypeColor(DotType dotType){
         switch (dotType)
        {
            case DotType.Red: return Color.red;
            case DotType.Green: return Color.green;
            case DotType.Blue: return Color.blue;
            case DotType.Yellow: return Color.yellow;
            case DotType.Gray: return Color.gray;
            case DotType.Pink: return new Color(1f,0.1f,1f);
            case DotType.Orange: return new Color(1f,0.5f,0f);
            case DotType.PerrasinGreen: return new Color(0f,0.6f,0.6f);
            default: return Color.white;
        }
    }
}