using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class HelpRequestButton : MonoBehaviour
{
    public TMP_Dropdown typeDropdown;
    public Image ButtonImage;
    public Button button;

    public GameObject requestTypeDropdown;
    public GameObject activeRequestText;

    public int pressed = 0;

    void Start() {
        ListenForValueChanged();
    }
    
    public void ListenForValueChanged() =>
        FirebaseDatabase.ListenForValueChanged("helpButton/request", gameObject.name, "CheckButtonStatus", "LogError");

    public void CheckButtonStatus() =>
        FirebaseDatabase.GetJSON("helpButton", gameObject.name, "ButtonDataReceived", "LogError");

    public void ButtonDataReceived(string JSONResult) {
        HelpRequest request = (HelpRequest) Deserialize(typeof(HelpRequest), JSONResult);
        curButtonStatus = request.request;
        if (pressed == 0) {
            SetButtonStatus();
        } else {
            pressed = pressed - 1;
        }
    }

    public void SetButtonStatus() {
        
        button.interactable = true;
        requestTypeDropdown.SetActive(false);
        activeRequestText.SetActive(false);
        
        if (curButtonStatus) {
            activeRequestText.SetActive(true);
            ButtonImage.color = Color.green;
        } else {
            requestTypeDropdown.SetActive(true);
            ButtonImage.color = Color.red;
        }
    }

    public bool curButtonStatus = false;

    public void HelpButtonPressed() {
        button.interactable = false;
        requestTypeDropdown.SetActive(false);
        activeRequestText.SetActive(false);
        ButtonImage.color = new Color(0.14f, 0.11f, 0.11f);
        pressed = 2;
        TryMakeHelpRequest();
    }

    public void TryMakeHelpRequest() =>
        FirebaseDatabase.GetJSON("helpButton", gameObject.name, "ButtonResponse", "LogError");

    public void ButtonResponse(string JSONResult) {
        HelpRequest request = (HelpRequest) Deserialize(typeof(HelpRequest), JSONResult);
        curButtonStatus = request.request;
        MakeHelpRequest(!curButtonStatus);
    }

    public void MakeHelpRequest(bool request) {

        HelpRequest h = new HelpRequest();
        h.request = request;
        h.affinity = typeDropdown.value + 1;
        h.location = DashboardData.currentUserDashboard.currentLocation;

        string jsonString = Serialize(typeof(HelpRequest), (object) h);
        FirebaseDatabase.PostJSON("helpButton", jsonString, gameObject.name, "UploadSuccess", "UploadError");

        string jsonString2 = "";
        if (request) {
            jsonString2 = Serialize(typeof(string), (object) DashboardData.locationNumbers[h.location] + " made a virtual help request asking about " + DashboardData.affinityTypeNumbers[h.affinity]);
        } else {
            jsonString2 = Serialize(typeof(string), (object) DashboardData.locationNumbers[h.location] + " fulfilled a help request virtually");
        }
        FirebaseDatabase.PostJSON("history/" + (System.DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"), jsonString2, gameObject.name, "UploadSuccess", "UploadError");
    }

    void UploadSuccess(string result) {
        Debug.Log(result);
    }

    void UploadError(string error) {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        Debug.Log(parsedError);
    }

    private static readonly fsSerializer _serializer = new fsSerializer();

    public static object Deserialize(Type type, string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }

    public static string Serialize(Type type, object value) {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }
}
