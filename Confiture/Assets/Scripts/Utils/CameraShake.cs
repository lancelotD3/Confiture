using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Durée du shake
    public float shakeDuration = 0.5f;

    // Intensité du shake
    public float shakeMagnitude = 0.2f;

    // Atténuation du shake
    public float dampingSpeed = 1.0f;

    // Position initiale de la caméra
    private Vector3 initialPosition;

    // Temps restant pour le shake
    private float shakeTimeRemaining;

    void Start()
    {
        mainCamera = Camera.main;
        initialPosition = mainCamera.transform.localPosition;
    }

    void Update()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;

        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerShake(.5f);
        }

        if (shakeTimeRemaining > 0)
        {
            mainCamera.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeTimeRemaining = 0f;
            mainCamera.transform.localPosition = initialPosition;
        }
    }

    // Appelle cette méthode pour déclencher le shake
    public void TriggerShake(float duration = 0.5f, float magnitude = 0.2f, float damping = 1f)
    {
        shakeMagnitude = magnitude;
        dampingSpeed = damping;

        shakeTimeRemaining = duration > 0 ? duration : shakeDuration;
    }
}
