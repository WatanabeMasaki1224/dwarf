using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;


public enum CatState
{
    Patrol,
    Chase,
    Search,
    SoundSearch
}

public class CatController : MonoBehaviour
{
    public Transform[] patrolPoints;　//じゅんかいポイント
    int currentIndex = 0;
    NavMeshAgent agent;
    public float noticeRange = 3f;  //気づく距離
    public float turnSpeed = 5f;  //振り向く速さ
    Transform player;
    public float viewAngle = 60f;  //視野角
    public float viewDistance = 8f; //見える距離
    CatState currentState = CatState.Patrol;
    public float searchTime = 3f;　//見失う時間
    float searchTimer;
    PlayerContollore playerControlle;
    Vector3 lastPosition;　//最後にプレイヤーを見た位置
    bool movingPatrolPoint; //巡回地点に戻り中か
    public float maxSearchMoveTime = 5f;
    float moveTimer;
    GameOverController gameOver;
    GameObject currentSound;
    public float soundStayTime = 2f; //音の場所で待つ時間
    float soundTimer;
    public float patrolSpeed = 1.5f; // 巡回速度
    public float chaseSpeed = 3.5f;  // 追跡速度
    public float soundSpeed = 2.5f;  // 音調査速度
    Animator anim;
    public float backDetectDistance = 1.5f;
    Transform target;
    public GameObject alertMark;
    public GameObject questionMark;
    public AudioClip alertSE;
    private AudioSource audioSource;
    public AudioClip meowSE;
    public float meowVolume = 0.5f;  
    public float alertVolume = 0.7f; 
    public float minMeowInterval = 5f;
    public float maxMeowInterval = 15f;
    float meowTimer;
    [Header("Catch")]
    public float catchDistance = 0.6f; // 猫が捕まえる距離
    bool isGameOver = false;




    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player.Find("TargetPoint");
        playerControlle = player.GetComponent<PlayerContollore>();
        gameOver =FindAnyObjectByType<GameOverController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ResetMeowTimer();
        MoveNextPoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case CatState.Patrol:
                PatrolUpdate();
                break;
            case CatState.Chase:
                ChaseUpdate();
                break;
            case CatState.Search:
                SearchUpdate();
                break;
            case CatState.SoundSearch:
                SoundUpdate();
                break;

        }
        UpdateAnimation();
        UpdateRotation();
        CheckCatchPlayer();
    }

    void PatrolUpdate()
    {
        agent.speed = patrolSpeed;
        //巡回処理
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveNextPoint();
        }

        if (CanDetectPlayer())
        {
            ChangeState(CatState.Chase);
        }
        meowTimer -= Time.deltaTime;
        if (meowTimer <= 0f)
        {
            if (meowSE != null)
            {
                audioSource.PlayOneShot(meowSE,meowVolume);
            }
            ResetMeowTimer();
        }
    }

    void ChaseUpdate()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
        lastPosition = target.position;

        // ハイド中or視界から消える
        if (!CanDetectPlayer() || playerControlle.isHidden)
        {
            ChangeState(CatState.Search);
            searchTimer = searchTime;
            moveTimer = maxSearchMoveTime;
            movingPatrolPoint = false;
            //最後に見た位置に向かう
            agent.SetDestination(lastPosition);
        }
    }

    void SearchUpdate()
    {
        agent.speed= soundSpeed;
        searchTimer -= Time.deltaTime;
        moveTimer -= Time.deltaTime;
        // ★ハイド中は絶対に追跡に戻らない
        if (!playerControlle.isHidden && CanDetectPlayer())
        {
            ChangeState(CatState.Chase);
            return;
        }

        // ★一定時間は必ず探索する（即Patrolに戻らない）
        if (searchTimer <= 0f)
        {
            Transform nearest = GetNearestPatrolPoint();
            agent.SetDestination(nearest.position);
            currentIndex = System.Array.IndexOf(patrolPoints, nearest);
            movingPatrolPoint = true;
        }

        // 巡回地点に戻ったら Patrol へ
        if (movingPatrolPoint && !agent.pathPending && agent.remainingDistance < 0.3f)
        {
            ChangeState(CatState.Patrol);
            MoveNextPoint();
        }
    }

    void SoundUpdate()
    {
        agent.stoppingDistance = 0f;

        if (currentSound == null)
        {
            agent.isStopped = false;
            ChangeState(CatState.Patrol);
            return;
        }

        Vector3 soundPos = currentSound.transform.position;

        // ★ 猫→鈴の方向
        Vector3 dir = (soundPos - transform.position).normalized;

        // ★ 鈴の手前で止まる位置
        float offset = 0.6f; // 猫の体サイズに合わせて調整
        Vector3 targetPos = soundPos - dir * offset;

        agent.SetDestination(targetPos);

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            agent.isStopped = true;
        }
    }

    void MoveNextPoint() //巡回
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentIndex].position);
        currentIndex = (currentIndex +1) % patrolPoints.Length;
    }

    bool CanDetectPlayer()
    {
        if (currentState == CatState.SoundSearch) return false;
        if (playerControlle.isHidden) return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dirToPlayer = target.position - origin;
        float distance = dirToPlayer.magnitude;

        // 背後でも超近距離なら感知
        if (distance <= backDetectDistance)
            return true;

        if (distance > viewDistance)
            return false;
        
  
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;
        // 遮蔽チェック
        RaycastHit hit;
        if (Physics.Raycast(origin, dirToPlayer.normalized, out hit, viewDistance))
        {
            if (hit.transform.root.CompareTag("Player"))
            {
                return true; // 直接見えている
            }
        }

        return false;
    }

    Transform GetNearestPatrolPoint()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform point in patrolPoints)
        {
            float dist = Vector3.Distance(transform.position, point.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = point;
            }
        }
        return nearest;
    }

    private void OnTriggerEnter(Collider other)
    {
        //soundsearch中はプレイヤー無視
        if (currentState == CatState.SoundSearch)
        {
            return;
        }

        if (other.CompareTag("SoundItem"))
        {
                currentSound = other.gameObject;
                ChangeState(CatState.SoundSearch);
        }
    }

    void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude;
        anim.SetBool("isWalk",speed > 0.1f);
        anim.speed = Mathf.Lerp(0.3f,1.0f,speed/chaseSpeed);
    }

    void UpdateRotation()
    {
        if(agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 dir;
            if(currentState == CatState.Chase)
            {
                dir = (target.position - transform.position).normalized;
            }
            else if(agent.velocity.sqrMagnitude > 0.01f)
            {
                dir = agent.velocity.normalized;
            }
            else return;
            dir.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime*10f);
        }
    }

    void ChangeState(CatState newState)
    {
        if (currentState == newState) return;
        CatState oldState = currentState;
        currentState = newState;
        //発見時！
        if (newState == CatState.SoundSearch || newState ==CatState.Chase)
        {
            AllMark(alertMark);
            if (alertSE != null)
            {
                audioSource.PlayOneShot(alertSE,alertVolume);
            }
        }
        //見失い？
        if(oldState == CatState.Chase && newState == CatState.Search)
        {
            AllMark(questionMark);
        }
    }

    void AllMark(GameObject mark)
    {
        if(alertMark == null) return;
        // 両方消す
        if (alertMark != null) alertMark.SetActive(false);
        if (questionMark != null) questionMark.SetActive(false);
        //指定したマークだけ表示
        mark.SetActive(true);
        CancelInvoke(nameof(HideAllMark));
        Invoke(nameof(HideAllMark),2.0f);
        
    }
    void HideAllMark()
    {
        if(alertMark!=null)alertMark.SetActive(false);
        if(questionMark!=null) questionMark.SetActive(false);
    }

    public void StopCat()
    {
        enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ===== 視界距離（前方） =====
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // 視界の中心線
        Gizmos.DrawLine(origin, origin + transform.forward * viewDistance);

        // 左右の視界ライン
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(origin, origin + leftDir * viewDistance);
        Gizmos.DrawLine(origin, origin + rightDir * viewDistance);

        // ===== 背後近距離感知 =====
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, backDetectDistance);
    }

    void ResetMeowTimer()
    {
        meowTimer = Random.Range(minMeowInterval, maxMeowInterval);
    }

    void CheckCatchPlayer()
    {
        if (isGameOver) return;
        if (currentState != CatState.Chase) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= catchDistance)
        {
            isGameOver = true;
            gameOver.PlayGameOver();
        }
    }

}
