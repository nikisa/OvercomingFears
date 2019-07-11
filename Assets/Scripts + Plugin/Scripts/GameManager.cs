using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Video;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}

public class GameManager : MonoBehaviour
{

    bool RunGameLoopStarted = false; 

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private bool _isGameplay;
    public bool IsGameplay { get { return _isGameplay; } set { _isGameplay = value; } }

    public Animator SMController;

    public Animator UIController;


    BoardManager m_board;
    PlayerManager m_player;

    EnemyMover m_enemyMover;

    public GameObject Boss;
    public Vector3 directionToMove;
    Vector3 startPos;

    Vector3 frontalDest;

    public List<EnemyManager> m_enemies;
    List<EnemyMover> m_enemiesMovers;
    List<MovableObject> m_movableObjects;
    List<Armor> m_armors;
    List<Sword> m_sword;

    public GameObject[] TutorialsKeyboard;
    public GameObject[] TutorialsController;

    [HideInInspector]
    public GameObject popup;

    //[HideInInspector]
    public int playerPopupID;

    public RawImage rawImage;
    public VideoPlayer firstCutscene;
    public VideoPlayer secondCutscene;

    public GameObject Cutscene;
    //public GameObject secondCutscene;


    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return m_currentTurn; } set { m_currentTurn = value; } }

    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }

    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }

    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

    LevelManager m_levelManager;


    public float delay = 0;

    public float shadowKillDelay;

    private int m_index;

    #region UnityEvents
    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;
    #endregion

    #region Events

    public delegate void OnClick();
    public static OnClick stateCover;
    public static OnClick stateMenu;
    public static OnClick statePause;
    public static OnClick stateGameplay;
    public static OnClick stateOption;
    public static OnClick statePlayMenu;
    public static OnClick stateLevelSelection;
    public static OnClick stateMainMenu;
    public static OnClick statePopup;
    public static OnClick stateGameplayUI;


    void OnEnable()
    {
        statePlayMenu += SetPlayMenuTrigger;
        stateCover += SetCoverTrigger;
        stateGameplay += SetGameplayTrigger;
        statePause += SetPauseTrigger;
        stateMenu += SetMenuTrigger;
        stateOption += SetOptionTrigger;
        stateLevelSelection += SetLevelSelectionTrigger;
        stateMainMenu += SetMainMenuTrigger;
        statePopup += SetPopupTrigger;
        stateGameplayUI += SetGameplayUITrigger;
    }




    void SetGameplayTrigger()
    {
        if (stateGameplay != null)
        {
            SMController.SetTrigger("Gameplay");
        }
        else
        {
            //Debug.Log("out");
        }
    }

    void SetPauseTrigger()
    {
        if (statePause != null)
        {
            SMController.SetTrigger("Pause");
        }
        else
        {
            //Debug.Log("out");
        }
    }


    public void SetOptionTrigger()
    {
        UIController.SetTrigger("Option");
    }
    public void SetLevelSelectionTrigger()
    {
        UIController.SetTrigger("LevelSelection");
    }
    public void SetMainMenuTrigger()
    {
        UIController.SetTrigger("MainMenu");
    }

    void SetCoverTrigger()
    {
        if (stateCover != null)
        {
            SMController.SetTrigger("Cover");
        }
        else
        {
            //Debug.Log("out");
        }
    }

    void SetPlayMenuTrigger()
    {
        if (statePlayMenu != null)
        {
            //Debug.Log("PlayMenu");
            UIController.SetTrigger("Play");
        }
        else
        {
            //Debug.Log("out");
        }
    }


    void SetMenuTrigger()
    {
        if (stateMenu != null)
        {
            SMController.SetTrigger("Menu");
        }
        else
        {
            //Debug.Log("out");
        }
    }

    void SetPopupTrigger()
    {
        if (statePopup != null)
        {
            UIController.SetTrigger("Popup");
        }
    }

    void SetGameplayUITrigger()
    {
        if (stateGameplayUI != null)
        {
            UIController.SetTrigger("Gameplay");
        }
    }


    private void OnDisable()
    {
        statePlayMenu -= SetPlayMenuTrigger;
        stateCover -= SetCoverTrigger;
        stateGameplay -= SetGameplayTrigger;
        statePause -= SetPauseTrigger;
        stateMenu -= SetMenuTrigger;
        stateOption -= SetOptionTrigger;
        stateLevelSelection -= SetLevelSelectionTrigger;
        stateMainMenu -= SetMainMenuTrigger;
        statePopup -= SetPopupTrigger;
        stateGameplayUI -= SetGameplayUITrigger;
    }

    #endregion

    public void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    void Setup()
    {


        playerPopupID = 1;


        m_currentTurn = Turn.Player;
        GetBoardManager();
        GetPlayerManager();
        GetLevelManager();
        PositionPlayerSetup();
        TriggerInit();


        SoundManager.Initialize();
        //GetUIManager();

        m_player.Setup();
        m_board.Setup(m_player);
        m_player.playerMover.Setup();

        EnemyManager[] enemies = GameObject.FindObjectsOfType<EnemyManager>() as EnemyManager[];
        m_enemies = enemies.ToList();

        EnemyMover[] enemiesMovers = GameObject.FindObjectsOfType<EnemyMover>() as EnemyMover[];
        m_enemiesMovers = enemiesMovers.ToList();

        MovableObject[] movableObjects = GameObject.FindObjectsOfType<MovableObject>() as MovableObject[];
        m_movableObjects = movableObjects.ToList();

        Armor[] armors = GameObject.FindObjectsOfType<Armor>() as Armor[];
        m_armors = armors.ToList();

        Sword[] swords = GameObject.FindObjectsOfType<Sword>() as Sword[];
        m_sword = swords.ToList();


        foreach (EnemyManager enemy in m_enemies)
        {
            enemy.m_enemyMover.UpdateCurrentNode();
        }

        foreach (MovableObject movable in m_movableObjects)
        {
            movable.UpdateCurrentNode();
        }
    }

    void TriggerInit()
    {
        foreach (Node triggerNode in m_board.TriggerNodes)
        {
            if (triggerNode != null)
            {
                if (triggerNode.triggerState)
                {
                    triggerNode.triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    triggerNode.triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

    }

    void GetBoardManager()
    {
        if (m_board == null)
        {
            m_board = FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        }
    }

    void GetPlayerManager()
    {
        if (m_player == null)
        {
            m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        }
    }

    void GetLevelManager()
    {
        if (m_levelManager == null)
        {
            m_levelManager = FindObjectOfType<LevelManager>().GetComponent<LevelManager>();
        }
    }

    void GetUIManager()
    {
        //if (m_UI == null)
        //{
        //    m_UI = GetComponent<UIManager>();
        //}
    }

    private void Awake()
    {
        Init();

        #region StateMachine and Initialization
        if (SceneManager.GetActiveScene().name == "Menu")
        {

            StateBehaviourBase.Context context = new StateBehaviourBase.Context()
            {
                SetupDone = false,
                id = 1,
            };
            foreach (StateBehaviourBase state in SMController.GetBehaviours<StateBehaviourBase>())
            {
                state.Setup(context);
            }
        }
        //else {
        //    Setup();

        //    EnemyManager[] enemies = GameObject.FindObjectsOfType<EnemyManager>() as EnemyManager[];
        //    m_enemies = enemies.ToList();

        //    EnemyMover[] enemiesMovers = GameObject.FindObjectsOfType<EnemyMover>() as EnemyMover[];
        //    m_enemiesMovers = enemiesMovers.ToList();

        //    MovableObject[] movableObjects = GameObject.FindObjectsOfType<MovableObject>() as MovableObject[];
        //    m_movableObjects = movableObjects.ToList();

        //    Armor[] armors = GameObject.FindObjectsOfType<Armor>() as Armor[];
        //    m_armors = armors.ToList();

        //    Sword[] swords = GameObject.FindObjectsOfType<Sword>() as Sword[];
        //    m_sword = swords.ToList();

        //    //directionToMove = new Vector3(0f, 0f, Board.spacing);
        //}


        #endregion

        _isGameplay = false;
    }

    public void StartGameLoop()
    {
        Setup();
        if (m_player != null && m_board != null)
        {
            InitSword();
            InitBoss();
            if (!RunGameLoopStarted)
            {
                StartCoroutine("RunGameLoop");
                RunGameLoopStarted = true;
            }
            
            

        }
        else
        {
            //Debug.LogWarning("GameManager ERROR: no player or board found");
        }
    }

    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        //yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2) {
            Cutscene.SetActive(true);
        }

        transform.GetChild(2).gameObject.SetActive(false);//Music Manager
        

        Debug.Log("SETUP LEVEL");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }


        Debug.Log("START LEVEL");

        m_player.playerInput.InputEnabled = false;
        while (!m_hasLevelStarted)
        {
            yield return null;
        }


        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
        
    }

    IEnumerator PlayLevelRoutine()
    {

        m_player.PauseCanvas.gameObject.SetActive(true);

        if (SceneManager.GetActiveScene().buildIndex == 1) {
            Cutscene.SetActive(true);
            StartCoroutine(PlayFirstCutscene());
        }

        if (SceneManager.GetActiveScene().buildIndex == 2) {
            Cutscene.SetActive(true);
            StartCoroutine(PlaySecondCutscene());
        }


        triggerSetup();

        //crackableSetup(); --> FUNZIONA MA DA NULL REFERENCE E NON CREA I NODI

        m_player.PlayerAnimatorController.SetInteger("PlayerState", 0);
        Debug.Log("PLAY LEVEL");

        m_player.PlayerAnimatorController.SetInteger("PlayerState", 0);


        //if che sceglie se pad o keyboard

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!m_player.usingController)
            {
                popup = Instantiate(TutorialsKeyboard[0]);
            }
            else
            {
                popup = Instantiate(TutorialsController[0]);
            }
            popup.transform.GetChild(0).GetComponent<ScreenFader>().FadeOn();
        }

        //foreach (Node node in m_board.playerNode.NeighborNodes) {
        //    //Debug.Log("ESKEREEE");
        //    //Debug.Log(node.ToString());
        //}

        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        if (SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 2) {
            m_player.playerInput.InputEnabled = true;
        }
        m_player.playerInput.PauseInputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver)
        {
            yield return null;
            //todo: Check win(end reached)/lose(player dead)
            m_isGameOver = IsWinner();
            RunGameLoopStarted = false;

        }

        m_isGameOver = false;
        //m_player.lr.transform.gameObject.SetActive(true);
        m_levelManager.SaveLevel();
        NextLevel();


        //Debug.Log("U got what I call ... swag!");
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    IEnumerator LoseLevelRoutine()
    {

        if (m_player.PauseCanvas.gameObject.activeSelf)
        {
            m_player.PauseCanvas.gameObject.SetActive(false);
            GameManager.stateGameplay();
        }


        m_isGameOver = true;
        m_player.playerInput.InputEnabled = false;
        m_player.playerInput.PauseInputEnabled = false;


        if (loseLevelEvent != null)
        {
            loseLevelEvent.Invoke();
        }

        yield return new WaitForSeconds(2f);

        //Debug.Log("Your swag has been turned off , m8");
        m_player.hasFlashLight = false;
        m_player.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        RestartLevel();

    }

    //IEnumerator EndLevelRoutine()
    //{

    //    //Debug.Log("END LEVEL");

    //    //m_player.playerInput.InputEnabled = false;

    //    if (endLevelEvent != null)
    //    {
    //        endLevelEvent.Invoke();
    //    }

    //    //todo: changing scene?

    //    while (!m_hasLevelFinished)
    //    {

    //        //HasLevelFinished = true;
    //        yield return null;
    //    }

    //    RestartLevel();

    //}

        IEnumerator StartPlay() {
            yield return new WaitForSeconds(.3f);
            Debug.Log("PLAY LEVEL");
            PlayLevel();
        }

        IEnumerator PlayFirstCutscene() {
        m_player.playerInput.InputEnabled = false;
        firstCutscene.Prepare();
            while (!firstCutscene.isPrepared) {
                yield return new WaitForSeconds(1);
                break;
            }
            rawImage.texture = firstCutscene.texture;
            firstCutscene.Play();
            yield return new WaitForSeconds(51);
            firstCutscene.Stop();
            Cutscene.SetActive(false);
            m_player.playerInput.InputEnabled = true;
    }

    IEnumerator PlaySecondCutscene() {
        m_player.playerInput.InputEnabled = false;
        secondCutscene.Prepare();
        while (!secondCutscene.isPrepared) {
            yield return new WaitForSeconds(1);
            break;
        }
        rawImage.texture = secondCutscene.texture;
        secondCutscene.Play();
        yield return new WaitForSeconds(16);
        secondCutscene.Stop();
        Cutscene.SetActive(false);
        m_player.playerInput.InputEnabled = true;
    }

    void crackableSetup()
    {
        foreach (Node crackableNode in m_board.CrackableNodes)
        {
            if (crackableNode != null)
            {
                crackableNode.transform.GetChild(2).GetComponent<Crackable>().crackableAnimator.SetInteger("CrackableState", crackableNode.crackableState);
            }
        }
    }

    void triggerSetup()
    {

        foreach (Node triggerNode in m_board.TriggerNodes)
        {
            //Debug.Log("a");
            if (triggerNode != null)
            {
                triggerNode.triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(triggerNode.triggerState);
                //triggerNode.GetTriggerId(triggerNode);
                triggerNode.TriggerOrLogic();

            }
        }
    }

    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

    }

    public void PlayLevel()
    {
        //Cursor.visible = false;
        this.m_hasLevelStarted = true;
    }

    bool IsWinner()
    {
        if (m_board.playerNode != null)
        {
            return (m_board.playerNode == m_board.GoalNode);
        }
        return false;
    }

    void PlayPlayerTurn()
    {


        m_currentTurn = Turn.Player;

        if (SceneManager.GetActiveScene().buildIndex != 6)
        {
            StartCoroutine(CheckSpottedPosition());
        }



        m_player.IsTurnComplete = false;

    }

    void PlayEnemyTurn()
    {
        m_currentTurn = Turn.Enemy;
        m_player.hasWand = false;

        //foreach (MovableObject movable in m_board.AllMovableObjects) {
        //    movable.resetAnimation();
        //    movable.transform.GetChild(1).gameObject.SetActive(false);
        //}

        foreach (EnemyManager enemy in m_enemies)
        { //play each enemy's turn
            if (enemy != null && !enemy.isDead)
            {
                enemy.IsTurnComplete = false;

                EnemyOnOff();
                enemy.PlayTurn();


                //if (enemy.isScared == false) {

                //}
                //else {

                //    enemy.PushLeft();
                //    enemy.PushRight();
                //    enemy.PushUp();
                //    enemy.PushDown();
                //    enemy.PlayTurn();
                //    enemy.m_enemyMover.spottedDest = startPos + transform.TransformVector(directionToMove);
                //}
            }
        }
    }


    bool IsEnemyTurnComplete()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (enemy.isDead)
            {
                continue;
            }
            if (!enemy.IsTurnComplete)
            {
                return false;
            }
        }
        return true;
    }


    bool AreEnemiesAllDead()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (!enemy.isDead)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateTurn()
    {

        checkNodeForObstacles();

        // CheckSword();
        //LightBulbNode();
        //FearEnemies();
        FlashLightNode();



        foreach (var enemy in m_enemies)
        {
            
            if (enemy != null)
            {

                if (m_board.FindNodeAt(enemy.transform.position) != null && m_board.FindNodeAt(enemy.transform.position).isATrigger && m_board.FindNodeAt(enemy.transform.position).triggerState)
                {
                        m_board.FindNodeAt(enemy.transform.position).triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                }

                enemy.isDetected = false;

                foreach (Sword sword in m_sword)
                {
                    if (sword != null)
                    {
                        if (m_board.FindNodeAt(enemy.transform.position) == m_board.FindNodeAt(sword.transform.position) && sword.gameObject.activeInHierarchy)
                        {
                            enemy.Die();
                        }
                    }
                }

                if (enemy.m_enemyMover.movementType == MovementType.Chaser && enemy.m_enemyMover.index > 0)
                {

                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(enemy.m_enemyMover.GetPlayerPath(enemy.m_enemyMover.index).transform.position)).Count == 1)
                    {
                        //enemy.SetMovementType(MovementType.Stationary);
                        //Debug.Log(m_board.FindMovableObjectsAt(m_board.FindNodeAt(enemy.m_enemyMover.GetPlayerPath(enemy.m_enemyMover.index - 1).transform.position)).Count);
                        enemy.m_enemyMover.spottedPlayer = false;
                        enemy.m_enemyMover.firstChaserMove = true;
                    }
                    else
                    {
                        enemy.SetMovementType(enemy.GetFirstMovementType());
                    }

                }

                if (m_board.FindNodeAt(enemy.transform.TransformVector(new Vector3(0, 0, 2f)) + enemy.transform.position) != null)
                { //probably sto if non serve

                    //if ((m_board.FindNodeAt(enemy.transform.TransformVector(new Vector3(0, 0, 2f)) + enemy.transform.position).isAGate
                    //&& !m_board.FindNodeAt(enemy.transform.TransformVector(new Vector3(0, 0, 2f)) + enemy.transform.position).gateOpen)) {

                    if (enemy.m_enemyMover.movementType == MovementType.Chaser && enemy.m_enemyMover.index > 0)
                    {

                        if (m_board.FindNodeAt(enemy.m_enemyMover.GetPlayerPath(enemy.m_enemyMover.index).transform.position).isAGate
                            && !m_board.FindNodeAt(enemy.m_enemyMover.GetPlayerPath(enemy.m_enemyMover.index).transform.position).gateOpen)
                        {
                            //enemy.SetMovementType(MovementType.Stationary);
                            enemy.m_enemyMover.spottedPlayer = false;
                            enemy.m_enemyMover.firstChaserMove = false;
                        }
                        else
                        {
                            enemy.SetMovementType(enemy.GetFirstMovementType());
                        }

                    }
                }




            }
        }


        if (m_currentTurn == Turn.Player && m_player != null)
        {


            triggerNodePlayerTurn();
            triggerNode();
            triggerNodeWithMovable();

            if (m_player.IsTurnComplete && !AreEnemiesAllDead())
            {
                PlayerInGate();
                foreach (EnemyManager enemy in m_enemies)
                {
                    if (enemy != null)
                    {
                        enemy.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 0);
                    }

                }
                //m_movableObjects = GetMovableObjects();
                PlayEnemyTurn();
            }
            else if (AreEnemiesAllDead())
            {
                m_board.m_gm.crackNode();
                PlayerInGate();
            }


        }


        else if (m_currentTurn == Turn.Enemy)
        {

            foreach (MovableObject movableObject in m_board.AllMovableObjects)
            {
                movableObject.resetAnimation();
                movableObject.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (IsEnemyTurnComplete())
            {
                crackNode();
                m_player.playerInput.InputEnabled = true;
                PlayPlayerTurn();
                NotMovingMovable();
            }
        }
    }


    public void crackNode()
    {



        //______ENEMY ON CRACKNODE___________________

        List<EnemyManager> enemies;
        List<MovableObject> movableObjects;
        List<Sword> swords;

        foreach (var node in m_board.CrackableNodes)
        {
            enemies = m_board.FindEnemiesAt(node);
            movableObjects = m_board.FindMovableObjectsAt(node);
            swords = m_board.FindSwordsAt(node);
            foreach (EnemyManager enemy in enemies)
            {
                node.UpdateCrackableState();
                node.UpdateCrackableTexture();
                //node.GetComponentInChildren<CrackableTexture>().UpdateCrackableTexture();
                if (node.GetCrackableState() == 2)
                {
                    enemy.DieOnCrackable();
                    node.crackableState = 2;
                }
            }

            //______M.O. ON CRACKNODE___________________
            foreach (MovableObject movableObject in movableObjects)
            {
                if (movableObject.hasMoved)
                {

                    node.UpdateCrackableState();
                    //Debug.Log(movableObjects.Count);
                    node.UpdateCrackableTexture();

                }


                if (node.GetCrackableState() == 2)
                {
                    m_board.AllMovableObjects.Remove(movableObject);
                    m_movableObjects.Remove(movableObject);

                    //Destroy(movableObject);
                    movableObject.inScene = false;

                    movableObject.fallingAnimation();
                    //movableObject.transform.GetChild(0).gameObject.SetActive(false);



                    //movableObject.GetComponent<Collider>().gameObject.SetActive(false); HERE

                    //node.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                    node.FromCrackableToNormal();
                    node.isCrackable = false;
                }
            }
            //______M.O. ON CRACKNODE___________________

        }

        //______ENEMY ON CRACKNODE___________________


        if (m_board.playerNode.isCrackable)
        {
            m_board.playerNode.UpdateCrackableState();
            m_board.playerNode.UpdateCrackableTexture();
        }

        if (m_board.playerNode.GetCrackableState() == 2)
        {

            StartCoroutine(PlayerDeathByCrackable());
        }
    }

    IEnumerator PlayerDeathByCrackable()
    {
        m_player.playerInput.InputEnabled = false;
        m_player.playerInput.PauseInputEnabled = false;
        m_player.PlayerAnimatorController.SetInteger("PlayerState", 8);
        yield return new WaitForSeconds(.2f);
        LoseLevel();
    }


    public void triggerNodePlayerTurn()
    {

        Node previousTempNode = m_board.GetPreviousPlayerNode(); //serve per vedere se il previous è un trigger altrimenti mette a false ad ogni turno

        if (m_board.playerNode.isATrigger)
        {
            m_board.SetPreviousPlayerNode(m_board.playerNode);

            if (!m_board.playerNode.triggerWithEnemy)
            {
                m_board.playerNode.UpdateTriggerToTrue();
            }

        }
        else if (m_board.GetPreviousPlayerNode() != null && previousTempNode.isATrigger)
        {
            m_board.UpdateTriggerToFalse(m_board.GetPreviousPlayerNode());
            //Debug.Log("TRIGGER A FALSE");
            m_board.SetPreviousPlayerNode(null);
        }


    }

    public void triggerNode()
    {

        List<EnemyManager> enemies;

        List<Armor> armors;

        foreach (var node in m_board.TriggerNodes)
        {
            enemies = m_board.FindEnemiesAt(node);
            foreach (EnemyManager enemy in enemies)
            {

                if (node.mover != enemy.m_enemyMover)
                {

                    node.mover = enemy.m_enemyMover;


                    //if (enemy.GetEnemySensor.FindEnemyNode().isATrigger) {
                    enemy.GetEnemySensor.SetPreviousEnemyNode(enemy.GetEnemySensor.FindEnemyNode());
                    enemy.GetEnemySensor.FindEnemyNode().UpdateTriggerToTrue();

                    ////Debug.Log(enemy.GetEnemySensor.GetPreviousEnemyNode());
                    //}
                    /*else*/
                    if (enemy.GetEnemySensor.GetPreviousEnemyNode() != null)
                    {
                        enemy.GetEnemySensor.GetPreviousEnemyNode().triggerState = false;
                    }
                }

                if (enemies.Count == 0)
                {
                    node.mover = null;
                }

                armors = m_board.FindArmorsAt(node);
                //foreach (Armor armor in armors)
                //{
                //    if (armor.FindSwordNode().isATrigger && armor.isActive)
                //    {
                //        //Debug.Log(m_board.FindNodeAt(transform.position + (transform.forward * BoardManager.spacing)));
                //        armor.FindSwordNode().UpdateTriggerToTrue();
                //    }
                //    else if (armor.FindSwordNode().isATrigger && !armor.isActive)
                //    {
                //        armor.FindSwordNode().triggerState = false;
                //    }
                //}

            }

        }

    }

    public void triggerNodeWithMovable()
    {

        List<MovableObject> movableObjects;
        foreach (Node node in m_board.AllNodes)
        {
            movableObjects = m_board.FindMovableObjectsAt(node);
            foreach (MovableObject movableObject in movableObjects)
            {

                if (node.mover != movableObject)
                {


                    if (!movableObject.FindMovableObjectNode().isATrigger && movableObject.GetPreviousMovableObjectNode() != null)
                    {
                        if (movableObject.GetPreviousMovableObjectNode().triggerTemp != null) {
                            movableObject.GetPreviousMovableObjectNode().triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(false);
                            movableObject.GetPreviousMovableObjectNode().triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                            movableObject.GetPreviousMovableObjectNode().UpdateTriggerToTrue();
                            movableObject.GetPreviousMovableObjectNode().mover = null;
                            movableObject.SetPreviousMovableObjectNode(movableObject.FindMovableObjectNode());
                        }
                        
                    }

                    else if (movableObject.FindMovableObjectNode().isATrigger)
                    {

                        node.mover = movableObject;
                        movableObject.SetPreviousMovableObjectNode(movableObject.FindMovableObjectNode());
                        movableObject.FindMovableObjectNode().UpdateTriggerToTrue();
                        movableObject.GetPreviousMovableObjectNode().triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(true);
                        movableObject.FindMovableObjectNode().triggerTemp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                    }
                }



            }
        }
    }

    public List<MovableObject> GetMovableObjects()
    {
        foreach (MovableObject movObj in m_board.AllMovableObjects)
        {
            foreach (Node node in m_board.playerNode.LinkedNodes)
            {
                if (movObj.transform.position == node.transform.position)
                {
                    m_movableObjects.Add(movObj);

                }
            }
        }
        return m_movableObjects;
    }

    
    //public void LightBulbNode()
    //{
    //    if (m_board.playerNode.hasLightBulb)
    //    {
    //        m_board.playerNode.hasLightBulb = false;
    //        m_board.playerNode.transform.GetChild(2).gameObject.SetActive(false);
    //        m_player.transform.GetChild(2).gameObject.SetActive(true);
    //        m_player.hasLightBulb = true;
    //        //m_player.spottedPlayer = false;
    //    }
    //}

    public void FlashLightNode()
    {
        if (m_board.playerNode.hasFlashLight)
        {
            m_board.playerNode.hasFlashLight = false;

            if (m_board.playerNode.isCrackable)
            {
                m_board.playerNode.transform.GetChild(3).gameObject.SetActive(false);
            }
            else if (!m_board.playerNode.isCrackable)
            {
                m_board.playerNode.transform.GetChild(2).gameObject.SetActive(false);
            }

            m_player.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            m_player.hasFlashLight = true;
        }
    }

    //Old FearEnemies
    #region
    //public void FearEnemies() {

    //    if (m_player.hasLightBulb) {
    //        foreach (var enemy in m_enemies) {

    //            startPos = new Vector3(m_board.FindNodeAt(enemy.transform.position).Coordinate.x, 0f, m_board.FindNodeAt(enemy.transform.position).Coordinate.y);

    //            if (enemy.GetMovementType() == MovementType.Chaser) {
    //                //Debug.Log(EnemyMover.index);
    //                frontalDest = m_player.GetPlayerPath(EnemyMover.index).transform.position;
    //            }
    //            else {
    //                frontalDest = startPos + enemy.transform.TransformVector(directionToMove);
    //            }


    //            if (enemy != null) {
    //                if (frontalDest == m_board.playerNode.transform.position) {
    //                    enemy.isScared = true;
    //                    enemy.wasScared = true;
    //                }
    //                else if (frontalDest != m_board.playerNode.transform.position) {
    //                    enemy.isScared = false;
    //                }
    //            }

    //        }
    //    }
    //}

    #endregion   

    public void EnemyOnOff()
    {
        foreach (var enemy in m_enemies)
        {
            if (enemy != null)
            {
                if (m_board.FindNodeAt(enemy.transform.position).isAGate && !m_board.FindNodeAt(enemy.transform.position).gateOpen)
                {
                    enemy.isOff = true;
                }
                else if (m_board.FindNodeAt(enemy.transform.position).isAGate && m_board.FindNodeAt(enemy.transform.position).gateOpen)
                {
                    enemy.isOff = false;

                }
            }
        }

    }

    public void SaveMO()
    {
        foreach (var crackcableNode in m_board.FindCrackableNodes())
        {
            foreach (var movableOnCrack in m_board.FindMovableObjectsAt(crackcableNode))   // prende il movableObject sopra il crackNode e lo salva 
            {
                crackcableNode.MO = movableOnCrack;
            }

        }
    }

    public void NotMovingMovable()
    {
        foreach (var movableObject in m_movableObjects)
        {
            movableObject.hasMoved = false;
            movableObject.hasStopped = true;
        }
    }

    public void InitBoss()
    {
        Boss.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void InitSword()
    {

        foreach (var armor in m_armors)
        {
            if (!armor.isActive)
            {
                armor.transform.GetChild(3).gameObject.SetActive(false);
                armor.AnimatorController.SetBool("ArmorState", false);
            }
            else
            {
                armor.transform.GetChild(3).gameObject.SetActive(true);
                armor.AnimatorController.SetBool("ArmorState", true);
                //Debug.Log(m_board.FindNodeAt(armor.transform.GetChild(3).gameObject.transform.position));
            }
        }
    }

    IEnumerator CheckSpottedPosition()
    {

        yield return new WaitForSeconds(0.05f);

        foreach (var enemy in m_enemies)
        {

            if (m_board.playerNode == m_board.FindNodeAt(enemy.m_enemyMover.spottedDest) && enemy.wasScared && m_board.FindNodeAt(enemy.m_enemyMover.firstDest).LinkedNodes.Contains(m_board.FindNodeAt(enemy.m_enemyMover.spottedDest)))
            {

                //Debug.Log("Spotted!");

                m_board.ChasingPreviousPlayerNode = m_board.playerNode;
                //m_player.spottedPlayer = true;
                m_player.UpdatePlayerPath();

            }
        }
    }


    public void checkNodeForObstacles()
    {
        foreach (MovableObject movableObject in m_movableObjects)
        {
            if (movableObject.isFloating) {
                movableObject.checkNodeForObstacle();
            }
            
        }
    }


    public void UpdateTriggerToFalseForMO(Node n)
    {

        foreach (var movableObject in m_board.FindMovableObjectsAt(n))
        {
            n.triggerState = false;
            n.UpdateGateToClose(movableObject.FindMovableObjectNode().GetGateID()); // era con movableObject.PreviousMovableObjectNode.GetGateID()
            //Debug.Log("CLOSE");
            n.ArmorDeactivation(movableObject.FindMovableObjectNode().GetArmorID());//  era con movableObject.PreviousMovableObjectNode.GetArmorID()
        }

    }

    public void PlayerInGate()
    {
        if (m_board.playerNode.isAGate && !m_board.playerNode.gateOpen)
        {
            m_player.playerInput.InputEnabled = false;
            m_player.playerInput.PauseInputEnabled = false;
            m_player.PlayerAnimatorController.SetInteger("PlayerState", 8);
            StartCoroutine(ShadowDeathDelay(shadowKillDelay));
        }
    }

    public IEnumerator ShadowDeathDelay(float time) {
        yield return new WaitForSeconds(time);
        LoseLevel();
    }


    public void NextLevel()
    {
        //Debug.Log("NEXTLEVEL");
        m_index = SceneManager.GetActiveScene().buildIndex % 10;
        m_index++;
        SceneManager.LoadScene(m_index);
    }

    public void PreviousLevel()
    {
        //Debug.Log("PREVIOUSLEVEL");
        m_index = SceneManager.GetActiveScene().buildIndex;
        m_index--;

        if (m_index < 1)
        {
            m_index = 10;
        }

        SceneManager.LoadScene(m_index);
    }


    public void PositionPlayerSetup()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                m_player.transform.position = new Vector3(4, 0, 0);

                break;
            case 2:
                m_player.transform.position = new Vector3(6, 0, -2);

                break;
            case 3:
                m_player.transform.position = new Vector3(6, 0, -2);

                break;
            case 4:
                //m_player.transform.position = new Vector3(6, 0, -2);
                m_player.transform.position = new Vector3(28, 0, -3);

                break;
            case 5:
                m_player.transform.position = new Vector3(4, 0, -2);

                break;
            case 6:
                m_player.transform.position = new Vector3(22, 0, -8);

                break;
            case 7:
                m_player.transform.position = new Vector3(6, 0, -4);

                break;
            case 8:
                m_player.transform.position = new Vector3(0, 0, 16);

                break;
            case 9:
                m_player.transform.position = new Vector3(14, 0, -18);

                break;
            case 10:
                m_player.transform.position = new Vector3(12, 0, -6);

                break;
        }

    }

    #region LevelSelection

    public void startLevel()
    {
        GameManager.stateGameplay();
        SceneManager.LoadScene(EventSystem.current.currentSelectedGameObject.name);
    }


    #endregion

}
