using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

// This class handles all of the functions that are called when buttons are selected, as well as their helper functions
public class Action_manager : MonoBehaviour
{
    [SerializeField]
    
    // Essential components
    public GameObject trackedBall;
    public GameObject ikeaTable;

    // Add on components
    public GameObject pins;
    public GameObject backWall;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject ballGlowFX;

    // Interaction components
    public GameObject right_controller;
    public GameObject left_controller;
    public GameObject ui_tools;
    public GameObject handle1;
    public GameObject handle2;
   
    // Text components
    public Text alertMessage;

    // Window panels
    public GameObject welcomeWindow;
    public GameObject configWindow;
    public GameObject controlWindow;
    public GameObject debugWindow;
    public GameObject settingsWindow;
    public GameObject customXWindow;
    public GameObject customYWindow;
    public GameObject alignHandlesWindow;
    public GameObject infoWindow;
    public GameObject alertWindow;

    // New lengths when the user changes the surface dimensions
    float customXlength;
    float customYlength;
    

    // Simple ball mode selected -> the surface top is cleared and only the ball is visible
    public void simpleButtonClicked(){
        hideBallGlowFX();
        showAlert();
        alertMessage.text = "Place the ball in the glowing ring";
        Invoke("hideAlert", 4);

        hideHandles();
        removePins();
        Invoke("showBallGlowFX", 0.5f);
        Invoke("addTrackedBall", 5);
        Invoke("hideBallGlowFX", 15);
    }

    // Bowling mode selected -> the users surface transforms into a table top bowling alley
    // Button exists in the control menu
    public void bowlingButtonClicked(){
        hideBallGlowFX();
        showAlert();
        alertMessage.text = "Place the ball in the glowing ring";
        Invoke("hideAlert", 4);

        hideHandles();
        Invoke("initialisePins", 2);
        Invoke("showBallGlowFX", 0.5f);
        Invoke("addTrackedBall", 5);
        Invoke("hideBallGlowFX", 15);
    }

    // Hand tracking selected -> controllers are removed and the user is instructed to switch the device to hand tracking mode
    // Currently no support exists to change the device into hand tracking without the user returning to Oculus home
    // Button exists in the control menu
    public void handTrackingButtonClick(){
        showAlert();
        alertMessage.text = "Press the oculus button to return to the oculus menu and switch to hand tracking";
        Invoke("hideAlert", 5);

        hideHandles();
        right_controller.SetActive(false);
        left_controller.SetActive(false);
        ui_tools.SetActive(false);
    }

    // Debug menu appears and remains present whilst the user is in Bowling mode or Simple ball mode
    // Provides information about the UDP information sent from the tracking script
    // Button exists in the control menu
    public void debugButtonClicked(){
        hideAlert();
        infoWindow.SetActive(false);
        settingsWindow.SetActive(false);
        hideHandles();
        debugWindow.SetActive(true);
    }
    public void hideDebug(){
        debugWindow.SetActive(false);
    }

    // Settings menu appears allowing the user to:
    // A) re-align the handles to fit their surface
    // B) change the size of the virtual surface to fit a different table
    // Button exists in the control menu
    public void settingsButtonClicked(){
        hideBallGlowFX();    
        hideAlert();
        removePins();
        hideDebug();
        settingsWindow.SetActive(true);
        hideInfo();
        controlWindow.SetActive(false);
    }
    public void hideSettings(){
        settingsWindow.SetActive(false);
        controlWindow.SetActive(true);
    }

    // Information about application author is displayed
    // Button exists in the control menu
    public void infoButtonClicked(){
        hideHandles();
        debugWindow.SetActive(false);
        settingsWindow.SetActive(false);
        infoWindow.SetActive(true);
    }
    public void hideInfo(){
        infoWindow.SetActive(false);
    }

    // The user is invited to align the virtual table to their physical space for the first time
    public void welcomeConfigButtonClicked(){
        showHandles();
        ikeaTable.SetActive(true);
        configWindow.SetActive(true);
        welcomeWindow.SetActive(false);
    }

    // User confirms the surface fits their physical space
    public void confirmConfigButtonClicked(){
        hideHandles();
        welcomeWindow.SetActive(true);
        configWindow.SetActive(false);
    }

