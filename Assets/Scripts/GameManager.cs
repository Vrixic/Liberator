using System.Collections;
using TMPro;
using UnityEngine;
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
    public GameObject shopCanvas;
    [HideInInspector]
    public TextMeshProUGUI cashCountText;
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

    private SpawnPoint[] spawnPoints;

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

        hostageSecured = GameObject.FindGameObjectWithTag("HostageSecuredScreen");
        hostageSecured.SetActive(false);

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
        cashCountText = GameObject.FindGameObjectWithTag("CashCountText").GetComponent<TextMeshProUGUI>();
        weaponUpgradeText = GameObject.FindGameObjectWithTag("UpgradeWeaponText").GetComponent<TextMeshProUGUI>();
        weaponMaxUpgradeText = GameObject.FindGameObjectWithTag("MaxWeaponUpgradeText").GetComponent<TextMeshProUGUI>();
        weaponMaxUpgradeText.enabled = false;
        isCurrentWeaponUpgraded = false;
        shopCanvas.SetActive(false);

        gunIcon = GameObject.FindGameObjectWithTag("GunIcon").GetComponent<Image>();


        sensorGrenadeCount = GameObject.FindGameObjectWithTag("SensorGrenadeCount").GetComponent<TextMeshProUGUI>();
        sensorGrenadeIcon = GameObject.FindGameObjectWithTag("SensorGrenadeIcon");

        if (player == null)
        {
            Debug.LogError("Player class cannot be found, does not exist");

            playerMoveScript = player.GetComponent<PlayerMotor>();
        }

        currentCash = startingCash;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found in scene");
        }

        spawnPoints = FindObjectsOfType<SpawnPoint>();
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
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.Respawn();
        }
    }

    public void SetGunIcon(Sprite icon)
    {
        float iconWidth = icon.rect.width * 0.5f;
        float iconHeight = icon.rect.height * 0.5f;

        gunIcon.rectTransform.sizeDelta = new Vector2(iconWidth, iconHeight);
        gunIcon.rectTransform.anchoredPosition = new Vector2(iconWidth * -0.5f - 10f, gunIcon.rectTransform.anchoredPosition.y);
        gunIcon.sprite = icon;

        Color color = Color.white;
        color.a = .4f;
        gunIcon.color = color;

        StartCoroutine(FadeOutGunIcon());
    }

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
    }
}
