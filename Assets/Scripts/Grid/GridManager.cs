using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TwodDotUIManager;
using TwoDotsDot;
using TwoDotsRequirementBar;
using TwoDotsLevelList;
using TwoDotsLevelData;


namespace TwoDotGridManager 
{
    public class GridManager : MonoBehaviour
    {
        [Header("Level Data")]
        [SerializeField] public int columns;
        [SerializeField] public int rows;
        [SerializeField] public float titleSpacing = 1f;
        [SerializeField] private LevelList levelList;
        [SerializeField] public LevelData levelData;

        [Header("Dot")]
        [SerializeField] public Dot dotPrefab;
        [SerializeField] public Dot[,] dotMatrix;
        public Dot lastRemovedDot;
        [SerializeField] public List<Dot> selectedDots = new List<Dot>();
        public HashSet<(Dot, Dot)> drawnConnections = new HashSet<(Dot, Dot)>();

        [Header("UI")]
        [SerializeField] public GameObject WinUI;
        [SerializeField] public GameObject LooseUI;
        [SerializeField] public Requirementbar requirementBar;
        [SerializeField] public int movesLeft;
        [SerializeField] public int levelIndex;

        [Header("Line Renderer")]
        [SerializeField] LineRenderer linePrefab;
        [SerializeField] List<LineRenderer> lineRenderers = new List<LineRenderer>();
        [SerializeField] LineRenderer currentLineRender;

        [Header("Sprite")]
        [SerializeField] private SpriteAtlas dotSpriteAtlas;
        [SerializeField] private Dictionary<DotType, string> dotTypeToSpriteNameMap;
        [SerializeField] public Sprite blockCell;

        [Header("DoTweenDuration")]
        [SerializeField] private float fallDuration = 0.5f;

        [Header("Manager")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private LevelManager levelManager;

        private void Start()
        {
            Input.multiTouchEnabled = false;
            IntilalizeDotTypeToSpriteMap();
            //linePrefab.positionCount = 2;
            linePrefab.startWidth = 0.1f;
            linePrefab.widthMultiplier = 1f;
            linePrefab.material = new Material(Shader.Find("Sprites/Default"));
            levelIndex = 0;
            dotMatrix = new Dot[rows, columns];
            LoadLevel(levelIndex);
        }
        private void Update()
        {
            if (selectedDots.Count > 0)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                if (currentLineRender != null)
                {
                    currentLineRender.SetPosition(1, mousePosition);
                }
                else
                {
                    return;
                }
            }
        }

