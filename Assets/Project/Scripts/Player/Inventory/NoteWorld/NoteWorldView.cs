using UnityEngine;

/// <summary>
/// Representa a ÚNICA nota 3D no "NoteWorld".
/// Troca a textura do objeto 3D sempre que o jogador pega uma nova nota.
///
/// ════════════════════════════════════════════════════════════════
/// COMO CONFIGURAR — PASSO A PASSO
/// ════════════════════════════════════════════════════════════════
///
/// ── NO MUNDO 3D (NoteWorld) ──
/// 1. Crie (ou já existe) um objeto 3D que represente a nota
///    (ex.: um Quad plano com um material de papel).
///    Renomeie para "NoteWorldObject".
/// 2. Coloque-o em uma área separada da cena (ex.: longe do cenário
///    principal, como uma "sala fantasma" fora do mapa), pois a
///    câmera do noteWorld vai olhar só para esse objeto.
/// 3. Certifique-se de que o objeto tem um componente MeshRenderer
///    com um Material que use o shader padrão (Standard ou Lit
///    do URP) — o script trocará a textura _MainTex dele.
/// 4. Arraste este script para "NoteWorldObject".
/// 5. No Inspector:
///    • Note Renderer → arraste o MeshRenderer do próprio objeto
///      (geralmente já preenchido automaticamente).
///    • Camera Point  → crie um GameObject filho vazio chamado
///      "CameraPoint", posicione-o na frente da nota, apontando
///      para ela, e arraste-o aqui.
///    • Default Texture → arraste uma textura padrão (ex.: papel em branco)
///      para exibir antes de qualquer nota ser coletada.
///
/// ── EM CADA NoteData (ScriptableObject) ──
/// 6. Abra cada NoteData no Project.
/// 7. No campo "Note Texture 3D", arraste a imagem/foto da nota
///    digitalizada. Essa textura aparecerá na nota 3D quando o
///    jogador pegar este bilhete.
/// ════════════════════════════════════════════════════════════════
/// </summary>
public class NoteWorldView : MonoBehaviour
{
    [Header("Renderer da Nota 3D")]
    [Tooltip("MeshRenderer do objeto 3D da nota. Se vazio, tenta GetComponent<Renderer>() automaticamente.")]
    [SerializeField] private Renderer noteRenderer;

    [Header("Ponto de câmera")]
    [Tooltip("Transform com a posição/rotação ideais para a câmera ver esta nota. Se vazio, usa o próprio transform.")]
    [SerializeField] private Transform cameraPoint;

    [Header("Textura padrão")]
    [Tooltip("Textura exibida antes de qualquer nota ser coletada (ex.: papel em branco).")]
    [SerializeField] private Texture2D defaultTexture;

    // Propriedade do shader da textura principal
    private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");

    private MaterialPropertyBlock _block;

    public Transform CameraPoint => cameraPoint != null ? cameraPoint : transform;

    private void Awake()
    {
        if (noteRenderer == null)
            noteRenderer = GetComponent<Renderer>();

        _block = new MaterialPropertyBlock();

        // Aplica textura padrão ao iniciar
        if (defaultTexture != null)
            ApplyTexture(defaultTexture);
    }

    /// <summary>
    /// Troca a textura da nota 3D para a textura definida no NoteData.
    /// Chamado pelo NoteCineController quando o jogador pega uma nova nota.
    /// </summary>
    public void ApplyNoteData(NoteData note)
    {
        if (note == null)
        {
            Debug.LogWarning("[NoteWorldView] ApplyNoteData recebeu nota nula.");
            return;
        }

        if (note.noteTexture3D != null)
        {
            ApplyTexture(note.noteTexture3D);
            Debug.Log("[NoteWorldView] Textura trocada para nota: " + note.title);
        }
        else if (defaultTexture != null)
        {
            ApplyTexture(defaultTexture);
            Debug.LogWarning("[NoteWorldView] Nota '" + note.title + "' não tem noteTexture3D. Usando textura padrão.");
        }
        else
        {
            Debug.LogWarning("[NoteWorldView] Nota '" + note.title + "' não tem noteTexture3D e não há textura padrão.");
        }
    }

    private void ApplyTexture(Texture2D tex)
    {
        if (noteRenderer == null)
        {
            Debug.LogError("[NoteWorldView] noteRenderer não está atribuído!", this);
            return;
        }

        noteRenderer.GetPropertyBlock(_block);
        _block.SetTexture(MainTexProp, tex);
        noteRenderer.SetPropertyBlock(_block);
    }

    private void OnDrawGizmos()
    {
        if (CameraPoint == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(CameraPoint.position, 0.1f);
        Gizmos.DrawLine(CameraPoint.position, CameraPoint.position + CameraPoint.forward * 0.5f);
    }
}
