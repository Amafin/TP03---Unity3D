using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 25f;
    private float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ne touche pas le joueur qui tire
        if (other.CompareTag("Player")) return;

        // Cherche le composant Health sur l'objet touché (ou son parent)
        Health h = other.GetComponentInParent<Health>();
        if (h != null)
        {
            h.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        // Sol / Décor
        if (other.gameObject.layer == LayerMask.NameToLayer("groundLayer"))
        {
            Destroy(gameObject);
        }
    }
}