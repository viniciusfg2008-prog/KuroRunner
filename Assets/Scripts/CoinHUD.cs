using TMPro;
using UnityEngine;

/// <summary>
/// Mostra as moedas na tela e zera o contador da fase ao entrar nela.
/// "levelText": moedas da fase; "totalText": moedas totais (útil na Loja). Qualquer um pode ficar vazio.
/// </summary>
public class CoinHUD : MonoBehaviour
{
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text totalText;
    [SerializeField] string levelPrefix = "Moedas: ";
    [SerializeField] bool resetLevelOnStart = true;

    void Start()
    {
        if (resetLevelOnStart && CoinManager.Instance != null)
            CoinManager.Instance.StartLevel();
    }

    void OnEnable()
    {
        if (CoinManager.Instance == null) return;
        CoinManager.Instance.OnCoinsChanged += Refresh;
        Refresh(CoinManager.Instance.LevelCoins, CoinManager.Instance.TotalCoins);
    }

    void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged -= Refresh;
    }

    void Refresh(int level, int total)
    {
        if (levelText != null) levelText.text = levelPrefix + level;
        if (totalText != null) totalText.text = total.ToString();
    }
}
