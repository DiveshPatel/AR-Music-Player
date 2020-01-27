using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;


public class PlaneScript : MonoBehaviour
{
    public GameObject objectToPlace;
    public string URL = "https://file-examples.com/wp-content/uploads/2017/11/file_example_OOG_1MG.ogg";
    public AudioSource audioSource;
    public GameObject objRef;
    private bool objectPlaced;
    public GameObject placementIndicator;
    private ARRaycastManager arOrigin;
    private Pose PlacementPose;
    private bool placementPoseIsValid;
    void Start()
    {


        arOrigin = FindObjectOfType<ARRaycastManager>();
        StartCoroutine(GetAudioClip());

    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !objectPlaced)
        {
            PlaceObject();
            audioSource.Play();

        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && objectPlaced) {
            RemoveObject();
            audioSource.Stop();
        }
    }

    private void RemoveObject()
    {
        Destroy(objRef);
        objectPlaced = false;

    }

    private void PlaceObject()
    {
        objRef = Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
        objectPlaced = true;
    }


    IEnumerator GetAudioClip()
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(URL, AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            audioSource.clip = clip;

        }
    }

    private void UpdatePlacementIndicator()
    {

        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);


        placementPoseIsValid = hits.Count > 0;
              
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
                            
    }
}