    // Initial welcome/config stage up is complete -> user now only sees the control menu
    public void welcomeAcceptButtonClicked(){
        ikeaTable.SetActive(true);
        welcomeWindow.SetActive(false);
        controlWindow.SetActive(true);
    }

    // User can align the virtual surface with their physical surface
    public void alignHandlesButtonClicked(){
        alignHandlesWindow.SetActive(true);
        settingsWindow.SetActive(false);
        showHandles();
    }

    // Return to settings window from handles
    public void confirmHandlesButtonClicked(){
        alignHandlesWindow.SetActive(false);
        settingsWindow.SetActive(true);
        hideHandles();
    }

    // User selects that they want to change the size of the surface
    public void changeSurfaceButtonClicked(){
        customXWindow.SetActive(true);
        settingsWindow.SetActive(false);
        showHandles();
        ikeaTable.SetActive(false);
    }

     // User has changed the size of one length of the surface and has confirmed
    public void confirmLength1ButtonClicked(){
        customXWindow.SetActive(false);
        customYWindow.SetActive(true);
        
        // new X length is determined by handle distance
        customXlength = Vector3.Distance (handle1.transform.position, handle2.transform.position);
    }

    // User has changed the size of the second length of the surface and has confirmed
    // This function re-sizes the surface size in both x and z directions
    // The new surface is shifted by an offset so that the handles are back in the right position 
    public void confirmLength2ButtonClicked(){
        showHandles();
        customYWindow.SetActive(false);  
        alignHandlesWindow.SetActive(true);
        ikeaTable.SetActive(true);

        // new Y length is determined by handle distance
        customYlength = Vector3.Distance (handle1.transform.position, handle2.transform.position);

        // Offset the surface in X direction by half of the new X length, which positions the handles at the end of the surface
        float myOffset = -0.5f * customXlength;
        Vector3 localPos = ikeaTable.transform.localPosition;
        localPos.x = myOffset;

        // Get the current lengths of the X and Z components
        float xSize = ikeaTable.GetComponent<Renderer> ().bounds.size.x;
        float zSize = ikeaTable.GetComponent<Renderer> ().bounds.size.z;

        // Rescale is used as the surface is a child of the alignment system, hence resizing deals with localScale
        Vector3 rescale = ikeaTable.transform.localScale;
        rescale.x = (customXlength * rescale.x / xSize) * -1;
        rescale.z = customYlength * rescale.z / zSize;
        ikeaTable.transform.localScale = rescale;

        // Resize the surface given the recalculated X and Z components 
        ikeaTable.transform.localPosition = localPos;
    }

    // Show / hide the glowing ring
    public void showBallGlowFX(){
        ballGlowFX.SetActive(true);
    }
    public void hideBallGlowFX(){
        ballGlowFX.SetActive(false);
    }

    // Show / hide handles for alignment / resizing
    public void showHandles(){
        handle1.SetActive(true);
        handle2.SetActive(true);
    }
    public void hideHandles(){
        handle1.SetActive(false);
        handle2.SetActive(false);
    }

    // Show the oculus controller models along with ray pointer (used for interactions)
    public void activateControllers(){
        right_controller.SetActive(true);
        left_controller.SetActive(true);
        ui_tools.SetActive(true);
    }

    // Show / hide alert message pop-up
    // Its text varies from case to case
    public void showAlert(){
        alertWindow.SetActive(true);
    }
    public void hideAlert(){
        alertWindow.SetActive(false);
    }
    
    // Bowling helper functions, sets up the pins and the bowling surround
    public void initialisePins(){
        pins.SetActive(true);
        backWall.SetActive(true);
        leftWall.SetActive(true);
        rightWall.SetActive(true);   
    }
    public void removePins(){
        pins.SetActive(false);
        backWall.SetActive(false);
        leftWall.SetActive(false);
        rightWall.SetActive(false);   
    }

    // Show / hide the tracked ball
    public void addTrackedBall(){
       trackedBall.SetActive(true);
    }
    public void removeTrackedBall(){
       trackedBall.SetActive(false);
    }
}
