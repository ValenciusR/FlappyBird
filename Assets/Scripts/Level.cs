using CodeMonkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POS = -200f;
    private const float PIPE_SPAWN_X_POS = 200f;
    private const float GROUND_DESTROY_X_POS = -200f;
    private const float CLOUD_DESTROY_X_POS = -160f;
    private const float CLOUD_SPAWN_X_POS = 160f;
    private const float BIRD_X_POS = 0f;

    private static Level instance;

    public static Level GetInstance() { return instance; }

    private List<Pipe> pipeList;
    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        Playing,
        BirdDead,
        WaitingToStart,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        SpawnInitialGround();
        spawnInitialCloud();
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
        
    }

    private void Bird_OnStartedPlaying(object sender, EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        CMDebug.TextPopupMouse("Dead");
        state = State.BirdDead;
    }

    private void Update()
    {
        if(state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleClouds();
        }
        
    }

    private void HandleClouds()
    {
        cloudSpawnTimer -= Time.deltaTime;
        if(cloudSpawnTimer < 0)
        {
            float cloudSpawnTimerMax = 7f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            float cloudY = 30f;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_X_POS, cloudY, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }
        //Cloud movement
        for (int i = 0; i <cloudList.Count; i++)
        {
            Transform t = cloudList[i];
            t.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * .7f;

            if (t.position.x < CLOUD_DESTROY_X_POS)
            {
                Destroy(t.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private Transform GetCloudPrefabTransform()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            default:
            case 0: return GameAssets.GetInstance().pfCloud_1;
            case 1: return GameAssets.GetInstance().pfCloud_2;
            case 2: return GameAssets.GetInstance().pfCloud_3;
                
        }
    }

    private void spawnInitialCloud()
    {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        float cloudY = 30f;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, cloudY, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundWidth = 200f;
        float groundY = -47.5f;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGround()
    {
        foreach (Transform t in groundList)
        {
            t.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if(t.position.x < GROUND_DESTROY_X_POS)
            {
                //Ground passed the left side, put to right side
                float rightMostXPosition = -100f;
                for(int i = 0; i < groundList.Count; i++)
                {
                    if (groundList[i].position.x > rightMostXPosition)
                    {
                        rightMostXPosition = groundList[i].position.x;  
                    }
                }
                //Place Ground on the right most position
                float groundWidth = 200f;
                t.position = new Vector3(rightMostXPosition + groundWidth, t.position.y, t.position.z);

            }
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if( pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;
            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POS);
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0;i<pipeList.Count;i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightofBird = pipe.GetXPos() > BIRD_X_POS;
            pipe.Move(PIPE_MOVE_SPEED);
            if(isToTheRightofBird && pipe.GetXPos() <= BIRD_X_POS && pipe.IsBottom())
            {
                //Pipe Passed Bird
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }
            if(pipe.GetXPos() < PIPE_DESTROY_X_POS)
            {
                //Destroy 
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty. Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Hard:
                gapSize = 35f;
                pipeSpawnTimerMax = 1f;
                break;
            case Difficulty.Impossible:
                gapSize = 28f;
                pipeSpawnTimerMax = 1f;
                break;
        }
    }
    private Difficulty GetDifficulty() {
        if (pipesSpawned >= 30) return Difficulty.Impossible;
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPos)
    {
        createPipe(gapY - gapSize * .5f, xPos, true);
        createPipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPos, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void createPipe(float height, float xPos, bool createBottom)
    {
        //Set up Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPos;
        if (createBottom)
        {
            pipeHeadYPos = -CAMERA_ORTHO_SIZE + height - HEAD_HEIGHT * .5f;
        }
        else
        {
            pipeHeadYPos = +CAMERA_ORTHO_SIZE - height + HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3 (xPos, pipeHeadYPos);

        //Set up Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPos;
        if (createBottom)
        {
            pipeBodyYPos = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPos = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPos, pipeBodyYPos);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetPipesPassedSpawned()
    {
        return pipesPassedCount;
    }

}
