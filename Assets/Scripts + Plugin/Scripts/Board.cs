using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour {

    public static float spacing = 2f;

    public static readonly Vector2[] directions = {
        new Vector2(spacing, 0f),
        new Vector2(-spacing , 0f),
        new Vector2(0f , spacing),
        new Vector2(-0f , -spacing)
    };

    //public enum consequence{nothing , lose , enemyDies , fallenObject};


    List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes { get { return m_allNodes; } set { GetNodeList(); } }

    List<Node> m_crackableNodes = new List<Node>();
    public List<Node> CrackableNodes { get { return m_crackableNodes; } }

    List<Node> m_triggerNodes = new List<Node>();
    public List<Node> TriggerNodes { get { return m_triggerNodes; } }

    List<MovableObject> m_AllmovableObjects = new List<MovableObject>();
    public List<MovableObject> AllMovableObjects { get { return m_AllmovableObjects; } }

    List<Mirror> m_AllMirrors = new List<Mirror>();
    public List<Mirror> AllMirrors { get { return m_AllMirrors; } }

    List<Trap> m_AllTraps = new List<Trap>();
    public List<Trap> AllTraps { get { return m_AllTraps; } }

    List<PushingWall> m_AllPushingWalls = new List<PushingWall>();
    public List<PushingWall> AllPushingWalls { get { return m_AllPushingWalls; } }

    List<Armor> m_AllArmors = new List<Armor>();
    public List<Armor> AllArmors { get { return m_AllArmors; } }

    List<Sword> m_AllSwords = new List<Sword>();
    public List<Sword> AllSwords { get { return m_AllSwords; } }

    List<Mover> m_AllMovers = new List<Mover>();
    public List<Mover> AllMovers { get { return m_AllMovers; } }



    Node m_playerNode;

    public Node playerNode { get { return m_playerNode; } }

    Node m_goalNode;
    public Node GoalNode { get { return m_goalNode; } }

    Node m_previousPlayerNode;
    public Node PreviousPlayerNode { get { return m_previousPlayerNode; } set { m_previousPlayerNode = playerNode; } }


    Node m_chasingPreviousPlayerNode;
    public Node ChasingPreviousPlayerNode { get { return m_chasingPreviousPlayerNode; } set { m_chasingPreviousPlayerNode = playerNode; } }

    Node m_chaserNewDest;
    public Node ChaserNewDest { get { return m_chaserNewDest; } set { m_chaserNewDest = ChasingPreviousPlayerNode; } }


    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    PlayerMover m_player;

    //---------------------------------------------------------------------
    //public List<Transform> capturePositions;
    //int m_currentCapturePosition = 0;

    //public int CurrentCapturePosition { get { return m_currentCapturePosition; } set { m_currentCapturePosition = value; } }

    //public float capturePositionIconSize = 0.4f;
    //public Color capturePositionIconColor = Color.blue;
    //---------------------------------------------------------------------

    void Awake() {
        m_player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();


        m_AllMirrors = FindMirrors();
        m_AllmovableObjects = FindMovableObjects();
        m_AllTraps = FindTraps();
        m_AllPushingWalls = FindPushingWalls();
        m_AllArmors = FindArmors();
        m_AllSwords = FindSwords();
        m_AllMovers = FindMovers();

        GetNodeList();
        m_crackableNodes = FindCrackableNodes();
        m_goalNode = FindGoalNode();
        m_triggerNodes = FindTriggerNodes();
    }

    public void GetNodeList() {
        Node[] nList = GameObject.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }

    public Node FindNodeAt(Vector3 pos) {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    Node FindGoalNode() {
        return m_allNodes.Find(n => n.isLevelGoal);
    }

    public Node FindPlayerNode() {
        if (m_player != null && !m_player.isMoving) {
            return FindNodeAt(m_player.transform.position);
        }
        return null;
    }

    public List<Node> FindCrackableNodes() {
        foreach (var node in m_allNodes) {
            if (node.isCrackable) {
                CrackableNodes.Add(node);
            }
        }
        return CrackableNodes;
    }

    public List<Node> FindTriggerNodes() {
        foreach (var node in m_allNodes) {
            if (node.isATrigger) {
                TriggerNodes.Add(node);
            }
        }
        return TriggerNodes;
    }
    public void CheckSword()
    {

        foreach (var sword in m_AllSwords)
        {
            if (sword.GetComponentInParent<Armor>().isActive)
                sword.CaptureEnemies();
        }
    }



    public List<EnemyManager> FindEnemiesAt(Node node) {
        List<EnemyManager> foundEnemies = new List<EnemyManager>();
        EnemyManager[] enemies = Object.FindObjectsOfType<EnemyManager>() as EnemyManager[];

        foreach (EnemyManager enemy in enemies) {
            EnemyMover mover = enemy.GetComponent<EnemyMover>();

            if (mover.CurrentNode == node) {
                foundEnemies.Add(enemy);
            }
        }
        return foundEnemies;
    }


    public List<MovableObject> FindMovableObjectsAt(Node node) {
        List<MovableObject> foundMovableObjects = new List<MovableObject>();
        MovableObject[] movableObjects = Object.FindObjectsOfType<MovableObject>() as MovableObject[];

        foreach (MovableObject movableObject in movableObjects) {
            if (movableObject.CurrentNode == node) {
                foundMovableObjects.Add(movableObject);
            }
        }
        return foundMovableObjects;
    }

    public List<Armor> FindArmorsAt(Node node) {
        List<Armor> foundArmors = new List<Armor>();
        Armor[] armors = Object.FindObjectsOfType<Armor>() as Armor[];

        foreach (Armor armor in armors) {
            if (armor.CurrentNode == node) {
                foundArmors.Add(armor);
            }
        }
        return foundArmors;
    }

    public List<Sword> FindSwordsAt(Node node) {
        List<Sword> foundSwords = new List<Sword>();
        Sword[] swords = Object.FindObjectsOfType<Sword>() as Sword[];

        foreach (Sword sword in swords) {
            if (sword.CurrentNode == node) {
                foundSwords.Add(sword);
            }
        }
        return foundSwords;
    }

    public void UpdatePlayerNode() {
        m_playerNode = FindPlayerNode();
    }


    public void SetPreviousPlayerNode(Node n) { //Cambiare il nome qui o su Enemy Mover
        PreviousPlayerNode = n;
    }

    public Node GetPreviousPlayerNode() {
        return PreviousPlayerNode;
    }

    public void UpdateTriggerToFalse() {    
        PreviousPlayerNode.triggerState = false;
        PreviousPlayerNode.UpdateGateToClose(PreviousPlayerNode.GetGateID());
        PreviousPlayerNode.ArmorDeactivation(PreviousPlayerNode.GetArmorID());
    }


    public List<MovableObject> FindMovableObjects() {
        List<MovableObject> foundMovableObjects = new List<MovableObject>();
        MovableObject[] movableObjects = Object.FindObjectsOfType<MovableObject>() as MovableObject[];
        foundMovableObjects = movableObjects.ToList();

        return foundMovableObjects;
    }

    public List<Mirror> FindMirrors() {
        List<Mirror> foundMirrors = new List<Mirror>();
        Mirror[] mirrors = Object.FindObjectsOfType<Mirror>() as Mirror[];
        foundMirrors = mirrors.ToList();

        return foundMirrors;
    }

    public List<Trap> FindTraps() {
        List<Trap> foundTraps = new List<Trap>();
        Trap[] traps = Object.FindObjectsOfType<Trap>() as Trap[];
        foundTraps = traps.ToList();

        return foundTraps;
    }

    public List<PushingWall> FindPushingWalls() {
        List<PushingWall> foundPushingWalls = new List<PushingWall>();
        PushingWall[] pushingWalls = Object.FindObjectsOfType<PushingWall>() as PushingWall[];
        foundPushingWalls = pushingWalls.ToList();

        return foundPushingWalls;
    }

    public List<Armor> FindArmors() {
        List<Armor> foundArmors = new List<Armor>();
        Armor[] armors = Object.FindObjectsOfType<Armor>() as Armor[];
        foundArmors = armors.ToList();
       
        return foundArmors;
    }

    public List<Sword> FindSwords() {
        List<Sword> foundSwords = new List<Sword>();
        Sword[] swords = Object.FindObjectsOfType<Sword>() as Sword[];
        foundSwords = swords.ToList();


        foreach (var sword in swords) {
            sword.gameObject.SetActive(sword.transform.parent.GetComponent<Armor>().isActive);
        }


        return foundSwords;
    }

    public List<Mover> FindMovers() {
        List<Mover> foundMovers = new List<Mover>();
        Mover[] movers = Object.FindObjectsOfType<Mover>() as Mover[];
        foundMovers = movers.ToList();

        return foundMovers;
    }


    public void DrawGoal() {
        if (goalPrefab != null && m_goalNode != null) {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position, Quaternion.identity);

            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType));
        }
    }

    public void InitBoard() {
        if (m_player != null) {
            m_playerNode.InitNode();
        }
    }

}