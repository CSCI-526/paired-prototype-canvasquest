// using UnityEngine;
// using UnityEngine.Events;

// [RequireComponent(typeof(Collider2D))]
// [RequireComponent(typeof(SpriteRenderer))]
// public class GateDoor : MonoBehaviour
// {
//     [Tooltip("If empty, all SumPlaceholders in the scene will be used.")]
//     public SumPlaceholder[] targets;

//     [Tooltip("Open only when ALL targets are satisfied. If OFF, open when ANY is satisfied.")]
//     public bool openOnAll = true;

//     [Header("Colors")]
//     public Color lockedColor = new Color(0.56f, 0.23f, 0.18f, 1f); // brown-red
//     public Color openColor   = new Color(0.25f, 0.85f, 0.45f, 1f); // green

//     [Header("Events (optional)")]
//     public UnityEvent OnOpen;
//     public UnityEvent OnClose;

//     bool isOpen;
//     Collider2D col;
//     SpriteRenderer sr;

//     void Awake()
//     {
//         col = GetComponent<Collider2D>();
//         sr  = GetComponent<SpriteRenderer>();

//         // auto-find all placeholders if none assigned
//         if (targets == null || targets.Length == 0)
//             targets = FindObjectsOfType<SumPlaceholder>();

//         ApplyVisuals(); // start locked by default
//     }

//     void Update()
//     {
//         bool shouldOpen = openOnAll ? AllSatisfied() : AnySatisfied();
//         if (shouldOpen != isOpen)
//         {
//             isOpen = shouldOpen;
//             ApplyVisuals();
//             if (isOpen) OnOpen?.Invoke(); else OnClose?.Invoke();
//         }
//     }

//     bool AllSatisfied()
//     {
//         if (targets == null || targets.Length == 0) return false;
//         for (int i = 0; i < targets.Length; i++)
//             if (!(targets[i] && targets[i].IsSatisfied)) return false;
//         return true;
//     }

//     bool AnySatisfied()
//     {
//         if (targets == null || targets.Length == 0) return false;
//         for (int i = 0; i < targets.Length; i++)
//             if (targets[i] && targets[i].IsSatisfied) return true;
//         return false;
//     }

//     void ApplyVisuals()
//     {
//         if (col) col.enabled = !isOpen;                // block when locked
//         if (sr)  sr.color    = isOpen ? openColor : lockedColor;
//     }
// }


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class GateDoor : MonoBehaviour
{
    [Tooltip("If empty, all SumPlaceholders in the scene will be used.")]
    public SumPlaceholder[] targets;

    [Tooltip("Open only when ALL targets are satisfied. If OFF, open when ANY is satisfied.")]
    public bool openOnAll = true;

    [Header("Colors")]
    public Color lockedColor = new Color(0.56f, 0.23f, 0.18f, 1f); // brown-red
    public Color openColor   = new Color(0.25f, 0.85f, 0.45f, 1f); // green

    [Header("Navigation")]
    [Tooltip("Optional explicit scene name. If empty, loads the next scene in Build Settings.")]
    public string nextSceneName = "";
    [Tooltip("Delay before loading (for whoosh/fade).")]
    public float loadDelay = 0f;
    [Tooltip("Tag used to recognize the player entering the open gate.")]
    public string playerTag = "Player";

    [Header("Events (optional)")]
    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnEnter; // fired when player steps into an open gate

    bool isOpen;
    Collider2D col;
    SpriteRenderer sr;
    bool loading; // guard against double-trigger

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr  = GetComponent<SpriteRenderer>();

        // auto-find all placeholders if none assigned
        if (targets == null || targets.Length == 0)
            targets = FindObjectsOfType<SumPlaceholder>();

        // ensure collider is enabled; we'll toggle trigger mode instead
        if (col) col.enabled = true;

        ApplyVisuals(); // start locked by default
    }

    void Update()
    {
        bool shouldOpen = openOnAll ? AllSatisfied() : AnySatisfied();
        if (shouldOpen != isOpen)
        {
            isOpen = shouldOpen;
            ApplyVisuals();
            if (isOpen) OnOpen?.Invoke(); else OnClose?.Invoke();
        }
    }

    bool AllSatisfied()
    {
        if (targets == null || targets.Length == 0) return false;
        for (int i = 0; i < targets.Length; i++)
            if (!(targets[i] && targets[i].IsSatisfied)) return false;
        return true;
    }

    bool AnySatisfied()
    {
        if (targets == null || targets.Length == 0) return false;
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] && targets[i].IsSatisfied) return true;
        return false;
    }

    void ApplyVisuals()
    {
        if (!col || !sr) return;

        // When locked: solid wall. When open: trigger zone to detect entry.
        col.isTrigger = isOpen;
        sr.color      = isOpen ? openColor : lockedColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen || loading) return;

        // If the child collider enters, this finds the Player root via attached Rigidbody
        var rb = other.attachedRigidbody;
        var root = rb ? rb.gameObject : other.gameObject;

        if (!root.CompareTag(playerTag)) return;

        loading = true;
        OnEnter?.Invoke();
        if (loadDelay > 0f) Invoke(nameof(LoadNextScene), loadDelay);
        else LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextIndex);
            else
                Debug.LogWarning("GateDoor: No next scene in Build Settings.");
        }
    }
}
