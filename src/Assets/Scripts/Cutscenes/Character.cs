//please forgive me i committed a sin
using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    /// <summary>
    /// Moves the character smoothly toward a world-space target position.
    /// </summary>
    public IEnumerator Move(Vector2 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            // Optional: flip sprite left/right
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

            yield return null;
        }

        transform.position = targetPos; // Snap exactly to the target
    }
}
