using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class BlockController : MonoBehaviour
{
    public GameObject blockPrefab;
    public AudioSource blockMove, gameOver, blockPlaced, gameWin;
    private int level = 1;
    private int winLevel = 11;
    private float timeoutMultiplier = 0.7f;
    private float moveTimeout = .5f;
    private float timeElapsed = 0.0f;
    private float direction = 1f;
    private int dropCount = 0;
    private int moveCount = 0;
    private GameObject grid;
    private Tilemap tilemap;
    private GameObject clone;
    private PersistantGameState gameStateRef;
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
        moveCount = 0;
        gameStateRef = GameObject.Find("PersistantGameState").GetComponent<PersistantGameState>();
        gameStateRef.gameScore = 0;
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
                if(blockMove && moveCount % 2 == 0){
                    // only play every two moves
                    blockMove.Play(0);
                }
                moveCount ++;
                timeElapsed = 0;
                transform.position = new Vector3(transform.position.x + direction, transform.position.y, transform.position.z);
            }
            // Check for space press
            if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space)){

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

                // Play block placed sound
                if(blockPlaced){
                    blockPlaced.Play(0);
                }

                // Update score
                gameStateRef.gameScore += tilemap.size.x;


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
        if(gameWin){
            gameWin.Play(0);
        }
        tilemap.color = Color.green;
        StartCoroutine("GameWinSceneAfterDelay");
        transform.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        InvokeRepeating("flicker", 0, 0.4f);
    }
    void triggerGameOverAnimation(){
        StartCoroutine("GameOverSceneAfterDelay");
         // Play block placed sound
        if(gameOver){
            gameOver.Play(0);
        }
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
        yield return new WaitForSeconds(3);
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
