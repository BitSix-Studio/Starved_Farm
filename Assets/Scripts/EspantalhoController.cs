using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EspantalhoController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speedEnemy;

    [Header("Ataque Settings")]
    [SerializeField] private float intervaloAtk;
    [SerializeField] private float duracaoAtk;
    [SerializeField] private GameObject areaAtkBaixo;
    [SerializeField] private GameObject areaAtkCima;
    private bool podeAtacar;

    [Header("Pontos de Patrulha")]
    public Transform[] pontosPatrulha;
    private Transform pontoAtual;

    [Header("Referências do Jogador")]
    private Transform player;
    private PlayerController scriptPlayer;

    [Header("Referências de Áudio")]
    [SerializeField] private AudioSource rosnado;

    private Rigidbody2D rb2dEnemy;
    private Vector2 directionEnemy;
    private bool canMoveEnemy = true;
    private Animator animator;

    private Transform pontos;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        rb2dEnemy = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        scriptPlayer = player.GetComponent<PlayerController>();
        pontoAtual = transform;
        pontos = GameObject.Find("PatrulhaMovimento").transform;
        gameManager = FindFirstObjectByType<GameManager>();
        animator = GetComponent<Animator>();
        areaAtkBaixo.SetActive(false);
        areaAtkCima.SetActive(false);
        animator.SetFloat("AxisX", 0);
        animator.SetFloat("AxisY", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMoveEnemy && scriptPlayer != null && !scriptPlayer.InStealth)
        {
            PerseguirJogador();
            pontos.transform.position = transform.position;
            AnimarEnemyPerseguir();
        }
        else
        {
            /*if (transform.position.x > 3)
            {
                pontos.transform.position = new Vector2(6,0);
            }
            else if (transform.position.x < -2)
            {
                pontos.transform.position = new Vector2(-6, 0);
            }
            else if (transform.position.y > 2)
            {
                pontos.transform.position = new Vector2(0, 6);
            }
            else if (transform.position.y < -2)
            {
                pontos.transform.position = new Vector2(0, -6);
            }*/
            Patrulhar();
            AnimarEnemyPatrulha();
        }

        if (!canMoveEnemy)
        {
            directionEnemy = transform.position;
            rb2dEnemy.MovePosition(directionEnemy);
        }
    }

    void PerseguirJogador()
    {
        directionEnemy = Vector2.MoveTowards(rb2dEnemy.position, player.position, speedEnemy * Time.fixedDeltaTime);

        rb2dEnemy.MovePosition(directionEnemy);
        animator.SetFloat("AxisX", directionEnemy.x);
        animator.SetFloat("AxisY", directionEnemy.y);
        animator.SetInteger("Movimento", 1);
    }

    void Patrulhar()
    {
        if (pontosPatrulha.Length == 0) return; // Se não houver pontos de patrulha, não faz nada

        // Move o inimigo em direção ao ponto atual
        directionEnemy = Vector2.MoveTowards(rb2dEnemy.position, pontoAtual.position, speedEnemy * Time.fixedDeltaTime);
        rb2dEnemy.MovePosition(directionEnemy);
        animator.SetFloat("AxisX", directionEnemy.x);
        animator.SetFloat("AxisY", directionEnemy.y);
        animator.SetInteger("Movimento", 1);

        // Verifica se chegou no ponto atual
        if (Vector2.Distance(transform.position, pontoAtual.position) < 0.1f)
        {
            // Escolhe um novo ponto de patrulha aleatoriamente
            Transform novoPonto;
            do
            {
                novoPonto = pontosPatrulha[Random.Range(0, pontosPatrulha.Length)];
            } while (novoPonto == pontoAtual && pontosPatrulha.Length > 1); // Garante que o ponto novo seja diferente do atual

            pontoAtual = novoPonto;
        }
    }

    void AnimarEnemyPerseguir()
    {
        if (transform.position.x > player.transform.position.x)
        {
            animator.SetFloat("AxisX", -1);
        }
        else if (transform.position.x < player.transform.position.x)
        {
            animator.SetFloat("AxisX", 1);
        }
        if (transform.position.y > player.transform.position.y)
        {
            animator.SetFloat("AxisY", -1);
        }
        else if (transform.position.y < player.transform.position.y)
        {
            animator.SetFloat("AxisY", 1);
        }
    }
    void AnimarEnemyPatrulha()
    {
        if (transform.position.x > pontoAtual.position.x)
        {
            animator.SetFloat("AxisX", -1);
        }
        else if (transform.position.x < pontoAtual.position.x)
        {
            animator.SetFloat("AxisX", 1);
        }
        if (transform.position.y > pontoAtual.position.y)
        {
            animator.SetFloat("AxisY", -1);
        }
        else if (transform.position.y < pontoAtual.position.y)
        {
            animator.SetFloat("AxisY", 1);
        }
    }

    IEnumerator Ataque()
    {
        podeAtacar = false;
        if (transform.position.x > player.transform.position.x)
        {
            areaAtkCima.gameObject.SetActive(true);
        }
        else if (transform.position.x < player.transform.position.x)
        {
            areaAtkBaixo.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(duracaoAtk);
        
        podeAtacar = true;
        ApagarColisorAtk();
        yield return new WaitForSeconds(intervaloAtk);
    }

    void ApagarColisorAtk() {
        if (podeAtacar == true) {
            animator.SetBool("Ataque", false);
            areaAtkBaixo.gameObject.SetActive(false);
            areaAtkCima.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("Ataque", true);
            animator.SetInteger("Movimento", 0);
            //canMoveEnemy = false;
        }
        if (collision.CompareTag("Esconderijo"))
        {
            StartCoroutine(Esperar());
            scriptPlayer.InStealth = false;
        }
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(5);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMoveEnemy = true;
            animator.SetBool("Ataque", false);
        }
    }
}
