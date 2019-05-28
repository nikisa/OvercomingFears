//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;


//public class LevelManager : MonoBehaviour {

//    //public Board m_board;

//    public const string LEVELSAVES = "LevelSaves";

//    public string LevelToLoad;

//    public Button ButtonLoadLevelPrefab;
//    public GameObject ButtonsContainer;


//    List<PlayerManager> m_player = new List<PlayerManager>();
//    List<Node> m_nodes = new List<Node>();
//    List<EnemyManager> m_enemies = new List<EnemyManager>();
//    List<MovableObject> m_movableObjects = new List<MovableObject>();
//    List<Obstacle> m_obstacles = new List<Obstacle>();
//    List<FloorData> m_floor = new List<FloorData>();

//    public GameObject PlayerPrefab;
//    public GameObject NodePrefab;
//    public GameObject EnemyPrefab;
//    public GameObject ObstaclePrefab;
//    public GameObject MovableObjectPrefab;
//    public GameObject BoardPrefab;

//    public List<string> LevelsID = new List<string>();

//    private void Awake() {
//        //m_board = FindObjectOfType<Board>().GetComponent<Board>();
//    }

//    void Start() {
//        string jsonStringLevelSaves = PlayerPrefs.GetString(LEVELSAVES);
//        LevelSaves levelSaves = JsonUtility.FromJson<LevelSaves>(jsonStringLevelSaves);
//        LevelsID = levelSaves.LevelsID;

//        foreach (string levelId in LevelsID) {
//            //Debug.LogFormat("Level {0} found!", levelId);
//            Button newButton = Instantiate<Button>(ButtonLoadLevelPrefab, ButtonsContainer.transform);
//            newButton.GetComponentInChildren<Text>().text = levelId;
//            newButton.onClick.AddListener(() => LoadLevel(levelId));
//        }
//    }

//        public void ClearAllData() {
//            GetAllInterestingData();
//            foreach (var item in m_nodes) {
//                item.isLevelGoal = false;
//                Destroy(item.gameObject);
//            }
//            foreach (var item in m_enemies) {
//                Destroy(item.gameObject);
//            }
//            foreach (var item in m_movableObjects) {
//                Destroy(item.gameObject);
//            }
//            foreach (var item in m_obstacles) {
//                Destroy(item.gameObject);
//            }
//            foreach (var item in m_floor) {
//                Destroy(item.gameObject);
//            }
//            foreach (var item in m_player) {
//               Destroy(item.gameObject);
//            }
//    }

//        List<ItemData> items = new List<ItemData>();

//        public void SaveLevel() {

//            items = new List<ItemData>();

//            GetAllInterestingData();
//            foreach (var item in m_nodes) {
//                items.Add(item.GetData());
//            }
//            foreach (var item in m_enemies) {
//                items.Add(item.GetData());
//            }
//            foreach (var item in m_movableObjects) {
//                items.Add(item.GetData());
//            }
//            foreach (var item in m_obstacles) {
//                items.Add(item.GetData());
//            }
//            foreach (var item in m_floor) {
//                items.Add(item.GetData());
//            }
//            foreach (var item in m_player) {
//                items.Add(item.GetData());
//            }

//        LevelData saveClass = new LevelData();

//            saveClass.ID = "Level" + (LevelsID.Count + 1).ToString();
//            LevelsID.Add(saveClass.ID);
//            Debug.LogFormat("{0} saved!", saveClass.ID);
//            saveClass.Items = items;
//            // Salvo livello
//            string jsonString = JsonUtility.ToJson(saveClass);
//            PlayerPrefs.SetString(saveClass.ID, jsonString);
//            // Salvo il nome del livello nella lista dei livelli
//            LevelSaves levelSaves = new LevelSaves();
//            levelSaves.LevelsID = LevelsID;
//            string jsonStringForLevelID = JsonUtility.ToJson(levelSaves);
//            PlayerPrefs.SetString(LEVELSAVES, jsonStringForLevelID);
//            PlayerPrefs.Save();
//        }
//        public void LoadLevel(string _levelID = "") {

//            ClearAllData();

//            if (_levelID == "")
//                _levelID = LevelToLoad;

//            string jsonLevelStringData = PlayerPrefs.GetString(_levelID);

//            LevelData level = JsonUtility.FromJson<LevelData>(jsonLevelStringData);

//            foreach (ItemData item in level.Items) {
//                switch (item.ItemType) {
//                    case ItemData.Type.Player:
//                        Instantiate(PlayerPrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                    case ItemData.Type.Node:
//                        Instantiate(NodePrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                    case ItemData.Type.Enemy:
//                        Instantiate(EnemyPrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                    case ItemData.Type.MovableObject:
//                        Instantiate(MovableObjectPrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                    case ItemData.Type.Obstacle:
//                        Instantiate(ObstaclePrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                    case ItemData.Type.Floor:
//                        Instantiate(BoardPrefab, item.BoardPosition, Quaternion.identity);
//                        break;
//                default:
//                        break;
//                }
//            }
//        }

//        public void GetAllInterestingData() {
//            m_player = FindObjectsOfType<PlayerManager>().ToList();
            
//            // Creo una lista di tutte le Board presenti nel livello.
//            m_floor = FindObjectsOfType<FloorData>().ToList();

//            // Creo una lista di tutti gli Enemy presenti nel livello.
//            m_enemies = FindObjectsOfType<EnemyManager>().ToList();

//            // Creo una lista di tutti i MovableObject presenti nel livello.
//            m_movableObjects = FindObjectsOfType<MovableObject>().ToList();

//            // Creo una lista di tutti i Node presenti nel livello.
//            m_nodes = FindObjectsOfType<Node>().ToList();
            
//            // Creo una lista di tutti gli Obstacle presenti nel livello.
//            m_obstacles = FindObjectsOfType<Obstacle>().ToList();

//            //m_board.InitBoard();
//        }

//}


//[Serializable]
//public class LevelData {
//    public string ID;
//    public List<ItemData> Items;
//}

//public class LevelSaves {
//    public List<string> LevelsID;
//}
