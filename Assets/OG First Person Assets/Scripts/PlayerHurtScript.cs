using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHurtScript : MonoBehaviour
{
    public GameObject deathScreen;
    private StarterAssetsInputs _inputs;
    public LayerMask enemyLayer;
    private float enemiesKilled;
    public GameObject winscreen;
    public float numberofEnemies;
    public float playerHealth = 3;
    public Slider playerHealthSlider;
    public float playerInvinceTime = 1;
    private float playerInvinceTimeTimer;
    public GameObject gameplayUI;

    public StartTimeScript startTimeScript;

    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInvinceTimeTimer -= Time.deltaTime;
        playerHealthSlider.value = playerHealth;

        if (_inputs.restart)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            gameplayUI.SetActive(true);
            SceneManager.LoadScene(currentSceneName);
        }

        if (enemiesKilled >= numberofEnemies)
        {
            winscreen.gameObject.SetActive(true);
            gameplayUI.SetActive(false);

            startTimeScript.PlayerRunEnd();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && playerInvinceTimeTimer <= 0)
        {
            playerHealth -= 1;
            if (playerHealth <= 0)
            {
                deathScreen.gameObject.SetActive(true);
                gameplayUI.SetActive(false);
            }
            playerInvinceTimeTimer = playerInvinceTime;
        } else if(other.gameObject.layer == 4)
        {
            playerHealth -= 3;
            if (playerHealth <= 0)
            {
                deathScreen.gameObject.SetActive(true);
                gameplayUI.SetActive(false);

            }
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled += 1;
    }
}
