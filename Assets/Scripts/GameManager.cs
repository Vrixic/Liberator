using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public PlayerMotor playerMoveScript;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Player playerScript;
    [HideInInspector]
    public GameObject mainCamera;
    [HideInInspector]
    public Vector3 playerAimVector;
    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public CharacterController playerCharacterController;
    [HideInInspector]
    public bool playerIsGrounded;
    [HideInInspector]
    public GameObject pause;
    [HideInInspector]
    public GameObject hostageSecured;
    [HideInInspector]
    public GameObject virtualCam;
    [HideInInspector]
    public GameObject reticle;
    [HideInInspector]
    public GameObject ui;
    [HideInInspector]
    public GameObject healthBar;
    [HideInInspector]
    public HealthBar healthBarScript;
    [HideInInspector]
    public GameObject ShieldBar;
    [HideInInspector]
    public ShieldBar shieldBarScript;
    [HideInInspector]
    public ButtonFunctionality buttonFuncScript;
    [HideInInspector]
    public GameObject secureHostageText;
    [HideInInspector]
    public GameObject openDoorInteractText;
    [HideInInspector]
    public GameObject closeDoorInteractText;
    [HideInInspector]
    public GameObject intelInteractText;
    [HideInInspector]
    public Image flashbangImage;
    [HideInInspector]
    public TextMeshProUGUI flashBangCount;
    [HideInInspector]
    public GameObject flashBangIcon;
    [HideInInspector]
    public GameObject ammoIcon;
    [HideInInspector]
    public TextMeshProUGUI ammoText;
    [HideInInspector]
    public Image gunIcon;
    [HideInInspector]
    public GameObject hostageProgressBar;
    [Header("Current cash for viewing, starting cash for testing")]
    [SerializeField] private int currentCash;
    public int CurrentCash { get { return currentCash; } set { currentCash = value; } }
    [SerializeField] private int startingCash = 1000;
    [HideInInspector]
    public int CurrentXP { get { return currentXP; } set { currentXP = value; } }
    private int currentXP = 0;
    public int PreviousXP { get; set;  }
    [SerializeField] public int maxXPAmount = 100;
    public Dictionary<string, int> enemiesKilled = new Dictionary<string, int>();
    [SerializeField] List<EnemyKillReward> enemyKillXPReward = new List<EnemyKillReward>();
    private Dictionary<string, int> enemiesKillXPReward = new Dictionary<string, int>();
    public int IntelCollected { get; set; }
    [HideInInspector]
    public GameObject shopCanvas;
    [HideInInspector]
    public TextMeshProUGUI itemTabCashCountText;
    [HideInInspector]
    public TextMeshProUGUI buyWeaponTabCashCountText;
    [HideInInspector]
    public TextMeshProUGUI weaponUpgradeText;
    [HideInInspector]
    public TextMeshProUGUI weaponMaxUpgradeText;
    [HideInInspector]
    public TextMeshProUGUI sensorGrenadeCount;
    [HideInInspector]
    public GameObject sensorGrenadeIcon;
    [HideInInspector]
    public bool isCurrentWeaponUpgraded;
    [HideInInspector]
    public GameObject buyWeaponsCanvas;
    [HideInInspector]
    public GameObject minimapCanvas;
    [HideInInspector]
    public bool isPauseMenuOpen;
    [HideInInspector]
    public GameObject ammoCanvas;
    [HideInInspector]
    public float masterVolume = 1f;
    [HideInInspector]
    public float playerSensitivity = 1f;
    [HideInInspector]
    public bool isShopMenuOpen;
    public bool IsUIOverlayVisible { get; set; } = false;

    public int RewardAmount { get; set; } = 0;
    public int RewardID { get; set; } = 0;
    public bool RewardCollected { get; set; } = true;

    //used to alert enemies in the AlertEnemies method, will pickup the head collider and body collider of each enemy
    private Collider[] enemyColliders = new Collider[18]; 
    private LayerMask enemyLayerMask;

    public bool GameWon { get; set; } = false;

    public Action OnOptionsUpdateAction;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManagers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        playerScript = player.GetComponent<Player>();
        playerCharacterController = player.GetComponent<CharacterController>();
        pause = GameObject.FindGameObjectWithTag("PauseMenu");
        pause.SetActive(false);


        virtualCam = GameObject.FindGameObjectWithTag("VirtualCam");
        reticle = GameObject.FindGameObjectWithTag("Reticle");
        ui = GameObject.FindGameObjectWithTag("UI");
        buttonFuncScript = ui.GetComponent<ButtonFunctionality>();

        healthBar = GameObject.FindGameObjectWithTag("HealthBar");
        healthBarScript = healthBar.GetComponent<HealthBar>();
        ShieldBar = GameObject.FindGameObjectWithTag("ShieldBar");
        shieldBarScript = ShieldBar.GetComponent<ShieldBar>();

        secureHostageText = GameObject.FindGameObjectWithTag("SecureHostageText");
        secureHostageText.SetActive(false);

        openDoorInteractText = GameObject.FindGameObjectWithTag("OpenDoorInteractText");
        openDoorInteractText.SetActive(false);

        closeDoorInteractText = GameObject.FindGameObjectWithTag("CloseDoorInteractText");
        closeDoorInteractText.SetActive(false);

        intelInteractText = GameObject.FindGameObjectWithTag("IntelInteractText");
        intelInteractText.SetActive(false);


        flashBangIcon = GameObject.FindGameObjectWithTag("FlashBangIcon");
        flashBangCount = GameObject.FindGameObjectWithTag("FlashbangCount").GetComponent<TextMeshProUGUI>();
        flashbangImage = GameObject.FindGameObjectWithTag("FlashbangImage").GetComponent<Image>();

        ammoIcon = GameObject.FindGameObjectWithTag("AmmoIcon");
        ammoText = GameObject.FindGameObjectWithTag("AmmoCount").GetComponent<TextMeshProUGUI>();

        hostageProgressBar = GameObject.FindGameObjectWithTag("HostageProgressBar");

        shopCanvas = GameObject.FindGameObjectWithTag("ShopCanvas");
        itemTabCashCountText = GameObject.FindGameObjectWithTag("CashCountText").GetComponent<TextMeshProUGUI>();
        weaponUpgradeText = GameObject.FindGameObjectWithTag("UpgradeWeaponText").GetComponent<TextMeshProUGUI>();
        weaponMaxUpgradeText = GameObject.FindGameObjectWithTag("MaxWeaponUpgradeText").GetComponent<TextMeshProUGUI>();
        weaponMaxUpgradeText.enabled = false;
        isCurrentWeaponUpgraded = false;
        shopCanvas.SetActive(false);
        buyWeaponsCanvas = GameObject.FindGameObjectWithTag("BuyWeaponsCanvas");
        buyWeaponTabCashCountText = GameObject.FindGameObjectWithTag("CashCountText").GetComponent<TextMeshProUGUI>();
        buyWeaponsCanvas.SetActive(false);

        minimapCanvas = GameObject.FindGameObjectWithTag("MinimapCanvas");


        gunIcon = GameObject.FindGameObjectWithTag("GunIcon").GetComponent<Image>();


        sensorGrenadeCount = GameObject.FindGameObjectWithTag("SensorGrenadeCount").GetComponent<TextMeshProUGUI>();
        sensorGrenadeIcon = GameObject.FindGameObjectWithTag("SensorGrenadeIcon");

        ammoCanvas = GameObject.FindGameObjectWithTag("AmmoCanvas");

        if (player == null)
        {
            Debug.LogError("Player class cannot be found, does not exist");


        }
        playerMoveScript = player.GetComponent<PlayerMotor>();

        currentCash = startingCash;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found in scene");
        }

        // Adds listeners to options update event
        OnOptionsUpdateAction += playerScript.OnOptionsUpdate;
        OnOptionsUpdateAction += player.GetComponent<PlayerLook>().OnOptionsUpdate;

        // Takes the info from enemyKillXpReward and populates it into a dictionary, so that accessing information can be constant time later on.
        for(int i = 0; i < enemyKillXPReward.Count; i++)
        {
            enemiesKillXPReward.Add(enemyKillXPReward[i].enemyName, enemyKillXPReward[i].rewardAmount);
        }

        enemyLayerMask = LayerMask.GetMask("Enemy");

        // Loads all player perferences from last save
        LoadGame();
    }

    private void Update()
    {
        //player's position and rotation in the world
        playerTransform = player.transform;

        //getting where the player is looking which includes the rotation up and down of the main camera
        playerAimVector = mainCamera.transform.forward;

        //implemented to use for footstep audio so it doesn't play if the player is in the air, can be used or anything
        playerIsGrounded = playerCharacterController.isGrounded;
    }

    public void ResetGame()
    {
        // set the previous xp to current xp
        PreviousXP = currentXP;

        Time.timeScale = 0f;

        // Show the xp screen
        if (GameWon)
            ScreenManager.Instance.ShowScreen("XP_Screen");
        else
            ScreenManager.Instance.ShowScreen("Death_Screen");

        Debug.LogWarning("Restart your game bud, you suck!");
        //SceneManager.LoadScene(0);
    }

    /* 
     * Sets the gun icon and fades it out
     */
    public void SetGunIcon(Sprite icon)
    {
        // Calculates icons width and heights 
        float iconWidth = icon.rect.width * 0.5f;
        float iconHeight = icon.rect.height * 0.5f;

        // resize the gunIcon rect transform to fit the incoming icon and set the sprite to the incoming icon
        gunIcon.rectTransform.sizeDelta = new Vector2(iconWidth, iconHeight);
        gunIcon.rectTransform.anchoredPosition = new Vector2(iconWidth * -0.5f - 10f, gunIcon.rectTransform.anchoredPosition.y);
        gunIcon.sprite = icon;

        // Make icon visible 
        Color color = Color.white;
        color.a = .4f;
        gunIcon.color = color;

        // enables the gunIcon
        gunIcon.gameObject.SetActive(true);

        // Starts fading the icon out
        StartCoroutine(FadeOutGunIcon());
    }

    /* 
     * Fades out the currently showing gun icon
     */
    IEnumerator FadeOutGunIcon()
    {
        Color c = gunIcon.color;
        for (float alpha = c.a; alpha >= 0; alpha = gunIcon.color.a)
        {
            alpha -= Time.deltaTime * 0.1f;

            c.a = alpha;
            gunIcon.color = c;

            yield return null;
        }

        gunIcon.gameObject.SetActive(false);
    }

    /* 
    * Loads player perferences
    */
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Master Volume"))
        {
            masterVolume = PlayerPrefs.GetFloat("Master Volume", 1f);
        }
        else
        {
            masterVolume = 1f;
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
        }

        if (PlayerPrefs.HasKey("Player Sensitivity"))
        {
            playerSensitivity = PlayerPrefs.GetFloat("Player Sensitivity", 1f);
        }
        else
        {
            playerSensitivity = 1f;
            PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);
        }
    }

    /* 
    * Saves player perferences
    */
    public void SaveGame()
    {
        SaveMasterVolume();
        SavePlayerSensitivity();
         
        // Fires an event to all listening clients that player perferences has been updated
        OnOptionsUpdateAction?.Invoke();
    }

    /* 
    * Saves master volume
    */
    private void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat("Master Volume", masterVolume);

    }

    /* 
    * Saves player sensitivity
    */
    private void SavePlayerSensitivity()
    {
        PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);
    }

    /* 
    * Calculates the xp earned by incoming enemyTag
    */
    public int CalculateXPForEnemy(string enemyTag)
    {
        //Debug.Log(enemyTag + " x" + enemiesKilled[enemyTag]);
        return enemiesKilled[enemyTag] * enemiesKillXPReward[enemyTag];
    }

    [System.Serializable]
    private class EnemyKillReward
    {
        // Name of enemy type
        public string enemyName;

        // The amount of reward player gets for killing this enemy
        public int rewardAmount;
    }

    //place an overlap sphere at a specified position and collects all enemy colliders within a given radius to alert them(if idle) based on a given player action(firing, opening door, grenade etc.)
    public void AlertEnemiesInSphere(Vector3 sphereCastPosition, float sphereRadius)
    {
        //collect all colliders on the enemy layer within the alert radius
        int numberOfColliders = Physics.OverlapSphereNonAlloc(sphereCastPosition, sphereRadius, enemyColliders, enemyLayerMask);
        
        //initialize AIAgent variable that will be assigned when we try to get the component from each collider
        AIAgent agent;

        //loop through all enemy colliders in the array
        for (int i = 0; i < numberOfColliders; i++)
        {
            //enemy heads and bodies are different colliders, but only the body has the AIAgent component, so TryGetComponent will end up passing on each agent's body
            if(enemyColliders[i].TryGetComponent<AIAgent>(out agent))
            {
                //if it found the AIAgent for a given enemy collider, alert that enemy if they are currently idle(not dead, chasing, or attacking)
                if(agent.currentState == AIStateID.Idle)
                    agent.stateMachine.ChangeState(AIStateID.Alerted);
            }
        }
    }
}
