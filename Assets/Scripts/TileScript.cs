using System;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    LevelGameManager gameManager;
    public bool isInverted = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = (LevelGameManager) GameObject.FindWithTag("LGM").GetComponent(typeof(LevelGameManager));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (gameObject.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
            {
                Click();
            }
        }
        if (Input.GetMouseButtonUp(0) && gameObject.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            Click();
        }
    }

    void Click()
    {
        Debug.Log("Click");
        InvertColor();
        gameManager.InvertTiles((int) transform.localPosition.x, (int) transform.localPosition.y);
    }

    public void InvertColor()
    {
        //Debug.Log("Inverting color of tile at " + transform.localPosition.x + ", " + transform.localPosition.y);
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.color = new Color(1f - renderer.color.r, 1f - renderer.color.g, 1f - renderer.color.b, 1f);
        isInverted = !isInverted;
    }
}
