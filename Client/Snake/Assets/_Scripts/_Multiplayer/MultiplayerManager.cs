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
        _room = await client.JoinOrCreate<State>(GameRoomName);
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
#endregion
    
#region Player
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    private void CreatePlayer(Player player) {
        Snake snake = Instantiate(_snakePrefab);
        snake.Init(player.d);
        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(snake);
    }
#endregion
    
#region Enemy
    private void CreateEnemy(string key, Player player) {
    }
    private void RemoveEnemy(string key, Player value) {
    }
#endregion
}