        #region LoadLevel
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levelList.levels.Length)
            {
                return;
            }
            levelData = levelList.levels[levelIndex];
            rows = levelList.levels[levelIndex].rows;
            columns = levelList.levels[levelIndex].columns;
            dotMatrix = new Dot[rows, columns];
            ApplyLevelData();
            requirementBar.SetRequirement(levelData);
            uiManager.moveLeft = levelData.numberOfMoves;
        }
        private void ApplyLevelData()
        {
            AdjustDotSize();
            CreateGrid();
        }
        #endregion


        #region CreateGrid and Spawn Dot
        private void CreateGrid()
        {
            RectTransform gridRectTransform = GetComponent<RectTransform>();
            RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();

            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            float maxGridWidth = screenSize.x * 0.6f;
            float maxGridHeight = screenSize.y * 0.6f;

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
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!IsBlockedCell(i, j))
                    {
                        SpawnDot(i, j, startPos);
                    }
                    else
                    {
                        CreateBlockedCell(i, j, startPos);
                    }

                }
            }
        }

        private void SpawnDot(int row, int column, Vector2 startPos)
        {
            Dot newDot = Instantiate(dotPrefab, transform);
            dotMatrix[row, column] = newDot;
            RectTransform rectTransform = newDot.GetComponent<RectTransform>();
            Vector2 targetPosition = new Vector2(startPos.x + column * (rectTransform.sizeDelta.x + titleSpacing), startPos.y - row * (rectTransform.sizeDelta.y + titleSpacing));
            rectTransform.anchoredPosition = new Vector2(targetPosition.x, startPos.y + (rectTransform.sizeDelta.y + titleSpacing) * rows);
            rectTransform.DOAnchorPos(targetPosition, fallDuration).SetEase(Ease.OutBounce);
            Dot dotComponent = newDot.GetComponent<Dot>();
            dotComponent.row = row;
            dotComponent.column = column;
            AssignDotColor(dotComponent);
        }
        private void FillEmptySpace()
        {
            RectTransform dotRect = dotPrefab.GetComponent<RectTransform>();

            float gridWidth = columns * (dotRect.sizeDelta.x + titleSpacing) - titleSpacing;
            float gridHeight = rows * (dotRect.sizeDelta.y + titleSpacing) - titleSpacing;
            Vector2 startPos = new Vector2(-gridWidth / 2 + dotRect.sizeDelta.x / 2, gridHeight / 2 - dotRect.sizeDelta.y / 2);

            for (int column = 0; column < columns; column++)
            {
                for (int row = rows - 1; row >= 0; row--)
                {
                    if (dotMatrix[row, column] == null && !IsBlockedCell(row, column))
                    {
                        for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--)
                        {
                            if (dotMatrix[aboveRow, column] != null && !IsBlockedCell(aboveRow, column))
                            {
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
                for (int row = 0; row < rows; row++)
                {
                    if (dotMatrix[row, column] == null && !IsBlockedCell(row, column))
                    {
                        Vector2 newPosition = new Vector2(
                            startPos.x + column * (dotRect.sizeDelta.x + titleSpacing),
                            startPos.y - row * (dotRect.sizeDelta.y + titleSpacing)
                        );
                        Dot newDot = Instantiate(dotPrefab, transform);
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
        #endregion

        #region Selection Dot
        public void OnSelectionStart(Dot startDot)
        {
            drawnConnections.Clear();
            selectedDots.Clear();
            foreach (LineRenderer item in lineRenderers)
            {
                Destroy(item.gameObject);
            }
            lineRenderers.Clear();
            currentLineRender.positionCount = 2;
            linePrefab.startColor = startDot.color;
            linePrefab.endColor = startDot.color;
            OnDotSelected(startDot);

        }
        public void OnSelectionEnd()
        {
            if (selectedDots.Count >= 2)
            {
                if (IsClosedLoop())
                {
                    ClearAllDotsOfColor(selectedDots[0].dotType);
                }
                else
                {
                    HandleSelectedDots();
                }
                uiManager.OnMoveDone();
                foreach (LineRenderer item in lineRenderers)
                {
                    Destroy(item.gameObject);
                }
            }
            foreach (LineRenderer line in lineRenderers)
            {
                Destroy(line.gameObject);
            }
            lineRenderers.Clear();
            selectedDots.Clear();
            if (uiManager.moveLeft == 0 && !requirementBar.trackingCondition)
            {
                LooseUI.SetActive(true);
            }
            if (requirementBar.trackingCondition)
            {
                WinUI.SetActive(true);
            }
        }
        #endregion

        #region OnDot
        public void OnDotSelected(Dot dot)
        {
            if (selectedDots.Count > 0)
            {
                Dot lastSelectedDot = selectedDots[selectedDots.Count - 1];


                if (!drawnConnections.Contains((lastSelectedDot, dot)) &&
                    !drawnConnections.Contains((dot, lastSelectedDot)))
                {
                    drawnConnections.Add((lastSelectedDot, dot));
                    if (CheckingColour(dot))
                    {
                        CreateLineBetweenDots(dot, lastSelectedDot);
                        selectedDots.Add(dot);
                        currentLineRender = null;
                        AddLineRenderer(dot);

                    }
                }
            }
            else
            {
                selectedDots.Add(dot);
                AddLineRenderer(dot);
            }
        }
        public void OnDotExit(Dot dot)
        {
            if (selectedDots.Count > 1 && selectedDots[selectedDots.Count - 2] == dot)
            {
                selectedDots.RemoveAt(selectedDots.Count - 1);

            }
        }
        #endregion

        #region HandleSelectedDot
        private void HandleSelectedDots()
        {
            requirementBar.UpdateCollectedDots(selectedDots[0].dotType, selectedDots.Count);
            foreach (var dot in selectedDots)
            {
                dotMatrix[dot.row, dot.column] = null;
                Destroy(dot.gameObject);
            }
            foreach (LineRenderer line in lineRenderers)
            {
                Destroy(line.gameObject);
            }
            lineRenderers.Clear();
            FillEmptySpace();
        }
        private void HandeldLoopSelectedDot()
        {
            requirementBar.UpdateCollectedDots(selectedDots[0].dotType, selectedDots.Count - 1);
            foreach (var dot in selectedDots)
            {
                dotMatrix[dot.row, dot.column] = null;
                Destroy(dot.gameObject);
            }
            foreach (LineRenderer line in lineRenderers)
            {
                Destroy(line.gameObject);
            }
            lineRenderers.Clear();
            FillEmptySpace();
        }
        #endregion

        #region BlockCell
        private void CreateBlockedCell(int row, int column, Vector2 startPos)
        {
            GameObject newBlock = new GameObject("BlockedCell");
            newBlock.transform.SetParent(transform, false);
            RectTransform rectTransform = newBlock.AddComponent<RectTransform>();
            rectTransform.sizeDelta = dotPrefab.GetComponent<RectTransform>().sizeDelta;
            rectTransform.anchoredPosition = new Vector2(startPos.x + column * (rectTransform.sizeDelta.x + titleSpacing),
                                                         startPos.y - row * (rectTransform.sizeDelta.y + titleSpacing));

            Image blockImage = newBlock.AddComponent<Image>();
            blockImage.sprite = blockCell; // Set the image color to white
        }

        public void ClearBlockedCells()
        {
            foreach (var blockedCell in levelData.blockedCells)
            {
                int row = blockedCell.x;
                int column = blockedCell.y;
                foreach (Transform child in transform)
                {
                    if (child.name == "BlockedCell")
                    {
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
        #endregion

        #region LineRenderer
        public void CreateLineBetweenDots(Dot startdot, Dot endDot)
        {
            currentLineRender.positionCount = 2;
            startdot.transform.position = new Vector3(startdot.transform.position.x, startdot.transform.position.y, 0);
            endDot.transform.position = new Vector3(endDot.transform.position.x, endDot.transform.position.y, 0);
            currentLineRender.SetPosition(0, startdot.transform.position);
            currentLineRender.SetPosition(1, endDot.transform.position);
        }
        public void RemoveLastLine()
        {
            if (lineRenderers.Count > 0)
            {
                Debug.Log("jumphere");
                LineRenderer lastLine = lineRenderers[lineRenderers.Count - 1];
                LineRenderer lastLine2nd = lineRenderers[lineRenderers.Count - 2];
                lineRenderers.RemoveAt(lineRenderers.Count - 1);
                Destroy(lastLine.gameObject);
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                currentLineRender = lastLine2nd;
                currentLineRender.SetPosition(0, selectedDots[selectedDots.Count - 1].transform.position);
            }
        }
        public void AddLineRenderer(Dot dot)
        {
            currentLineRender = Instantiate(linePrefab);
            currentLineRender.startColor = dot.color;
            currentLineRender.endColor = dot.color;
            dot.transform.position = new Vector3(dot.transform.position.x, dot.transform.position.y, 0);
            currentLineRender.SetPosition(0, dot.transform.position);
            lineRenderers.Add(currentLineRender);
        }
        #endregion

        private bool CheckingColour(Dot dot)
        {
            if (selectedDots.Count == 0)
            {
                return true;
            }
            DotType firstDotColor = selectedDots[0].dotType;
            DotType newDotColor = dot.dotType;
            return firstDotColor == newDotColor;
        }

        public void RemoveLastDot()
        {
            if (selectedDots.Count > 0)
            {
                if (selectedDots.Count > 1)
                {
                    Dot lastDot = selectedDots[selectedDots.Count - 1];
                    Dot secondLastDot = selectedDots[selectedDots.Count - 2];

                    drawnConnections.Remove((secondLastDot, lastDot));
                    drawnConnections.Remove((lastDot, secondLastDot));
                    selectedDots.RemoveAt(selectedDots.Count - 1);
                    RemoveLastLine();

                }
            }
        }
        public void OnWinClick()
        {
            ClearBlockedCells();
            ClearAllDotMatrix();
            levelIndex++;
            LoadLevel(levelIndex);
            uiManager.ClosePopUp();
            selectedDots.Clear();
        }
        public void OnLooseClick()
        {
            ClearBlockedCells();
            ClearAllDotMatrix();
            LoadLevel(levelIndex);
            uiManager.ClosePopUp();
            selectedDots.Clear();
        }
        public void ClearAllDotMatrix()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (dotMatrix[i, j] != null)
                    {
                        Destroy(dotMatrix[i, j].gameObject);
                        dotMatrix[i, j] = null;
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
        private void AssignDotColor(Dot dot)
        {
            DotType randomDotType = levelData.spawnableDotTypes[Random.Range(0, levelData.spawnableDotTypes.Length)];
            dot.dotType = randomDotType;
            string spriteName;
            if (dotTypeToSpriteNameMap.TryGetValue(randomDotType, out spriteName))
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
            {DotType.Green,"green" },
            {DotType.BabyBlueEyes,"baby blue eyes" },
            {DotType.Brown,"brown" },
            {DotType.CersizePink,"cersize pink" },
            {DotType.Chatreuse,"chartreuse" },
            {DotType.CherryBlossomPink,"cherry blossom pink" },
            {DotType.Chocolate,"chocolate"},
            {DotType.DarkMagneta,"dark magneta"},
            {DotType.DiscoBallBlue,"disco" },
            {DotType.Gray,"gray" },
            {DotType.Indigo,"idingo" },
            {DotType.Orange,"orange"},
            {DotType.OxfordBlue,"oxford blue"},
            {DotType.PerrasinGreen,"perrasin" },
            {DotType.PhtaloBlue,"phtalo blue"},
            {DotType.Raspberry,"raspberry" },
            {DotType.TyrianPurple,"tyrian purple"},
        };
        }
        public Dot GetDotAt(int row, int column)
        {
            if (row >= 0 && row < dotMatrix.GetLength(0) && column >= 0 && column < dotMatrix.GetLength(1))
            {
                return dotMatrix[row, column];
            }
            return null;
        }
    }
}
