using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public static GameManager instance;

    [SerializeField] private string CenaJogar;
    [SerializeField] private string CenaSair;

    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject painelWin; // Tela de parabéns que será exibida quando o tempo zerar

    public PlayerController player; // Referência ao script do player para verificar se ele está morto

    private bool gameWin = false; // Para verificar se o jogo já foi vencido
    private bool gameOver = false;
    private bool paused = false;
    public bool Paused
    {
        get { return paused; }
        set { paused = value; }
    }
    public bool GameOverVar
    {
        get { return gameOver; }
        set { gameOver = value; }
    }
    public bool GameWin
    {
        get { return gameWin; }
        set { gameWin = value; }
    }

    public AudioSource musicGame;
    public AudioSource musicGameOver;

    public Slider timeSlider; // Slider que representa o tempo

    public float timeChase = 60f; // Tempo inicial em segundos
    private float currentTimeChase; // Tempo atual do cronômetro

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "StarvedFarm_Menu")
        {
            painelMenu.SetActive(false);
        }
        if(painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
        if(painelWin != null)
        {
            painelWin.SetActive(false);
        }
        if(timeSlider != null)
        {
            timeSlider.gameObject.SetActive(true);
            // Configura o tempo inicial e o valor máximo do Slider
            currentTimeChase = timeChase;
            timeSlider.maxValue = timeChase;
            timeSlider.value = currentTimeChase;
        }
        if (musicGame != null)
        {
            musicGame.Play();
        }
        Time.timeScale = 1f;
        gameOver = false;
    }

    void Update() 
    {
        if(SceneManager.GetActiveScene().name != "StarvedFarm_Menu")
        {
            if(!gameWin && SceneManager.GetActiveScene().name == "StarvedFarm_Sacada")
            {
                TimeChasePaiFartura();
            }
            else if (SceneManager.GetActiveScene().name == "Espantalho")
            {
                TimeChaseEspantalho();
            }
            if(Input.GetKeyDown(KeyCode.Escape) && !gameOver)
            {
                if(paused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver = true;
        timeSlider.gameObject.SetActive(false);
        //Ativa o painel de game over
        painelGameOver.SetActive(true);
        //Pausa o tempo do jogo, congelando tudo
        Time.timeScale = 0f;
        //Pausa a musica do jogo
        if (musicGame != null)
        {
            musicGame.Stop();
        }
        //Inicia a música de game over
        //musicGameOver.Play();
        //Torna o cursor do mouse visível
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Função que Pausa o Jogo
    public void ResumeGame()
    {
        //Desativa o painel de pause para aparecer
        painelMenu.SetActive(false);
        //Despausa o tempo do jogo, congelando tudo
        Time.timeScale = 1f;
        //Despausa todos os áudios do jogo
        AudioListener.pause = false;

        //Torna o cursor do mouse invisível
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Torna a variável de pause falsa
        paused = false;
    }

    public void PauseGame()
    {
        //Ativa o painel de pause para aparecer
        painelMenu.SetActive(true);
        //Pausa o tempo do jogo, congelando tudo
        Time.timeScale = 0f;
        //Pausa todos os áudios do jogo
        AudioListener.pause = true;

        //Torna o cursor do mouse visível
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        //Torna a variável de pause verdadeira
        paused = true;
    }

    public void Jogar()
    {
        SceneManager.LoadScene(CenaJogar);
    }

    public void SairJogo()
    {
        //Faz abrir outra cena
        SceneManager.LoadScene(CenaSair);
        //Despausa todos os áudios do jogo
        AudioListener.pause = false;
        //Torna a variável de pause falsa
        paused = false;
        gameOver = false;
        gameWin = false;
    }

    public void SairMenu()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Despausa todos os áudios do jogo
        AudioListener.pause = false;
    }

    public void TimeChasePaiFartura()
    {
        // Reduz o tempo e atualiza o Slider
        currentTimeChase -= Time.deltaTime;
        timeSlider.value = currentTimeChase;

        // Verifica se o tempo chegou a zero
        if (currentTimeChase <= 0)
        {
            SceneManager.LoadScene("Espantalho");
        }
    }

    public void TimeChaseEspantalho()
    {
        // Reduz o tempo e atualiza o Slider
        currentTimeChase -= Time.deltaTime;
        timeSlider.value = currentTimeChase;

        // Verifica se o tempo chegou a zero
        if (currentTimeChase <= 0)
        {
            WinGame();
        }
    }

    // Função que é chamada quando o tempo chega a zero e o jogador não morreu
    void WinGame()
    {
        gameWin = true;  // Marca que o jogo foi vencido
        painelWin.SetActive(true);  // Exibe a tela de vitória
        //Pausa o tempo do jogo, congelando tudo
        Time.timeScale = 0f;
        //Pausa todos os áudios do jogo
        AudioListener.pause = true;

        //Torna o cursor do mouse visível
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
