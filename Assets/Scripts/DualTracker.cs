using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class DualTracker : MonoBehaviour, ITrackableEventHandler
{
    public AudioSource audioSource;
    public float volume = 0.5f;

    public bool trackingState;

    public enum Orientation
    {
        horizonal,
        vertical
    }

    public Orientation currentRotation; // this can only be the value of horizontal or vertical results 

    public Transform arCamera;// a reference to our AR camera
    public TrackableBehaviour trackableBehaviour; // a reference to our AR marker script

    private Transform rotationalHelper; // a runtime transform create to assist with the determining the orientation of the marker.

    public float trackerthreshold = 0.6f; // this determines when the marker is horizontal, or vertical

    public Text debugDeviceAngle; // a piece text used to show what angle device is.
    public Text debugMarkerOrientation; // a piece of text used to show what orientation the marker currently is.

    public bool trackerFound = false; // have we dected a tracker?

    public float currentAngleCompared; // this is going to be the current value of our angle comparions.


    public GameObject gameWorld; // a reference to our gameworld.

    public Transform verticalTracker; // vertical tracker reference
    public Transform horizontalTracker; // horizontal tracker reference 

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        // if the reference has been assigned
        if (trackableBehaviour != null)
        {
            trackableBehaviour.RegisterTrackableEventHandler(this);
        }

        rotationalHelper = new GameObject(trackableBehaviour.TrackableName.ToString() + " Rotation Helper").transform; /// we are creating an empty gameobject at run time, and giving it the same name as the tracker
        rotationalHelper.position = Vector3.zero; // reset its position to 0,0,0
        rotationalHelper.rotation = Quaternion.identity; // resets it's rotation to 0,0,0,0
        OnTrackingLost();         
        gameWorld.SetActive(true);        
    }


    void Update()
    {
        if (debugDeviceAngle != null)
        {
            debugDeviceAngle.text = "Current Angle: " + currentAngleCompared;// set the text to the angle
        }
        if(debugMarkerOrientation != null)
        { 
            debugMarkerOrientation.text = currentRotation.ToString(); // set the text to the rotation 
        }
    }

    /// <summary>
    /// A vuforia function we need to implemenet to detect changes with the markers.
    /// </summary>
    /// <param name="previousStatus"></param>
    /// <param name="newStatus"></param>
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        // checking the current status of the marker.
        if(newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if(trackerFound == false)
            {
                trackerFound = true;
                OnTrackingFound(); // we got tracking
            }
        }
        else
        {
            // we've lost tracking
            if (trackerFound == true)
            {
                trackerFound = false;
                OnTrackingLost(); // we lost tracking
            }

        }
    }

    /// <summary>
    /// what we want to do when the marker is found
    /// </summary>
    private void OnTrackingFound()
    {
        trackingState = true;
        Toggle();
        UpdateTrackerRotation(); // update the tracker rotation of the tracker.
         
    }


    /// <summary>
    /// what we want to do when the marker is lost.
    /// </summary>
    private void OnTrackingLost()
    {
        trackingState = false;
        Toggle();
    }


    /// <summary>
    /// Gets and updates the rotation of our devices camera.
    /// </summary>
    void UpdateTrackerRotation()
    {
        ConvertDeviceRotation(); // convert our current device input.

        rotationalHelper.eulerAngles = new Vector3(rotationalHelper.eulerAngles.x, arCamera.eulerAngles.y, arCamera.eulerAngles.z); // grab the cameras local y and z orientation and match it to the rotation helper.
        currentAngleCompared = Vector3.Dot(rotationalHelper.rotation * Vector3.forward, arCamera.forward); // compare the difference in the world forward, and the ar cameras forward direction

        if(currentAngleCompared <trackerthreshold)
        {
            // we are less threshold, we must be horizontal!
            currentRotation = Orientation.horizonal;
            transform.position = horizontalTracker.position; // set our position to our horizontal tracker
            transform.rotation = horizontalTracker.rotation; // set our rotation to our horiztonal tracker
        }
        else if(currentAngleCompared > trackerthreshold)
        {
            // we are more than threshold, we must be vertical!
            currentRotation = Orientation.vertical;
            transform.position = verticalTracker.position; // set our position to our vertical tracker
            transform.rotation = verticalTracker.rotation; // set our rotation to our vertical tracker
        }

    }

    /// <summary>
    /// essentially the gyro information and convert it so unity can handle the input.
    /// </summary>
    private void ConvertDeviceRotation()
    {
        rotationalHelper.rotation = Input.gyro.attitude; // match the rotation of the gyro;
        rotationalHelper.Rotate(0, 0, 180f, Space.Self);// rotate around its local axis and swap it from the gyro's input
        rotationalHelper.Rotate(90, 180, 0f, Space.World);// rotate it make sense of the camera facing from the back of your phone camera // save the rotation
    }

    public void Toggle()
    {
        if(trackerFound)
        {
            audioSource.Play();
            Time.timeScale = 1f;
        }
        else
        {
            audioSource.Pause();
            Time.timeScale = 0f;
        }
        
    }
}
