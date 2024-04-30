using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // Hide the system cursor
        Cursor.visible = false;

        // Get the RectTransform component to position
        rectTransform = GetComponent<RectTransform>();

        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Crosshair Position: " + rectTransform.position);
        rectTransform.position = Input.mousePosition;
    }
}
