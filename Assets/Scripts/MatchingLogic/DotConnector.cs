using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotConnector : MonoBehaviour
{
    [SerializeField]private List<Dot> selectedDots = new List<Dot>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToViewportPoint(Input.mousePosition),Vector2.zero,10,0);
            Debug.Log(hit.collider);
            if(hit.collider != null){
                Dot dot = hit.collider.GetComponent<Dot>();
                if(dot != null){
                    SelectDot(dot);
                }
            }
        
        }
    }
    void SelectDot(Dot dot){
      if(selectedDots.Count > 0 && selectedDots[selectedDots.Count -1].color != dot.color){
        return;
      }
      selectedDots.Add(dot);
      dot.transform.localScale *=1.2f;
    }
}
