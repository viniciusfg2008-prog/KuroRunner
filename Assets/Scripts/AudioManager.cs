using UnityEngine;

/// <summary>
/// Criado automaticamente ao iniciar o jogo (não precisa estar na cena).
/// Controla volume de música/efeitos e garante salvamento ao fechar/pausar.
/// Para ter música: coloque um AudioClip chamado "Musica" em Assets/Resources.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    AudioSource musica, sfx;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Criar()
    {
        if (Instance != null) return;
        new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musica = gameObject.AddComponent<AudioSource>();
        musica.loop = true;
        musica.playOnAwake = false;
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        var clip = Resources.Load<AudioClip>("Musica");
        if (clip != null) { musica.clip = clip; musica.Play(); }
        AplicarVolumes();
    }

    public void AplicarVolumes()
    {
        musica.volume = GameData.VolMusica;
        sfx.volume = GameData.VolSfx;
    }

    public static void TocarSfx(AudioClip clip)
    {
        if (Instance != null && clip != null) Instance.sfx.PlayOneShot(clip);
    }

    // Salvamento automático
    void OnApplicationPause(bool pausado) { if (pausado) GameData.Salvar(); }
    void OnApplicationQuit() { GameData.Salvar(); }
}
