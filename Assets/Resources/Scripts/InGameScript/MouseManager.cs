using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private bool IsVisible;

    void Start()
    {
        IsVisible = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLockInScene();
    }

    private void MouseLockInScene()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsVisible = !IsVisible;

            Cursor.visible = IsVisible;
            if (IsVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }        
    }
}
