using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class BlockController : MonoBehaviour
{
    public GameObject blockPrefab;
    private int level = 1;
    private int winLevel = 11;
    private float timeoutMultiplier = 0.7f;
    private float moveTimeout = .5f;
    private float timeElapsed = 0.0f;
    private float direction = 1f;
    private int dropCount = 0;
    private GameObject grid;
    private Tilemap tilemap;
    private GameObject clone;
    private enum trimDirection{
        right,
        left
    }
    private enum gameStates{
        gameOver, 
        playing,
        won
    }

    private gameStates gameState = gameStates.playing;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid");
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemap.CompressBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == gameStates.playing){
            // Check if won
            if(level == winLevel){
                gameState = gameStates.won;
                triggerGameWinAnimation();
                return;
            }
            timeElapsed += Time.deltaTime;
            if(timeElapsed > moveTimeout){
                timeElapsed = 0;
                transform.position = new Vector3(transform.position.x + direction, transform.position.y, transform.position.z);
            }
            // Check for space press
            if(Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)){
                // Check alignment
                if(clone != null){
                    // Trim player
                    if(transform.position.x < clone.transform.position.x){
                    trimStackr(trimDirection.left, (int) (clone.transform.position.x - transform.position.x));
                    } else if(transform.position.x > clone.transform.position.x){
                    trimStackr(trimDirection.right, (int) (transform.position.x - clone.transform.position.x));
                    }
                    // tilemap.SetTile(tilemap.origin, null);
                    tilemap.CompressBounds();
                }

                // Check for gameOver
                if(gameState == gameStates.gameOver){
                    triggerGameOverAnimation();
                    enabled = false;
                    return;
                }
                
                // Clone player blocks at location & disable controller script on clone
                clone = Instantiate(this.gameObject, transform.position, transform.rotation);
                clone.GetComponent<BlockController>().enabled = false;
                clone.name = "drop"+dropCount++;
                clone.transform.parent = grid.transform;

                // Raise player y by + 1
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                level++;

                // Decrease move timeout
                moveTimeout *= timeoutMultiplier;
            }
        }
    }

    void trimStackr(trimDirection dir, int trimSize){
        // Check if lose condition
        if(trimSize >= tilemap.size.x){
            Debug.Log("Triggering gameover state");
            gameState = gameStates.gameOver;
            // Set floating blocks to red
            tilemap.color = Color.red;
            return;
        }
        Vector3Int trimTarget;
        if(dir == trimDirection.left){
            // Trim from origin
            for(int i = 0; i < trimSize; i ++){
                trimTarget = new Vector3Int(tilemap.origin.x + i, tilemap.origin.y, tilemap.origin.z);
                tilemap.SetTile(trimTarget, null);
                spawnAndTriggerFall(tilemap.GetCellCenterWorld(trimTarget));
            }
        } else{
            // Trim from origin + size, backwards
            for(int i = 1; i <= trimSize; i ++){
                trimTarget = new Vector3Int(tilemap.origin.x + tilemap.size.x - i, tilemap.origin.y, tilemap.origin.z);
                tilemap.SetTile(trimTarget, null);
                spawnAndTriggerFall(tilemap.GetCellCenterWorld(trimTarget));
            }
        }
    }

    void triggerGameWinAnimation(){
        tilemap.color = Color.green;
        StartCoroutine("GameWinSceneAfterDelay");
        transform.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        InvokeRepeating("flicker", 0, 0.4f);
    }
    void triggerGameOverAnimation(){
        StartCoroutine("GameOverSceneAfterDelay");
        transform.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        InvokeRepeating("flicker", 0, 0.4f);
    }

    void flicker(){
        transform.gameObject.GetComponent<TilemapRenderer>().enabled = 
            !transform.gameObject.GetComponent<TilemapRenderer>().enabled;
    }
    IEnumerator GameOverSceneAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator GameWinSceneAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameWin");
    }

    void spawnAndTriggerFall(Vector3 coords){
        coords.z = coords.z + 1;
        Instantiate(blockPrefab, coords, transform.rotation);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Wall"){
            direction *= -1;
        }
    }
}
