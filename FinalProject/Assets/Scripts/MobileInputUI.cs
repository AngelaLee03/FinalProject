using UnityEngine;
using UnityEngine.UI;

public class MobileInputUI : MonoBehaviour
{
    public Text statusText;

    void Update()
    {
        // Touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                statusText.text = "Touch Started";
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                statusText.text = "Drawing Path...";
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                statusText.text = "Path Submitted!";
            }
        }

        // Mouse (for testing in editor)
        if (Input.GetMouseButtonDown(0))
        {
            statusText.text = "Touch Started";
        }

        if (Input.GetMouseButton(0))
        {
            statusText.text = "Drawing Path...";
        }

        if (Input.GetMouseButtonUp(0))
        {
            statusText.text = "Path Submitted!";
        }
    }

    // Button function
    public void ResetPath()
    {
        statusText.text = "Path Reset!";
        Debug.Log("Reset button pressed");
    }
}