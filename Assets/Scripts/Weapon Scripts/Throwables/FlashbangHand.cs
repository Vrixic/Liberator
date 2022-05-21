using UnityEngine;

public class FlashbangHand : BaseThrowableHands
{
    public override void Start()
    {
        base.Start();

        GameManager.Instance.playerScript.UpdateFlashbangCount();
    }
}
