using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;

    [Header("UI Objects")]
    public GameObject titleUI;
    public GameObject gameOverUI;

    [Header("Setup Objects")]
    public GameObject basicKnife;
    public GameObject knifePrefab;
    public GameObject ballPrefab;
    public Transform knifePosition;
    public Transform ballPosition;
    public ParticleSystem knifeHitParticles;
    public ParticleSystem ballHitParticles;
    public Ball ball;

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
    private int currentStage;
    private float knifeTravelDistance;
    private float stageStartTime;

    #region PRIVATE METHODS ====================================================================================================================

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
        knifeTravelDistance = (ballPosition.position - knifePosition.position).magnitude - ballRadius;
        titleUI.SetActive(true);
        gameOverUI.SetActive(false);
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
        ballPosition.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
    }

    private void CreateKnife ()
    {
        currentKnife = Instantiate(knifePrefab, knifePosition.position, Quaternion.identity, knifePosition).GetComponent<Knife>();
    }

    private void StartStage (Stage stage)
    {
        stageStartTime = Time.time;
        //Ball setup
        GameObject newBall = Instantiate(ballPrefab, ballPosition) as GameObject;
        newBall.GetComponent<SpriteRenderer>().sprite = stage.ballGraphics;
        //Stage setup
        knifeQuantity = stage.knifeQuantity;
        rotationSpeedCurve = stage.rotationSpeedCurve;
        rotationSpeedCurveLength = stage.rotationSpeedCurveLength;
        //Starting knives
        for (int i = 0; i < stage.startingKnifePositions.Count; i++)
        {
            float angle = stage.startingKnifePositions[i];
            Vector3 anglePosition = new Vector3(ballRadius * Mathf.Cos(angle * Mathf.Deg2Rad), ballRadius * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 posUpwards = -anglePosition;
            Instantiate(basicKnife, ballPosition.position + anglePosition, Quaternion.LookRotation(Vector3.forward, posUpwards), ballPosition);
        }
        CreateKnife();
    }

    private IEnumerator EndStage (bool isWin)
    {
        Debug.Log("Game Ended with a " + (isWin ? "WIN!" : "LOSS..."));
        if (isWin)
        {
            ball.Explode();
            
            currentStage++;
            if (currentStage >= stages.Count)
            {
                EndGame();
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                StartStage(stages[currentStage]);
            }
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame ()
    {
        titleUI.SetActive(false);
        gameOverUI.SetActive(true);
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
            StartCoroutine(EndStage(true));
        }
    }
    #endregion

    #region PUBLIC METHODS ====================================================================================================================

    public void StartGame ()
    {
        titleUI.SetActive(false);
        gameOverUI.SetActive(false);
        StartStage(stages[0]);
    }

    public void HitAKnife ()
    {
        knifeHitParticles.Play();
        currentRotationSpeed = 0;
        StartCoroutine(EndStage(false));
    }

    public void HitBall ()
    {
        ballHitParticles.Play();
    }
    #endregion
}
