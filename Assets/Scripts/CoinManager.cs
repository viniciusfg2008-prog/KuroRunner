using System;
using UnityEngine;

/// <summary>
/// Controla as moedas coletadas na fase.
/// - LevelCoins: moedas pegas na fase atual (zera ao entrar na fase).
/// - TotalCoins: moedas guardadas (GameData.Moedas), as mesmas que a Loja usa para comprar skins.
/// Cada moeda coletada é salva na hora, então persiste ao sair da fase ou fechar o jogo.
/// Cria-se sozinho ao iniciar o jogo; não precisa colocar em cena.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public int LevelCoins { get; private set; }
    public int TotalCoins => GameData.Moedas;

    /// <summary>(moedasDaFase, moedasTotais)</summary>
    public event Action<int, int> OnCoinsChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("CoinManager");
        go.AddComponent<CoinManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Zera o contador da fase (chamado pelo CoinLevelStart ao entrar na fase).</summary>
    public void StartLevel()
    {
        LevelCoins = 0;
        Notify();
    }

    /// <summary>Chamado pela moeda ao ser coletada. Grava em GameData na hora.</summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        LevelCoins += amount;
        GameData.AdicionarMoedas(amount);
        Notify();
    }

    void Notify() => OnCoinsChanged?.Invoke(LevelCoins, TotalCoins);

    void OnApplicationPause(bool paused) { if (paused) GameData.Salvar(); }
    void OnApplicationQuit() => GameData.Salvar();
}
