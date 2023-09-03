using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Colyseus;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : ColyseusManager<MultiplayerManager> {
#region Server
    private const string GameRoomName = "state_handler";
    private ColyseusRoom<State> _room;
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeClient();
        Connection();
    }

    private async void Connection() {
        
        Dictionary<string, object> data = new Dictionary<string, object>(){
            { "login", PlayerSettings.Instance.Login},
            { "skin", Skins.Instance.GetLength() }
        };
        _room = await Instance.client.JoinOrCreate<State>(GameRoomName, data);
        _room.OnStateChange += OnChange;
    }

    private void OnChange(State state, bool isfirststate) {
        if(isfirststate == false) return;
        _room.OnStateChange -= OnChange;
        
        state.players.ForEach((key, player) => {
            if(key == _room.SessionId) CreatePlayer(player);
            else CreateEnemy(key, player);
        });
        
        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;

        _room.State.apples.ForEach(CreateApple);
        _room.State.apples.OnAdd += (key, apple) => CreateApple(apple);
        _room.State.apples.OnRemove += (key, apple) => RemoveApple(apple);
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();
        LeaveRoom();
    }
    public void LeaveRoom() => _room?.Leave();

    public void SendMessage(string key, Dictionary<string, object> data) => _room.Send(key, data);
    public void SendMessage(string key, string data) => _room.Send(key, data);
#endregion
    
#region Player
    [SerializeField] private PlayerAim _playerAim;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    
    public void CreatePlayer(Player player) {
        Vector3 position = new Vector3(player.x, 0,  player.z);
        Quaternion rotation = Quaternion.identity;
        
        Snake snake = Instantiate(_snakePrefab, position, rotation);
        snake.Init(player.d, player.clr, player.login, true);

        PlayerAim aim = Instantiate(_playerAim, position, rotation);
        aim.Init(snake.Head, snake.Speed);
        
        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(_room.SessionId, aim, player, snake);
        
        AddLeader(_room.SessionId, player);
    }
#endregion
    
#region Enemy
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    private void CreateEnemy(string key, Player player) {
        Vector3 position = new Vector3(player.x, 0,  player.z);
        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d, player.clr, player.login);
        
        EnemyController enemy = snake.gameObject.AddComponent<EnemyController>();
        enemy.Init(key, player, snake);
        
        _enemies.Add(key, enemy);
        
        AddLeader(key, player);
    }
    private void RemoveEnemy(string key, Player value) {
        RemoveLeader(key);
        
        if (_enemies.ContainsKey(key) == false) {
            Debug.LogError("Попытка уничтожения енеми, которого не было в словаре");
            return;
        }

        EnemyController enemy = _enemies[key];
        _enemies.Remove(key);
        enemy.Destroy();
    }
#endregion

#region Apple
    [SerializeField] private Apple _applePrefab;
    private Dictionary<Vector2Float, Apple> _apples = new Dictionary<Vector2Float, Apple>();
    
    private void CreateApple(Vector2Float vector2Float) {
        Vector3 position = new Vector3(vector2Float.x, 0, vector2Float.z);
        
        Apple apple = Instantiate(_applePrefab, position, Quaternion.identity);
        apple.Init(vector2Float);
        
        _apples.Add(vector2Float, apple);
    }
    
    private void RemoveApple(Vector2Float vector2Float) {
        if (_apples.ContainsKey(vector2Float) == false) return;

        Apple apple = _apples[vector2Float];
        _apples.Remove(vector2Float);

        apple.Destroy();

    }
#endregion

#region LeaderBoard

    private class LoginScorePair {
        public string Login;
        public float Score;
    }
    
    [SerializeField] private Text _text;

    private Dictionary<string, LoginScorePair> _leaders = new Dictionary<string, LoginScorePair>();

    private void AddLeader(string sessionID, Player player) {
        if(_leaders.ContainsKey(sessionID)) return;
        
        _leaders.Add(sessionID, new LoginScorePair {
            Login = player.login, 
            Score = player.score
        });
        
        UpdateBoard();
    }

    private void RemoveLeader(string sessionID) {
        if(_leaders.ContainsKey(sessionID) == false) return;
        _leaders.Remove(sessionID);
        
        UpdateBoard();
    }

    public void UpdateScore(string sessionID, int score) {
        if(_leaders.ContainsKey(sessionID) == false) return;

        _leaders[sessionID].Score = score;
        UpdateBoard();
    }

    private void UpdateBoard() {
        int topCount = Mathf.Clamp(_leaders.Count, 0, 5);
        var top5 = _leaders.OrderByDescending(pair => pair.Value.Score).Take(topCount);

        string text = "";
        int i = 1;
        foreach (var item in top5) {
            text += $"{i}. {item.Value.Login} : {item.Value.Score}\n";
            i++;
            
        }
        _text.text = text;
    }
#endregion
}