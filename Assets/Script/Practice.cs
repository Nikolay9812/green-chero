using UnityEngine;
using System.Collections.Generic;

public class Practice : MonoBehaviour
{
    void Start()
    {
        Weapon sword = new Weapon("Sword", 25);
        Weapon axe = new Weapon("Axe", 40);
        Weapon bow = new Weapon("Bow", 15);

        Debug.Log($"{sword.name} - {sword.damage}");
        Debug.Log($"{axe.name} - {axe.damage}");
        Debug.Log($"{bow.name} - {bow.damage}");

        // Exercies 1
        int result = Damage(20, true);
        Debug.Log($"Damage: {result}");

        // Exercies 2
        for (int level = 1; level <= 10; level++)
        {
            int xp = 20 * (level - 1) * 10;
            Debug.Log($"Level {level} needs {xp} XP");
        }

        // Exercies 3
        List<string> enemies = new List<string>()
        {
            "Goblin",
            "Troll",
            "Orc"
        };

        string longestName = "";

        foreach (string enemy in enemies)
        {
            if (enemy.Length > longestName.Length)
            {
                longestName = enemy;
            }
        }

        Debug.Log($"Longest enemy name: {longestName}");
    }

    int Damage(int attack, bool critical)
    {
        if (critical)
        {
            return attack * 2;
        }

        return attack;
    }
}
