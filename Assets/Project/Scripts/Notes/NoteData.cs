using UnityEngine;

/// <summary>
/// Dados de uma nota/bilhete do jogo. Serve tanto para:
/// 1) Texto simples (campo "content") � exibir o conte�do do bilhete;
/// 2) Di�logo em m�ltiplas linhas (campo "lines") � com tipo "Speech" ou "Thought"
///    para aplicar estilos diferentes no DialogueUI.
/// 3) (Opcional) Uma �nica linha "ap�s a leitura" (pensamento final do personagem).
/// 4) (NOVO) Visual espec�fico por nota: fundo 2D (sprite/cor) e um prefab 3D opcional.
/// </summary>
[CreateAssetMenu(menuName = "Game/Note Data", fileName = "NewNote", order = 0)]
public class NoteData : ScriptableObject
{
    [Header("Identifica��o (opcional)")]
    [Tooltip("Um ID �nico para a nota (�til para sistemas de save).")]
    public string noteId;

    [Header("Exibi��o (t�tulo e texto corrido)")]
    [Tooltip("T�tulo da nota que aparece em listas/di�rio (ex: 'Bilhete da Diretoria').")]
    public string title;

    [Tooltip("Conte�do completo da nota (modo texto corrido).")]
    [TextArea(5, 20)]
    public string content;

    // ========= VISUAL ESPEC�FICO DA NOTA (NOVO) =========

    [Header("Visual 2D da Nota")]
    [Tooltip("Sprite de fundo da nota (papel rasgado, recorte de jornal, foto etc.). Se estiver vazio, ser� usado o sprite padr�o do NoteViewerUI.")]
    public Sprite backgroundSprite;

    [Tooltip("Cor aplicada sobre o fundo da nota. Se deixar em branco (0,0,0,0), o NoteViewerUI usar� a cor padr�o.")]
    public Color backgroundColor = Color.clear;

    [Header("Visual 3D (opcional)")]
    [Tooltip("Prefab 3D ou VFX espec�fico desta nota (ex.: livro na mesa, crucifixo flutuando). Opcional.")]
    public GameObject backgroundPrefab3D;

    [Tooltip("Textura que ser� exibida na nota 3D do NoteWorld ao pegar esta nota. " +
             "Arraste aqui a imagem/foto da nota escaneada.")]
    public Texture2D noteTexture3D;

    // ========= DI�LOGO EM LINHAS =========

    public enum LineKind { Speech, Thought }

    [System.Serializable]
    public struct DialogueLine
    {
        [TextArea(2, 4)] public string text;
        public LineKind kind; // Speech = fala, Thought = pensamento
    }

    [Header("Di�logo em linhas (opcional)")]
    [Tooltip("Se quiser exibir o bilhete como falas/pensamentos em m�ltiplas linhas, use este array.")]
    public DialogueLine[] lines;

    [Header("Estilo de fala/pensamento (para DialogueUI)")]
    public Color speechColor = Color.white;
    public Color thoughtColor = new Color(1f, 1f, 1f, 0.9f);

    [Tooltip("Se true, linhas do tipo Thought s�o exibidas em it�lico.")]
    public bool thoughtItalic = true;

    // ========= P�S-LEITURA (PENSAMENTO FINAL) =========

    [Header("Ap�s a leitura (opcional)")]
    [Tooltip("Se verdadeiro, ap�s terminar as 'lines' ser� exibida esta linha final (pensamento/fala).")]
    public bool hasAfterReadingLine = false;

    [Tooltip("Linha �nica exibida ap�s a leitura (pensamento ou fala final do personagem).")]
    public DialogueLine afterReadingLine;
}