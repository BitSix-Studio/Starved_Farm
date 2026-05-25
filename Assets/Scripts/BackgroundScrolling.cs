using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    public MeshRenderer meshRender;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        meshRender.material.mainTextureOffset -= new Vector2(0, speed * Time.deltaTime);
    }
}
