using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health targetHealth;
    public Image fillImage;
    private Camera mainCam;

    void Awake()
    {
        // Trouve automatiquement la cible si elle n'est pas assignée à la main
        if (targetHealth == null)
        {
            targetHealth = GetComponentInParent<Health>();
        }

        // Trouve l'image fille si non assignée
        if (fillImage == null)
        {
            fillImage = transform.Find("Background/FillBar")?.GetComponent<Image>();
        }
    }

    void Start()
    {
        mainCam = Camera.main;

        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateBar;
            // Force l'affichage initial
            UpdateBar(targetHealth.currentHealth, targetHealth.maxHealth);
        }
        else
        {
            Debug.LogError($"[HealthBarUI] Aucun composant Health trouvé pour {gameObject.name} !");
        }
    }

    void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateBar;
        }
    }

    public void UpdateBar(int current, int max)
    {
        if (fillImage != null && max > 0)
        {
            fillImage.fillAmount = (float)current / (float)max;
        }
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.forward = mainCam.transform.forward;
        }
    }
}