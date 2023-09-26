using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class DragManager : MonoBehaviour
{
    public DraggablePreview DragEvent;

    public CustomizationManager c;

    public void StartDrag(GameObject draggedObject) {
        DragEvent = draggedObject.GetComponent<DraggablePreview>();
    }

    public void FinishDrag() {

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        List<GameObject> editableElements = new List<GameObject>();
        DashboardDisplay display = null;

        foreach (RaycastResult r in raycastResults) {

            DashboardDisplay d = r.gameObject.GetComponent<DashboardDisplay>();

            if (d != null) {
                display = d;
                if (!d.isEnabled) {
                    Debug.Log("PROFILE IS INACTIVE");
                    return;
                } else {

                    if (DragEvent.draggedAffinity) {
                        d.AddAffinity(DragEvent.GetComponent<Affinity>().affinityName, false);
                        c.LogChange();
                        return;
                    }

                    continue;
                }
            } else {
                if (r.gameObject.GetComponent<EditableElement>() != null) {
                    editableElements.Add(r.gameObject);
                }
            }
        }

        if (editableElements.Count == 0 || display == null) {
            if (editableElements.Count == 0) {
                Debug.Log("NO EDITABLE ELEMENTS");
            } else if (display == null) {
                Debug.Log("NO Dashboard Display found");
            }
            return;
        }
        editableElements = editableElements.OrderBy(e => e.GetComponent<EditableElement>().Layer).ToList();

        GameObject top = editableElements.Last();
        if (DragEvent.draggedImage != -1 && (top.GetComponent<EditableElement>().type == "bio" || top.GetComponent<EditableElement>().type == "username")) {
            editableElements.RemoveAt(editableElements.Count - 1);
            if (editableElements.Count > 0) {
                top = editableElements.Last();
            } else {
                return;
            }
        }
        ChangeData(top, display);
    }

    protected void ChangeData(GameObject toChange, DashboardDisplay display) {
        c.LogChange();

        EditableElement e = toChange.GetComponent<EditableElement>();

        if (DragEvent.draggedColor) {
            Color c = DragEvent.GetComponent<Image>().color;
            if (e.type == "bio") {
                display.bio.color = c; 
            } else if (e.type == "username") {
                display.username.color = c;
            } else if (e.type == "background") {
                display.dashboardBackground.color = c; 
                display.dashboardBackground.sprite = null;
            } else if (e.type == "profile") {
                display.profilePicture.color = c; 
                display.profilePicture.sprite = null;
            } else if(e.type == "header") {
                display.header.color = c; 
                display.header.sprite = null;
            }
        }
        
        if (DragEvent.draggedImage != -1) {

            toChange.GetComponent<Image>().color = Color.white;

            if (e.type == "background") {
                display.curData.backgroundImage = DragEvent.draggedImage;
                display.LoadLocalImages(e.type);

            } else if (e.type == "profile") {
                display.curData.profileImage = DragEvent.draggedImage;
                display.LoadLocalImages(e.type);
                
                toChange.GetComponent<Image>().SetNativeSize();
                toChange.GetComponent<DraggableMaskedImage>().UpdateAutoSize();
                toChange.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                toChange.GetComponent<DraggableMaskedImage>().UpdateAutoSize();
                LayoutRebuilder.MarkLayoutForRebuild(toChange.GetComponent<RectTransform>());
            } else if (e.type == "header") {
                display.curData.headerImages[0] = DragEvent.draggedImage;
                display.LoadLocalImages(e.type);
                
                toChange.GetComponent<Image>().SetNativeSize();
                toChange.GetComponent<DraggableMaskedImage>().UpdateAutoSize();
                toChange.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                toChange.GetComponent<DraggableMaskedImage>().UpdateAutoSize();
                LayoutRebuilder.MarkLayoutForRebuild(toChange.GetComponent<RectTransform>());
            }
        }
        Debug.Log(toChange);
    }

}
