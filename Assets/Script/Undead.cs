using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Undead : MonoBehaviour
{
    [Header("Attaque")]
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;
    public float attackRange = 1.6f;

    [Header("Déplacement")]
    public float moveSpeed = 2.5f;
    public float turnSpeed = 360f;

    [Header("Composants")]
    public Animator anim;

    private Transform targetPlayer;
    private Health health;
    private bool canAttack = true;
    private bool isDead = false;

    void Awake()
    {
        health = GetComponent<Health>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isDead || targetPlayer == null) return;

        Vector3 direction = targetPlayer.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance <= attackRange)
        {
            if (anim != null) anim.SetBool("IsWalking", false);

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            if (canAttack)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            if (anim != null) anim.SetBool("IsWalking", true);
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (anim != null) anim.SetTrigger("Attack");

        // Inflige des dégâts au composant Health du Player
        if (targetPlayer != null)
        {
            Health playerHealth = targetPlayer.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void OnDeath()
    {
        isDead = true;
        targetPlayer = null;

        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetTrigger("Die");
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayer = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayer = null;
            if (anim != null) anim.SetBool("IsWalking", false);
        }
    }
}