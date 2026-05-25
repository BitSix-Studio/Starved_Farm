using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaiFarturaController : MonoBehaviour
{
    [SerializeField] private Transform[] pontosDoCaminho;
    private int pontoAtual;
    [SerializeField] private float speed;

    [SerializeField] private float dashDistance;
    private float dashCooldownMin = 2f;
    [SerializeField] private float dashCooldownMax;
    [SerializeField] private float dashSpeed;

    private float initialPositionY;
    private bool isDashing = false;
    private Animator paiFarturaAnimator;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        pontoAtual = 0;
        initialPositionY = transform.position.y;
        paiFarturaAnimator = GetComponent<Animator>();
        StartCoroutine(DashRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing && gameManager.Paused == false && gameManager.GameWin == false && gameManager.GameOverVar == false)
        {
            MoveEnemy();
        }
    }

    private void MoveEnemy()
    {
        transform.position = Vector2.MoveTowards(transform.position, pontosDoCaminho[pontoAtual].position, speed * Time.deltaTime);

        if (transform.position == pontosDoCaminho[pontoAtual].position)
        {
            pontoAtual += 1;

            if (pontoAtual >= pontosDoCaminho.Length)
            {
                pontoAtual = 0;
            }
        }
        paiFarturaAnimator.SetBool("CorrendoPai", true);
    }

    // Corrotina para gerenciar o tempo de espera e o dash
    IEnumerator DashRoutine()
    {
        while (true)
        {
            // Espera por um tempo aleatório entre minWaitTime e maxWaitTime
            float waitTime = Random.Range(dashCooldownMin, dashCooldownMax);
            yield return new WaitForSeconds(waitTime);

            // Realiza o dash para baixo
            yield return StartCoroutine(Dash());

            // Espera um curto tempo antes de retomar o movimento horizontal
            yield return new WaitForSeconds(1f);
        }
    }

    // Corrotina para realizar o dash para baixo
    IEnumerator Dash()
    {
        isDashing = true;  // Define que o inimigo está realizando o dash
        Vector3 dashTarget = new Vector3(transform.position.x, transform.position.y - dashDistance, transform.position.z); // Calcula a posição do dash (somente no eixo Y)

        // Movimenta o inimigo para baixo
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante que o inimigo esteja exatamente na posição final do dash
        transform.position = dashTarget;

        // Retorna o inimigo apenas para a altura original no eixo Y, mantendo a posição no eixo X
        Vector3 returnTarget = new Vector3(transform.position.x, initialPositionY, transform.position.z);

        // Volta o inimigo para a posição original após o dash
        while (Vector3.Distance(transform.position, returnTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnTarget, dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante que o inimigo esteja exatamente na posição original
        transform.position = returnTarget;

        isDashing = false;  // O dash terminou, retoma o movimento horizontal
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.GameOver();
        }
    }
}
