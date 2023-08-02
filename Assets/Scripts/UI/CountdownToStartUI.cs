using System;
using TMPro;
using UnityEngine;

public class CountdownToStartUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private static readonly int NUMBER_POP_UP = Animator.StringToHash("NumberPopUp");

    private int recordCountdownNumber;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        MainGameManager.Instance.GameStateChangedAction += OnGameStateChangedAction;

        Hide();
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsCountDownToStartState()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        var countdownNumber = MainGameManager.Instance.GetCountdownToStartTimer();
        if (countdownNumber == recordCountdownNumber) {
            return;
        }

        recordCountdownNumber = countdownNumber;
        countdownText.text = countdownNumber.ToString();
        animator.SetTrigger(NUMBER_POP_UP);
        SoundManager.Instance.PlayWarning();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}