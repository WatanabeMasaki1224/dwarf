using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerControlle = player.GetComponent<PlayerContollore>();
        gameOver =FindAnyObjectByType<GameOverController>();
        anim = GetComponent<Animator>();
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
    }

    void PatrolUpdate()
    {   
        agent.speed = patrolSpeed;
        //巡回処理
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveNextPoint();
        }
        //近づいたら振り返る
       // TurnAroundCat();

        if(CanDetectPlayer())
        {
            currentState = CatState.Chase;
        }
    }

    void ChaseUpdate()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        lastPosition = player.position;

        // ハイド中or視界から消える
        if (!CanDetectPlayer() || playerControlle.isHidden)
        {
            currentState = CatState.Search;
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
        //プレイヤーが視界に入ったら追跡
        if (CanDetectPlayer())
        {
            currentState = CatState.Chase;
            return;
        }
        // 最後に見た位置に到達or時間切れ
        if (!movingPatrolPoint && (!agent.pathPending && agent.remainingDistance < 0.3f || moveTimer <= 0f))
        {
            Transform nearest = GetNearestPatrolPoint();
            agent.SetDestination(nearest.position);
            currentIndex = System.Array.IndexOf(patrolPoints, nearest);
            movingPatrolPoint = true;
        }

        // 巡回地点に戻ったら Patrol へ
        if (movingPatrolPoint && !agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentState = CatState.Patrol;
            MoveNextPoint();
        }
    }

    void SoundUpdate()
    {
        //音アイテムが消えたら巡回に戻る
        if(currentSound  == null)
        {
            agent.isStopped = false;
            currentState = CatState.Patrol;
            return;
        }
        //音の位置へ移動
        agent.SetDestination(currentSound.transform.position);
        //到着したらその場で待機
        if(!agent.pathPending && agent.remainingDistance < 0.3f)
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

    /*void TurnAroundCat() //振り向く
    {
        float distance = Vector3.Distance(transform.position,player.position);
        if (distance < noticeRange)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }*/

    bool CanDetectPlayer()
    {
        Vector3 dirToPlayer =(player.position - transform.position);
        float distance = dirToPlayer.magnitude;
        //背後でも近距離なら感知
        if (distance <= backDetectDistance)
            return true;

        // ② 前方視界判定
        dirToPlayer.Normalize();
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if(angle <= viewAngle * 0.5f && Vector3.Distance(transform.position, player.position) <= viewDistance)
        {
            return true;
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

        if (other.CompareTag("Player"))
        {
            gameOver.PlayGameOver();
            return;
        }

        if (other.CompareTag("SoundItem"))
        {
            if(currentState != CatState.Chase)
            {
                currentSound = other.gameObject;
                currentState = CatState.SoundSearch;
            }
            
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
            Vector3 dir = agent.velocity.normalized;
            dir.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime*10f);
        }
    }

}
