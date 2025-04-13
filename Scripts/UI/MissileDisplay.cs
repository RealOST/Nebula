using UnityEngine;
using UnityEngine.UI;

public class MissileDisplay : MonoBehaviour {
    private static Text amountText;
    private static Image cooldownImage;

    private void Awake() {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>();
    }

    public static void UpdateAmountText(int amount) => amountText.text = amount.ToString();

    public static void UpdateCooldownImage(float fillAmount) => cooldownImage.fillAmount = fillAmount;
}