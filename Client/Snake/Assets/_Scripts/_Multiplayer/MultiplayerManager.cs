using System.Collections.Generic;
using Colyseus;
using UnityEngine;

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
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();
        LeaveRoom();
    }
    public void LeaveRoom() => _room?.Leave();

    public void SendMessage(string key, Dictionary<string, object> data) => _room.Send(key, data);
#endregion
    
#region Player
    [SerializeField] private PlayerAim _playerAim;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    private void CreatePlayer(Player player) {
        Vector3 position = new Vector3(player.x, 0,  player.z);
        Quaternion rotation = Quaternion.identity;
        
        Snake snake = Instantiate(_snakePrefab, position, rotation);
        snake.Init(player.d, player.clr);

        PlayerAim aim = Instantiate(_playerAim, position, rotation);
        aim.Init(snake.Speed);
        
        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(aim, player, snake);
    }
#endregion
    
#region Enemy
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    private void CreateEnemy(string key, Player player) {
        Vector3 position = new Vector3(player.x, 0,  player.z);
        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d, player.clr);
        
        EnemyController enemy = snake.gameObject.AddComponent<EnemyController>();
        enemy.Init(player, snake);
        
        _enemies.Add(key, enemy);
    }
    private void RemoveEnemy(string key, Player value) {
        if (_enemies.ContainsKey(key) == false) {
            Debug.LogError("Попытка уничтожения енеми, которого не было в словаре");
            return;
        }

        EnemyController enemy = _enemies[key];
        _enemies.Remove(key);
        enemy.Destroy();
    }
#endregion
}