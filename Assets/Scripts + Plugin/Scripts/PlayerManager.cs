using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerManager : TurnManager
{
    bool isPressed;

    static int i = 0; //indice provvisorio per il cambio della scena

    //public EnemyManager enemyManager;

    public Button StartButton;

    public GameObject localCamera;

    public PlayerMover playerMover;
    public PlayerInput playerInput;

    public LayerMask obstacleLayer;

    public Animator PlayerAnimatorController;

    public Canvas PauseCanvas;

    public UiManager UiManager;

    public LightShaft lightShaft;

    //public bool spottedPlayer = false;

    public bool GodMode = false;

    public bool hasLightBulb = false;
    public bool hasFlashLight = false;
    public bool hasWand = false;

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

    //public LineRenderer lr;


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
        isPressed = false;

        base.Awake();

        usingController = false;

        playerMover = GetComponent<PlayerMover>();

        playerInput = GetComponent<PlayerInput>();

        

        m_inputType inputDevice = m_inputType.Controller;

        //lr = Object.FindObjectOfType<LineRenderer>().GetComponent<LineRenderer>();
        //lr.gameObject.SetActive(false);

        //enemyManager = GetComponent<EnemyManager>();

        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_gm = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();

        playerPath = new ArrayList();

        if(SceneManager.GetActiveScene().buildIndex > 1) {
            transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
        }

    }

    IEnumerator DisableLineRenderer()
    {
        //lr.SetPosition(0, transform.position);
        yield return new WaitForSeconds(.5f);
        //lr.gameObject.SetActive(false);
        //lr.SetPosition(0, transform.position);
        //lr.SetPosition(1, transform.position);

    }

    IEnumerator DisableLightShaft()
    {
        yield return new WaitForSeconds(.5f);
        lightShaft.gameObject.SetActive(false);
        lightShaft.transform.localScale = new Vector3(1,1,1);
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
        PlayerAnimatorController.SetInteger("PlayerState", 0);
        playerMover.isMoving = false;
    }

    void DrawFlashlight()
    {
        if (playerInput.F && hasFlashLight)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 6);
            
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (playerInput.F_up)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 0);
            //transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
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
            hasWand = true;
    //Bacchetta set active sulla mano
}
        else if (playerInput.P_up)
        {
            PlayerAnimatorController.SetInteger("PlayerState", 0);
            hasWand = false;
        }
    }

    void skipCutscene() {
        if (m_gm.Cutscene.activeSelf) {
            if (Input.anyKeyDown || isControllerInputSkip()) {
                m_gm.firstCutscene.Stop();
                m_gm.secondCutscene.Stop();
                m_gm.Cutscene.SetActive(false);
                playerInput.InputEnabled = true;
            }
        }
    }
 

    void Update()
    {
        
        if (UiManager.isCover && Input.anyKeyDown) {
            GameManager.stateMainMenu();
        }



        if (isKeyboardInput() && !isControllerInput()) {
            usingController = false;
        }
        else if (!isKeyboardInput() && isControllerInput()) {
            usingController = true;
        }


        m_timer += Time.deltaTime;
        
        enemyDetection();
        movableFloating();

        CaptureEnemies();

        StartCoroutine(PlayAttackAnimation());

        if (GameManager.Instance.IsGameplay)
        {

            
            skipCutscene();
            

            if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player)
            {
                if (!reset)
                {
                    reset = true;

                    if (m_timer < 1f)
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
                PlayerAnimatorController.SetInteger("PlayerState", 0);
                playerInput.GetKeyInput();
                DrawWand();
                DrawFlashlight();
            }

            if (m_board.playerNode.isATrigger)
            {
                m_board.playerNode.triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(true);
                m_board.playerNode.triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
            }


            //enemyInGateDetection();


            if (Input.GetKeyDown(KeyCode.KeypadPlus)  && GodMode)
            {
                transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                hasFlashLight = false;

                //lr.transform.gameObject.SetActive(true);
                m_gm.NextLevel();
            } //Switch livello successivo
            else if (Input.GetKeyDown(KeyCode.KeypadMinus) && GodMode)
            {
                transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                hasFlashLight = false;

                //lr.transform.gameObject.SetActive(true);
                m_gm.PreviousLevel();
            } //Switch livello precedente


            if (m_board.playerNode != null)
            {

                if (m_board.playerNode.isASwitch && playerInput.S)
                {
                    //Debug.Log("S");
                    StartCoroutine("InputDelay");
                    PlayerAnimatorController.SetInteger("PlayerState", 2);
                    bool switchState = m_board.playerNode.GetSwitchState();


                    if (switchState)
                    {
                        m_board.playerNode.UpdateSwitchToFalse();
                    }
                    else
                    {
                        m_board.playerNode.UpdateSwitchToTrue();
                        if (SceneManager.GetActiveScene().buildIndex == 3)
                        {
                            m_gm.CurrentTurn = Turn.Enemy;
                        }
                        m_gm.CurrentTurn = Turn.Player;
                    }
                }



                if (playerInput.ESC && playerInput.PauseInputEnabled)
                {
                    if (PauseCanvas.transform.GetChild(0).gameObject.activeSelf)
                    {
                        PauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.stateGameplay();
                    }
                    else
                    {
                        PauseCanvas.transform.GetChild(0).gameObject.SetActive(true);
                        GameManager.statePause();
                    }

                }


                if (playerInput.R)
                {
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    //lr.transform.gameObject.SetActive(true);
                    m_gameManager.LoseLevel();
                }


                #region Setup input levels 1 , 2 , 3 , 6 , 10
                if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 10)
                {
                    if (playerInput.V == 0 && !playerInput.F)
                    {

                        if (playerInput.H < 0)
                        {
                            EnemyAnimationReset();

                            //foreach (EnemyManager enemy in m_gm.m_enemies)
                            //{
                            //    enemy.m_enemySensor.m_foundPlayer = false;
                            //}

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                            { //Aggiunto AND per evtiare di entrare nei MO facendo la pull verso di essi
                                playerMover.MoveLeft();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.left);
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0)))[0].leftBlocked)
                                { //Se alla nostra Sx c'è un M.O e non è bloccato

                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.left);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);

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
                            //foreach (EnemyManager enemy in m_gm.m_enemies)
                            //{
                            //    enemy.m_enemySensor.m_foundPlayer = false;
                            //}

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)).gateOpen)
                            {
                                playerMover.MoveRight();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.right);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)))[0].rightBlocked)
                                { //Se alla nostra Dx c'è un M.O e non è bloccato
                                    playerMover.MoveRight();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.right);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);
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
                            //foreach (EnemyManager enemy in m_gm.m_enemies)
                            //{
                            //    enemy.m_enemySensor.m_foundPlayer = false;
                            //}

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                            {
                                playerMover.MoveBackward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.back);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f)))[0].downBlocked)
                                { //Se sotto non c'è un M.O e non è bloccato
                                    playerMover.MoveBackward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.back);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);
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
                            //foreach (EnemyManager enemy in m_gm.m_enemies)
                            //{
                            //    enemy.m_enemySensor.m_foundPlayer = false;
                            //}

                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                            {
                                playerMover.MoveForward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.forward);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f)))[0].upBlocked)
                                { //Se sopra non c'è un M.O e non è bloccato
                                    playerMover.MoveForward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.forward);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);


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
                                //Debug.Log("Shoot up");
                                
                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z) / 2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z) / 2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                //Debug.Log("case 0");
                                                break;

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                //Debug.Log("case 1");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;

                                }

                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.V < 0)
                        {//sparo in basso
                            transform.DORotate(Vector3.back , 0);

                            //lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            //lr.gameObject.SetActive(true);

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.back, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.back * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z + transform.position.z) / 4);
                                        hit.collider.GetComponent<EnemyManager>().Die();

                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z) / 2);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                //Debug.Log("case 2");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                //Debug.Log("case 3");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }

                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.H > 0)
                        {//sparo a destra

                            transform.DORotate(Vector3.right, 0);

                            //lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            //lr.gameObject.SetActive(true);

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.right, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x - transform.position.x) / 2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x - transform.position.x) / 2);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                //Debug.Log("case 1");
                                                break;

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                //Debug.Log("case 2");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }

                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.H < 0)
                        {//sparo a sinistra

                            transform.DORotate(Vector3.left, 0);

                            //lr.gameObject.SetActive(true);
                            RaycastHit hit;
                            //lr.gameObject.SetActive(true);

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.left, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x + transform.position.x) / 4);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x + transform.position.x) / 4);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;

                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                //Debug.Log("case 0");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                //Debug.Log("case 3");
                                                break;

                                        }


                                        break;
                                    case "Wall":
                                        break;
                                    case "Sword":
                                        break;

                                }

                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }
                    }

                }//if

                #endregion 


                #region Setup input levels 4 , 5 , 7 , 8, 9
                else if (SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 7 || SceneManager.GetActiveScene().buildIndex == 8 || SceneManager.GetActiveScene().buildIndex == 9)
                {
                    if (playerInput.H == 0 && !playerInput.F)
                    {
                        if (playerInput.V < 0)
                        {
                            //if (isPressed == false)
                            //{
                                EnemyAnimationReset();
                                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0)
                                { //Aggiunto AND per evtiare di entrare nei MO facendo la pull verso di essi
                                    playerMover.MoveLeft();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Pull(MovableObject.direction.left);

                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 5);
                                }
                                else
                                {
                                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0)))[0].leftBlocked)
                                    { //Se alla nostra Sx c'è un M.O e non è bloccato

                                        foreach (var movableObject in m_gm.GetMovableObjects())
                                        {
                                            movableObject.Push(MovableObject.direction.left);

                                        }
                                        PlayerAnimatorController.SetInteger("PlayerState", 4);

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
                            //}

                            
                            //END LEFT
                        }
                        else if (playerInput.V > 0)
                        {
                            isPressed = true;
                            EnemyAnimationReset();
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                            {
                                playerMover.MoveRight();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.right);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)))[0].rightBlocked)
                                { //Se alla nostra Dx c'è un M.O e non è bloccato
                                    playerMover.MoveRight();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.right);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveRight();
                                }
                            }

                        }
                        isPressed = true;

                    }
                    else if (playerInput.V == 0 && !playerInput.F)
                    {
                        if (playerInput.H > 0)
                        {
                            EnemyAnimationReset();
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                            {
                                playerMover.MoveBackward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.back);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f)))[0].downBlocked)
                                { //Se sotto non c'è un M.O e non è bloccato
                                    playerMover.MoveBackward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.back);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);
                                }
                                else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindArmorsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0 && m_board.FindSwordsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0)
                                { //Se non c'è nulla muovi solo il pg
                                    playerMover.MoveBackward();
                                }
                            }
                        }
                        else if (playerInput.H < 0)
                        {
                            EnemyAnimationReset();
                            if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0)
                            {
                                playerMover.MoveForward();
                                foreach (var movableObject in m_gm.GetMovableObjects())
                                {
                                    movableObject.Pull(MovableObject.direction.forward);
                                    
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 5);
                            }
                            else
                            {
                                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f)))[0].upBlocked)
                                { //Se sopra non c'è un M.O e non è bloccato
                                    playerMover.MoveForward();
                                    foreach (var movableObject in m_gm.GetMovableObjects())
                                    {
                                        movableObject.Push(MovableObject.direction.forward);
                                        
                                    }
                                    PlayerAnimatorController.SetInteger("PlayerState", 4);
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

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));
                            

                            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100, obstacleLayer))
                            {
                                //Debug.Log("Shoot up");
                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState" , 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z)/2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z) / 2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                //Debug.Log("case 0");
                                                break;

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                //Debug.Log("case 1");
                                                break;

                                        }


                                        break;
                                    case "Wall":
                                        break;
                                    case "Sword":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.H > 0)
                        {//sparo in basso

                            transform.DORotate(Vector3.back, 0);

                            RaycastHit hit;


                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.back, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.back * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z + transform.position.z) / 4);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.z - transform.position.z) / 2);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                                //Debug.Log("case 2");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                                //Debug.Log("case 3");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.V > 0)
                        {//sparo a destra

                            transform.DORotate(Vector3.forward + new Vector3(0, 90, 0), 0);

                            RaycastHit hit;

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));

                            if (Physics.Raycast(transform.position, Vector3.right, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);
                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x - transform.position.x) / 2);
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;
                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x - transform.position.x) / 2);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 1:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                //Debug.Log("case 1");
                                                break;

                                            case 2:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                //Debug.Log("case 2");
                                                break;

                                        }

                                        break;
                                    case "Wall":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
                            }
                        }

                        if (playerInput.F && playerInput.V < 0)
                        {//sparo a sinistra
                            
                            transform.DORotate(Vector3.forward - new Vector3(0,90,0) , 0);
                            
                            RaycastHit hit;

                            //lr.SetPosition(0, transform.GetChild(3).gameObject.transform.position + new Vector3(0, 1, 0));


                            if (Physics.Raycast(transform.position, Vector3.left, out hit, 100, obstacleLayer))
                            {

                                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);
                                PlayerAnimatorController.SetInteger("PlayerState", 9);

                                switch (hit.collider.tag)
                                {
                                    case "Enemy":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 1));
                                        //float conversion = (hit.collider.transform.position.x + transform.position.x) / 4;
                                        //Mathf.Abs(conversion);
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x + transform.position.x) / 4);
                                        if (lightShaft) {

                                        }
                                        hit.collider.GetComponent<EnemyManager>().Die();
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        break;

                                    case "Mirror":
                                        lightShaft.gameObject.SetActive(true);
                                        //lr.gameObject.SetActive(true);
                                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                                        //lr.SetPosition(1, hit.point + new Vector3(0, 1, 0));
                                        lightShaft.lightShaftScale((hit.collider.transform.position.x + transform.position.x) / 4);
                                        transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                                        hasFlashLight = false;
                                        switch (index)
                                        {

                                            case 0:
                                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                                //Debug.Log("case 0");
                                                break;

                                            case 3:
                                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                                //Debug.Log("case 3");
                                                break;

                                        }

                                        
                                        break;
                                    case "Wall":
                                        break;

                                    case "Sword":
                                        break;
                                }
                                PlayerAnimatorController.SetInteger("PlayerState", 0);
                                //StartCoroutine(DisableLineRenderer());
                                StartCoroutine(DisableLightShaft());
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

    void movableFloating() {

        rayFront = new Ray(transform.position, Vector3.forward);
        rayBack = new Ray(transform.position, Vector3.back);
        rayLeft = new Ray(transform.position, Vector3.left);
        rayRight = new Ray(transform.position, Vector3.right);

        RaycastHit hit;

        if (hasWand) {
            if (Physics.Raycast(rayFront, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position , transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().floatingAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            

            if (Physics.Raycast(rayLeft, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().floatingAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            if (Physics.Raycast(rayRight, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().floatingAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            if (Physics.Raycast(rayBack, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.down * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().floatingAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
        else if (!hasWand && SceneManager.GetActiveScene().buildIndex >= 6) {
            if (Physics.Raycast(rayFront, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.up * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().resetAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            if (Physics.Raycast(rayLeft, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().resetAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            if (Physics.Raycast(rayRight, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().resetAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            if (Physics.Raycast(rayBack, out hit, 100, obstacleLayer)) {
                Debug.DrawRay(GetComponent<PlayerManager>().transform.position + new Vector3(0, 0.5f), Vector3.down * hit.distance, Color.red);

                if (hit.collider.tag == "Wall" && Vector3.Distance(hit.collider.transform.position, transform.position) < 2.2f) {
                    hit.collider.GetComponent<MovableObject>().resetAnimation();
                    hit.collider.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
        
        
    }


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
            //Debug.Log(m_board.FindNodeAt(enemy.transform.position).gateOpen);
            if (enemy != null && m_board.FindNodeAt(enemy.transform.position).isAGate && m_board.FindNodeAt(enemy.transform.position).gateOpen && enemy.GetEnemySensor.FoundPlayer)
            {
                playerInput.InputEnabled = false;
                playerInput.PauseInputEnabled = false;
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

    public bool isControllerInput(){
        
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) ||
            Input.GetKeyDown(KeyCode.Joystick1Button1) ||
            Input.GetKeyDown(KeyCode.Joystick1Button2) ||
            Input.GetKeyDown(KeyCode.Joystick1Button3) ||
            Input.GetKeyDown(KeyCode.Joystick1Button4) ||
            Input.GetKeyDown(KeyCode.Joystick1Button5) ||
            Input.GetKeyDown(KeyCode.Joystick1Button6) ||
            Input.GetKeyDown(KeyCode.Joystick1Button7) ||
            Input.GetKeyDown(KeyCode.Joystick1Button8) ||
            Input.GetKeyDown(KeyCode.Joystick1Button9) ||
            Input.GetKeyDown(KeyCode.Joystick1Button10) ||
            Input.GetKeyDown(KeyCode.Joystick1Button11) ||
            Input.GetKeyDown(KeyCode.Joystick1Button12) ||
            Input.GetKeyDown(KeyCode.Joystick1Button13) ||
            Input.GetKeyDown(KeyCode.Joystick1Button14) ||
            Input.GetKeyDown(KeyCode.Joystick1Button15) ||
            Input.GetKeyDown(KeyCode.Joystick1Button16) ||
            Input.GetKeyDown(KeyCode.Joystick1Button17) ||
            Input.GetKeyDown(KeyCode.Joystick1Button18) ||
            Input.GetKeyDown(KeyCode.Joystick1Button19)){
            return true;
        }

        if (Input.GetAxisRaw("VerticalJAnalog") > 0 || Input.GetAxisRaw("VerticalJ") < 0) {
            return true;
        }

        if (Input.GetAxisRaw("HorizontalJAnalog") > 0 || Input.GetAxisRaw("HorizontalJ") < 0) {
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

    public bool isControllerInputSkip() {

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) ||
            Input.GetKeyDown(KeyCode.Joystick1Button1) ||
            Input.GetKeyDown(KeyCode.Joystick1Button2) ||
            Input.GetKeyDown(KeyCode.Joystick1Button3) ||
            Input.GetKeyDown(KeyCode.Joystick1Button4) ||
            Input.GetKeyDown(KeyCode.Joystick1Button5) ||
            Input.GetKeyDown(KeyCode.Joystick1Button6) ||
            Input.GetKeyDown(KeyCode.Joystick1Button7) ||
            Input.GetKeyDown(KeyCode.Joystick1Button8) ||
            Input.GetKeyDown(KeyCode.Joystick1Button9) ||
            Input.GetKeyDown(KeyCode.Joystick1Button10) ||
            Input.GetKeyDown(KeyCode.Joystick1Button11) ||
            Input.GetKeyDown(KeyCode.Joystick1Button12) ||
            Input.GetKeyDown(KeyCode.Joystick1Button13) ||
            Input.GetKeyDown(KeyCode.Joystick1Button14) ||
            Input.GetKeyDown(KeyCode.Joystick1Button15) ||
            Input.GetKeyDown(KeyCode.Joystick1Button16) ||
            Input.GetKeyDown(KeyCode.Joystick1Button17) ||
            Input.GetKeyDown(KeyCode.Joystick1Button18) ||
            Input.GetKeyDown(KeyCode.Joystick1Button19)) {
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