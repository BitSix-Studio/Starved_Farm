using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObj : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float valueTransparency = 0.7f;
    [SerializeField] private float fadeTimeTransparency = .4f;

    private SpriteRenderer spriteRender;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            StartCoroutine(FadeTree(spriteRender, fadeTimeTransparency, spriteRender.color.a, valueTransparency));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            StartCoroutine(FadeTree(spriteRender, fadeTimeTransparency, spriteRender.color.a, 1f));
        }
    }

    private IEnumerator FadeTree(SpriteRenderer spriteTransparency, float fadeTime, float startValue, float targetTransparency)
    {
        float timeElapsed = 0;
        while(timeElapsed < fadeTime)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetTransparency, timeElapsed / fadeTime);
            spriteTransparency.color = new Color(spriteTransparency.color.r, spriteTransparency.color.g, spriteTransparency.color.b, newAlpha);
            yield return null;
        }
        
    }
}
