using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionController : MonoBehaviour
{
    public string tagTargetDetection = "Player";
    public List<Collider2D> detectObj = new List<Collider2D>();
    private FatherController fatherMareen;
    public GameObject areaDetecta;

    private void Start()
    {
        fatherMareen = FindFirstObjectByType<FatherController>();
    }

    public void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == tagTargetDetection)
        {
            detectObj.Add(collision);
            if (areaDetecta.CompareTag("DetectaFatherMareen"))
            {
                fatherMareen.canMove = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == tagTargetDetection)
        {
            detectObj.Clear();
            if (areaDetecta.CompareTag("DetectaFatherMareen"))
            {
                fatherMareen.canMove = false;
            }
        }
    }
}
