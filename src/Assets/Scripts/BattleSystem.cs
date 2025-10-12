using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST} // public enum to save the game state
public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab; // Allows you to drag the player prefab + enemy prefab into the inspector menu
    public GameObject enemyPrefab;

    Unit playerUnit;
    Unit enemyUnit;
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    public DialogueManager managerReference;

    Scene m_Scene;
    string sceneName;
    public bool isEffective = false; //Bool check for the tutorial
    public bool combat2done;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.START; //get default state which is start
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;
        
        StartCoroutine(SetupBattle()); //use co routines to create delay between initializing and starting gameplay
    }
    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation); //Spawn the objects + make gameobject for ui changes
        playerUnit = playerGO.GetComponent<Unit>(); //Get the unit script to keep track of hp, damage, etc.

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation); //Create game objects
        enemyUnit = enemyGO.GetComponent<Unit>();
        if (GameFlow.Instance.currentDay == 0)
        {
            dialogueText.text = "Try to defeat this " + enemyUnit.unitName;
        }
        else if (GameFlow.Instance.currentDay == 1)
        {
            dialogueText.text = "Now, face " + enemyUnit.unitName;
        }
        else if (GameFlow.Instance.currentDay == 1)
        {
            dialogueText.text = "Defeat " + enemyUnit.unitName;
        }

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn(); //Start the battle with the player's turn

    }

    IEnumerator PlayerAttack() //Attact function
    {
        // Damage the enemy
        if (GameFlow.Instance.currentDay == 0 && !isEffective)
        {
            dialogueText.text = "Normal magic won't effect this voidling...";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn()); //Switch to the enemy's turn
        }
        else
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "Hit registered";

            yield return new WaitForSeconds(2f);

        // Check if the enemy is dead
            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();   //Stop the battle if you kill enemy
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn()); //Switch to the enemy's turn
            }   
        }
    }
    IEnumerator EnemyTurn() //Function for enemy turn
    {
        int randomNumber = Random.Range(0, 2);
        if (randomNumber == 0)
        {
            dialogueText.text = enemyUnit.unitName + " attacks!";
            yield return new WaitForSeconds(1f);
            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                state = BattleState.LOST; //If player dies, game over
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN; //Switch to the player's turn
                PlayerTurn();
            }
        }
        else
        {
            dialogueText.text = enemyUnit.unitName + " heals!";
            yield return new WaitForSeconds(1f);
            enemyUnit.Heal(3);
            bool isDead = playerUnit.TakeDamage(0);
            enemyHUD.SetHP(enemyUnit.currentHP);
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
            state = BattleState.LOST; //If player dies, game over
            EndBattle();
            }
            else
            {
            state = BattleState.PLAYERTURN; //Switch to the player's turn
            PlayerTurn();
            }
        }
    }
    void EndBattle()
{
    if (state == BattleState.WON)
    {
        dialogueText.text = "You won the battle!";
        if (GameFlow.Instance.currentDay == 0)
    {
        GameFlow.Instance.currentDay++;
        SceneManager.LoadScene("bedroom");
    }

            if (sceneName == "combat 1")
            {
                StoryFlags.Instance.SetFlag("combat2done", true);
                if (StoryFlags.Instance.GetFlag("combat2done"))
                {
                    Debug.Log("Player has completed combat 2!");
                }
                SceneManager.LoadScene("warehouse");
            }
            if (sceneName == "combat 2")
            {
                StoryFlags.Instance.SetFlag("combat3done", true);
                if (StoryFlags.Instance.GetFlag("combat3done"))
                {
                    Debug.Log("Player has completed combat 3!");
                }
                SceneManager.LoadScene("office 1");
            }
            if (sceneName == "combat 3")
            {
                StoryFlags.Instance.SetFlag("combat4done", true);
                if (StoryFlags.Instance.GetFlag("combat4done"))
                {
                    Debug.Log("Player has completed combat 4!");
                }
                SceneManager.LoadScene("final 1");
            }

    }
    else if (state == BattleState.LOST)
    {
        dialogueText.text = "You lost!";
        SceneManager.LoadScene("friendend");
    }

}

    void PlayerTurn() //What to display on the player's turn
    {
        dialogueText.text = "Choose an action";
    }

    IEnumerator PlayerHeal() //Heal button
    {
        playerUnit.Heal(5);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel refreshed";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerBuff()
{
        if (!isEffective && GameFlow.Instance.currentDay == 0)
        {
            playerUnit.Buff(5);
            isEffective = true;
            dialogueText.text = "Voldem: You are filled with effective power. Use it.";

            yield return new WaitForSeconds(1f);

            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.SetHP(playerUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (GameFlow.Instance.currentDay == 0)
        {
            playerUnit.Buff(5);
            dialogueText.text = "Voldem: You are filled with effective power. Use it.";

            yield return new WaitForSeconds(1f);

            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.SetHP(playerUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (GameFlow.Instance.currentDay == 1) //Player gets stronger by the day
        {
            playerUnit.Buff(10);
            dialogueText.text = "You feel powered but a bit hurt...";

            yield return new WaitForSeconds(1f);

            bool isDead = playerUnit.TakeDamage(10);
            playerHUD.SetHP(playerUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (GameFlow.Instance.currentDay == 2) //Player gets stronger by the day
        {
            playerUnit.Buff(15);
            dialogueText.text = "You feel powered but a bit hurt...";

            yield return new WaitForSeconds(1f);

            bool isDead = playerUnit.TakeDamage(15);
            playerHUD.SetHP(playerUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
}

    public void OnAttackButton() // Attack trigger, put it on the inspector
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() //Heal trigger, put it on the inspector
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
    
    public void OnBuffButton() //Buff trigger, put it on the inspector
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerBuff());
    }
}
