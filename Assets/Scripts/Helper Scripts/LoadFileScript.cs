using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
 
public class LoadFileScript : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void ImageUploaderCaptureClick();

    public CustomizationManager c;
 
    IEnumerator LoadTexture (string url) {
        WWW image = new WWW (url);
        yield return image;
        Texture2D texture = new Texture2D (1, 1);
        image.LoadImageIntoTexture (texture);
        Debug.Log ("Loaded image size: " + texture.width + "x" + texture.height);

        DashboardData.currentUserDashboard.userPictures.Add(DashboardData.currentUserDashboard.userPictures.Count, texture);
        c.AddImage(texture, DashboardData.currentUserDashboard.userPictures.Count - 1);
        c.LogChange();
    }
 
    void FileSelected (string url) {
        StartCoroutine(LoadTexture (url));
    }
 
    public void OnButtonPointerDown () {
        #if UNITY_EDITOR
                string path = UnityEditor.EditorUtility.OpenFilePanel("Open image","","jpg,png,bmp");
                if (!System.String.IsNullOrEmpty (path))
                    FileSelected ("file:///" + path);
        #else
                ImageUploaderCaptureClick ();
        #endif
    }
}