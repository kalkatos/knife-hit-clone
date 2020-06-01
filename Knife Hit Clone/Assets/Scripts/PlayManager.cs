using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Classe básica com a funcionalidade do jogo.
/// Atualmente é responsável por 3 coisas:
/// - Controlar as mecânicas do jogo;
/// - Guardar, inicializar e trocar fases;
/// - Atualizar a UI
/// </summary>
public class PlayManager : MonoBehaviour
{
	public static PlayManager instance;

	[Header("UI Objects")]
	public GameObject titleUI;
	public GameObject gameOverUI;
	public GameObject gameUI;
	public TextMeshProUGUI gameOverScoreUI;
	public KnifeCounters knifeCountersUI;
	public TextMeshProUGUI stageNameUI;
	public TextMeshProUGUI scoreCounterUI;

	[Header("Setup Objects")]
	public GameObject basicKnife;
	public GameObject knifePrefab;
	public GameObject ballPrefab;
	public Transform knifePosition;
	public Transform ballPosition;
	public ParticleSystem knifeHitParticles;
	public ParticleSystem ballHitParticles;

	[Header("Variables")]
	public float fallSpeed;
	public float knifeSpeed;
	public float timeForNewKnife;
	public float ballRadius;
	public float baseRotationSpeed;
	public float currentRotationSpeed;

	[Header("Stage Variables")]
	public int knifeQuantity;
	public AnimationCurve rotationSpeedCurve;
	public float rotationSpeedCurveLength;
	public List<Stage> stages;

	private Knife currentKnife;
	private Transform movingKnife;
	private float knifeTravelDistance;
	private float stageStartTime;
	private bool gameRunning;
	private int currentStage;
	private int knivesThrew;

	#region PRIVATE METHODS ====================================================================================================================

	private void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

	private void Start ()
	{
		knifeTravelDistance = (ballPosition.position - knifePosition.position).magnitude - ballRadius;
		
		if (GameManager.instance.skipMenu)
		{
			StartGame();
		}
		else
		{
			titleUI.SetActive(true);
			gameOverUI.SetActive(false);
			gameUI.SetActive(false);
		}
	}

	private void Update ()
	{
		//Se o jogador clicou, atirar uma faca
		if (gameRunning)
		{
			if (Input.anyKeyDown)
			{
				if (currentKnife != null)
				{
					knifeQuantity--;
					knifeCountersUI.Expire();
					knivesThrew++;
					scoreCounterUI.text = knivesThrew.ToString();
					StartCoroutine(MoveKnife());
					if (knifeQuantity > 0)
						StartCoroutine(CreateKnifeAfterSeconds());
				}
			}

			//Gira a bola
			float currentStageTime = Time.time - stageStartTime;
			currentRotationSpeed = rotationSpeedCurve.Evaluate((currentStageTime % rotationSpeedCurveLength) / rotationSpeedCurveLength) * baseRotationSpeed;
			ballPosition.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
		}
	}

	private void StartStage (Stage stage)
	{
		stageStartTime = Time.time;
		//Setup da bola
		GameObject newBall = Instantiate(ballPrefab, ballPosition) as GameObject;
		SpriteRenderer[] renderers = newBall.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sprite = stage.ballGraphics;
		}
		//Setup da fase
		gameRunning = true;
		knifeQuantity = stage.knifeQuantity;
		knifeCountersUI.Set(knifeQuantity);
		stageNameUI.text = stage.stageInfo;
		rotationSpeedCurve = stage.rotationSpeedCurve;
		rotationSpeedCurveLength = stage.rotationSpeedCurveLength;
		//Setup das facas iniciais
		for (int i = 0; i < stage.startingKnifePositions.Count; i++)
		{
			float angle = stage.startingKnifePositions[i];
			Vector3 anglePosition = new Vector3(ballRadius * Mathf.Cos(angle * Mathf.Deg2Rad), ballRadius * Mathf.Sin(angle * Mathf.Deg2Rad));
			Vector3 posUpwards = -anglePosition;
			Instantiate(basicKnife, ballPosition.position + anglePosition, Quaternion.LookRotation(Vector3.forward, posUpwards), ballPosition);
		}
		CreateKnife();
	}

	private void CreateKnife ()
	{
		if (!gameRunning)
			return;
		currentKnife = Instantiate(knifePrefab, knifePosition.position, Quaternion.identity, knifePosition).GetComponent<Knife>();
		currentKnife.PlayAnimation();
	}

	private IEnumerator CreateKnifeAfterSeconds ()
	{
		yield return new WaitForSeconds(timeForNewKnife);
		CreateKnife();
	}

	private IEnumerator MoveKnife ()
	{
		movingKnife = currentKnife.transform;
		currentKnife = null;
		float amountToMove = knifeTravelDistance;
		while (amountToMove > 0)
		{
			if (!gameRunning)
			{
				yield break;
			}
			float movingAmount = knifeSpeed * Time.deltaTime;
			if (movingAmount > amountToMove)
				movingAmount = amountToMove;
			movingKnife.position += Vector3.up * movingAmount;
			amountToMove -= movingAmount;
			yield return null;
		}
	}

	private void Explode ()
	{
		for (int i = 0; i < ballPosition.childCount; i++)
		{
			Transform child = ballPosition.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				StartCoroutine(MoveRandomly(child.GetChild(j)));
			}
			StartCoroutine(MoveRandomly(child));
		}
	}
	private IEnumerator MoveRandomly (Transform obj, float fallSpeedMult = 1.0f)
	{
		obj.SetParent(null);
		float randomAngle = Random.Range(180f, 360f);
		float randomRotationSpeed = Random.Range(80f, 180f);
		Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
		for (int i = 0; i < 100; i++)
		{
			obj.position += randomDirection * fallSpeed * fallSpeedMult * Time.deltaTime;
			obj.Rotate(0, 0, randomRotationSpeed * Time.deltaTime);
			yield return null;
		}
		Destroy(obj.gameObject);
	}

	private IEnumerator EndStage (bool isWin)
	{
		Debug.Log("Game Ended with a " + (isWin ? "WIN!" : "LOSS..."));
		if (isWin)
		{
			Explode();

			yield return new WaitForSeconds(0.5f);
			currentStage++;
			if (currentStage >= stages.Count)
			{
				EndGame();
			}
			else
			{
				StartStage(stages[currentStage]);
			}
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			EndGame();
		}
	}

	private void EndGame ()
	{
		titleUI.SetActive(false);
		gameUI.SetActive(false);
		gameOverUI.SetActive(true);
		gameOverScoreUI.text = knivesThrew.ToString();
	}

	#endregion

	#region PUBLIC METHODS ====================================================================================================================

	public void StartGame ()
	{
		titleUI.SetActive(false);
		gameOverUI.SetActive(false);
		gameUI.SetActive(true);
		StartStage(stages[0]);
	}

	public void HitAKnife ()
	{
		gameRunning = false;

		if (movingKnife)
			StartCoroutine(MoveRandomly(movingKnife, 0.75f));
		knifeHitParticles.Play();
		StartCoroutine(EndStage(false));
	}

	public void HitBall ()
	{
		if (!gameRunning)
			return;

		ballHitParticles.Play();
		if (knifeQuantity <= 0)
		{
			StartCoroutine(EndStage(true));
		}
	}

	public void RestartGame ()
	{
		GameManager.instance.RestartGame();
	}
	#endregion
}
