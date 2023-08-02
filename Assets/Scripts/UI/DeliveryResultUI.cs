using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI deliveryResultText;
    [SerializeField] private Image deliveryResultIcon;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private Animator animator;
    private static readonly int RESULT_POP_UP = Animator.StringToHash("ResultPopUp");

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DeliveryManager.Instance.DeliverySuccessAction += OnDeliverySuccessAction;
        DeliveryManager.Instance.DeliveryFailAction += OnDeliveryFailAction;

        Hide();
    }

    private void OnDeliverySuccessAction() {
        background.color = successColor;
        deliveryResultText.text = "DELIVERY\nSUCCESS!";
        deliveryResultIcon.sprite = successSprite;

        Show();

        animator.SetTrigger(RESULT_POP_UP);
    }

    private void OnDeliveryFailAction() {
        background.color = failColor;
        deliveryResultText.text = "DELIVERY\nFAIL!";
        deliveryResultIcon.sprite = failSprite;

        Show();

        animator.SetTrigger(RESULT_POP_UP);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}