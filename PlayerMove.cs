using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float MoveSpeed = 5f;

    public int hp = 100;

    public int Defensive = 0;

    public int AttackPower = 10;

    public float attackDistance = 3f;

    private float curTime;

    public float coolTime = 0.5f;

    public Transform pos;

    public Vector2 boxSize;

    int Maxhp = 100;

    public Slider hpSlider;

    public GameObject PauseObject;

    public GameObject GameOverUI;

    public float jumpPower = 10f;

    bool isJumping = false;

    bool isflip = false;

    bool isattacked = false;

    bool isknockback = false;

    private Animator animator;

    private new Rigidbody2D rigidbody;

    public GameObject smObject;

    public Transform melee;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        smObject = GameObject.Find("ScoreManager");
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name.Contains("Potion"))
        {
            hp = Maxhp;
            other.gameObject.SetActive(false);
        }
        if (other.gameObject.name.Contains("Portal"))
        {
            ScoreManager sm = smObject.GetComponent<ScoreManager>();

            if (sm.currentScore == 0)
            {
                SceneManager.LoadScene("ClearMenu");
            }
        }
    }   
        void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseObject.SetActive(true);

            Time.timeScale = 0f;
        }

        float h = Input.GetAxis("Horizontal");

        if (h < 0)
        {
            if (!isflip)
            {
                this.transform.Rotate(0, 180, 0);
            }
            animator.SetTrigger("isRun");
            isflip = true;
            
        }
        else if (h > 0)
        {
            if (isflip)
            {
                this.transform.Rotate(0, 180, 0);
            }
            animator.SetTrigger("isRun");
            isflip = false;
        }
        else
        {
            animator.SetTrigger("isIdle");
        }

        rigidbody.velocity = new Vector2(h * MoveSpeed, rigidbody.velocity.y);

        hpSlider.value = (float)hp / (float)Maxhp;

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rigidbody.velocity = Vector2.up * jumpPower;
            isJumping = true;
        }

        if (curTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                animator.SetTrigger("isAttack");
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.gameObject.name.Contains("Enemy"))
                    {
                        collider.GetComponent<Enemy>().HitEnemy(AttackPower,transform.position);
                    }
                   
                }
                curTime = coolTime;
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        Debug.DrawRay(rigidbody.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigidbody.position, Vector3.down, 1,LayerMask.GetMask("Ground"));

        if (rayHit.collider != null)
        {
            isJumping = false;
        }
    }
    public void HitPlayer(int hitPower, Vector2 pos) //맞고 계산되는 함수
    {
        if (!isattacked)
        {
            isattacked = true;
            hp -= (hitPower - Defensive);

            if (hp > 0)
            {
                animator.SetTrigger("isDamaged");
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
                Die();
            }
        }
    }
    IEnumerator Knockback(float dir)
    {
        isknockback = true;
        float ctime = 0;
        while (ctime < 0.6f)
        {
            if (transform.rotation.y == 0)
                transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime * dir * -1f);

            ctime += Time.deltaTime;
            yield return null;
        }
        isknockback = false;
    }

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(5f);
        isattacked = false;
    }
    private void Die()
    {
        animator.SetTrigger("isDie");

        GameOverUI.SetActive(true);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
