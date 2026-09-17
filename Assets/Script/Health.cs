using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"{gameObject.name} a pris {damage} dégâts ! PV restants : {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");

        // Si c'est l'Undead, on prévient son script
        Undead undead = GetComponent<Undead>();
        if (undead != null)
        {
            undead.OnDeath();
            return;
        }

        // Si c'est le joueur
        // Tu pourras ajouter ton Game Over ici
    }
}