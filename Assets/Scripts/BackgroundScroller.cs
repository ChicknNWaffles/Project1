using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // For changing the scroll speed of the background and for pulsing the color
    public float scrollSpeed = 0.1f;
    public float backgroundWidth = 10f;
    public Color pulseColor = new Color(0.8f ,0.6f ,1f ,1f);
    public float pulseSpeed = 0.7f;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    // Start is called before the first frame update
    void Start() {
        // stores the initial position and sprite renderer
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

         // stores the original color
         if (spriteRenderer != null) {
            originalColor = spriteRenderer.color; 
        }
    }

    // Update is called once per frame
    void Update() {
        // scrolling the background left over tijme
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, backgroundWidth);
        transform.position = startPosition + Vector3.left * newPosition;

        // pulsing the background lighter
        float pulseValue = Mathf.PingPong(Time.time * pulseSpeed, 1f); // creates smoothe oscillation
        Color lerpedColor = Color.Lerp(originalColor, pulseColor, pulseValue);
        spriteRenderer.color = lerpedColor;
    }
}
