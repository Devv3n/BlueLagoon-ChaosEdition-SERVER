namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public enum NetworkType : byte {
        SendMap = 210,
        HexUpate = 211,
        ClearMap = 212,
        EndGame = 213,
        
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
}
