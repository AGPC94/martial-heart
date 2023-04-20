using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    [Header("AI")]

    [Header("Ranges")]
    //[SerializeField] float tomaMaaiRange;
    [SerializeField] float issokuRange;
    [SerializeField] float chikamaRange;
    [SerializeField] float attackRange;

    [Header("Other")]
    [SerializeField] float stepTime = 0.1f;

    bool actionAllowed;
    float cpuDifficulty;
    const int MIN = 0;
    const int MAX = 101;
    int nDecision;
    float rnd;
    float cooldown;
    float clashWait;

    Collider2D collider;

    public bool ActionAllowed { get => actionAllowed; set => actionAllowed = value; }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        collider = GetComponent<Collider2D>();

        cpuDifficulty = GameManager.instance.cpuDifficulty;

        if (cpuDifficulty == 3)
        {
            nDecision = 70;
            cooldown = 0.10f;
        }
        if (cpuDifficulty == 2)
        {
            nDecision = 40;
            cooldown = 0.10f;
        }
        if (cpuDifficulty == 1)
        {
            nDecision = 10;
            cooldown = 0.10f;
        }

        ActionAllowed = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        TakeDecision();
    }

    void TakeDecision()
    {
        if (actionAllowed)
        {
            switch (action)
            {
                case Action.MOVE:
                    rnd = Random.Range(MIN, MAX);

                    if (rnd >= 0 && rnd <= nDecision)
                    {
                        if (IsAtIssoku())
                        {
                            if (opponent.action == Action.ATTACK)
                            {
                                Block();
                            }
                            else
                            {
                                rnd = Random.Range(1, 6);

                                switch (rnd)
                                {
                                    case 1:
                                        ForDashAtk();
                                        break;
                                    case 2:
                                        StepBack();
                                        break;
                                    case 3:
                                        StepFor();
                                        break;
                                    case 4:
                                        Stop();
                                        break;
                                    case 5:
                                        Debug.Log("No hace nada");
                                        break;
                                }
                            }
                        }
                        else if (IsAtChikama())
                        {
                            if (opponent.action == Action.ATTACK)
                            {
                                Block();
                            }
                            else
                            {
                                rnd = Random.Range(1, 4);
                                switch (rnd)
                                {
                                    case 1:
                                        ForDashAtk();
                                        break;
                                    case 2:
                                        StepBack();
                                        break;
                                }
                            }
                        }
                        else if (IsAtAttack())
                        {
                            if (opponent.action == Action.ATTACK)
                            {
                                Block();
                            }
                            else
                            {
                                rnd = Random.Range(1, 4);
                                switch (rnd)
                                {
                                    case 1:
                                        StepFor();
                                        break;
                                    case 2:
                                        Attack();
                                        break;
                                }
                            }
                        }
                        else
                            StepFor();
                    }
                    break;
                case Action.BLOCK:
                    DropBlock();
                    break;
                /*
                rnd = Random.Range(1, 101);
                if (rnd >= 1 && rnd <= 20)
                    Parry();
                else if (rnd >= 21 && rnd <= 90)
                    DropBlock();
                else if (rnd >= 91 && rnd <= 100)
                    Debug.Log("No hace nada");
                break;
                */
                case Action.CLASHSTAY:
                    rnd = Random.Range(0.10f, 0.30f);
                    Invoke("ClashAction", rnd);
                    break;
            }

            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        ActionAllowed = false;
        while (action != Action.MOVE && action != Action.CLASHSTAY)
            yield return null;
        yield return new WaitForSeconds(cooldown);
        ActionAllowed = true;
    }

    bool IsAtIssoku()
    {
        return Raycast(issokuRange, 0.1f, Color.yellow);
    }

    bool IsAtChikama()
    {
        return Raycast(chikamaRange, 0.2f, Color.magenta);
    }

    bool IsAtAttack()
    {
        return Raycast(attackRange, 0.3f, Color.red);
    }

    bool Raycast(float range, float posY, Color color)
    {
        Vector2 origin = new Vector3((collider.bounds.size.x / 1.5f) * face, posY) + transform.position;
        Vector2 direc = new Vector2(face, 0);
        Debug.DrawRay(origin, direc * range, color);
        return Physics2D.Raycast(origin, direc, range, whatIsPlayer);
    }

    void StepFor()
    {
        rb.velocity = new Vector2(face * walkSpeed, 0f);
    }

    void StepBack()
    {
        rb.velocity = new Vector2(-face * walkSpeed, 0f);
    }

    void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    void ForDashAtk()
    {
        StartCoroutine(Dash(face, dashSpeed, dashTime));
        Attack();
    }

    void ClashAction()
    {
        rnd = Random.Range(1, 3);

        switch (rnd)
        {
            case 1:
                ClashBackDashAtk();
                break;
            case 2:
                Push();
                break;
        }
    }

    void ClashBackDashAtk()
    {
        if (action == Action.CLASHSTAY && !IsWalled)
        {
            StartCoroutine(Dash(-face, clashBackDashSpeed, clashBackDashTime));
            Attack();
        }
    }

    void Push()
    {
        if (action == Action.CLASHSTAY && !opponent.IsWalled)
            anim.SetTrigger("Push");
    }

    void Attack()
    {
        int n = Random.Range(0, 2);

        switch (n)
        {
            case 0:
                state = State.VERTICAL;
                break;

            case 1:
                state = State.HORIZONTAL;
                break;
        }

        switch (state)
        {
            case State.VERTICAL:
                atkTime = Time.time;
                anim.SetTrigger("AtkV");
                break;

            case State.HORIZONTAL:
                atkTime = Time.time;
                anim.SetTrigger("AtkH");
                break;
        }
    }

    void Block()
    {
        if (action == Action.BLOCK)
        {
            rnd = Random.Range(1, 101);
            if (rnd >= 1 && rnd <= 20)
                Parry();
        }
        else
        {
            if (opponent.action == Action.ATTACK)
            {
                int n = Random.Range(0, 3);

                switch (n)
                {
                    case 0:
                        state = State.VERTICAL;
                        break;

                    case 1:
                        state = State.HORIZONTAL;
                        break;
                }

                switch (state)
                {
                    case State.VERTICAL:
                        anim.SetBool("BlockV", true);
                        anim.SetBool("BlockH", false);
                        break;

                    case State.HORIZONTAL:
                        anim.SetBool("BlockH", true);
                        anim.SetBool("BlockV", false);
                        break;
                }
            }
            else
                DropBlock();
        }

    }

    void DropBlock()
    {
        anim.SetBool("BlockV", false);
        anim.SetBool("BlockH", false);
    }

    void Parry()
    {
        switch (state)
        {
            case State.VERTICAL:
                anim.SetTrigger("AtkV");
                break;

            case State.HORIZONTAL:
                anim.SetTrigger("AtkH");
                break;
        }

        if (!IsAtAttack())
            StartCoroutine(Dash(face, dashSpeed, dashTime));
        DropBlock();
    }

    void OnDrawGizmos()
    {
        Vector2 origin1 = new Vector3((collider.bounds.size.x / 1.5f) * face, 0.1f) + transform.position;
        Vector2 origin2 = new Vector3((collider.bounds.size.x / 1.5f) * face, 0.2f) + transform.position;
        Vector2 origin3 = new Vector3((collider.bounds.size.x / 1.5f) * face, 0.3f) + transform.position;
        Vector2 direc = new Vector2(face, 0);

        Gizmos.DrawRay(origin1, direc * issokuRange);
        Gizmos.DrawRay(origin2, direc * chikamaRange);
        Gizmos.DrawRay(origin3, direc * attackRange);
    }
}

/*
 Descartado

    IEnumerator Step(float direction, float speed, float time)
    {
        rb.velocity = new Vector2(direction * speed, 0f);

        yield return new WaitForSeconds(time);

        rb.velocity = Vector2.zero;
    }

    Bugs
    
    Cuando se aleja del clash, lo empieza otra vez varias veces seguidas

 */
