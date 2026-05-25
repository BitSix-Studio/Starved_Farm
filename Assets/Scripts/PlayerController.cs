using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //vaiável da física do personagem
    private Rigidbody2D playerRb2d;
    //variável das animações do personagem
    private Animator playerAnimator;
    //variável da direção que o personagem vai seguir
    private Collider2D playerCollider;
    private SpriteRenderer spritePlayer;
    private float opacityPlayer;
    private Vector2 playerDirection;
    //variável da velocidade inicial do personagem
    public float playerInitialSpeed = 2f;
    //variável da aceleração do personagem
    public float playerRunSpeed;
    //variável da velocidade principal do personagem
    public float playerSpeed;
    //variável do tempo até acelerar
    public bool canMovePlayer;
    public bool InStealth;
    private float timeVelocity;
    //variável que verifica em qual cena o player está
    private string currentScene;
    //variável que tem o botão de interação aparecendo
    [SerializeField] private Image imageButtonInteractive;

    public FatherController fatherMarren;
    public DetectionController detectionArea;
    private NpcDialogue dialogue;

    // Start é chamada antes da primeira atualização do frame (quando inicia o a cena)
    void Start()
    {
        //adicionando as informações do componente RigidBody2d à minha variável
        playerRb2d = GetComponent<Rigidbody2D>();
        //adicionando as informações da aba Animator à minha variável
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        //faz desaparecer a imagem
        if (imageButtonInteractive != null)
        {
            imageButtonInteractive.gameObject.SetActive(false);
        }
        canMovePlayer = true;
        dialogue = GetComponent<NpcDialogue>();
        spritePlayer = GetComponent<SpriteRenderer>();
        InStealth = false;
    }

    // Update é chamado once per frame
    void Update()
    {
        // Flip();
        currentScene = SceneManager.GetActiveScene().name;
    }

    void FixedUpdate()
    {
        if(currentScene == "StarvedFarm_Sacada")
        {
            //Aplica uma movimentação constante para baixo
            playerDirection = new Vector2(Input.GetAxisRaw("Horizontal") * playerSpeed, -playerSpeed);
            //Impede o jogador de se mover para cima
            
        }
        else
        {
            if (canMovePlayer)
            {
                playerDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            else
            {
                playerRb2d.velocity = new Vector2(0, playerRb2d.velocity.y);
            }
        }
        if (canMovePlayer) {
            if (playerDirection.sqrMagnitude > 0.1)
            {
                MovePlayer();
                playerAnimator.SetFloat("AxisX", playerDirection.x);
                playerAnimator.SetFloat("AxisY", playerDirection.y);
                playerAnimator.SetInteger("Movimento", 1);
                //atribui o valor de velocidade inicial igual a velocidade principal do player
                playerSpeed = playerInitialSpeed;
                timeVelocity++;
                timeVelocity = timeVelocity + Time.deltaTime;
                PlayerRun();
            }
            else
            {
                playerSpeed = playerInitialSpeed;
                playerAnimator.SetInteger("Movimento", 0);
                playerAnimator.SetBool("Run", false);
                timeVelocity = 0f;
            }
        }
    }

    void MovePlayer()
    {
        playerRb2d.MovePosition(playerRb2d.position + playerDirection.normalized * playerSpeed * Time.deltaTime);
    }

    /*void Flip()
    {
        if (playerDirection.x < 0)
        {
            transform.eulerAngles = new Vector2(0f, 0f);
        } else if (playerDirection.x > 0)
        {
            transform.eulerAngles = new Vector2(0f, 180f);
        }
    }*/

    void PlayerRun()
    {
        if (timeVelocity >= 70f)
        {
            playerSpeed = playerRunSpeed;
            playerAnimator.SetBool("Run", true);
            playerAnimator.SetInteger("Movimento", 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entrada"))
        {
            SceneManager.LoadScene("CasaBaixo");
        }
        if (collision.gameObject.CompareTag("SubirCasa"))
        {
            SceneManager.LoadScene("CasaCima");
        }
        if (collision.gameObject.CompareTag("Sacada"))
        {
            SceneManager.LoadScene("StarvedFarm_Sacada");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FatherMareen"))
        {
            fatherMarren.canMove = false;
            detectionArea.detectObj.Clear();
        }
        if (collision.CompareTag("MotherMareen"))
        {
            imageButtonInteractive.gameObject.SetActive(true);
        }
        if (collision.CompareTag("NPC"))
        {
            imageButtonInteractive.gameObject.SetActive(true);
        }
        if (collision.gameObject.CompareTag("BarreiraMae"))
        {
            DialogoBarreira();
        }
        if (collision.CompareTag("Esconderijo"))
        {
            SeEsconder();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FatherMareen"))
        {
            fatherMarren.canMove = true;
            detectionArea.OnTriggerEnter2D(playerCollider);
        }
        if (collision.CompareTag("MotherMareen"))
        {
            imageButtonInteractive.gameObject.SetActive(false);
        }
        if (collision.CompareTag("NPC"))
        {
            imageButtonInteractive.gameObject.SetActive(false);
        }
        if (collision.CompareTag("Esconderijo"))
        {
            SairEsconderijo();
        }
    }

    void DialogoBarreira()
    {
        playerAnimator.SetInteger("Movimento", 0);
        playerAnimator.SetBool("Run", false);
        timeVelocity = 0f;
        canMovePlayer = false;
        fatherMarren.canMove = false;
        dialogue.StartDialogue();
        StartCoroutine(SeMoverParaBaixoAuto());
        if (GameObject.FindGameObjectWithTag("BarreiraMae"))
        {
            Destroy(GameObject.FindGameObjectWithTag("BarreiraMae"));
        }
    }

    IEnumerator SeMoverParaBaixoAuto()
    {
        playerAnimator.SetInteger("Movimento",1);
        playerAnimator.SetFloat("AxisY", -1);
        float tempoDecorrido = 0f;
        float tempoDescer = 0.7f;
        while (tempoDecorrido < tempoDescer)
        {
            playerRb2d.velocity = new Vector2(0, -playerInitialSpeed);
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }
        playerAnimator.SetInteger("Movimento", 0);
        playerRb2d.velocity = Vector2.zero;
    }

    void SeEsconder()
    {
        opacityPlayer = 0.7f;
        Color opaco = spritePlayer.color;
        opaco.a = opacityPlayer;
        spritePlayer.color = opaco;
        InStealth = true;
    }

    void SairEsconderijo()
    {
        opacityPlayer = 1f;
        Color opaco = spritePlayer.color;
        opaco.a = opacityPlayer;
        spritePlayer.color = opaco;
        InStealth = false;
    }
}
