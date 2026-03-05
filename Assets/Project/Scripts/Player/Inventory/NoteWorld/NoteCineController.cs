using UnityEngine;

/// <summary>
/// Controla a exibição da nota 3D no "NoteWorld" (câmera + RenderTexture).
/// Agora usa UMA ÚNICA NoteWorldView e troca a textura quando o jogador pega novas notas.
/// Também toca um som ao abrir a área de nota.
///
/// ════════════════════════════════════════════════════════════════
/// COMO CONFIGURAR — PASSO A PASSO
/// ════════════════════════════════════════════════════════════════
///
/// ── HIERARQUIA ──
/// Coloque este script em um GameObject vazio chamado "NoteCineController"
/// (pode ficar na raiz ou dentro de um "Managers" GameObject).
///
/// ── INSPECTOR ──
/// • Player Inventory → arraste o componente PlayerInventory do Player
/// • Note Camera      → câmera que renderiza o NoteWorld (Target Texture = RT_Nota)
/// • Note Panel       → painel da HUD com o RawImage da RT_Nota
/// • Note World View  → arraste o NoteWorldObject (que tem o script NoteWorldView)
/// • Note Sound       → arraste o AudioClip do som de selecionar/trocar nota
///                      (apenas 1 som, conforme solicitado)
///
/// ── SOBRE O SOM ──
/// O AudioSource é criado automaticamente neste GameObject.
/// Você só precisa arrastar o AudioClip no campo "Note Sound".
/// ════════════════════════════════════════════════════════════════
/// </summary>
public class NoteCineController : MonoBehaviour
{
    public static NoteCineController Instance { get; private set; }

    [Header("Referências principais")]
    [SerializeField] private PlayerInventory playerInventory;

    [Tooltip("Camera que renderiza a nota para a Render Texture (NoteRenderCamera).")]
    [SerializeField] private Camera noteCamera;

    [Header("UI")]
    [Tooltip("Painel de notas (NotePanel) que contém o RawImage com a RT_Nota.")]
    [SerializeField] private GameObject notePanel;

    [Header("Nota 3D (UMA ÚNICA)")]
    [Tooltip("O único NoteWorldView da cena — a nota 3D cuja textura muda conforme as notas coletadas.")]
    [SerializeField] private NoteWorldView noteWorldView;

    [Header("Som")]
    [Tooltip("Som reproduzido ao abrir/trocar nota (apenas 1 AudioClip).")]
    [SerializeField] private AudioClip noteSound;
    [Range(0f, 1f)]
    [SerializeField] private float noteSoundVolume = 1f;

    // ── Estado ──
    private bool isShowing = false;
    private AudioSource _audioSource;

    public bool IsShowing => isShowing;

    // ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[NoteCine] Já existe uma instância, destruindo duplicata.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Cria AudioSource automaticamente
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f; // som 2D (UI)

        ValidateReferences();

        if (notePanel != null)
            notePanel.SetActive(false);
    }

    private void Start()
    {
        // Fallback: busca PlayerInventory na cena
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogWarning("[NoteCine] PlayerInventory NÃO encontrado. Troca de textura desativada.");
            return;
        }

        // ── Assina o evento: quando uma nota é coletada, troca a textura ──
        playerInventory.OnNoteAdded += OnNoteCollected;
        Debug.Log("[NoteCine] Assinado evento OnNoteAdded do PlayerInventory.");
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnNoteAdded -= OnNoteCollected;
    }

    // ─────────────────── EVENTOS ────────────────────────────────────

    /// <summary>Chamado automaticamente quando o jogador pega uma nova nota.</summary>
    private void OnNoteCollected(NoteData newNote)
    {
        if (newNote == null) return;

        Debug.Log("[NoteCine] Nova nota coletada: " + newNote.title + ". Trocando textura da nota 3D.");

        if (noteWorldView != null)
            noteWorldView.ApplyNoteData(newNote);
        else
            Debug.LogWarning("[NoteCine] NoteWorldView não atribuída — textura não foi trocada.");
    }

    // ─────────────────── ABRIR / FECHAR ─────────────────────────────

    /// <summary>
    /// Abre a tela da nota (NotePanel com RT_Nota).
    /// Chamado pelo botão/slot de notas no inventário.
    /// </summary>
    public void ShowNoteArea()
    {
        if (isShowing)
            return;

        if (playerInventory == null || playerInventory.GetAllNotes().Count == 0)
        {
            Debug.Log("[NoteCine] Nenhuma nota no inventário. Tela não será aberta.");
            return;
        }

        if (noteCamera == null || notePanel == null)
        {
            Debug.LogWarning("[NoteCine] noteCamera ou notePanel não atribuídos.");
            return;
        }

        if (noteWorldView == null)
        {
            Debug.LogWarning("[NoteCine] NoteWorldView não atribuída.");
            return;
        }

        // Posiciona a câmera no ponto de visão da nota 3D
        Transform camPoint = noteWorldView.CameraPoint;
        noteCamera.transform.SetPositionAndRotation(camPoint.position, camPoint.rotation);
        noteCamera.enabled = true;

        notePanel.SetActive(true);
        isShowing = true;

        // Toca o som de nota
        PlayNoteSound();

        Debug.Log("[NoteCine] Área de nota ABERTA.");
    }

    private void CloseNoteArea()
    {
        if (!isShowing) return;

        isShowing = false;

        if (notePanel != null)
            notePanel.SetActive(false);

        if (noteCamera != null)
            noteCamera.enabled = false;

        Debug.Log("[NoteCine] Área de nota FECHADA.");
    }

    // ─────────────────── SOM ────────────────────────────────────────

    private void PlayNoteSound()
    {
        if (noteSound == null || _audioSource == null) return;
        _audioSource.PlayOneShot(noteSound, noteSoundVolume);
    }

    // ─────────────────── UPDATE ─────────────────────────────────────

    private void Update()
    {
        if (!isShowing) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
            CloseNoteArea();
    }

    // ─────────────────── UTILITÁRIOS ────────────────────────────────

    private void ValidateReferences()
    {
        if (noteCamera == null)
            Debug.LogWarning("[NoteCine] noteCamera NÃO atribuída no Inspector.");
        if (notePanel == null)
            Debug.LogWarning("[NoteCine] notePanel NÃO atribuído no Inspector.");
        if (noteWorldView == null)
            Debug.LogWarning("[NoteCine] noteWorldView NÃO atribuída no Inspector.");
    }
}
