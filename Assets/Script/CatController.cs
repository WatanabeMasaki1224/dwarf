using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum CatState
{
    Patrol,
    Chase,
    Search
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


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerControlle = player.GetComponent<PlayerContollore>();
        gameOver =FindAnyObjectByType<GameOverController>();
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
        }
    }

    void PatrolUpdate()
    {
        //巡回処理
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveNextPoint();
        }
        //近づいたら振り返る
        TurnAroundCat();

        if(PlayerInView())
        {
            currentState = CatState.Chase;
        }
    }

    void ChaseUpdate()
    {
        agent.SetDestination(player.position);
        lastPosition = player.position;

        // ハイド中or視界から消える
        if (!PlayerInView() || playerControlle.isHidden)
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
        searchTimer -= Time.deltaTime;
        moveTimer -= Time.deltaTime;
        //プレイヤーが視界に入ったら追跡
        if (PlayerInView())
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

    void MoveNextPoint() //巡回
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentIndex].position);
        currentIndex = (currentIndex +1) % patrolPoints.Length;
    }

    void TurnAroundCat() //振り向く
    {
        float distance = Vector3.Distance(transform.position,player.position);
        if (distance < noticeRange)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }

    bool PlayerInView()
    {
        Vector3 dirToPlayer =(player.position - transform.position).normalized;
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
        if(other.CompareTag("Player"))
        {
            gameOver.PlayGameOver();
        }
    }

}
