using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//El Rigidbody del padre debe estar en Never Sleep para que se ejecute tanto quieto como en movimiento
public class Hitbox : MonoBehaviour
{
    [SerializeField] List<GameObject> goHits;
    float timeWindow = 0.12f;

    void Start()
    {
        goHits = new List<GameObject>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            Transform root1 = transform.root;
            Transform root2 = other.transform.root;

            if (root2.CompareTag("Player"))
            {
                Player p1 = root1.GetComponent<Player>();
                Player p2 = root2.GetComponent<Player>();

                Debug.Log("La hitbox es de " + p1.name);

                //if (Time.time - p1.AtkTime <= timeWindow && Time.time - p2.AtkTime <= timeWindow)
                if (p1.AtkTime == p2.AtkTime)
                {
                    AudioManagerBrackeys.instance.Play(p1.Character.clashEnter);

                    Vector2 pos = new Vector3((p1.transform.position.x + p2.transform.position.x) / 2, p1.transform.position.y);

                    ParticleManager.instance.PlayOne("Clash", pos);

                    Debug.Log(p1.name + " choca las espadas");

                    gameObject.SetActive(false);

                    if (!goHits.Contains(p2.gameObject))
                        goHits.Add(p2.gameObject);
                }
                else if (p1.NPlayer != p2.NPlayer)
                {
                    if (!goHits.Contains(p2.gameObject))
                    {
                        goHits.Add(p2.gameObject);

                        //Si bloquea
                        if (p2.action == Player.Action.BLOCK)
                        {
                            //Tipo de ataque
                            switch (p1.state)
                            {
                                //Ataque vertical
                                case Player.State.VERTICAL:
                                    //Tipo de bloqueo
                                    switch (p2.state)
                                    {
                                        case Player.State.VERTICAL:
                                            p2.InBlockStun(p1.Character);
                                            ParticleManager.instance.Play("Clash", transform.position);
                                            break;
                                        case Player.State.HORIZONTAL:
                                            p2.Hurt(p1.Character);
                                            break;
                                    }
                                    break;
                                //Ataque horizontal
                                case Player.State.HORIZONTAL:
                                    //Tipo de bloqueo
                                    switch (p2.state)
                                    {
                                        case Player.State.VERTICAL:
                                            p2.Hurt(p1.Character);
                                            break;
                                        case Player.State.HORIZONTAL:
                                            p2.InBlockStun(p1.Character);
                                            ParticleManager.instance.Play("Clash", transform.position);
                                            break;
                                    }
                                    break;
                            }
                        }
                        //Si para
                        else if (p2.action == Player.Action.PARRY)
                        {
                            //Tipo de ataque
                            switch (p1.state)
                            {
                                //Ataque vertical
                                case Player.State.VERTICAL:
                                    //Tipo de parry
                                    switch (p2.state)
                                    {
                                        case Player.State.VERTICAL:
                                            p1.Parried(p2.Character);
                                            ParticleManager.instance.Play("Parry", transform.position);
                                            break;
                                        case Player.State.HORIZONTAL:
                                            p2.Hurt(p1.Character);
                                            break;
                                    }
                                    break;
                                //Ataque horizontal
                                case Player.State.HORIZONTAL:
                                    //Tipo de bloqueo
                                    switch (p2.state)
                                    {
                                        case Player.State.VERTICAL:
                                            p2.Hurt(p1.Character);
                                            break;
                                        case Player.State.HORIZONTAL:
                                            p1.Parried(p2.Character);
                                            ParticleManager.instance.Play("Parry", transform.position);
                                            break;
                                    }
                                    break;
                            }
                        }
                        //Si no bloquea ni para
                        else
                        {
                            p2.Hurt(p1.Character);
                        }
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        goHits = new List<GameObject>();
    }
}
