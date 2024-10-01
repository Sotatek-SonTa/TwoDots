using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using TwoDotGridManager;

namespace TwoDotsDot
{
    public class Dot : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        [Header("Attribute")]
        public Color color;
        public DotType dotType;
        public int row;
        public int column;
        private Image spriteRenderer;

        [Header("NeighboorDot")]
        public Dot up;
        public Dot right;
        public Dot down;
        public Dot left;

        [SerializeField] private GridManager gridManager;
        [SerializeField] private SpriteAtlas spriteAtlas;
        private Dictionary<DotType, string> dotTypeToSpriteName;
        public void OnPointerDown(PointerEventData eventData)
        {
            gridManager.OnSelectionStart(this);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (gridManager.selectedDots.Count >= 2)
            {
                Dot lastSelected = gridManager.selectedDots[gridManager.selectedDots.Count - 1];
                Dot secondLastSelected = gridManager.selectedDots[gridManager.selectedDots.Count - 2];
                if (this == secondLastSelected)
                {

                    gridManager.RemoveLastDot();
                }
                else if (IsAdjacent(lastSelected))
                {
                    gridManager.OnDotSelected(this);
                }
            }
            else if (gridManager.selectedDots.Count == 1)
            {
                Dot lastSelected = gridManager.selectedDots[gridManager.selectedDots.Count - 1];
                if (this == lastSelected)
                {
                    gridManager.RemoveLastDot();
                }
                else if (IsAdjacent(lastSelected))
                {
                    gridManager.OnDotSelected(this);
                }
            }
            else
            {
                gridManager.OnDotSelected(this);
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            gridManager.OnSelectionEnd();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            // gridManager.OnDotExit(this);
        }
        private void Awake()
        {
            dotTypeToSpriteName = new Dictionary<DotType, string>
        {
            {DotType.Red,"egg_1" },
            {DotType.Blue,"egg_2" },
            {DotType.Yellow,"egg_3" },
            {DotType.Pink, "egg_4" },
            {DotType.Green, "green" },
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
        private void Start()
        {

            spriteRenderer = GetComponent<Image>();
            gridManager = FindAnyObjectByType<GridManager>();
            color = DotTypeColor(dotType);
            InitializeNeighbors();
        }
        private void Update()
        {
            InitializeNeighbors();
        }
        DotType GetRandomDotType()
        {
            return (DotType)Random.Range(0, System.Enum.GetValues(typeof(DotType)).Length);
        }
        void AssignSpriteFromAtlas(DotType type)
        {
            if (dotTypeToSpriteName.ContainsKey(type))
            {
                Sprite dotSprite = spriteAtlas.GetSprite(dotTypeToSpriteName[type]);
                if (dotSprite != null)
                {
                    spriteRenderer.sprite = dotSprite;
                }
            }
        }
        public DotType GetDotType()
        {
            return dotType;
        }
        private bool IsAdjacent(Dot lastDot)
        {
            if (lastDot.up == this || lastDot.down == this || lastDot.left == this || lastDot.right == this)
            {
                return true;
            }
            return false;
        }
        public void InitializeNeighbors()
        {
            up = gridManager.GetDotAt(row - 1, column);
            down = gridManager.GetDotAt(row + 1, column);
            left = gridManager.GetDotAt(row, column - 1);
            right = gridManager.GetDotAt(row, column + 1);
            if (up == null || down == null || left == null || right == null)
            {
                return;
            }
        }
        private Color DotTypeColor(DotType dotType)
        {
            switch (dotType)
            {
                case DotType.Red: return Color.red;
                case DotType.Green: return Color.green;
                case DotType.Blue: return Color.blue;
                case DotType.Yellow: return Color.yellow;
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
                default: return Color.white;
            }
        }
    }
}
public enum DotType
{
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
