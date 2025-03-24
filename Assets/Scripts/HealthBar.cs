using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour {
    public Slider healthFill;

    // shake intensity and duration
    public float shakeDuration = 0.5f; 
    public float shakeAmount = 5f; 

    // Stores original position of health bar
    private Vector3 originalPosition;
    void Start() {
        healthFill.maxValue = 1;
        healthFill.value = 1;
        originalPosition = healthFill.transform.position;
    }

    // reduces the hp bar visually and changes the color over time to red
    public void UpdateHealth(float healthAmount) {
        healthFill.value = healthAmount;
        Image fillImage = healthFill.fillRect.GetComponent<Image>();
        float normalizedHealth = healthAmount / healthFill.maxValue;
        fillImage.color = Color.Lerp(new Color(1f, 0.2f, 0.4f), new Color(0.2f, 1f, 0.8f), normalizedHealth);
    }

    public void ShakeHealth(){
        StartCoroutine(Shake());
    }

    private IEnumerator Shake() {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration) {
            // geenrate offset based on shake amount
            float xOffset = Random.Range(-shakeAmount, shakeAmount);
            float yOffset = Random.Range(-shakeAmount, shakeAmount);

            // apply offset and increase time
            healthFill.transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    // reset position after shaking
    healthFill.transform.position = originalPosition;
    }
}
