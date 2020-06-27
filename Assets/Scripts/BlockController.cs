using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockController : MonoBehaviour
{
    private float moveTimeout = 0.5f;
    private float timeElapsed = 0.0f;
    private int direction = 1;
    private int dropCount = 0;
    private GameObject grid;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid");
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > moveTimeout){
            timeElapsed = 0;
            transform.position = new Vector3(transform.position.x + direction, transform.position.y, transform.position.z);
        }
        // Check for space press
        if(Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)){
            // Clone player blocks at location & disable controller script on clone
            GameObject clone = Instantiate(this.gameObject, transform.position, transform.rotation);
            clone.GetComponent<BlockController>().enabled = false;
            clone.name = "drop"+dropCount++;
            clone.transform.parent = grid.transform;

            // Raise player y by + 1
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

            // Decrease move timeout
            moveTimeout *= .7f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Wall"){
            direction *= -1;
        }
    }
}
