using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTrigger : MonoBehaviour
{
    [SerializeField] List<GameObject> goHits;

    void Start()
    {
        goHits = new List<GameObject>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player p1 = transform.root.GetComponent<Player>();
            Player p2 = other.GetComponent<Player>();

            if (p1.NPlayer != p2.NPlayer)
            {
                if (!goHits.Contains(p2.gameObject))
                {
                    goHits.Add(p2.gameObject);

                    p2.KnockBack();
                }
            }
        }
    }
    

    void OnDisable()
    {
        goHits = new List<GameObject>();
    }
}
