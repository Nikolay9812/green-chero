using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;

    // Every living enemy in the scene. The player's auto-shoot looks here for the nearest target,
    // so we don't need tags or FindObjectsByType every frame.
    public static readonly List<Enemy> All = new List<Enemy>();

    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;

        if (IsDead) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
