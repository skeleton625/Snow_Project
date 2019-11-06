using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        MouseLock(false);
    }

    private void MouseLock(bool _isVisible)
    {
        Cursor.visible = _isVisible;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
