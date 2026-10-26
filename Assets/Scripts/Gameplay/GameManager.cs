using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("=== SKOR KESELURUHAN ===")]
    public int scoreLevel1 = 0;
    public int scoreLevel2 = 0;
    public int scoreLevel3 = 0;
    public int totalFinalScore = 0;

    [Header("=== PELACAK LEVEL 2 ===")]
    public bool isLevel2FailedOnce = false; // Menandai apakah pemain sempat mengulang di Level 2

    private void Awake()
    {
        // Singleton pattern agar GameManager tetap ada saat pindah scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fungsi untuk menghitung dan menyimpan skor Level 2 berdasarkan aturan Anda
    public void CalculateLevel2Score()
    {
        if (isLevel2FailedOnce)
        {
            scoreLevel2 = 1; // Mengulang berapapun kali = 1 poin
        }
        else
        {
            scoreLevel2 = 2; // Lulus sekali coba di awal = 2 poin
        }
    }

    // Hitung total skor keseluruhan di akhir Level 3
    public int CalculateTotalScore(int currentLevel3Score)
    {
        scoreLevel3 = currentLevel3Score;
        
        // Pastikan skor level 2 sudah dihitung sebelum ditotal
        CalculateLevel2Score();

        totalFinalScore = scoreLevel1 + scoreLevel2 + scoreLevel3;
        return totalFinalScore;
    }
}