using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn {
    Player,
    Enemy
}

public class GameManager : MonoBehaviour {

    Board m_board;
    PlayerManager m_player;

    EnemyMover m_enemyMover;

    public Vector3 directionToMove;
    Vector3 startPos;

    Vector3 frontalDest;

    List<EnemyManager> m_enemies;
    List<EnemyMover> m_enemiesMovers;
    List<MovableObject> m_movableObjects;
    List<Armor> m_armors;
    List<Sword> m_sword;

    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return m_currentTurn; } }

    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }

    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }

    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

    public float delay  = 0;


    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();


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

        //directionToMove = new Vector3(0f, 0f, Board.spacing);

    }

    void Start() {

        if (m_player != null && m_board != null) {
            InitSword();
            StartCoroutine("RunGameLoop");
        }
        else {
            Debug.LogWarning("GameManager ERROR: no player or board found");
        }
    }

    IEnumerator RunGameLoop() {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine() {

        Debug.Log("SETUP LEVEL");
        if (setupEvent != null) {
            setupEvent.Invoke();
        }

        Debug.Log("START LEVEL");

        m_player.playerInput.InputEnabled = false;//
        while (!m_hasLevelStarted) {


            yield return null;
        }

        if (startLevelEvent != null) {
            startLevelEvent.Invoke();
        }

    }

    IEnumerator PlayLevelRoutine() {

        Debug.Log("PLAY LEVEL");

        //foreach (Node node in m_board.playerNode.NeighborNodes) {
        //    Debug.Log("ESKEREEE");
        //    Debug.Log(node.ToString());
        //}

        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        if (playLevelEvent != null) {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver) {

            yield return null;
            //todo: Check win(end reached)/lose(player dead)

            m_isGameOver = IsWinner();


        }

        Debug.Log("U got what I call ... swag!");
    }

    public void LoseLevel() {
        StartCoroutine(LoseLevelRoutine());
    }

    IEnumerator LoseLevelRoutine() {
        m_isGameOver = true;

        if (loseLevelEvent != null) {
            loseLevelEvent.Invoke();
        }

        yield return new WaitForSeconds(2f);

        Debug.Log("Your swag has been turned off , m8");

        RestartLevel();

    }

    IEnumerator EndLevelRoutine() {

        Debug.Log("END LEVEL");

        m_player.playerInput.InputEnabled = false;//

        if (endLevelEvent != null) {
            endLevelEvent.Invoke();
        }

        //todo: changing scene?

        while (!m_hasLevelFinished) {

            //todo: user confirm button --> HasLevelFinished = true
            yield return null;
        }

        RestartLevel();

    }

    void RestartLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel() {
        m_hasLevelStarted = true;
    }

    bool IsWinner() {
        if (m_board.playerNode != null) {
            return (m_board.playerNode == m_board.GoalNode);
        }
        return false;
    }

    void PlayPlayerTurn() {


        m_currentTurn = Turn.Player;

        StartCoroutine(CheckSpottedPosition());


        m_player.IsTurnComplete = false;

    }

    void PlayEnemyTurn() {
        m_currentTurn = Turn.Enemy;

        foreach (EnemyManager enemy in m_enemies) { //play each enemy's turn
            if (enemy != null && !enemy.isDead) {
                enemy.IsTurnComplete = false;

                if (enemy.isScared == false) {
                    EnemyOnOff();
                    enemy.PlayTurn();
                }
                else {

                    enemy.PushLeft();
                    enemy.PushRight();
                    enemy.PushUp();
                    enemy.PushDown();
                    enemy.PlayTurn();
                    enemy.m_enemyMover.spottedDest = startPos + transform.TransformVector(directionToMove);
                }
            }
        }
    }


    bool IsEnemyTurnComplete() {
        foreach (EnemyManager enemy in m_enemies) {
            if (enemy.isDead) {
                continue;
            }
            if (!enemy.IsTurnComplete) {
                return false;
            }
        }
        return true;
    }


    bool AreEnemiesAllDead() {
        foreach (EnemyManager enemy in m_enemies) {
            if (!enemy.isDead) {
                return false;
            }
        }
        return true;
    }

    public void UpdateTurn() {

        // CheckSword();
        checkNodeForObstacles();
        LightBulbNode();
        FearEnemies();
        FlashLightNode();


        foreach (var enemy in m_enemies) {
            if (enemy != null) {

                foreach (Sword sword in m_sword) {
                    if (sword != null) {
                        if (m_board.FindNodeAt(enemy.transform.position) == m_board.FindNodeAt(sword.transform.position) && sword.gameObject.activeInHierarchy) {
                            enemy.Die();
                        }
                    }
                }

                if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(enemy.transform.TransformVector(new Vector3(0, 0, 2f)) + enemy.transform.position)).Count != 0) {
                    enemy.SetMovementType(MovementType.Stationary);
                }
                else {
                    enemy.SetMovementType(enemy.GetFirstMovementType());
                }

            }
        }

        if (m_currentTurn == Turn.Player && m_player != null) {

            foreach (var trap in m_board.AllTraps) {
                trap.canShoot = true;
            }

            triggerNodePlayerTurn();
            if (m_player.IsTurnComplete && !AreEnemiesAllDead()) {
                PlayEnemyTurn();
                m_movableObjects = GetMovableObjects();
            }
            triggerNode();
        }


        else if (m_currentTurn == Turn.Enemy) {


            if (IsEnemyTurnComplete()) {
                crackNode();
                PlayPlayerTurn();

            }
            NotMovingMovable();
        }

    }


    public void crackNode() {

        //______ENEMY ON CRACKNODE___________________

        List<EnemyManager> enemies;
        List<MovableObject> movableObjects;
        List<Sword> swords;

        foreach (var node in m_board.CrackableNodes) {
            enemies = m_board.FindEnemiesAt(node);
            movableObjects = m_board.FindMovableObjectsAt(node);
            swords = m_board.FindSwordsAt(node);
            foreach (EnemyManager enemy in enemies) {
                node.UpdateCrackableState();
                node.UpdateCrackableTexture();
                //node.GetComponentInChildren<CrackableTexture>().UpdateCrackableTexture();
                if (node.GetCrackableState() == 0) {
                    enemy.Die();
                }
            }

            //______M.O. ON CRACKNODE___________________
            foreach (MovableObject movableObject in movableObjects) {
                if (movableObject.hasMoved) {

                    node.UpdateCrackableState();
                    node.UpdateCrackableTexture();
                }


                if (node.GetCrackableState() == 0) {
                    m_board.AllMovableObjects.Remove(movableObject);
                    m_movableObjects.Remove(movableObject);

                    //Destroy(movableObject);
                    movableObject.inScene = false;
                    movableObject.transform.GetChild(0).gameObject.SetActive(false);
                    movableObject.GetComponent<Collider>().gameObject.SetActive(false);


                    node.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                    node.FromCrackableToNormal();
                    node.isCrackable = false;
                }
            }
            //______M.O. ON CRACKNODE___________________

            //______Swords ON CRACKNODE

            foreach (Sword sword in swords) {
                node.DestroyCrackableInOneHit();
                Debug.Log("NODE DESTROYED");
                node.UpdateCrackableTexture();
            }


            //______Swords ON CRACKNODE___________________


        }

        //______ENEMY ON CRACKNODE___________________


        if (m_board.playerNode.isCrackable) {
            m_board.playerNode.UpdateCrackableState();
            m_board.playerNode.UpdateCrackableTexture();
        }

        if (m_board.playerNode.GetCrackableState() == 0) {
            loseLevelEvent.Invoke();
        }
    }


    public void triggerNodePlayerTurn() {
        Node previousTempNode = m_board.GetPreviousPlayerNode(); //serve per vedere se il previous è un trigger altrimenti mette a false ad ogni turno

        if (m_board.playerNode.isATrigger) {
            m_board.SetPreviousPlayerNode(m_board.playerNode);
            m_board.playerNode.UpdateTriggerToTrue();
        }
        else if (m_board.GetPreviousPlayerNode() != null && previousTempNode.isATrigger) {
            m_board.UpdateTriggerToFalse();
            Debug.Log("TRIGGER A FALSE");
            m_board.SetPreviousPlayerNode(null);
        }
        

    }

    public void triggerNode() {

        List<EnemyManager> enemies;
        List<MovableObject> movableObjects;
        List<Armor> armors;

        foreach (var node in m_board.TriggerNodes) {
            enemies = m_board.FindEnemiesAt(node);
            foreach (EnemyManager enemy in enemies) {

                if (node.mover != enemy.m_enemyMover) {


                    node.mover = enemy.m_enemyMover;


                    //if (enemy.GetEnemySensor.FindEnemyNode().isATrigger) {
                    enemy.GetEnemySensor.SetPreviousEnemyNode(enemy.GetEnemySensor.FindEnemyNode());
                    enemy.GetEnemySensor.FindEnemyNode().UpdateTriggerToTrue();

                    //Debug.Log(enemy.GetEnemySensor.GetPreviousEnemyNode());
                    //}
                    /*else*/
                    if (enemy.GetEnemySensor.GetPreviousEnemyNode() != null) {
                        enemy.GetEnemySensor.GetPreviousEnemyNode().triggerState = false;
                    }
                }
            }


            movableObjects = m_board.FindMovableObjectsAt(node);
            foreach (MovableObject movableObject in movableObjects) {

                if (node.mover != movableObject) {

                    node.mover = movableObject;
                    if (movableObject.FindMovableObjectNode().isATrigger) {
                        movableObject.SetPreviousMovableObjectNode(movableObject.FindMovableObjectNode());
                        movableObject.FindMovableObjectNode().UpdateTriggerToTrue();
                    }
                    else if (movableObject.GetPreviousMovableObjectNode() != null) {
                        movableObject.GetPreviousMovableObjectNode().triggerState = false;
                    }
                }
            }

            if (enemies.Count == 0 && movableObjects.Count == 0) {
                node.mover = null;
            }

            armors = m_board.FindArmorsAt(node);
            foreach (Armor armor in armors) {
                if (armor.FindSwordNode().isATrigger && armor.isActive) {
                    Debug.Log(m_board.FindNodeAt(transform.position + (transform.forward * Board.spacing)));
                    armor.FindSwordNode().UpdateTriggerToTrue(); //COSì NON FUNZIONA MADONNA PORCONA 
                }
                else if (armor.FindSwordNode().isATrigger && !armor.isActive) {
                    armor.FindSwordNode().triggerState = false;
                }
            }

        }
    }



    public List<MovableObject> GetMovableObjects() {
        foreach (var movObj in m_board.AllMovableObjects) {
            foreach (var node in m_board.playerNode.LinkedNodes) {
                if (movObj.transform.position == node.transform.position) {
                    m_movableObjects.Add(movObj);

                }
            }
        }
        return m_movableObjects;
    }


    public void LightBulbNode() {
        if (m_board.playerNode.hasLightBulb) {
            m_board.playerNode.hasLightBulb = false;
            m_board.playerNode.transform.GetChild(2).gameObject.SetActive(false);
            m_player.transform.GetChild(2).gameObject.SetActive(true);
            m_player.hasLightBulb = true;
            //m_player.spottedPlayer = false;
        }
    }

    public void FlashLightNode() {
        if (m_board.playerNode.hasFlashLight) {
            m_board.playerNode.hasFlashLight = false;
            m_board.playerNode.transform.GetChild(2).gameObject.SetActive(false);
            m_player.transform.GetChild(3).gameObject.SetActive(true);
            m_player.hasFlashLight = true;
        }
    }


    public void FearEnemies() {

        if (m_player.hasLightBulb) {
            foreach (var enemy in m_enemies) {

                startPos = new Vector3(m_board.FindNodeAt(enemy.transform.position).Coordinate.x, 0f, m_board.FindNodeAt(enemy.transform.position).Coordinate.y);

                if (enemy.GetMovementType() == MovementType.Chaser) {
                    Debug.Log(EnemyMover.index);
                    frontalDest = m_player.GetPlayerPath(EnemyMover.index).transform.position;
                }
                else {
                    frontalDest = startPos + enemy.transform.TransformVector(directionToMove);
                }


                if (enemy != null) {
                    if (frontalDest == m_board.playerNode.transform.position) {
                        enemy.isScared = true;
                        enemy.wasScared = true;
                    }
                    else if (frontalDest != m_board.playerNode.transform.position) {
                        enemy.isScared = false;
                    }
                }

            }
        }
    }
    public void EnemyOnOff() {
        foreach (var enemy in m_enemies) {
            if (enemy != null) {
                if (m_board.FindNodeAt(enemy.transform.position).isAGate && !m_board.FindNodeAt(enemy.transform.position).gateOpen) {
                    enemy.isOff = true;
                }
                else if (m_board.FindNodeAt(enemy.transform.position).isAGate && m_board.FindNodeAt(enemy.transform.position).gateOpen) {
                    enemy.isOff = false;


                }
            }
        }

    }
    public void SaveMO() {
        foreach (var crackcableNode in m_board.FindCrackableNodes()) {
            foreach (var movableOnCrack in m_board.FindMovableObjectsAt(crackcableNode))   // prende il movableObject sopra il crackNode e lo salva 
            {
                crackcableNode.MO = movableOnCrack;
            }

        }
    }
    public void NotMovingMovable() {
        foreach (var movableObject in m_movableObjects) {
            movableObject.hasMoved = false;
            movableObject.hasStopped = true;
        }
    }

    public void InitSword() {
        foreach (var armor in m_armors) {
            if (!armor.isActive) {
                armor.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    IEnumerator CheckSpottedPosition() {

        yield return new WaitForSeconds(0.05f);

        foreach (var enemy in m_enemies) {

            if (m_board.playerNode == m_board.FindNodeAt(enemy.m_enemyMover.spottedDest) && enemy.wasScared && m_board.FindNodeAt(enemy.m_enemyMover.firstDest).LinkedNodes.Contains(m_board.FindNodeAt(enemy.m_enemyMover.spottedDest))) {

                Debug.Log("Spotted!");

                m_board.ChasingPreviousPlayerNode = m_board.playerNode;
                //m_player.spottedPlayer = true;
                m_player.UpdatePlayerPath();

            }
        }
    }


    public void checkNodeForObstacles() {
        foreach (var movableObject in m_movableObjects) {
            movableObject.checkNodeForObstacle();
        }
    }
}
