using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Setup Objects")]
    public GameObject basicKnife;
    public GameObject knifePrefab;
    public Transform knifePosition;
    public Transform ball;
    public SpriteRenderer ballGraphics;

    [Header("Knife Variables")]
    public float knifeSpeed;
    public float timeForNewKnife;

    [Header("Ball Variables")]
    public float ballRadius;
    public float baseRotationSpeed;
    public float currentRotationSpeed;

    [Header("Stage Variables")]
    public int knifeQuantity;
    public AnimationCurve rotationSpeedCurve;
    public float rotationSpeedCurveLength;
    public List<Stage> stages;

    private Knife currentKnife;
    private float knifeTravelDistance;
    private float stageStartTime;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start ()
    {
        CreateKnife();
        knifeTravelDistance = (ball.position - knifePosition.position).magnitude - ballRadius;
        StartStage(stages[0]);
    }

    private void Update ()
    {
        if (Input.anyKeyDown)
        {
            if (currentKnife != null)
            {
                knifeQuantity--;
                StartCoroutine(MoveKnife());
                if (knifeQuantity > 0)
                    StartCoroutine(SpawnNewKnife());
            }
        }

        float currentStageTime = Time.time - stageStartTime;
        currentRotationSpeed = rotationSpeedCurve.Evaluate((currentStageTime % rotationSpeedCurveLength) / rotationSpeedCurveLength) * baseRotationSpeed;
        ball.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
    }

    private void CreateKnife ()
    {
        currentKnife = Instantiate(knifePrefab, knifePosition.position, Quaternion.identity, knifePosition).GetComponent<Knife>();
    }

    private void StartStage (Stage stage)
    {
        stageStartTime = Time.time;
        ballGraphics.sprite = stage.ballGraphics;
        knifeQuantity = stage.knifeQuantity;
        rotationSpeedCurve = stage.rotationSpeedCurve;
        rotationSpeedCurveLength = stage.rotationSpeedCurveLength;
        for (int i = 0; i < stage.startingKnifePositions.Count; i++)
        {
            float angle = stage.startingKnifePositions[i];
            Vector3 anglePosition = new Vector3(ballRadius * Mathf.Cos(angle * Mathf.Deg2Rad), ballRadius * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 posUpwards = -anglePosition;
            Instantiate(basicKnife, ball.position + anglePosition, Quaternion.LookRotation(Vector3.forward, posUpwards), ball);
        }
    }

    private void EndStage (bool isWin)
    {
        
    }

    private IEnumerator SpawnNewKnife ()
    {
        yield return new WaitForSeconds(timeForNewKnife);
        CreateKnife();
    }

    private IEnumerator MoveKnife ()
    {
        GameObject movingKnife = currentKnife.gameObject;
        currentKnife = null;
        float amountToMove = knifeTravelDistance;
        while (amountToMove > 0)
        {
            float movingAmount = knifeSpeed * Time.deltaTime;
            if (movingAmount > amountToMove)
                movingAmount = amountToMove;
            movingKnife.transform.position += Vector3.up * movingAmount;
            amountToMove -= movingAmount;
            yield return null;
        }

        if (knifeQuantity <= 0)
        {
            EndStage(true);
        }
    }

    public void HitAKnife ()
    {
        EndStage(false);
    }
}
