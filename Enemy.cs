using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    EnemyState m_State;

    public int hp = 30;

    public int defence = 0;

    public int attackDamage = 10;

    public float findDistance = 5f; // 플레이어 발견 범위

    public float attackDistance = 2f;

    public float moveSpeed = 3f;

    float currentTime = 0f;

    public float attackDelay = 2f;

    Transform player; // 플레이어 트랜스폼

    Vector3 originPos; // 초기 위치 저장용

    public float MoveDistance = 3f; // 이동 가능 범위

    private Animator animator;

    public Transform pos;

    public Vector2 boxSize;

    bool isflip = false;

    bool isattacked = false;

    bool isattack = false;

    bool isknockback = false;

    private new Rigidbody2D rigidbody;

    public int nextMove;
    void Start()
    {
        m_State = EnemyState.Idle;

        player = GameObject.Find("Player").transform;

        originPos = transform.position;

        currentTime = 0;
            
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();

        Invoke("NextActionIdle", 5);
    }

    void Update()
    {
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged(); -> 상태 전환 시에 한 번만 실행되어야 함
                break;
            case EnemyState.Die:
                //Die(); -> 한 번만 실행되어야 함
                break;
        }
    }
    void Idle()
    {
        if (Vector2.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
        }
        else
        {
            Vector2 frontVec = new Vector2(rigidbody.position.x + nextMove, rigidbody.position.y);

            Debug.DrawRay(frontVec, Vector3.down, new Color(1, 0, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));

            if (rayHit.collider != null)
            {
                if (nextMove > 0)
                {
                    if (!isflip)
                    {
                        this.transform.Rotate(0, 180, 0);
                    }
                    isflip = true;
                    rigidbody.velocity = new Vector2(nextMove * moveSpeed, 0);
                    animator.SetTrigger("isERun");

                }
                else if (nextMove < 0)
                {
                    if (isflip)
                    {
                        this.transform.Rotate(0, 180, 0);
                    }
                    isflip = false;
                    rigidbody.velocity = new Vector2(nextMove * moveSpeed, 0);
                    animator.SetTrigger("isERun");
                }
                else
                {
                    rigidbody.velocity = new Vector2(nextMove * moveSpeed, 0);
                    animator.SetTrigger("isEIdle");
                }
            }
            else
            {
                animator.SetTrigger("isEIdle");
                nextMove = -1 * nextMove;
            }
        }
    }

    private void NextActionIdle()
    {
        nextMove = Random.Range(-1, 2); // -1~1

        float nextTime = Random.Range(3f, 5f);
        Invoke("NextActionIdle", nextTime);

    }
    void Move()
    {
        
        if (Vector2.Distance(transform.position, originPos) > MoveDistance)
        {
            m_State = EnemyState.Return;
        }

        if (Vector2.Distance(transform.position, player.position) > attackDistance) // 공격 범위 밖
        {
            Vector3 dir = (player.position - transform.position).normalized;

            if (dir.x > 0)
            {
                if (!isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = true;

            }
            else if (dir.x < 0)
            {
                if (isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = false;
            }

            rigidbody.velocity = new Vector2(dir.x * moveSpeed, 0);
            animator.SetTrigger("isERun");
        }

        else
        {
            m_State = EnemyState.Attack;
            animator.SetTrigger("isERun");
        }
    } 
    void Attack()
    {
        AttackProcess();
    }

    public void AttackProcess()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float DistanceAssist = Vector3.Distance(transform.position,new Vector3(0,0,0));

        if (distance- DistanceAssist <= attackDistance)
        {
            float h = transform.position.x - player.position.x;

            currentTime += Time.deltaTime;

            if (h < 0)
            {
                if (!isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = true;

                if ((currentTime > attackDelay) && !isattack)
                {
                    rigidbody.velocity = new Vector2(0, 0);
                    isattack = true;
                    animator.SetTrigger("isEAttack");
                    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                    currentTime = 0;
                }
                else 
                {
                    animator.SetTrigger("isEIdle");
                }
            }
            else
            {
                if (isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = false;

                if ((currentTime > attackDelay) && !isattack)
                {
                    rigidbody.velocity = new Vector2(0, 0);
                    isattack = true;
                    animator.SetTrigger("isEAttack");
                    currentTime = 0;
                }
                else
                {
                    animator.SetTrigger("isEIdle");
                }
            }
        }
        else
        {
            m_State = EnemyState.Move;
            isattack = false;
        }
    }

    public void attackTiming()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<PlayerMove>().HitPlayer(attackDamage, transform.position);
            }
        }
    }
    public void changestate()
    {
        m_State = EnemyState.Move;
        isattack = false;
    }
    void Return()
    {
        if (Vector3.Distance(transform.position, originPos) > 0.228f)
        {
            Vector3 dir = (originPos - transform.position).normalized;

            if (dir.x > 0)
            {
                if (!isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = true;

            }
            else if (dir.x < 0)
            {
                if (isflip)
                {
                    this.transform.Rotate(0, 180, 0);
                }
                isflip = false;
            }

            rigidbody.velocity = new Vector2(dir.x * moveSpeed, 0);
        }
        else //복귀 중 원래 자리 근처에 왔을 때
        {
           
            transform.position = originPos;
            m_State = EnemyState.Idle;
            rigidbody.velocity = new Vector2(0, 0);
        }
    }

    public void changeAttackedState()
    {
        m_State = EnemyState.Move;
        isattacked = false;
    }
    public void HitEnemy(int hitPower, Vector2 pos) //맞고 계산되는 함수
    {
        if (!isattacked)
        {
            if (m_State == EnemyState.Damaged || m_State == EnemyState.Die)
            {
                return;
            }
            hp -= (hitPower - defence);

            if (hp > 0)
            {
                m_State = EnemyState.Damaged;
                animator.SetTrigger("isEDamaged");
                float x = transform.position.x - pos.x;
                if (x < 0)
                    x = 1;
                else
                    x = -1;

                StartCoroutine(Knockback(x));
                StartCoroutine(DamageRoutine());
            }
            else
            {
                rigidbody.velocity = new Vector2(0, 0);
                m_State = EnemyState.Die;
                Die(); // 여기서 실행
            }
        }
    }
    IEnumerator Knockback(float dir)
    {
        isknockback = true;
        float ctime = 0;
        while (ctime < 0.4f)
        {
            if (transform.rotation.y == 0)
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime * dir * -1f);

            ctime += Time.deltaTime;
            yield return null;
        }
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime * dir * -1f * 0);
        isknockback = false;
    }

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(5f);
        isattacked = false;
    }

    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        animator.SetTrigger("isEDie");

        yield return new WaitForSeconds(3f); // n초 뒤 소멸

        Destroy(this.gameObject);

        GameObject smObject = GameObject.Find("ScoreManager");

        ScoreManager sm = smObject.GetComponent<ScoreManager>();

        sm.currentScore--;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
