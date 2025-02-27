using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTutorial : MonoBehaviour
{
    public Animator menuAnimator; // Reference to the Animator
    public Image wKey, aKey, sKey, dKey; // References to the UI images
    private bool wPressed, aPressed, sPressed, dPressed;

    void Start()
    {
        // Show the menu at the start of the game
        menuAnimator.SetBool("Active", true);
    }

    void Update()
    {
        CheckInput(KeyCode.W, ref wPressed, wKey);
        CheckInput(KeyCode.D, ref aPressed, aKey);
        CheckInput(KeyCode.S, ref sPressed, sKey);
        CheckInput(KeyCode.A, ref dPressed, dKey);

        // Check if all keys have been pressed
        if (wPressed && aPressed && sPressed && dPressed)
        {
            menuAnimator.SetBool("Active", false);
        }
    }

    void CheckInput(KeyCode key, ref bool keyPressed, Image keyImage)
    {
        if (Input.GetKeyDown(key) && !keyPressed)
        {
            keyPressed = true;
            Color tempColor = keyImage.color;
            tempColor.a = 0.5f; // Grey out the key
            keyImage.color = tempColor;
        }
    }
}
