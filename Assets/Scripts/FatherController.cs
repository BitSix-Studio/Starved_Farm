using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FatherController : MonoBehaviour
{
    public float fatherSpeed = 3.5f;
    private Vector2 fatherDirection;
    private Rigidbody2D fatherRb2d;

    public bool canMove = true;

    public DetectionController detectionArea;
    private SpriteRenderer spriteRenderer;
    private Animator fatherAnimator;

    // Start is called before the first frame update
    void Start()
    {
        fatherRb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fatherAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        if (!canMove)
        {
            fatherDirection = Vector2.zero;
            FatherMove();
            fatherAnimator.SetInteger("Movimento", 0);
        }
        else
        {
            fatherDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (fatherDirection.sqrMagnitude > 0.1)
            {
                FatherMove();
                fatherAnimator.SetFloat("AxisX", fatherDirection.x);
                fatherAnimator.SetFloat("AxisY", fatherDirection.y);
                fatherAnimator.SetInteger("Movimento", 1);
            }
            else
            {
                fatherAnimator.SetInteger("Movimento", 0);
            }

            /*if(fatherDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            } 
            else if (fatherDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }*/
        }

    }

    void FatherMove()
    {
        if(detectionArea.detectObj.Count > 0)
        {
            fatherDirection = (detectionArea.detectObj[0].transform.position - transform.position).normalized;
            fatherRb2d.MovePosition(fatherRb2d.position + fatherDirection * fatherSpeed * Time.fixedDeltaTime);
        }
        else
        {
            fatherRb2d.MovePosition(fatherRb2d.position + fatherDirection * fatherSpeed * Time.fixedDeltaTime);
        }
    }
}
