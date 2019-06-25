using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerManager : TurnManager
{

    static int i = 0; //indice provvisorio per il cambio della scena

    //public EnemyManager enemyManager;

    public GameObject localCamera;

    public PlayerMover playerMover;
    public PlayerInput playerInput;

    public LayerMask obstacleLayer;

    public Animator PlayerAnimatorController;

    public Canvas PauseCanvas;

    //public bool spottedPlayer = false;


    public bool hasLightBulb = false;
    public bool hasFlashLight = false;

    public bool reset = false;

    float m_timer;
    public bool speedUp;

    public bool usingController;

    BoardManager m_board;

    [HideInInspector]
    public GameManager m_gm;

    ArrayList playerPath;

    public float killDelay = 1f; //Maggiore è il valore , più tardi parte l'animazion e uccisione
    public float attackDelay = .7f;

    public LineRenderer lr;


    Ray rayFront;
    Ray rayLeft;
    Ray rayRight;
    Ray rayBack;

    enum m_inputType {
        Controller,
        Keyboard
    };

    

    public void Setup()
    {
        base.Awake();

        usingController = false;

        playerMover = GetComponent<PlayerMover>();

        playerInput = GetComponent<PlayerInput>();

        m_inputType inputDevice = m_inputType.Controller;

        //lr = Object.FindObjectOfType<LineRenderer>().GetComponent<LineRenderer>();
        lr.gameObject.SetActive(false);

        //enemyManager = GetComponent<EnemyManager>();

        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_gm = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();

        playerPath = new ArrayList();

        if(SceneManager.GetActiveScene().buildIndex > 1) {
            //show the porcodio wand
        }

    }

    IEnumerator DisableLineRenderer()
    {
        lr.SetPosition(0, transform.position);
        yield return new WaitForSeconds(.5f);
        lr.gameObject.SetActive(false);
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position);

    }
    public void EnemyAnimationReset()
    {
        foreach (EnemyManager enemy in m_gm.m_enemies)
        {
            if (enemy != null)
            {
                enemy.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 0);
            }
        }

    }

   

    IEnumerable InputDelay()
    {
        playerMover.isMoving = true;
        yield return new WaitForSeconds(1f);
        playerMover.isMoving = false;
    }

    void DrawFlashlight()
    {
        if (playerInput.F && hasFlashLight)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 6);
            transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (playerInput.F_up)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 0);
            transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
            if (hasFlashLight) {
                transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    void DrawWand()
    {
        if (playerInput.P && SceneManager.GetActiveScene().buildIndex >= 6) 
        {
            PlayerAnimatorController.SetInteger("PlayerState", 3);
            //Bacchetta set active sulla mano
        }
        else if (playerInput.P_up)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 0);
        }
    }
 

    void Update()
    {

        if (isKeyboardInput() && !isControllerInput()) {
            usingController = false;
        }
        else if (!isKeyboardInput() && isControllerInput()) {
            usingController = true;
        }


        m_timer += Time.deltaTime;
        
        enemyDetection();

        CaptureEnemies();

        StartCoroutine(PlayAttackAnimation());

        if (GameManager.Instance.IsGameplay)
        {
            if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player)
            {
                if (!reset)
                {
                    reset = true;

                    if (m_timer < .5f)
                    {

                        speedUp = true;
                    }
                    else
                    {
                        
                        speedUp = false;
                    }
                    m_timer = 0;
                    return;
                }

            }
            else
            {
                reset = false;
            }


            if (!playerMover.isMoving)
            {
                playerInput.GetKeyInput();
                DrawWand();
                DrawFlashlight();
            }

            if (m_board.playerNode.isATrigger)
            {
                m_board.playerNode.StopTriggerRotation(true);
            }


            //enemyInGateDetection();


            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                transform.GetChild(3).gameObject.SetActive(false);
                hasFlashLight = false;

                lr.transform.gameObject.SetActive(true);
                m_gm.NextLevel();
            } //Switch livello successivo
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                transform.GetChild(3).gameObject.SetActive(false);
                hasFlashLight = false;

                lr.transform.gameObject.SetActive(true);
                m_gm.PreviousLevel();
            } //Switch livello precedente


            if (m_board.playerNode != null)
            {

                if (m_board.playerNode.isASwitch && playerInput.S)
                {
                    Debug.Log("S");
                    StartCoroutine("InputDelay");
                    PlayerAnimatorController.SetInteger("PlayerState", 2);
                    bool switchState = m_board.playerNode.GetSwitchState();


                    if (switchState)
                    {
                        m_board.playerNode.UpdateSwitchToFalse();
                        m_gm.CurrentTurn = Turn.Enemy;
                        m_gm.CurrentTurn = Turn.Player;


                    }
                    else
                    {
                        m_board.playerNode.UpdateSwitchToTrue();
                        m_gm.CurrentTurn = Turn.Enemy;
                        m_gm.CurrentTurn = Turn.Player;
                    }
                }



                if (playerInput.ESC)
                {
                    if (PauseCanvas.gameObject.activeSelf)
                    {
                        PauseCanvas.gameObject.SetActive(false);
                    }
                    else
                    {
                        PauseCanvas.gameObject.SetActive(true);
                    }

                }


                if (playerInput.R)
                {
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    lr.transform.gameObject.SetActive(true);
                    m_gameManager.LoseLevel();
                }

                
                #region Setup input levels 1 , 2 , 3 , 10
                if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 10)
                {
                    if (playerInput.V == 0 && !playerInput.F)
                    {

                        if (playerInput.H < 0)
                        {
                            EnemyAnimationReset();

                            foreach (EnemyManager enemy in m_gm.m_enemies)
                            {
                                enemy.m_enemySensor.m_foundPlayer = false;
                            }

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                            { //Aggiunto AND per evtiare di entrare nei MO facendo la pull verso di essi
                                playerMover.MoveLeft();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullLeft();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0)))[0].leftBlocked)
                                { //Se alla nostra Sx c'è un M.O e non è bloccato

                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushLeft();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }

                                    if (m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                                    {
                                        playerMover.MoveLeft();
                                    }


                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveLeft();
                                }
                            }
                            //END LEFT
                        }
                        else if (playerInput.H > 0)
                        {
                            EnemyAnimationReset();
                            foreach (EnemyManager enemy in m_gm.m_enemies)
                            {
                                enemy.m_enemySensor.m_foundPlayer = false;
                            }

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                            {
                                playerMover.MoveRight();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullRight();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)))[0].rightBlocked)
                                { //Se alla nostra Dx c'è un M.O e non è bloccato
                                    playerMover.MoveRight();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushRight();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveRight();
                                }
                            }

                        }

                    }
                    else if (playerInput.H == 0 && !playerInput.F)
                    {
                        if (playerInput.V < 0)
                        {
                            EnemyAnimationReset();
                            foreach (EnemyManager enemy in m_gm.m_enemies)
                            {
                                enemy.m_enemySensor.m_foundPlayer = false;
                            }

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                            {
                                playerMover.MoveBackward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullBackward();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f)))[0].downBlocked)
                                { //Se sotto non c'è un M.O e non è bloccato
                                    playerMover.MoveBackward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushBackward();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveBackward();
                                }
                            }
                        }
                        else if (playerInput.V > 0)
                        {
                            EnemyAnimationReset();
                            foreach (EnemyManager enemy in m_gm.m_enemies)
                            {
                                enemy.m_enemySensor.m_foundPlayer = false;
                            }

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                            {
                                playerMover.MoveForward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullForward();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f)))[0].upBlocked)
                                { //Se sopra non c'è un M.O e non è bloccato

                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushForward();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }


                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveForward();
                                }
                            }
                        }
                    }
                    
                    if (hasFlashLight)
                    {

                        if (playerInput.F && playerInput.V > 0)
                        {//sparo in alto

                            transform.DORotate(Vector3.forward, 0);

                            RaycastHit hit;
                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(1000, 1000, 1000));

                            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100, obstacleLayer))
                            {
                                Debug.Log("Shoot up");
                                
                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.gameObject.SetActive(true);
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        break;
                                    case "Mirror":
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.gameObject.SetActive(true);
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                Debug.Log("case 0");
                                                break;

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                Debug.Log("case 1");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;

                                }

                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.V < 0)
                        {//sparo in basso

                            transform.DORotate(Vector3.back , 0);

                            lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            lr.gameObject.SetActive(true);

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.back, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.back * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        break;
                                    case "Mirror":

                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                Debug.Log("case 2");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                Debug.Log("case 3");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }

                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.H > 0)
                        {//sparo a destra

                            transform.DORotate(Vector3.right, 0);

                            lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            lr.gameObject.SetActive(true);

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.right, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        break;
                                    case "Mirror":

                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                Debug.Log("case 1");
                                                break;

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                Debug.Log("case 2");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }

                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.H < 0)
                        {//sparo a sinistra

                            transform.DORotate(Vector3.left, 0);

                            lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            lr.gameObject.SetActive(true);

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.left, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        break;
                                    case "Mirror":

                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));

                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                Debug.Log("case 0");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                Debug.Log("case 3");
                                                break;

                                        }


                                        break;
                                    case "Wall":
                                        break;
                                    case "Sword":
                                        break;

                                }

                                StartCoroutine(DisableLineRenderer());
                            }
                        }
                    }

                }//if

                #endregion

                #region Setup input levels 4 , 5 , 6 , 7 , 8, 9
                else if (SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 7 || SceneManager.GetActiveScene().buildIndex == 8 || SceneManager.GetActiveScene().buildIndex == 9)
                {
                    if (playerInput.H == 0 && !playerInput.F)
                    {
                        if (playerInput.V < 0)
                        {
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                            { //Aggiunto AND per evtiare di entrare nei MO facendo la pull verso di essi
                                playerMover.MoveLeft();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullLeft();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0)))[0].leftBlocked)
                                { //Se alla nostra Sx c'è un M.O e non è bloccato

                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushLeft();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }

                                    if (m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                                    {

                                        playerMover.MoveLeft();
                                    }


                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveLeft();
                                }
                            }
                            //END LEFT
                        }
                        else if (playerInput.V > 0)
                        {
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                            {
                                playerMover.MoveRight();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullRight();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)))[0].rightBlocked)
                                { //Se alla nostra Dx c'è un M.O e non è bloccato
                                    playerMover.MoveRight();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushRight();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveRight();
                                }
                            }

                        }

                    }
                    else if (playerInput.V == 0 && !playerInput.F)
                    {
                        if (playerInput.H > 0)
                        {
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                            {
                                playerMover.MoveBackward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullBackward();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f)))[0].downBlocked)
                                { //Se sotto non c'è un M.O e non è bloccato
                                    playerMover.MoveBackward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushBackward();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveBackward();
                                }
                            }
                        }
                        else if (playerInput.H < 0)
                        {
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                            {
                                playerMover.MoveForward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.PullForward();
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f)))[0].upBlocked)
                                { //Se sopra non c'è un M.O e non è bloccato
                                    playerMover.MoveForward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.PushForward();
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);
                                    }
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveForward();
                                }
                            }
                        }
                    }

                    if (hasFlashLight)
                    {
                        if (playerInput.F && playerInput.H < 0)
                        {//sparo in alto


                            transform.DORotate(Vector3.forward, 0);

                            RaycastHit hit;

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100, obstacleLayer))
                            {
                                Debug.Log("Shoot up");
                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState" , 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lr.gameObject.SetActive(true);
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                Debug.Log("case 0");
                                                break;

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                Debug.Log("case 1");
                                                break;

                                        }


                                        break;
                                    case "Wall":
                                        break;
                                    case "Sword":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.H > 0)
                        {//sparo in basso

                            transform.DORotate(Vector3.back, 0);

                            RaycastHit hit;


                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.back, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.back * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lr.gameObject.SetActive(true);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                Debug.Log("case 2");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                Debug.Log("case 3");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.V > 0)
                        {//sparo a destra

                            transform.DORotate(Vector3.forward + new Vector3(0, 90, 0), 0);

                            RaycastHit hit;

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.right, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lr.gameObject.SetActive(true);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                Debug.Log("case 1");
                                                break;

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                Debug.Log("case 2");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                StartCoroutine(DisableLineRenderer());
                            }
                        }

                        if (playerInput.F && playerInput.V < 0)
                        {//sparo a sinistra
                            
                            transform.DORotate(Vector3.forward - new Vector3(0,90,0) , 0);
                            
                            RaycastHit hit;

                            lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));


                            if (Physics.Raycast(transform.position, Vector3.left, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);

                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lr.gameObject.SetActive(true);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;

                                    case "Mirror":
                                        lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        transform.GetChild(3).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                Debug.Log("case 0");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                Debug.Log("case 3");
                                                break;

                                        }


                                        break;
                                    case "Wall":
                                        break;

                                    case "Sword":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                StartCoroutine(DisableLineRenderer());
                            }
                        }
                    }
                }
                #endregion
            }
        }

    }

    IEnumerator PlayAttackAnimation() {
        if (m_board != null) {

            List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position + transform.forward * (BoardManager.spacing - attackDelay)));
            
            //PlayerAnimatorController.SetInteger("PlayerState" , 1);
            if (enemies.Count != 0) {
                foreach (EnemyManager enemy in enemies) {
                    if (enemy != null && enemy.GetMovementType() != MovementType.Boss) {
                        PlayerAnimatorController.SetInteger("PlayerState", 1);
                        //WAND SETACTIVE FALSE
                        //HANDWAND SETACTIVE TRUE
                    }
                }
                yield return new WaitForSeconds(.28f);//.28 seconds --> durata dell'animazione d'attacco
                PlayerAnimatorController.SetInteger("PlayerState", 0);

                //WAND SETACTIVE TRUE
                //HANDWAND SETACTIVE FALSE

            }
            //PlayerAnimatorController.SetInteger("PlayerState", 0);
        }
    }

    void CaptureEnemies()
    {
        if (m_board != null)
        {
            
            List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position + transform.forward * (BoardManager.spacing - killDelay)));
            


            //PlayerAnimatorController.SetInteger("PlayerState" , 1);
            if (enemies.Count != 0)
            {
                foreach (EnemyManager enemy in enemies)
                {
                    if (enemy != null && enemy.GetMovementType() != MovementType.Boss)
                    {
                        
                        enemy.Die();
                    }
                }
            }
            //PlayerAnimatorController.SetInteger("PlayerState", 0);
        }
    }//Uccisione che avviene quando il player va ad occupare la casella del nemico

    void enemyDetection()
    {
        
        rayFront = new Ray(transform.position, Vector3.forward);
        rayBack = new Ray(transform.position, Vector3.back);
        rayLeft = new Ray(transform.position, Vector3.left);
        rayRight = new Ray(transform.position, Vector3.right);

        RaycastHit hit;

        if (hasFlashLight)
        {
            if (Physics.Raycast(rayFront, out hit, 100, obstacleLayer))
            {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);

                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<EnemyManager>().isDetected = true;
                }
            }

            if (Physics.Raycast(rayLeft, out hit, 100, obstacleLayer))
            {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);

                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<EnemyManager>().isDetected = true;
                }
            }

            if (Physics.Raycast(rayRight, out hit, 100, obstacleLayer))
            {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);

                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<EnemyManager>().isDetected = true;
                }
            }

            if (Physics.Raycast(rayBack, out hit, 100, obstacleLayer))
            {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.down * hit.distance, Color.red);

                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<EnemyManager>().isDetected = true;
                }
            }
        }



    }//Setta il nemico a isDetected quando si trova in una delle nostre 4 direzioni


    public void UpdatePlayerPath()
    {
        playerPath.Add(m_board.playerNode);

    }//Aggiornamento path del player per permettere al chaser di seguirlo

    public Node GetPlayerPath(int i)
    {
        Node playerNode = (Node)playerPath[i];
        return playerNode;
    }

    //public void clearPlayerPath()
    //{
    //    playerPath.Clear();
    //    EnemyMover.index = 0;

    //}


    public override void FinishTurn()
    {

        base.FinishTurn();
    }

    public void PlayerDead()
    {
        m_gm.LoseLevel();
    }


    public ItemData GetData()
    {
        ItemData itemData = new ItemData()
        {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Player,
        };
        return itemData;
    }

    public void enemyInGateDetection()
    {

        foreach (var enemy in m_gameManager.m_enemies)
        {
            Debug.Log(m_board.FindNodeAt(enemy.transform.position).gateOpen);
            if (enemy != null && m_board.FindNodeAt(enemy.transform.position).isAGate && m_board.FindNodeAt(enemy.transform.position).gateOpen && enemy.GetEnemySensor.FoundPlayer)
            {
                Debug.Log("LoseLevel");
                m_gameManager.LoseLevel();
            }
        }

    }//Se l'enemy è in una shadow allora non può ucciderci ma attivera un'animazione di "danger"


    bool isKeyboardInput() {
        if (Input.anyKeyDown) {
            return true;
        }
        return false;
    }

    bool isControllerInput(){
        
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
            Input.GetKey(KeyCode.Joystick1Button1) ||
            Input.GetKey(KeyCode.Joystick1Button2) ||
            Input.GetKey(KeyCode.Joystick1Button3) ||
            Input.GetKey(KeyCode.Joystick1Button4) ||
            Input.GetKey(KeyCode.Joystick1Button5) ||
            Input.GetKey(KeyCode.Joystick1Button6) ||
            Input.GetKey(KeyCode.Joystick1Button7) ||
            Input.GetKey(KeyCode.Joystick1Button8) ||
            Input.GetKey(KeyCode.Joystick1Button9) ||
            Input.GetKey(KeyCode.Joystick1Button10) ||
            Input.GetKey(KeyCode.Joystick1Button11) ||
            Input.GetKey(KeyCode.Joystick1Button12) ||
            Input.GetKey(KeyCode.Joystick1Button13) ||
            Input.GetKey(KeyCode.Joystick1Button14) ||
            Input.GetKey(KeyCode.Joystick1Button15) ||
            Input.GetKey(KeyCode.Joystick1Button16) ||
            Input.GetKey(KeyCode.Joystick1Button17) ||
            Input.GetKey(KeyCode.Joystick1Button18) ||
            Input.GetKey(KeyCode.Joystick1Button19)){
            return true;
        }

        if (Input.GetAxisRaw("VerticalJ") > 0 || Input.GetAxisRaw("VerticalJ") < 0) {
            return true;
        }

        if (Input.GetAxisRaw("HorizontalJ") > 0 || Input.GetAxisRaw("HorizontalJ") < 0) {
            return true;
        }

        return false;
    }

    #region sceneChanger

    public void SceneChanger(int i)
    {

        switch (i)
        {
            case 1:
                SceneManager.LoadScene("Level 1");
                break;
            case 2:
                SceneManager.LoadScene("Level 2");
                break;
            case 3:
                SceneManager.LoadScene("Level 3");
                break;
            case 4:
                SceneManager.LoadScene("Level 4");
                break;
            case 5:
                SceneManager.LoadScene("Level 5");
                break;
            case 6:
                SceneManager.LoadScene("Level 6");
                break;
            case 7:
                SceneManager.LoadScene("Level 7");
                break;
            case 8:
                SceneManager.LoadScene("Level 8");
                break;
            case 9:
                SceneManager.LoadScene("Level 9");
                break;
            case 10:
                SceneManager.LoadScene("Level 10");
                break;
        }



    }



    #endregion //PROVVISORIO

}