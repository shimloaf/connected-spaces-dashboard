using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class TestManager : MonoBehaviour
{
    public DashboardDisplay diu;

    public TMP_InputField pathInputField;
    public TMP_InputField valueInputField;

    public void UpdateDashboard() {
        diu.UpdateDisplay();
    }

    public void PostJSON() => FirebaseDatabase.PostJSON(pathInputField.text, valueInputField.text, gameObject.name,
            "DisplayInfo", "DisplayErrorObject");

    public void DisplayInfo(string info)
    {
        Debug.Log(info);
    }

    public void DisplayErrorObject(string error)
        {
            var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
            DisplayError(parsedError.message);
        }

        public void DisplayError(string error)
        {
            Debug.LogError(error);
        }
}
