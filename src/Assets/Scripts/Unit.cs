using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string unitName;
    public int damage;
    public int maxHP;
    public int currentHP;
    public DialogueManager managerReference;

    public bool TakeDamage(int dmg) //Function that deducts health for damage
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            return true;
        else
            return false;
    }

    public void Heal (int amount) //Function for healing
    {
        currentHP += amount;
        if (currentHP > amount)
        {
            currentHP = maxHP;
        }
    }

    public void Buff(int amount) // Function for attack buffing + use it for story purposes
    {
        damage += amount;
    }

}
