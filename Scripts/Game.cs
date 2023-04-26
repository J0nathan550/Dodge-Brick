using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject informationPanel; 
    [SerializeField] private Animator brickAnimator;
    [SerializeField] private AudioSource gameSource;
    [SerializeField] private AudioClip hitCatSound;
    [SerializeField] private AudioClip hitPlayerSound;
    [SerializeField] private AudioClip hitPlayerMissSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private float momentToHit = 1.2f;
    [SerializeField] private float loseTime = 1.6f;
    [SerializeField] private float flyBackTime = 1.6f;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private SpriteRenderer cat;
    [SerializeField] private Sprite ralsei;
    [SerializeField] private Sprite ralseiHappy;
    [SerializeField] private Sprite catSprite;

    private int score; 
    private int bestScore;

    private bool isGameRunning = false;
    private bool isHitted = false;
    private bool isRalseiTurnOn = false;
    private bool animationRalseiPlayed = false;

    private float reactTime = 0.8f;
    private float ralseiHappyTime = 0.2f;

    private void Start()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("bestScore", bestScore);
        }
        scoreText.text = $"Score: {score}";
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        bestScoreText.text = $"Best Score: {bestScore}";
        informationPanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isRalseiTurnOn = !isRalseiTurnOn;
            if (isRalseiTurnOn)
            {
                cat.sprite = ralsei;
            }
            else
            {
                cat.sprite = catSprite;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && !isGameRunning)
        {
            informationPanel.SetActive(false);
            score = 0;
            momentToHit = 1.2f;
            loseTime = 1.6f;
            flyBackTime = 1.6f;
            isHitted = false;
            isGameRunning = true;
            gameSource.PlayOneShot(hitCatSound);
        }
        if (isGameRunning)
        {
            reactTime -= Time.deltaTime;
            if (isRalseiTurnOn)
            {
                ralseiHappyTime -= Time.deltaTime;
            }

            if (!isHitted)
            {
                brickAnimator.SetInteger("STATE", 1);
                momentToHit -= Time.deltaTime;
                loseTime -= Time.deltaTime;
            }
            else
            {
                brickAnimator.SetInteger("STATE", 2);
                flyBackTime -= Time.deltaTime;
            }

            if (ralseiHappyTime <= 0 && isRalseiTurnOn && !animationRalseiPlayed)
            {
                cat.sprite = ralsei;
                animationRalseiPlayed = true; 
            }

            if (flyBackTime <= 0)
            {
                isHitted = false;
                if (isRalseiTurnOn)
                {
                    cat.sprite = ralseiHappy;
                    ralseiHappyTime = 0.2f;
                    animationRalseiPlayed = false;
                }
                gameSource.PlayOneShot(hitCatSound);
                flyBackTime = 1.6f;
            }

            if (reactTime <= 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (momentToHit <= 0)
                    {
                        gameSource.PlayOneShot(hitPlayerSound);
                        score++; 
                        isHitted = true;
                        momentToHit = 1.2f;
                        loseTime = 1.6f;
                        flyBackTime = 1.6f;
                        reactTime = 0.8f; 
                    }
                    else
                    {
                        gameSource.PlayOneShot(hitPlayerMissSound);
                        reactTime = 0.8f;
                    }
                }
            }
            if (loseTime <= 0 && !isHitted)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    PlayerPrefs.SetInt("bestScore", bestScore);
                }
                scoreText.text = $"Score: {score}";
                bestScore = PlayerPrefs.GetInt("bestScore", 0);
                bestScoreText.text = $"Best Score: {bestScore}";
                brickAnimator.SetInteger("STATE", 0);
                informationPanel.SetActive(true);
                gameSource.PlayOneShot(loseSound); 
                isGameRunning=false;
            }
        }
        else
        {
            brickAnimator.SetInteger("STATE", 0);
        }
    }
}