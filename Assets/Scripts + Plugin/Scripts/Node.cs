using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }


    Board m_board;

    public GameObject switchPrefab;
    public GameObject gatePrefab;
    public GameObject triggerPrefab;
    public GameObject lightBulbPrefab;
    public GameObject flashlitePrefab;

    GameObject gateTemp;


    public GameObject geometry;

    public GameObject linkPrefab;

    public float scaleTime = 0.3f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;


    public float delay = 1f;

    bool m_isInitialized = false;

    public LayerMask obstacleLayer;

    [HideInInspector]
    public MovableObject MO;

    [HideInInspector]
    public Mover mover;


    public bool isLevelGoal = false;

    public bool isCrackable = false;

    public int crackableState = 2;

    public Sprite[] currentTexture = new Sprite[3];

    public bool isATrigger = false;

    public bool triggerState = false;

    public bool isASwitch = false;

    public bool switchState = false;

    public bool isAGate = false;

    public bool gateOpen = false;

    public bool hasLightBulb = false;

    public bool hasFlashLight = false;

    public int gateID = 0;

    public int mirrorID = 0;

    public int trapID = 0;

    public int pushingWallID = 0;

    public int armorID = 0;

    private Vector3 m_nodePosition;

    public Sprite[] sprites;


    private void Awake() {
        m_board = Object.FindObjectOfType<Board>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
        UpdateCrackableTexture();
        m_nodePosition = new Vector3(1000f, 1000f, 1000f);
    }

    // Use this for initialization
    void Start() {

        if (geometry != null) {
            geometry.transform.localScale = Vector3.zero;

            if (m_board != null) {
                m_neighborNodes = FindNeighbors(m_board.AllNodes);
                showModel();
            }
        }

        if (isATrigger) {
            foreach (var _mover in m_board.FindMovers()) {
                if (_mover.transform.position == transform.position) {
                    mover = _mover;
                    //UpdateTriggerToTrue();
                }
            }
        }
    }

    public void ShowGeometry() {
        if (geometry != null) {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes) {

        List<Node> n_List = new List<Node>();

        foreach (Vector2 dir in Board.directions) {
            Node foundNeighbor = FindNeighborAt(nodes, dir);

            if (foundNeighbor != null && !n_List.Contains(foundNeighbor)) {
                n_List.Add(foundNeighbor);
            }
        }
        return n_List;
    }

    public Node FindNeighborAt(List<Node> nodes, Vector2 dir) {
        return nodes.Find(n => n.Coordinate == Coordinate + dir);
    }

    public Node FindNeighborAt(Vector2 dir) {
        return FindNeighborAt(NeighborNodes, dir);
    }

    public void InitNode() {

        if (!m_isInitialized) {
            ShowGeometry();
            InitNeighbors();
            m_isInitialized = true;
        }

    }

    void InitNeighbors() {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine() {
        yield return new WaitForSeconds(delay);

        foreach (Node n in m_neighborNodes) {

            if (!m_linkedNodes.Contains(n)) {

                Obstacle obstacle = FindObstacle(n);

                if (obstacle == null) {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode) {
        if (linkPrefab != null) {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            Link link = linkInstance.GetComponent<Link>();
            if (link != null) {
                link.DrawLink(transform.position, targetNode.transform.position);
            }

            if (!m_linkedNodes.Contains(targetNode)) {
                m_linkedNodes.Add(targetNode);
            }

            if (!targetNode.LinkedNodes.Contains(this)) {
                targetNode.LinkedNodes.Add(this);
            }
        }
    }

    Obstacle FindObstacle(Node targetNode) {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, Board.spacing + 0.1f, obstacleLayer)) {
            //Debug.Log("Node FindObstacle: Ha colpito un ostacolo da " + this.name + " a " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }


    public int GetCrackableState() {
        return crackableState;
    }

    public void UpdateCrackableState() {
        this.crackableState--;
    }

    public void DestroyCrackableInOneHit() {
        this.crackableState = 0;
    }

    public void FromCrackableToNormal() {
        this.crackableState = 100;
    }


    public void UpdateCrackableTexture() {
        if (this.isCrackable) {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[crackableState];
        }
        else {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public bool UpdateTriggerToTrue() {
        UpdateGateToOpen(gateID);
        TrapActivation(trapID);
        PushingWallActivation(pushingWallID);
        ArmorActivation(armorID);

        return triggerState = true;

    }//UpdateTriggerToFalse --> in Board


    public bool GetSwitchState() {
        return switchState;
    }

    public bool UpdateSwitchToTrue() {
        UpdateGateToOpen(gateID);
        ArmorActivation(armorID);

        PushingWallActivation(pushingWallID);

        if (mirrorID != 0) {
            foreach (var mirror in m_board.AllMirrors) {
                if (mirror.mirrorID == mirrorID) {
                    mirror.UpdateMirrorRotation();
                }
            }
        }
        return switchState = true;
    }

    public bool UpdateSwitchToFalse() {
        UpdateGateToClose(gateID);
        ArmorDeactivation(armorID);

        if (mirrorID != 0) {
            foreach (var mirror in m_board.AllMirrors) {
                if (mirror.mirrorID == mirrorID) {
                    mirror.UpdateMirrorRotation();
                }
            }
        }

        return switchState = false;
    }

    public bool GetGateState() {
        return gateOpen;
    }

    public int GetGateID() {
        return gateID;
    }

    public int GetArmorID() {
        return armorID;
    }

    public int GetMirrorID() {
        return mirrorID;
    }


    public void SetGateOpen() { //PROVA____________________________________________________________________________________________________________________________________
        gateOpen = !gateOpen;
        if (gateTemp != null)
        {
            gateTemp.SetActive(!gateOpen);
        }

    }

    public void SetGateClose() {
        gateOpen = !gateOpen;
        if (gateTemp!=null)
        {
             gateTemp.SetActive(!gateOpen);
        }
    }

    public bool UpdateGateToOpen(int id) {
        gateOpen = false;

        foreach (var node in m_board.AllNodes) {
            if (node.GetGateID() == id) {
                node.SetGateOpen();
            }
        }
        return gateOpen;
    }

    public void TrapActivation(int id) {

        foreach (var trap in m_board.AllTraps) {
            if (trap.GetID() == id) {
                trap.Shoot();
            }
        }
    }

    public void PushingWallActivation(int id) {
        foreach (var pushingWall in m_board.AllPushingWalls) {
            if (pushingWall.GetID() == id) {
                pushingWall.Push();
            }
        }
    }

    public void ArmorActivation(int id) {
        foreach (var armor in m_board.AllArmors) {
            if (armor.GetID() == id) {
                armor.ActivateSword();
            }
        }
    }

    public void ArmorDeactivation(int id) {
        foreach (var armor in m_board.AllArmors) {
            if (armor.GetID() == id) {
                armor.DeactivateSword();
            }
        }
    }

    public bool UpdateGateToClose(int id) {
        gateOpen = true;

        foreach (var node in m_board.AllNodes) {
            if (node.GetGateID() == id) {
                node.SetGateClose();
            }
        }
        return gateOpen;
    }

    public void showModel() {
        if (isASwitch) {
            Instantiate(switchPrefab, transform.position, Quaternion.identity);
        }

        if (isATrigger) {
            GameObject triggerTemp;
            triggerTemp = Instantiate(triggerPrefab, transform.position, Quaternion.identity);
            triggerTemp.transform.Rotate(-90, 0, 0);
        }

        if (isAGate) {

            gateTemp = Instantiate(gatePrefab, transform.position, Quaternion.identity);
            
        }

        if (hasLightBulb) {
            GameObject lightBulbTemp;
            lightBulbTemp = Instantiate(lightBulbPrefab, transform.position, Quaternion.identity);
            lightBulbTemp.transform.position += new Vector3(0, 1.5f, 0);
            lightBulbTemp.transform.parent = transform;
        }

        if (hasFlashLight) {
            GameObject flashliteTemp;
            flashliteTemp = Instantiate(flashlitePrefab, transform.position, Quaternion.identity);
            flashliteTemp.transform.position += new Vector3(0, -.8f, 0);
            flashliteTemp.transform.parent = transform;

        }
        if (isCrackable)
        {
            transform.GetChild(1).gameObject.transform.Rotate(90,0, 0);
            transform.GetChild(1).gameObject.transform.position += new Vector3(0, 0.1f, 0);
        }
    }


    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Node,
        };
        return itemData;
    }
}