using System.Net.Sockets;
using System.Text;

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public enum NetworkType : byte {
        SendMap = 210,
        HexUpate = 211,
        NaturalDisaster = 212,
        ClearMap = 218,
        EndGame = 219,
        
        PlayerJoin = 220,
        PlayerLeave = 221,
        PlayerTurn = 222,

        CounterUpdate = 230,

        IncrementStatistic = 240,
    }

    public enum StatisticsType : byte {
        SettlersPlaced,
        VillagesPlaced,

        ServersJoined,
        GamesPlayed,
    
        ExplorationPhasesWon,
        ExplorationPhasesLost,

        SettlementPhasesWon,
        SettlementPhasesLost,
        SettlementPhasesUnplayable,
    }

    public abstract class Player {
        #region Variables
        // Const reference
        public static int defaultSettlerCount;
        readonly static Random rng = new Random();

        // Client Configuration
        public bool alive = true;
        public string username;
        public Color color = Color.FromArgb(255, rng.Next(256), rng.Next(256), rng.Next(256));

        // Gameplay Variables
        public int[] resourceCount = new int[6];
        public int settlerCount = 0;
        public int villageCount = 0;
        public bool villagePlaced = false;

        // temp variables for end score calculating
        public int _score = 0;
        public int _islandSettlerCount = 0;
        public bool[] _uniqueIslands = new bool[8];
        public int _linkedIslands = 0;
        #endregion

        #region Client Life Handling
        public abstract bool IsAlive();
        public abstract void Shutdown();
        #endregion

        #region Data Handling Functions
        public abstract void SendData(NetworkType type, byte[] data);
        #endregion

        #region Gameplay Functions
        public abstract void SendCounterUpdate(int type);
        public abstract void SendStatistic(StatisticsType statisticType);

        // Resets gameplay variables making them ready for next round
        public void Reset(bool full) {
            resourceCount = new int[6];
            settlerCount = defaultSettlerCount;
            villageCount = 3;

            if (full)
                villagePlaced = true;
        }
        #endregion
    }
}
