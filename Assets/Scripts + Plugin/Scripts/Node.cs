using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{

    public Material[] materials;
    public Material[] shadowMaterials;
    public Material[] shadowOffMaterials;

    public GameObject[] shadows;

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }


    BoardManager m_board;

    public GameObject switchPrefab;
    public GameObject gatePrefab;
    public GameObject triggerPrefab;
    public GameObject lightBulbPrefab;
    public GameObject flashlitePrefab;

    GameObject gateTemp;
    GameObject switchTemp;
    [HideInInspector]
    public GameObject triggerTemp;


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


    private void Awake()
    {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
        UpdateCrackableTexture();
        m_nodePosition = new Vector3(1000f, 1000f, 1000f);
    }

    // Use this for initialization
    void Start()
    {

        if (geometry != null)
        {
            geometry.transform.localScale = Vector3.zero;

            if (m_board != null)
            {
                m_neighborNodes = FindNeighbors(m_board.AllNodes);
                showModel();
            }
        }

        if (isATrigger)
        {
            foreach (var _mover in m_board.FindMovers())
            {
                if (_mover.transform.position == transform.position)
                {
                    mover = _mover;
                    //UpdateTriggerToTrue();
                }
            }
        }
    }

    public void ShowGeometry()
    {
        if (geometry != null)
        {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes)
    {

        List<Node> n_List = new List<Node>();

        foreach (Vector2 dir in BoardManager.directions)
        {
            Node foundNeighbor = FindNeighborAt(nodes, dir);

            if (foundNeighbor != null && !n_List.Contains(foundNeighbor))
            {
                n_List.Add(foundNeighbor);
            }
        }
        return n_List;
    }

    public Node FindNeighborAt(List<Node> nodes, Vector2 dir)
    {
        return nodes.Find(n => n.Coordinate == Coordinate + dir);
    }

    public Node FindNeighborAt(Vector2 dir)
    {
        return FindNeighborAt(NeighborNodes, dir);
    }

    public void InitNode()
    {

        if (!m_isInitialized)
        {
            ShowGeometry();
            InitNeighbors();
            m_isInitialized = true;
        }

    }

    void InitNeighbors()
    {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (Node n in m_neighborNodes)
        {

            if (!m_linkedNodes.Contains(n))
            {

                Obstacle obstacle = FindObstacle(n);

                if (obstacle == null)
                {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode)
    {
        if (linkPrefab != null)
        {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            Link link = linkInstance.GetComponent<Link>();
            if (link != null)
            {
                link.DrawLink(transform.position, targetNode.transform.position);
            }

            if (!m_linkedNodes.Contains(targetNode))
            {
                m_linkedNodes.Add(targetNode);
            }

            if (!targetNode.LinkedNodes.Contains(this))
            {
                targetNode.LinkedNodes.Add(this);
            }
        }
    }

    Obstacle FindObstacle(Node targetNode)
    {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, BoardManager.spacing + 0.1f, obstacleLayer))
        {
            //Debug.Log("Node FindObstacle: Ha colpito un ostacolo da " + this.name + " a " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }


    public int GetCrackableState()
    {
        return crackableState;
    }

    public void UpdateCrackableState()
    {
        this.crackableState--;
    }

    public void DestroyCrackableInOneHit()
    {
        this.crackableState = 0;
    }

    public void FromCrackableToNormal()
    {
        this.crackableState = 100;
    }


    public void UpdateCrackableTexture()
    {
        if (this.isCrackable)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[crackableState];
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public bool UpdateTriggerToTrue()
    {
        if (isATrigger && TriggerOrLogic() == false)
        {
            triggerTemp.transform.GetChild(1).transform.gameObject.SetActive(true);

            ArmorActivation(armorID);
            UpdateGateToOpen(gateID);
            TrapActivation(trapID);
            //PushingWallActivation(pushingWallID);

            

        }

        return triggerState = true;

    }//UpdateTriggerToFalse --> in Board


    public bool UpdateTriggerToFalse()
    {
        if (isATrigger)
        {
            foreach (Node triggerNode in m_board.TriggerNodes)
            {
                if (!TriggerOrLogic())
                {
                    Debug.Log("OR: " + TriggerOrLogic());
                    triggerTemp.transform.GetChild(1).transform.gameObject.SetActive(false);
                    ArmorDeactivation(armorID);
                    UpdateGateToOpen(gateID);
                    TrapActivation(trapID);
                }


            }
        }

        return triggerState = false;
    }

    public bool GetSwitchState()
    {
        return switchState;
    }

    public bool UpdateSwitchToTrue()
    {

        

        switchTemp.transform.localScale = new Vector3(this.transform.localScale.x * -30, this.transform.localScale.y * 30, this.transform.localScale.z * 30);
        UpdateGateToOpen(gateID);
        ArmorActivation(armorID);

        

        //PushingWallActivation(pushingWallID);

        if (mirrorID != 0)
        {
            foreach (var mirror in m_board.AllMirrors)
            {
                if (mirror.mirrorID == mirrorID)
                {
                    mirror.UpdateMirrorRotation();
                }
            }
        }
        return switchState = true;
    }

    public bool UpdateSwitchToFalse()
    {
        
        switchTemp.transform.localScale = new Vector3(this.transform.localScale.x * 30, this.transform.localScale.y * 30, this.transform.localScale.z * 30);
        UpdateGateToClose(gateID);
        ArmorDeactivation(armorID);

        if (SceneManager.GetActiveScene().buildIndex == 3) {
            foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {

                if (enemy.isOff) {
                    enemy.isOff = false;
                }

                

                if (enemy.m_enemySensor.FoundPlayer && enemy.isOff == false && m_board.FindNodeAt(enemy.transform.position).gateOpen == true) {
                    //attack player
                    //notify the GM to lose the level
                    Debug.Log(this.name + "MORTE");
                    m_board.m_gm.LoseLevel();
                }
            }
        }

        if (mirrorID != 0)
        {
            foreach (var mirror in m_board.AllMirrors)
            {
                if (mirror.mirrorID == mirrorID)
                {
                    mirror.UpdateMirrorRotation();
                }
            }
        }

        return switchState = false;
    }

    public bool GetGateState()
    {
        return gateOpen;
    }

    public int GetGateID()
    {
        return gateID;
    }

    public int GetArmorID()
    {
        return armorID;
    }

    public int GetMirrorID()
    {
        return mirrorID;
    }

    public int GetTrapID()
    {
        return trapID;
    }


    public void SetGateOpen()
    { //PROVA____________________________________________________________________________________________________________________________________
        gateOpen = !gateOpen;
        if (gateTemp != null)
        {
            gateTemp.transform.GetChild(0).gameObject.SetActive(gateOpen);
            gateTemp.transform.GetChild(1).gameObject.SetActive(!gateOpen);
        }
        Level3Patch();
    }

    public void SetGateClose()
    {
        gateOpen = !gateOpen;
        if (gateTemp != null)
        {
            gateTemp.transform.GetChild(0).gameObject.SetActive(gateOpen);
            gateTemp.transform.GetChild(1).gameObject.SetActive(!gateOpen);
        }
        Level3Patch();
    }

    public bool UpdateGateToOpen(int id)
    {
        //gateOpen = false;

        foreach (var node in m_board.AllNodes)
        {
            if (node.GetGateID() == id)
            {
                node.SetGateOpen();
            }
        }
        

        return gateOpen;
    }

    public void TrapActivation(int id)
    {

        foreach (var trap in m_board.AllTraps)
        {
            if (trap.GetID() == id)
            {
                trap.isShooting = true;
                trap.Shoot();
            }
        }
    }

    public void TrapDeactivation(int id)
    {

        foreach (var trap in m_board.AllTraps)
        {
            if (trap.GetID() == id)
            {
                trap.isShooting = false;

            }
        }
    }

    public void PushingWallActivation(int id)
    {
        foreach (var pushingWall in m_board.AllPushingWalls)
        {
            if (pushingWall.GetID() == id)
            {
                pushingWall.Push();
            }
        }
    }

    public void ArmorActivation(int id)
    {
        foreach (var armor in m_board.AllArmors)
        {
            if (armor.GetID() == id)
            {
                armor.ActivateSword();
            }
        }

    }

    public void ArmorDeactivation(int id)
    {
        foreach (var armor in m_board.AllArmors)
        {
            if (armor.GetID() == id)
            {
                armor.DeactivateSword();
            }
        }
    }

    public bool UpdateGateToClose(int id)
    {
        gateOpen = !gateOpen;

        foreach (var node in m_board.AllNodes)
        {
            if (node.GetGateID() == id)
            {
                node.SetGateClose();
            }
        }
        

        return gateOpen;
    }

    public void showModel()
    {
        if (isASwitch)
        {

            switchTemp = Instantiate(switchPrefab, transform.position, Quaternion.identity);

            if (armorID <= 6 && armorID >= 1)
            {
                switchTemp.GetComponent<Renderer>().material = materials[armorID - 1];
            }
            else if (trapID <= 6 && trapID >= 1)
            {
                switchTemp.GetComponent<Renderer>().material = materials[trapID - 1];
            }
            else if (gateID <= 6 && gateID >= 1)
            {
                switchTemp.GetComponent<Renderer>().material = materials[gateID - 1];

            }

        }

        if (isATrigger)
        {

            triggerTemp = Instantiate(triggerPrefab, transform.position, Quaternion.identity);
            triggerTemp.transform.Rotate(-90, 0, 0);

            if (armorID <= 6 && armorID >= 1)
            {
                triggerTemp.GetComponent<Renderer>().material = materials[armorID - 1];
                triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[armorID - 1];
            }
            else if (trapID <= 6 && trapID >= 1)
            {
                triggerTemp.GetComponent<Renderer>().material = materials[trapID - 1];
                triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[trapID - 1];
            }
            else if (gateID <= 6 && gateID >= 1)
            {
                triggerTemp.GetComponent<Renderer>().material = materials[gateID - 1];
                triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[gateID - 1];
            }

        }

        if (isAGate)
        {

            gateTemp = Instantiate(gatePrefab, transform.position, Quaternion.identity);
            
            if (gateID <= 6 && gateID >= 1)
            {
                gateTemp.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = shadowOffMaterials[gateID - 1];
                gateTemp.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = shadowMaterials[gateID - 1];
            }

            if (gateOpen)
            {
                gateTemp.transform.GetChild(0).gameObject.SetActive(true);
                gateTemp.transform.GetChild(1).gameObject.SetActive(false);
            }
            else {
                gateTemp.transform.GetChild(0).gameObject.SetActive(false);
                gateTemp.transform.GetChild(1).gameObject.SetActive(true);
            }

        }

        if (hasLightBulb)
        {
            GameObject lightBulbTemp;
            lightBulbTemp = Instantiate(lightBulbPrefab, transform.position, Quaternion.identity);
            lightBulbTemp.transform.position += new Vector3(0, 1.5f, 0);
            lightBulbTemp.transform.parent = transform;
        }

        if (hasFlashLight)
        {
            GameObject flashliteTemp;
            flashliteTemp = Instantiate(flashlitePrefab, transform.position, Quaternion.identity);
            flashliteTemp.transform.position += new Vector3(0, -1.8f, 0);
            flashliteTemp.transform.parent = transform;

        }
        if (isCrackable)
        {
            transform.GetChild(1).gameObject.transform.Rotate(90, 0, 0);
            transform.GetChild(1).gameObject.transform.position += new Vector3(0, 0.1f, 0);
        }
    }

    public bool TriggerOrLogic()
    {
        int result = 0;
        bool ris = false;

        foreach (Node triggerNode in m_board.TriggerNodes)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                foreach (EnemyManager enemy in m_board.m_gm.m_enemies)
                {
                    if (enemy != null)
                    {
                        if (m_board.FindNodeAt(enemy.transform.position).isATrigger)
                        {
                            result++;
                        }
                    }
                    
                }
            }
            
        }

        if (result > 0)
            ris = true;

        Debug.Log(ris);
        return ris;
    }


    void Level3Patch() {
        if (SceneManager.GetActiveScene().buildIndex == 3) {

            if (m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position)).Count != 0) {
                foreach (EnemyManager enemy in m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position))) {
                    enemy.isOff = false;

                    if (enemy.isOff == false && enemy.m_enemySensor.FoundPlayer) {
                        m_board.m_gm.LoseLevel();
                    }

                    enemy.m_enemySensor.m_foundPlayer = false;
                }
            }




            //foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {
            //    if (enemy.isOff) {
            //        enemy.isOff = false;
            //    }



            //    if (enemy.m_enemySensor.FoundPlayer && enemy.isOff == false && m_board.FindNodeAt(enemy.transform.position).gateOpen == true) {
            //        //attack player
            //        //notify the GM to lose the level
            //        Debug.Log(enemy.name + "MORTE");
                    
            //        m_board.m_gm.LoseLevel();
            //    }
            //    enemy.m_enemySensor.FoundPlayer = false;

            //}
        }
    }



    public ItemData GetData()
    {
        ItemData itemData = new ItemData()
        {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Node,
        };
        return itemData;
    }
}