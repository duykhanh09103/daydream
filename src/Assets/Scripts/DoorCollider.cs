    using UnityEngine;
using UnityEngine.SceneManagement;

    public class TriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // This code will execute when another collider enters this trigger.
        Debug.Log("Triggered by: " + other.gameObject.name);

        // You can perform actions based on the 'other' collider,
        // for example, checking its tag or component.
        if (this.gameObject.CompareTag("doorSchool"))
        {
            SceneManager.LoadScene("school");
            // Perform specific actions for the player.
        }
        if (this.gameObject.CompareTag("doorLibrary"))
        {
            SceneManager.LoadScene("library");
            // Perform specific actions for the player.
        }
        if (this.gameObject.CompareTag("doorOffice") && GameFlow.Instance.currentDay == 2)
        {
            SceneManager.LoadScene("office");
            // Perform specific actions for the player.
        }
        if (this.gameObject.CompareTag("doorBedroom"))
        {
            SceneManager.LoadScene("bedroom");
            // Perform specific actions for the player.
        }
    }
}