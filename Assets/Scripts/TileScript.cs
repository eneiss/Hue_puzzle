using UnityEngine;

public class TileScript : MonoBehaviour
{

    LevelManager gameManager;
    public bool isInverted = false;
    public GameObject dot;
    bool isFixed = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = (LevelManager) GameObject.FindWithTag("LevelManager").GetComponent(typeof(LevelManager));
    }

    // OLD CODE
    /*
    // Update is called once per frame
    // TODO move to LevelManager.Update()
    void Update()
    {
        //foreach (Touch touch in Input.touches)
        //{
        //    if (touch.phase == TouchPhase.Ended && gameObject.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
        //    {
        //        Debug.Log("touch id :" + touch.fingerId);
        //        Click();
        //    }
        //}
        //if (Input.GetMouseButtonUp(0) && gameObject.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        //{
        //    Click();
        //}
    }

    void Click()
    {
        Debug.Log("Click on tile at " + gameObject.transform.position.x + ", " + gameObject.transform.position.y);

        if (!this.isFixed)
        {
            gameManager.InvertTiles((int)transform.localPosition.x, (int)transform.localPosition.y);
        }
        // todo else animate
    }
    */

    public void InvertColor()
    {
        //Debug.Log("Inverting color of tile at " + transform.localPosition.x + ", " + transform.localPosition.y);
        if (!this.isFixed)
        {
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color = new Color(1f - renderer.color.r, 1f - renderer.color.g, 1f - renderer.color.b, 1f);
            isInverted = !isInverted;
        }
    }

    public void SetFixed(bool isFixed)
    {
        if (isFixed)
        {
            this.isFixed = true;
            this.dot.SetActive(true);
        }
    }

    public bool GetFixed()
    {
        return isFixed;
    }
}
