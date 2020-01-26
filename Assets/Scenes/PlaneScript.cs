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

        if(placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && objectPlaced) {
            RemoveObject();
        }
    }

    private void RemoveObject()
    {
        Destroy(objectToPlace);
        objectPlaced = false;
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
        objectPlaced = true;
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