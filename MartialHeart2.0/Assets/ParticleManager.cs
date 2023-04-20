using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    GameObject clone = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string particle, Vector3 position)
    {
        GameObject go = Resources.Load<GameObject>("Particles/" + particle);
        Instantiate(go, position, Quaternion.identity);
    }

    public void PlayOne(string particle, Vector3 position)
    {
        GameObject go = Resources.Load<GameObject>("Particles/" + particle);

        if (clone == null)
            clone = Instantiate(go, position, Quaternion.identity);


    }
}
