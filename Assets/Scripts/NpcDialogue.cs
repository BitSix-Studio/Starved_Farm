using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcDialogue : MonoBehaviour
{
    [Header("Diálogos e Configurações")]
    [SerializeField] private string[] dialogueNpc; // Lista de falas do NPC
    private int dialogueIndex; // Índice da fala atual
    [SerializeField] private string npcName;

    [Header("Referências de UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameNpc;
    [SerializeField] private Image imageNpc;
    [SerializeField] private Sprite spriteNpc;

    [Header("Configurações de Comportamento")]
    [SerializeField] private float delayEntreFalas = 2f; // Tempo entre as falas (automático)
    private bool readToSpeak; // Verifica se o player está no trigger
    private bool startDialogue; // Indica se o diálogo está ativo

    private Coroutine currentDialogueRoutine; // Armazena a rotina de diálogo atual para controle
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Inicia o diálogo ao pressionar E quando próximo ao NPC
        if (Input.GetKeyDown(KeyCode.E) && readToSpeak && !startDialogue)
        {
            player.canMovePlayer = false; // Parar o jogador
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        nameNpc.text = npcName;
        imageNpc.sprite = spriteNpc;
        startDialogue = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);

        if (currentDialogueRoutine != null)
        {
            StopCoroutine(currentDialogueRoutine);
        }
        currentDialogueRoutine = StartCoroutine(HandleDialogue());
    }

    IEnumerator HandleDialogue()
    {
        // Mostra cada linha do diálogo
        while (dialogueIndex < dialogueNpc.Length)
        {
            yield return StartCoroutine(ShowDialogue(dialogueNpc[dialogueIndex]));
            yield return new WaitForSeconds(delayEntreFalas); // Espera antes de avançar
            dialogueIndex++;
        }

        EndDialogue();
    }

    IEnumerator ShowDialogue(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        startDialogue = false;
        dialogueIndex = 0;
        // Restaurar movimento do jogador
        player.canMovePlayer = true;
        player.playerSpeed = player.playerInitialSpeed;
        if (this.gameObject.CompareTag("MotherMareen"))
        {
            Transform barreira = transform.Find("BarreiraDeInteração");
            if (barreira != null)
            {
                Destroy(barreira.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!this.gameObject.CompareTag("FatherMareen"))
            {
                readToSpeak = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readToSpeak = false;

            if (startDialogue)
            {
                StopCurrentDialogue();
            }
        }
    }

    public void StopCurrentDialogue()
    {
        if (currentDialogueRoutine != null)
        {
            StopCoroutine(currentDialogueRoutine);
        }

        EndDialogue();
    }

    public void SetNewDialogue(string[] newDialogue, string newNpcName = null, Sprite newSpriteNpc = null)
    {
        // Permite alterar o diálogo, nome e imagem do NPC para futuras interações
        dialogueNpc = newDialogue;

        if (!string.IsNullOrEmpty(newNpcName))
        {
            npcName = newNpcName;
        }

        if (newSpriteNpc != null)
        {
            spriteNpc = newSpriteNpc;
        }
    }
}
