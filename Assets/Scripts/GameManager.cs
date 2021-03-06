using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject levelOne;
    [SerializeField] GameObject levelTwo;
    [SerializeField] GameObject levelThree;
    [SerializeField] GameObject levelFour;
    [SerializeField] GameObject levelFive;

    #region Player related variables
    [HideInInspector]
    public PlayerMotor playerMoveScript;
    [HideInInspector]
    public PlayerLook playerLookScript;
    [HideInInspector]
    public PlayerInteract playerInteractScript;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Player playerScript;
    [HideInInspector]
    public CameraShake cameraShakeScript;
    [HideInInspector]
    public GameObject mainCamera;
    [HideInInspector]
    public Camera mainCameraComponent;
    [HideInInspector]
    public Vector3 playerAimVector;
    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public CharacterController playerCharacterController;
    //   [HideInInspector]
    public bool playerIsGrounded;
    #endregion

    #region UI Variables
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
    public GameObject infomationTriggerText;
    [HideInInspector]
    public GameObject secureHostageText;
    [HideInInspector]
    public GameObject openDoorInteractText;
    [HideInInspector]
    public GameObject closeDoorInteractText;
    [HideInInspector]
    public GameObject intelInteractText;
    [HideInInspector]
    public GameObject openShopInteractText;
    [SerializeField] public GameObject doorLockedPrompt;
    [SerializeField] public GameObject hostageDoorPrompt;
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
    public DISystem damageIndicatorSystem;
    [HideInInspector]
    public GameObject hostageProgressBar;
    [HideInInspector]
    public GameObject shopCanvas;
    [HideInInspector]
    public TMP_Text itemTabCashCountText;
    [HideInInspector]
    public TMP_Text buyWeaponTabCashCountText;
    [HideInInspector]
    public TMP_Text weaponUpgradeText;
    [HideInInspector]
    public TMP_Text weaponMaxUpgradeText;
    [HideInInspector]
    public TMP_Text sensorGrenadeCount;
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
    public TMP_Text cashGainedText;
    [HideInInspector]
    public TMP_Text ammoGainedText;
    [HideInInspector]
    public TMP_Text healthGainedText;
    [HideInInspector]
    float ItemGainedDecaytimer = 0;
    [HideInInspector]
    public int cashRewardAmount;
    [HideInInspector]
    public bool isShopMenuOpen;
    [HideInInspector]
    public GameObject settingsMenu;
    [HideInInspector]
    public bool canOpenPauseMenu;
    [HideInInspector]
    public Fps_Counter fpsCounter;
    [HideInInspector]
    public Button weaponUpgradeButton;
    [HideInInspector]
    public bool isXPScreenActive;
    Color textColor = new Color(39, 255, 0);
    Color clearcolor = Color.clear;
    public bool IsUIOverlayVisible { get; set; } = false;
    #endregion

    #region Player Stats and info Variables
    [Header("Current cash for viewing, starting cash for testing")]
    [SerializeField] private int currentCash;
    public int CurrentCash { get { return currentCash; } set { currentCash = value; } }
    [SerializeField] private int startingCash = 1000;
    // public int PreviousXP { get; set; }
    [SerializeField] public int maxXPAmount = 100;
    public Dictionary<string, int> enemiesKilled = new Dictionary<string, int>();
    [SerializeField] List<EnemyKillReward> enemyKillXPReward = new List<EnemyKillReward>();
    private Dictionary<string, int> enemiesKillXPReward = new Dictionary<string, int>();
    public int IntelCollected { get; set; }


    public int RewardAmount { get; set; } = 0;
    public int RewardID { get; set; } = 0;
    public bool RewardCollected { get; set; } = true;
    #endregion

    //current hostage door transform for playing dooropen audio once the XP screen is closed(justified I promise)
    [HideInInspector]
    public Transform currentHostageDoorTransform = null;

    //used to alert enemies in the AlertEnemies method, will pickup the head collider and body collider of each enemy
    private Collider[] enemyColliders = new Collider[18];
    private LayerMask enemyLayerMask;

    public bool GameWon { get; set; } = false;
    public bool HostageDied { get; set; } = false;

    public int HeadshotHits { get; set; } = 0;
    public int BodyshotHits { get; set; } = 0;

    public int HeadshotPercentage { get; set; } = 0;

    public Action OnRewardCollected;

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

        if(levelTwo != null)
        {
            levelTwo.SetActive(false);
            levelThree.SetActive(false);
            levelFour.SetActive(false);
            levelFive.SetActive(false);
        }

        Instance = this;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Hostage.hostagesSecured = 0;

        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        playerScript = player.GetComponent<Player>();
        playerCharacterController = player.GetComponent<CharacterController>();
        cameraShakeScript = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();

        #region Ui Related Variables being Set
        pause = GameObject.FindGameObjectWithTag("PauseMenu");
        pause.SetActive(false);

        virtualCam = GameObject.FindGameObjectWithTag("VirtualCam");
        reticle = GameObject.FindGameObjectWithTag("Reticle");
        ui = GameObject.FindGameObjectWithTag("UI");
        fpsCounter = ui.GetComponentInChildren<Fps_Counter>();
        buttonFuncScript = ui.GetComponent<ButtonFunctionality>();
        damageIndicatorSystem = ui.GetComponent<DISystem>();

        healthBar = GameObject.FindGameObjectWithTag("HealthBar");
        healthBarScript = healthBar.GetComponent<HealthBar>();
        ShieldBar = GameObject.FindGameObjectWithTag("ShieldBar");
        shieldBarScript = ShieldBar.GetComponent<ShieldBar>();

        infomationTriggerText = GameObject.FindGameObjectWithTag("InformationalTriggerText");
        infomationTriggerText.SetActive(false);

        secureHostageText = GameObject.FindGameObjectWithTag("SecureHostageText");
        secureHostageText.SetActive(false);

        openDoorInteractText = GameObject.FindGameObjectWithTag("OpenDoorInteractText");
        openDoorInteractText.SetActive(false);

        closeDoorInteractText = GameObject.FindGameObjectWithTag("CloseDoorInteractText");
        closeDoorInteractText.SetActive(false);

        intelInteractText = GameObject.FindGameObjectWithTag("IntelInteractText");
        intelInteractText.SetActive(false);

        openShopInteractText = GameObject.FindGameObjectWithTag("OpenShopInteractText");
        openShopInteractText.SetActive(false);

        doorLockedPrompt.SetActive(false);
        hostageDoorPrompt.SetActive(false);

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
        weaponUpgradeButton = GameObject.FindGameObjectWithTag("UpgradeWeaponButton").GetComponent<Button>();
        shopCanvas.SetActive(false);
        buyWeaponsCanvas = GameObject.FindGameObjectWithTag("BuyWeaponsCanvas");
        buyWeaponTabCashCountText = GameObject.FindGameObjectWithTag("CashCountText").GetComponent<TextMeshProUGUI>();
        buyWeaponsCanvas.SetActive(false);

        minimapCanvas = GameObject.FindGameObjectWithTag("MinimapCanvas");
        minimapCanvas.SetActive(true);

        gunIcon = GameObject.FindGameObjectWithTag("GunIcon").GetComponent<Image>();


        sensorGrenadeCount = GameObject.FindGameObjectWithTag("SensorGrenadeCount").GetComponent<TextMeshProUGUI>();
        sensorGrenadeIcon = GameObject.FindGameObjectWithTag("SensorGrenadeIcon");

        ammoCanvas = GameObject.FindGameObjectWithTag("AmmoCanvas");

        cashGainedText = GameObject.FindGameObjectWithTag("CashGainedText").GetComponent<TextMeshProUGUI>();
        cashGainedText.enabled = false;
        cashRewardAmount = 0;

        ammoGainedText = GameObject.FindGameObjectWithTag("AmmoGainedText").GetComponent<TextMeshProUGUI>();
        ammoGainedText.enabled = false;

        healthGainedText = GameObject.FindGameObjectWithTag("HealthGainedText").GetComponent<TextMeshProUGUI>();
        healthGainedText.enabled = false;

        settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        settingsMenu.SetActive(false);

        #endregion

        if (player == null)
        {
            Debug.LogError("Player class cannot be found, does not exist");


        }
        playerMoveScript = player.GetComponent<PlayerMotor>();
        playerInteractScript = player.GetComponent<PlayerInteract>();

        // Sets starting cash to player pref for starting cash
        startingCash = PlayerPrefManager.Instance.startingCash;
        currentCash = startingCash;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found in scene");
        }
        mainCameraComponent = mainCamera.GetComponent<Camera>();

        playerLookScript = player.GetComponent<PlayerLook>();
        playerLookScript.OnSensitivityUpdate();

        // Takes the info from enemyKillXpReward and populates it into a dictionary, so that accessing information can be constant time later on.
        for (int i = 0; i < enemyKillXPReward.Count; i++)
        {
            enemiesKillXPReward.Add(enemyKillXPReward[i].enemyName, enemyKillXPReward[i].rewardAmount);
        }

        enemyLayerMask = LayerMask.GetMask("Enemy");

        // Loads all player perferences from last save
        PlayerPrefManager.Instance.LoadGame();
        RenderSettings.ambientLight = new Color(PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, 1.0f);

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
        //make sure that the interaction prompt gets disabled
        if (Instance.playerInteractScript.currentInteractPrompt != null)
            Instance.playerInteractScript.currentInteractPrompt.SetActive(false);

        Time.timeScale = 0f;

        // Show the xp screen
        if (GameWon || HostageDied)
            ScreenManager.Instance.ShowScreen("XP_Screen");
        else
            ScreenManager.Instance.ShowScreen("Death_Screen");

        //SceneManager.LoadScene(0);
    }

    #region Text Pop up Coroutines/Methods
    public void StartDisplayCashCoroutine()
    {
        ResetTimer();
        StartCoroutine(DisplayCashReward());
    }
    public void StartDisplayAmmoGainedCoroutine()
    {
        ResetTimer();
        StartCoroutine(DisplayAmmoGained());
    }
    public void StartDisplayHealthGainedCoroutine()
    {
        ResetTimer();
        StartCoroutine(DisplayHealthGained());
    }

    public void ResetTimer()
    {
        StopCoroutine(DisplayCashReward());
        ItemGainedDecaytimer = 0;
    }
    IEnumerator DisplayAmmoGained()
    {
        // Reset timer to 0
        ItemGainedDecaytimer = 0;

        // Enable text 
        ammoGainedText.enabled = true;
        // Set cashGainedText color to current color of text
        ammoGainedText.color = textColor;
        // Change text to ++
        ammoGainedText.text = "Ammo++";

        // Begin Timer
        while (ItemGainedDecaytimer < 1)
        {

            ammoGainedText.color = Color.Lerp(textColor, clearcolor, ItemGainedDecaytimer);
            ItemGainedDecaytimer += Time.unscaledDeltaTime * 0.5f;
            yield return null;
        }

    }

    IEnumerator DisplayHealthGained()
    {
        // Reset timer to 0
        ItemGainedDecaytimer = 0;

        // Enable text 
        healthGainedText.enabled = true;
        // Set cashGainedText color to current color of text
        healthGainedText.color = textColor;
        // Change text to ++
        healthGainedText.text = "Health++";

        // Begin Timer
        while (ItemGainedDecaytimer < 1)
        {

            healthGainedText.color = Color.Lerp(textColor, clearcolor, ItemGainedDecaytimer);
            ItemGainedDecaytimer += Time.unscaledDeltaTime * 0.5f;
            yield return null;
        }

    }

    IEnumerator DisplayCashReward()
    {
        // Reset timer to 0
        ItemGainedDecaytimer = 0;

        // Enable text 
        cashGainedText.enabled = true;
        // Set cashGainedText color to current color of text
        cashGainedText.color = textColor;
        // Change text to cash rewards
        cashGainedText.text = "+ $" + cashRewardAmount;
        cashRewardAmount = 0;

        // Begin Timer
        while (ItemGainedDecaytimer < 1)
        {

            cashGainedText.color = Color.Lerp(textColor, clearcolor, ItemGainedDecaytimer);
            ItemGainedDecaytimer += Time.unscaledDeltaTime * 0.5f;
            yield return null;
        }

    } 

    #endregion



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
            if (enemyColliders[i].TryGetComponent<AIAgent>(out agent))
            {
                //if it found the AIAgent for a given enemy collider, alert that enemy if they are currently idle(not dead, chasing, or attacking)
                if (agent.currentState == AIStateID.Idle)
                    agent.stateMachine.ChangeState(AIStateID.Alerted);
            }
        }
    }

    public void ToggleFpsCounter()
    {
        fpsCounter.ToggleCounterDisplay();
    }
}
