using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    [Header("=== MAIN ===")]
    public string nextLevelScene;
    public GameObject finishPanel;
    
    [Header("=== QUEST ===")]
    public string questName;
    public TextMeshProUGUI questText;
    public GameObject questPanel;
    public GameObject questNamePanel;
    public Button questButton;
    private bool isQuestDone = false;
    
    [Header("=== DIALOG ===")]
    public DialogData[] dialogData;
    private IEnumerator routine;

    [Header("UI")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogName;
    public TextMeshProUGUI dialogDescription;
    public Button nextButton;

    [Header("Joystick Control")]
    public GameObject joystickObject; // Masukkan objek Fixed Joystick di sini

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f; // Kecepatan efek ketik

    [Header("=== POP-UP TUTORIAL ===")]
    public GameObject tutorialPopupPanel;   // Panel utama pop-up tutorial
    public AudioClip tutorialAudio;         // Audio narasi pop-up tutorial

    [Header("=== SCORING SETTINGS ===")]
    public int currentLevelID = 1;          // Tentukan level saat ini di Inspector (1, 2, atau 3)
    public int levelScoreValue = 0;         // Jumlah poin yang didapat di level ini (misal: jumlah objek terkumpul)

    [Header("=== UI SKOR AKHIR (LEVEL 3) ===")]
    public TextMeshProUGUI finalScoreDisplayText; // Masukkan TextMeshProUGUI untuk skor akhir di Finish Panel Level 3

    private int currentDialogIndex = 0;
    private bool isTyping = false;

    private void Start()
    {
        // Matikan joystick di awal saat dialog mulai
        if (joystickObject != null)
        {
            joystickObject.SetActive(false);
        }

        // Sembunyikan panel pop-up tutorial di awal
        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(false);
        }

        dialogPanel.SetActive(false); // Pastikan panel dialog tersembunyi di awal
        nextButton.onClick.AddListener(NextDialog); // Tambahkan listener untuk tombol next
        ShowDialog();

        questButton.onClick.AddListener(delegate
        {
            OnShowQuest(!questNamePanel.activeInHierarchy, questNamePanel);
        });

        isQuestDone = false;

        AudioManager.Instance.PlaySFX(SceneManager.GetActiveScene().name);
        AudioManager.Instance.PlayBGM("Dialogue");
    }

    private void ShowDialog()
    {
        if (currentDialogIndex < dialogData.Length)
        {
            dialogPanel.SetActive(true); // Tampilkan panel dialog

            // Putar audio dialog khusus untuk kalimat yang sedang aktif
            if (dialogData[currentDialogIndex].dialogAudio != null)
            {
                AudioManager.Instance.PlayCustomSFX(dialogData[currentDialogIndex].dialogAudio);
            }

            routine = TypeText(dialogData[currentDialogIndex]);
            StartCoroutine(routine);
        }
        else
        {
            dialogPanel.SetActive(false); // Sembunyikan panel jika dialog selesai
            OnShowQuest(true, questPanel);
            Debug.Log("Dialog done!");

            // Hentikan suara dialog yang masih berjalan saat seluruh dialog selesai
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopSFX();
            }

            // MUNCULKAN POP-UP TUTORIAL SETELAH DIALOG SELESAI
            ShowTutorialPopup();

            AudioManager.Instance.PlayBGM("Gameplay");
        }
    }

    private void ShowTutorialPopup()
    {
        // Tampilkan panel pop-up tutorial
        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(true);

            // Memastikan komponen Button otomatis ada dan terhubung ke fungsi CloseTutorial
            Button panelButton = tutorialPopupPanel.GetComponent<Button>();
            if (panelButton == null)
            {
                panelButton = tutorialPopupPanel.AddComponent<Button>();
            }

            // Bersihkan listener lama agar tidak menumpuk, lalu pasang yang baru
            panelButton.onClick.RemoveAllListeners();
            panelButton.onClick.AddListener(CloseTutorial);
        }

        // Putar audio narasi tutorial
        if (tutorialAudio != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCustomSFX(tutorialAudio);
        }
    }

    // Fungsi ini dipanggil otomatis ketika pemain menyentuh layar pop-up tutorial di bagian mana saja
    public void CloseTutorial()
    {
        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(false);
        }

        // NYALAKAN KEMBALI JOYSTICK DI SINI
        if (joystickObject != null)
        {
            joystickObject.SetActive(true);
        }

        Debug.Log("Tutorial ditutup via sentuhan layar, joystick diaktifkan!");
    }

    private IEnumerator TypeText(DialogData dialog)
    {
        isTyping = true; // Tandai sedang mengetik
        dialogName.text = dialog.name;
        dialogDescription.text = "";

        foreach (char letter in dialog.description.ToCharArray())
        {
            dialogDescription.text += letter; // Tambahkan huruf satu per satu
            yield return new WaitForSeconds(typingSpeed); // Tunggu sejenak
        }

        isTyping = false; // Selesai mengetik
    }

    private void NextDialog()
    {
        if (isTyping)
        {
            dialogDescription.text = dialogData[currentDialogIndex].description; // Tampilkan semua teks jika sedang mengetik
            isTyping = false;
            if (routine != null) StopCoroutine(routine);
            return; // Jika sedang mengetik, abaikan input
        }
        currentDialogIndex++; // Pindah ke dialog berikutnya
        ShowDialog();
    }

    private void OnShowQuest(bool value, GameObject panel)
    {
        panel.SetActive(value);
        questText.text = questName;
    }

    public void OnQuestDone()
    {
        isQuestDone = true;
        Debug.Log("Quest Done");
        finishPanel.SetActive(true);

        // Jika berada di Level 3 (atau level akhir), hitung total skor dan tampilkan ke UI teks
        if (currentLevelID == 3 && GameManager.Instance != null)
        {
            int totalFinal = GameManager.Instance.CalculateTotalScore(levelScoreValue);
            if (finalScoreDisplayText != null)
            {
                finalScoreDisplayText.text = "Total Skor Akhir: " + totalFinal;
            }
        }

        AudioManager.Instance.PlaySFX("Win");
    }

    public void NextLevel()
    {
        // SIMPAN SKOR KE GAMEMANAGER BERDASARKAN LEVEL SAAT INI SEBELUM PINDAH SCENE
        if (GameManager.Instance != null)
        {
            if (currentLevelID == 1)
            {
                GameManager.Instance.scoreLevel1 = levelScoreValue;
            }
            else if (currentLevelID == 2)
            {
                // Level 2 memiliki aturan khusus (2 poin jika mulus, 1 poin jika sempat gagal/mengulang)
                GameManager.Instance.CalculateLevel2Score();
            }
            else if (currentLevelID == 3)
            {
                // Di level 3, hitung total akumulasi keseluruhan skor akhir
                int finalScore = GameManager.Instance.CalculateTotalScore(levelScoreValue);
                Debug.Log("Total Skor Akhir Game: " + finalScore);
            }
        }

        // Matikan suara dialog sebelum pindah ke level berikutnya
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSFX();
        }
        SceneManager.LoadScene(nextLevelScene);
    }

    public void Menu()
    {
        // Matikan suara dialog sebelum kembali ke Main Menu agar tidak bocor
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSFX();
        }
        SceneManager.LoadScene("MainMenu");
    }

    [System.Serializable]
    public class DialogData
    {
        public string name;
        public string description;
        public AudioClip dialogAudio; // Kotak untuk memasukkan file suara spesifik per kalimat dialog
    }
}