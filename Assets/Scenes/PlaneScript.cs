using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;

using System;

public class PlaneScript : MonoBehaviour
{
    public GameObject objectToPlace;

    public GameObject objRef;
    private bool objectPlaced = false;
    public GameObject placementIndicator;
    private ARRaycastManager arOrigin;
    private Pose PlacementPose;
    private bool placementPoseIsValid = false;
    void Start()
    {
        
        
        arOrigin = FindObjectOfType<ARRaycastManager>();
    
    }
     
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !objectPlaced)
        {
            PlaceObject();
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && objectPlaced) {
            RemoveObject();
        }
    }

    private void RemoveObject()
    {
        Destroy(objRef);
        objectPlaced = false;

    }

    private IEnumerator PlaceObject()
    {
        objRef = Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
        objectPlaced = true;

        string url = "https://cdn.apps.playnetwork.com/master/65e2c70ba416c00cf710bc3dc698c38108c19ccf778aabe36098d8d1d225fb5a.ogg?Signature=BknX0Q6lPu58UV6OaH01DqP4PpzsxpkY7XuFh8J5JL6x0DolJ-vgBzEY6TJlNU-mGeRiwxZ4oBn-ihrc~CRMwZqlxV8b0q7NIfY38J6Se8kWG0Am-E0s-9ZrjI~zTR4RDGgIKkYLfNF8G9WwrIFY~7BEkq2u4ombuuV6XiFyOY-lnSHYFDt7MNS6mjPlQRQZfGCBRzI0nuYvOL0ZJpH9rT4U9SR791cdpNJE27pDxYlyt6gSKh0Sg5Aa98sVLBnnyetFaTRdZbWNd8Tbdpni-uuxwKei25Sk9aPQz08gWoplUtYRFzP36aazgotetVBAWrO5EicVKbPd79zxrlaVtA__&Key-Pair-Id=APKAJ4GOPJEICF5TREYA&Expires=1580025515";
        GameObject Musique = GameObject.Find("pb-MergedObject-67100");
        AudioSource audioSource = Musique.GetComponent<AudioSource>();
        WWW music = new WWW(url);
        yield return music;
        AudioClip lamusic = music.GetAudioClipCompressed(true, AudioType.OGGVORBIS);
        audioSource.clip = lamusic;
        audioSource.Play();
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