using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] bool showHitboxes;
    [SerializeField] bool showHurtboxes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Show("Hitbox", showHitboxes);
        Show("Hurtbox", showHurtboxes);
    }

    void Show(string theTag, bool b)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(theTag);

        foreach (GameObject go in gos)
            if (go.TryGetComponent(out SpriteRenderer renderer))
                renderer.enabled = b;
    }
}