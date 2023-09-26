using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggablePreview : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    
    protected Vector3 initialPosition;
    protected RectTransform display;
    protected Canvas canvas;
    protected DragManager dragManager;

    public int draggedImage = -1;
    public bool draggedColor = false;
    public bool draggedAffinity = false;

    void Start() {
        canvas = GetComponentsInParent<Canvas>()[0];
        dragManager = GetComponentInParent<DragManager>();
        display = GetComponent<RectTransform>();
        initialPosition = display.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        dragManager.StartDrag(this.gameObject);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        display.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        display.anchoredPosition = initialPosition;
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
        dragManager.FinishDrag();
    }

}
