using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class PassableObject : MonoBehaviour
{
    private Collider2D myCollider;
    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        IgnoreExistingEntities();
    }
    private void IgnoreExistingEntities()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entities");
        foreach (GameObject entity in entities)
        {
            Collider2D entityCollider = entity.GetComponent<Collider2D>();
            if (entityCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, entityCollider, true);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entities") || collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, myCollider, true);
        }
    }
}
