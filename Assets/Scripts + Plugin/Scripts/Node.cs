using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{

    public float puzzleActivationDelay;

    public Animator TriggerController;

    public Material[] materials;
    public Material[] shadowMaterials;
    public Material[] shadowOffMaterials;

    public GameObject[] triggers;

    public GameObject[] shadows;
    public GameObject[] offShadows;

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }

    BoardManager m_board;
    PlayerManager m_player;

    public GameObject switchPrefab;
    public GameObject gatePrefab;
    public GameObject triggerPrefab;
    public GameObject lightBulbPrefab;
    public GameObject flashlitePrefab;

    public GameObject crackablePrefab;
    public GameObject crackableChoked;

    GameObject gateTemp;
    GameObject switchTemp;
    GameObject crackableTemp;
    
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

    public int crackableState = 0;

    public Sprite[] currentTexture = new Sprite[3];

    public bool isATrigger = false;

    public bool triggerState = false;

    public bool triggerWithEnemy = false; //triggerWithEnemy va messo a true nell'inspector solo se lo static viene ucciso dal player (no flashlight o armor)

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

    public Animator SwitchAnimator;

    private void Awake()
    {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
        //UpdateCrackableTexture();
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
            ////Debug.Log("Node FindObstacle: Ha colpito un ostacolo da " + this.name + " a " + targetNode.name);
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
        this.crackableState++;
    }

    public void DestroyCrackableInOneHit()
    {
        this.crackableState = 0;
    }

    public void FromCrackableToNormal()
    {
        this.crackableState = -1;
        //transform.GetChild(2).GetComponent<Crackable>().crackableAnimator.SetInteger("CrackableState" , crackableState);
        Destroy(crackableTemp);
        crackableTemp = Instantiate(crackableChoked , transform.position , Quaternion.identity);
    }


    public void UpdateCrackableTexture()
    {
        //if (this.isCrackable) {
        //    if (crackableState < 0) {
        //        crackableState = 0;
        //    }
        //    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[crackableState];
        //}
        //else {
        //    transform.GetChild(1).gameObject.SetActive(false);
        //}

        if (isCrackable) {

            if (crackableState > 2) {
                crackableState = 2;
            }
            
            transform.GetChild(2).GetComponent<Crackable>().crackableAnimator.SetInteger("CrackableState", crackableState);
            
            //crackableTemp.GetComponent<Crackable>().crackableAnimator.SetInteger("CrackableState", crackableState);
        }

    }


    public bool UpdateTriggerToTrue()
    {
        if (isATrigger && TriggerOrLogic() == false) {

            //==============================================

            ArmorActivation(armorID);
            UpdateGateToOpen(gateID);
            TrapActivation(trapID);

            //===============================================

            //triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(true);
            //PushingWallActivation(pushingWallID);
            foreach (Node trigger in m_board.TriggerNodes) {
                if (trigger != null && m_board.PreviousPlayerNode.GetTriggerId(m_board.PreviousPlayerNode) == m_board.PreviousPlayerNode.GetTriggerId(trigger)) {
                    trigger.triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(true);
                }

            }
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
                    //Debug.Log("OR: " + TriggerOrLogic());
                    //triggerTemp.transform.GetChild(1).transform.gameObject.SetActive(false);
                    ArmorDeactivation(armorID);
                    UpdateGateToOpen(gateID);
                    TrapActivation(trapID);
                    triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(false);
                    TriggerOrLogicByID(GetTriggerId(m_board.PreviousPlayerNode), false);
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
        
        switchTemp.transform.localScale = new Vector3(this.transform.localScale.x  , this.transform.localScale.y , this.transform.localScale.z * -1);
        SwitchAnimator.SetInteger("SwitchState" , 1);
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

        switchTemp.transform.localScale = new Vector3(this.transform.localScale.x , this.transform.localScale.y , this.transform.localScale.z * 1);
        SwitchAnimator.SetInteger("SwitchState", 0);
        UpdateGateToClose(gateID);
        ArmorDeactivation(armorID);
        
        //if (SceneManager.GetActiveScene().buildIndex == 3) {
        //    foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {

        //        if (enemy.isOff) {
        //            enemy.isOff = false;
        //        }



        //        if (enemy.m_enemySensor.FoundPlayer && enemy.isOff == false && m_board.FindNodeAt(enemy.transform.position).gateOpen == true) {
        //            //attack player
        //            //notify the GM to lose the level
        //            //Debug.Log(this.name + "MORTE");
        //            m_board.m_gm.LoseLevel();
        //        }
        //    }
        //}

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
            Destroy(gateTemp);
            if (gateOpen)
            {
                gateTemp = Instantiate(offShadows[gateID - 1], transform.position, Quaternion.identity);
            }
            else
            {
                gateTemp = Instantiate(shadows[gateID - 1], transform.position, Quaternion.identity);
            }
            

        }

        //Level3Patch();
    }

    public void SetGateClose()
    {
        gateOpen = !gateOpen;
        if (gateTemp != null)
        {
            Destroy(gateTemp);
            if (gateOpen)
            {
                gateTemp = Instantiate(offShadows[gateID - 1], transform.position, Quaternion.identity);
            }
            else
            {
                gateTemp = Instantiate(shadows[gateID - 1], transform.position, Quaternion.identity);
            }

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
                switchTemp.transform.GetChild(2).GetComponent<Renderer>().material = materials[armorID - 1];
            }
            else if (trapID <= 6 && trapID >= 1)
            {
                switchTemp.transform.GetChild(2).GetComponent<Renderer>().material = materials[trapID - 1];
            }
            else if (gateID <= 6 && gateID >= 1)
            {
                switchTemp.transform.GetChild(2).GetComponent<Renderer>().material = materials[gateID - 1];

            }

        }

        if (isATrigger)
        {

            //triggerTemp = Instantiate(triggerPrefab, transform.position, Quaternion.identity);
            //triggerTemp.transform.Rotate(-90, 0, 0);


            Vector3 position;

            if (SceneManager.GetActiveScene().buildIndex != 2)
            {
                position = transform.position;
            }
            else
            {
                position = new Vector3(transform.position.x , (transform.position.y + .1f) , transform.position.z);
            }

            if (armorID <= 6 && armorID >= 1)
            {

                triggerTemp = Instantiate(triggers[armorID - 1], position, Quaternion.identity);


                //triggerTemp.GetComponent<Renderer>().material = materials[armorID - 1];
                //triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[armorID - 1];
            }
            else if (trapID <= 6 && trapID >= 1)
            {

                triggerTemp = Instantiate(triggers[trapID - 1], position, Quaternion.identity);

                //triggerTemp.GetComponent<Renderer>().material = materials[trapID - 1];
                //triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[trapID - 1];
            }
            else if (gateID <= 6 && gateID >= 1)
            {

                triggerTemp = Instantiate(triggers[gateID - 1], position, Quaternion.identity);

                //triggerTemp.GetComponent<Renderer>().material = materials[gateID - 1];
                //triggerTemp.transform.GetChild(0).GetComponent<Renderer>().material = materials[gateID - 1];
            }
            
        }

        if (isAGate)
        {

            //gateTemp = Instantiate(gatePrefab, transform.position, Quaternion.identity);
            

            if (gateID <= 6 && gateID >= 1)
            {
                //gateTemp.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = shadowOffMaterials[gateID - 1];
                //gateTemp.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = shadowMaterials[gateID - 1];

                if (gateOpen)
                {
                    gateTemp = Instantiate(offShadows[gateID - 1], transform.position, Quaternion.identity);
                }
                else
                {
                    gateTemp = Instantiate(shadows[gateID - 1], transform.position, Quaternion.identity);
                }
            }
        }

        if (hasLightBulb)
        {
            GameObject lightBulbTemp;
            lightBulbTemp = Instantiate(lightBulbPrefab, transform.position, Quaternion.identity);
            lightBulbTemp.transform.position += new Vector3(0, 1.5f, 0);
            lightBulbTemp.transform.parent = transform;
        }

        if (isCrackable) {
            //transform.GetChild(1).gameObject.transform.Rotate(90, 0, 0);
            //transform.GetChild(1).gameObject.transform.position += new Vector3(0, 0.1f, 0);

            crackableTemp = Instantiate(crackablePrefab, transform.position , Quaternion.identity);
            crackableTemp.transform.parent = transform;

        }

        if (hasFlashLight)
        {
            GameObject flashliteTemp;
            flashliteTemp = Instantiate(flashlitePrefab, transform.position, Quaternion.identity);
            flashliteTemp.transform.position += new Vector3(0, -0.8f, 0);
            flashliteTemp.transform.parent = transform;

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
                            TriggerOrLogicByID(GetTriggerId(m_board.FindNodeAt(enemy.transform.position)) , true);
                        }
                        //else if (m_board.PreviousPlayerNode != null && m_board.PreviousPlayerNode.isATrigger) {
                        //    TriggerOrLogicByID(GetTriggerId(m_board.PreviousPlayerNode), false);
                        //}


                    }
                    
                }
                
            }
            
        }

        if (result > 0)
            ris = true;

        //Debug.Log(ris);
        return ris;
    }

    public void CheckTriggerById(int id) {

        foreach (Node triggerNode in m_board.TriggerNodes) {
            if (m_board.FindNodeAt(triggerNode.transform.position).armorID == id || m_board.FindNodeAt(triggerNode.transform.position).trapID == id || m_board.FindNodeAt(triggerNode.transform.position).gateID == id) {
                
                foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {
                    if (enemy != null && m_board.FindNodeAt(enemy.transform.position) == triggerNode) {
                        TriggerOrLogicByID(id, true);
                    }
                    else {
                        TriggerOrLogicByID(id, false);
                    }
                }
             }



            

        }

      
    }



    public void TriggerOrLogicByID(int id, bool state) {
        
        if (id > 0 && id <= 6) {
            foreach (Node triggerNode in m_board.TriggerNodes) {
                //Debug.Log(triggerNode);
                //Debug.Log(m_board.TriggerNodes.Count);
                if (triggerNode.armorID == id || triggerNode.trapID == id || triggerNode.gateID == id) {
                        triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(state);
                    }
                
                
            }

        }
    }

    public void CheckTriggerId(Node n) {

        int ris = 0;

        if (n.isATrigger) {
            if (n.armorID != 0 && n.trapID == 0 && n.gateID == 0) {
                ris = n.armorID;
            }
            else if (n.armorID == 0 && n.trapID != 0 && n.gateID == 0) {
                ris = n.trapID;
            }
            else if (n.armorID == 0 && n.trapID == 0 && n.gateID != 0) {
                ris = n.gateID;
            }
        }

        TriggerOrLogicByID(ris , TriggerOrLogic());
    }

    

    public int GetTriggerId(Node n) {

        int ris = 0;

        if (n.isATrigger) {
            if (n.armorID != 0 && n.trapID == 0 && n.gateID == 0) {
                ris = n.armorID;
            }
            else if (n.armorID == 0 && n.trapID != 0 && n.gateID == 0) {
                ris = n.trapID;
            }
            else if (n.armorID == 0 && n.trapID == 0 && n.gateID != 0) {
                ris = n.gateID;
            }
        }

        return ris;
    }


    IEnumerator Death() {
        m_player.playerInput.InputEnabled = false;
        m_player.playerInput.PauseInputEnabled = false;
        yield return new WaitForSeconds(.35f);

        m_player.PlayerAnimatorController.SetInteger("PlayerState", 7);

        yield return new WaitForSeconds(.2f);
        m_board.m_gm.LoseLevel();
    }


    void Level3Patch() {
        if (SceneManager.GetActiveScene().buildIndex == 3) {

            if (m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position)).Count != 0)
            {
                foreach (EnemyManager enemy in m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position)))
                {
                    enemy.isOff = false;

                    if (enemy.isOff == false && enemy.m_enemySensor.FoundPlayer)
                    {
                        enemy.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
                        StartCoroutine(Death());
                    }

                    enemy.m_enemySensor.m_foundPlayer = false;
                }
            }


            //if (m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2 , 0 , 0))).Count != 0 &&
            //    m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2, 0, 0)).isAGate &&
            //    m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2, 0, 0)).gateOpen)
            //{
            //    enemyTemp = m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2, 0, 0))).ToArray()[0];
            //    enemyTemp.isOff = false;

            //    if (enemyTemp.isOff == false && enemyTemp.m_enemySensor.FoundPlayer)
            //    {
            //        enemyTemp.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
            //        StartCoroutine(Death());
            //    }

            //    enemyTemp.m_enemySensor.m_foundPlayer = false;
            //}
            //    else if (m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2, 0, 0))).Count != 0 &&
            //            m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2, 0, 0)).isAGate &&
            //            m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2, 0, 0)).gateOpen){
            //                enemyTemp = m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2, 0, 0))).ToArray()[0];
            //                enemyTemp.isOff = false;

            //        if (enemyTemp.isOff == false && enemyTemp.m_enemySensor.FoundPlayer)
            //        {
            //            enemyTemp.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
            //            StartCoroutine(Death());
            //        }

            //            enemyTemp.m_enemySensor.m_foundPlayer = false;
            //            }


            //        else if (m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2))).Count != 0 &&
            //                m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2)).isAGate &&
            //                m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2)).gateOpen){

            //                    enemyTemp = m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2))).ToArray()[0];
            //                    enemyTemp.isOff = false;

            //            if (enemyTemp.isOff == false && enemyTemp.m_enemySensor.FoundPlayer)
            //            {
            //                enemyTemp.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
            //                StartCoroutine(Death());
            //            }

            //            enemyTemp.m_enemySensor.m_foundPlayer = false;
            //            }
            //            else if (m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2))).Count != 0 &&
            //                    m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2)).isAGate &&
            //                    m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2)).gateOpen){
            //                        enemyTemp = m_board.FindEnemiesAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2))).ToArray()[0];
            //                        enemyTemp.isOff = false;

            //                if (enemyTemp.isOff == false && enemyTemp.m_enemySensor.FoundPlayer)
            //                {
            //                    enemyTemp.m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
            //                    StartCoroutine(Death());
            //                }

            //                enemyTemp.m_enemySensor.m_foundPlayer = false;
            //}




            //foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {
            //    if (enemy.isOff) {
            //        enemy.isOff = false;
            //    }



            //    if (enemy.m_enemySensor.FoundPlayer && enemy.isOff == false && m_board.FindNodeAt(enemy.transform.position).gateOpen == true) {
            //        //attack player
            //        //notify the GM to lose the level
            //        //Debug.Log(enemy.name + "MORTE");

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