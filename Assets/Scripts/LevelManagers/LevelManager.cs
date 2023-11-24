using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    [field: Header("Player properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerInput playerInputs { get; set; }

    [field: Header("Victory properties")]
    [field: SerializeField, ReadOnlyField] private bool isVictory { get; set; }
    [field: SerializeField] private Image victoryPanel { get; set; }
    [field: SerializeField] private Text victoryText { get; set; }
    [field: SerializeField] private Image gameOverPanel { get; set; }
    [field: SerializeField] private Text gameOverText { get; set; }

    private bool gameEnded { get; set; }

    void Start() {
        victoryPanel.color = new Color(victoryPanel.color.r, victoryPanel.color.g, victoryPanel.color.b, 0);
        victoryPanel.gameObject.SetActive(false);
        victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, 0);
        victoryText.gameObject.SetActive(false);
        gameOverPanel.color = new Color(gameOverPanel.color.r, gameOverPanel.color.g, gameOverPanel.color.b, 0);
        gameOverPanel.gameObject.SetActive(false);
        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 0);
        gameOverText.gameObject.SetActive(false);

#if !UNITY_EDITOR
        // Mouse cursor is locked and invisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            isVictory = true;
            EndGame();
        }
    }

    public void EndGame() {
        if (gameEnded) 
            return;

        StartCoroutine(ShowEndPanelCo());
    }

    IEnumerator ShowEndPanelCo() {
        gameEnded = true;
        playerInputs.DeactivateInput();

        if (isVictory) {
            victoryPanel.gameObject.SetActive(true);
            victoryText.gameObject.SetActive(true);
        } else {
            gameOverPanel.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(true);
        }

        // Fade in
        for (float i = 0; i <= 1; i += Time.deltaTime) {
            if (isVictory) {
                if (i < 0.39f)
                    victoryPanel.color = new Color(victoryPanel.color.r, victoryPanel.color.g, victoryPanel.color.b, i);
                victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, i);
            } else {
                if (i < 0.39f)
                    gameOverPanel.color = new Color(gameOverPanel.color.r, gameOverPanel.color.g, gameOverPanel.color.b, i);
                gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, i);
            }
        }

        yield return new WaitForSeconds(5f);
        Application.Quit();
    }
}
