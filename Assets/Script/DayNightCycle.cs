using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Light directionalLight;

    public float dayDurationInSeconds = 120f;

    public float timeScaleMultiplier = 1f;

    private float minIntensity = 0f;
    private float maxIntensity = 1.2f;

    public float currentHour = 6f;

    void Start()
    {
        directionalLight = GetComponent<Light>();
    }

    void Update()
    {
        float hoursPerSecond = (24f / dayDurationInSeconds) * timeScaleMultiplier;
        currentHour = (currentHour + hoursPerSecond * Time.deltaTime) % 24f;

        UpdateSun();
    }

    void UpdateSun()
    {
        float sunAngle = (currentHour / 24f) * 360f - 90f;
        transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (directionalLight == null) return;

        float dotProduct = Vector3.Dot(transform.forward, Vector3.down);

        if (dotProduct > 0f)
        {
            // Jour : l'intensité augmente jusqu'à midi
            directionalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, dotProduct);
            directionalLight.enabled = true;
        }
        else
        {
            // Nuit : la lumière s'éteint pour éviter d'éclairer par le bas
            directionalLight.intensity = minIntensity;
            directionalLight.enabled = minIntensity > 0f;
        }
    }
}