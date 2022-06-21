using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameSelectionScreen : BaseScreen
{
    [SerializeField] private TMP_InputField inputField; 
    [SerializeField] private Button submitButton;

    [SerializeField] private TextMeshProUGUI errorText;

    public override void Start()
    {
        if (PlayerPrefManager.Instance.PlayerName != "null_name")
        {
            base.Start();
        }
        else
        {
            Debug.Log("First time login -> showing player name selection screen");
        }
    }

    public void OnSubmitButtonClick()
    {
        string input = inputField.text;

        if(input.Length < 6)
        {
            errorText.text = "Name length has to be greater than or equal to 6.";
            return;
        }
        else if(input.Length > 12)
        {
            errorText.text = "Name length has to be less than or equal to 12.";
            return;
        }

        PlayerPrefManager.Instance.PlayerName = input;
        PlayerPrefManager.Instance.SavePlayerName();

        Hide();
    }
}
