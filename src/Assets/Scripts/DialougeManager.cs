using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueArea;
    public List<Button> choiceButtons;

    private Queue<DialogueLine> lines;
    private DialogueLine currentLine;
    private Dialogue currentDialogue;
    private Coroutine typingCoroutine;

    public bool isDialogueActive = false;
    public bool isTyping;
    public float typingSpeed = 0.2f;
    public bool isChoosing = false;
    private bool jumpedToLine = false;

    public Animator animator;
    public int day;

    Scene m_Scene;
    string sceneName;

    public bool combat1done = false;

    [Header("Story Progression")]
    public StoryFlags storyFlags;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ✅ Ensure this always points to the SINGLE global instance
        storyFlags = StoryFlags.Instance;
        storyFlags.Initialize();

        lines = new Queue<DialogueLine>();
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;

        HideAllChoices();
    }

    public IEnumerator StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        isDialogueActive = true;
        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            if (AreConditionsMet(dialogueLine))
                lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();

        while (isDialogueActive)
            yield return null;

        animator.Play("hide");
    }

    private bool AreConditionsMet(DialogueLine line)
    {
        if (line.conditions == null || line.conditions.Count == 0)
            return true;

        if (storyFlags == null)
            storyFlags = StoryFlags.Instance;

        foreach (var c in line.conditions)
        {
            bool currentFlag = storyFlags.GetFlag(c.flagName);
            if (currentFlag != c.requiredValue)
            {
                Debug.Log($"❌ Condition not met: {c.flagName} = {currentFlag}, required = {c.requiredValue}");
                return false;
            }
            else
            {
                Debug.Log($"✅ Condition met: {c.flagName} = {currentFlag}");
            }
        }
        return true;
    }

    public void DisplayNextDialogueLine()
    {
        HideAllChoices();

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";

        foreach (char letter in dialogueLine.line)
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (dialogueLine.hasChoices && dialogueLine.choices != null && dialogueLine.choices.Count > 0)
        {
            ShowChoices(dialogueLine.choices);
        }
        else
        {
            StartCoroutine(WaitForNext());
        }
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        isChoosing = true;
        HideAllChoices();

        for (int i = 0; i < choices.Count && i < choiceButtons.Count; i++)
        {
            Button btn = choiceButtons[i];
            btn.gameObject.SetActive(true);

            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = choices[i].choiceText;

            int choiceIndex = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                OnChoiceSelected(choices[choiceIndex]);
            });
        }
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        HideAllChoices();

        if (!string.IsNullOrEmpty(choice.flagToSet) && storyFlags != null)
        {
            storyFlags.SetFlag(choice.flagToSet, choice.flagValue);
        }

        if (choice.jumpToLineInSameDialogue)
        {
            DisplaySpecificDialogueLine(choice.targetLineIndex);
        }
        else if (choice.nextDialogue != null)
        {
            StartCoroutine(StartDialogue(choice.nextDialogue));
        }
        else
        {
            EndDialogue();
        }
    }

    void HideAllChoices()
    {
        foreach (var btn in choiceButtons)
            btn.gameObject.SetActive(false);
        isChoosing = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("hide");

        if (currentDialogue != null && currentDialogue.isEndingDialogue && storyFlags != null)
        {
            if (!string.IsNullOrEmpty(currentDialogue.endingFlagToSet))
            {
                storyFlags.SetFlag(currentDialogue.endingFlagToSet, currentDialogue.endingFlagValue);
                Debug.Log($"🏁 Ending flag set: {currentDialogue.endingFlagToSet} = {currentDialogue.endingFlagValue}");
            }
        }

        // (Scene transitions kept same)
        if (GameFlow.Instance.currentDay == 0)
        {
            if (sceneName == "intro") SceneManager.LoadScene("bedroom");
            else if (sceneName == "library") SceneManager.LoadScene("combat intro");
            else if (sceneName == "combat intro") SceneManager.LoadScene("combat");
        }
        if (GameFlow.Instance.currentDay == 1 && !StoryFlags.Instance.GetFlag("combat2done"))
        {
            if (sceneName == "warehouse")
            {
                Debug.Log("activate combat again");
                SceneManager.LoadScene("combat 1");
                combat1done = true;
                Debug.Log("combat1 is done");
            }
        }
        else if (GameFlow.Instance.currentDay == 1 && StoryFlags.Instance.GetFlag("combat2done"))
        {
            if (StoryFlags.Instance.GetFlag("friendend") == true)
            {
                SceneManager.LoadScene("friendend");
            }
            else if (StoryFlags.Instance.GetFlag("day2done") == true)
            {
                StoryFlags.Instance.SetFlag("day2done", true);
                SceneManager.LoadScene("bedroom");
                GameFlow.Instance.currentDay++;
            }
        }
        if (GameFlow.Instance.currentDay == 2 && !StoryFlags.Instance.GetFlag("combat3done") && sceneName == "office")
        {
            SceneManager.LoadScene("combat 2");

        }
        if (StoryFlags.Instance.GetFlag("teacherend"))
        {
            SceneManager.LoadScene("friendend");
        }
        else if (StoryFlags.Instance.GetFlag("isfinalbattle"))
        {
            SceneManager.LoadScene("final");
        }
        if (sceneName == "final")
        {
            SceneManager.LoadScene("combat 3");
        }

        if (StoryFlags.Instance.GetFlag("goodend")) //Getting endings
        {
            SceneManager.LoadScene("goodend");
        }
        else if (StoryFlags.Instance.GetFlag("badend"))
        {
            SceneManager.LoadScene("badend");
        }

        if (sceneName == "goodend" || sceneName == "badend" || sceneName == "friendend") //thank you screen
    
        {
            SceneManager.LoadScene("Thankyou");
        }
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueArea.text = currentLine.line;
                isTyping = false;

                if (currentLine.hasChoices && currentLine.choices != null && currentLine.choices.Count > 0)
                {
                    ShowChoices(currentLine.choices);
                }
            }
            else if (!currentLine.hasChoices)
            {
                DisplayNextDialogueLine();
            }
        }
    }

    public void DisplaySpecificDialogueLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= currentDialogue.dialogueLines.Count)
        {
            Debug.LogWarning("Invalid line index: " + lineIndex);
            EndDialogue();
            return;
        }

        lines.Clear();
        currentLine = currentDialogue.dialogueLines[lineIndex];

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    private IEnumerator WaitForNext()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        if (!jumpedToLine)
            DisplayNextDialogueLine();
        else
            jumpedToLine = false;
    }

    public void onMenuClick()
    {
        Update();
    }
}
