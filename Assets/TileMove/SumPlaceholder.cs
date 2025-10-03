// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;

// [RequireComponent(typeof(BoxCollider2D))]
// [RequireComponent(typeof(SpriteRenderer))]
// public class SumPlaceholder : MonoBehaviour
// {
//     [Header("Rule")]
//     [Min(0)] public int targetSum = 5;
//     [Tooltip("If ON and count > 0, requires exactly this many boxes be ON the placeholder.")]
//     public bool requireExactCount = true;
//     [Min(0)] public int requiredBoxCount = 0;

//     [Header("Detection")]
//     [Tooltip("Inset inside the trigger bounds that a box's CENTER must be within (world units). 0.45–0.5 works for 1×1 tiles.")]
//     [Range(0f, 0.5f)] public float innerMargin = 0.48f;

//     public enum LabelMode { None, TargetOnly, CurrentOverTarget }

//     [Header("UI (optional)")]
//     public LabelMode labelMode = LabelMode.TargetOnly;
//     public TextMeshPro targetLabel;
//     public Color unsatisfiedTint = new Color(1f, 0.4f, 0.4f, 0.25f);
//     public Color satisfiedTint   = new Color(0.3f, 1f, 0.4f, 0.35f);

//     public int CurrentSum { get; private set; }
//     public bool IsSatisfied { get; private set; }

//     private readonly HashSet<BoxNumber> _inside = new();
//     private SpriteRenderer _sr;
//     private BoxCollider2D _col;

//     void Awake()
//     {
//         _sr  = GetComponent<SpriteRenderer>();
//         _col = GetComponent<BoxCollider2D>();
//         if (!targetLabel) targetLabel = GetComponentInChildren<TextMeshPro>();
//         _col.isTrigger = true;
//         RefreshVisual();
//         UpdateLabel();
//     }

//     void OnTriggerEnter2D(Collider2D other)
//     {
//         var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
//         if (bn != null) { _inside.Add(bn); Recalc(); }
//     }

//     void OnTriggerExit2D(Collider2D other)
//     {
//         var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
//         if (bn != null && _inside.Remove(bn)) Recalc();
//     }

//     void OnTriggerStay2D(Collider2D other) => Recalc();

//     bool IsBoxCenteredInside(BoxNumber b)
//     {
//         if (b == null) return false;
//         Bounds inner = _col.bounds;
//         inner.Expand(-2f * innerMargin);          // shrink bounds by margin on all sides
//         return inner.Contains(b.transform.position);
//     }

//     void Recalc()
//     {
//         _inside.RemoveWhere(b => b == null);

//         int countInside = 0;
//         int sum = 0;
//         foreach (var b in _inside)
//         {
//             if (IsBoxCenteredInside(b))
//             {
//                 countInside++;
//                 sum += b.Value;
//             }
//         }

//         bool countOK = !requireExactCount || requiredBoxCount == 0 || countInside == requiredBoxCount;
//         bool satisfied = (sum == targetSum) && countOK;

//         if (satisfied != IsSatisfied) IsSatisfied = satisfied;

//         CurrentSum = sum;
//         RefreshVisual();
//         UpdateLabel();
//     }

//     void RefreshVisual()
//     {
//         if (_sr) _sr.color = IsSatisfied ? satisfiedTint : unsatisfiedTint;
//     }

//     void UpdateLabel()
//     {
//         if (!targetLabel) return;

//         switch (labelMode)
//         {
//             case LabelMode.None:
//                 targetLabel.text = "";
//                 break;
//             case LabelMode.TargetOnly:
//                 targetLabel.text = targetSum.ToString();
//                 break;
//             case LabelMode.CurrentOverTarget:
//                 targetLabel.text = $"{CurrentSum}/{targetSum}";
//                 break;
//         }
//     }
// }

// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;

// [RequireComponent(typeof(BoxCollider2D))]
// [RequireComponent(typeof(SpriteRenderer))]
// public class SumPlaceholder : MonoBehaviour
// {
//     [Header("Rule")]
//     [Min(0)] public int targetSum = 5;
//     public bool requireExactCount = true;     // require exactly 'requiredBoxCount'?
//     [Min(0)] public int requiredBoxCount = 2; // set 0 to ignore count

//     [Header("UI (optional)")]
//     public TextMeshPro targetLabel;           // assign child label if you want text
//     public bool showLiveSum = true;           // show "current/target" during play

//     public int CurrentSum { get; private set; }
//     public int CurrentCount => _inside.Count;
//     public bool IsSatisfied { get; private set; }

//     readonly HashSet<BoxNumber> _inside = new();
//     SpriteRenderer _sr;
//     BoxCollider2D _col;

//     void Awake()
//     {
//         _sr = GetComponent<SpriteRenderer>();
//         _col = GetComponent<BoxCollider2D>();
//         if (!targetLabel) targetLabel = GetComponentInChildren<TextMeshPro>();
//         _col.isTrigger = true;
//         RefreshVisual();
//     }

//     void OnTriggerEnter2D(Collider2D other)
//     {
//         var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
//         if (bn != null) { _inside.Add(bn); Recalc(); }
//     }

//     void OnTriggerExit2D(Collider2D other)
//     {
//         var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
//         if (bn != null && _inside.Remove(bn)) Recalc();
//     }

//     void OnTriggerStay2D(Collider2D other)
//     {
//         // Safety if a box's number changed while inside
//         Recalc();
//     }

//     void Recalc()
//     {
//         _inside.RemoveWhere(b => b == null);
//         CurrentSum = _inside.Sum(b => b.Value);

//         bool countOK = !requireExactCount || requiredBoxCount == 0 || CurrentCount == requiredBoxCount;
//         bool satisfied = (CurrentSum == targetSum) && countOK;

//         if (satisfied != IsSatisfied)
//             IsSatisfied = satisfied;

//         RefreshVisual();
//     }

//     void RefreshVisual()
//     {
//         if (_sr)
//         {
//             // translucent red → not satisfied; green → satisfied
//             _sr.color = IsSatisfied ? new Color(0.3f, 1f, 0.4f, 0.35f)
//                                     : new Color(1f, 0.4f, 0.4f, 0.25f);
//         }
//         if (targetLabel)
//             targetLabel.text = showLiveSum ? $"{CurrentSum}/{targetSum}" : targetSum.ToString();
//     }
// }



using System.Collections.Generic;
using TMPro;                 // supports TextMeshPro and TextMeshProUGUI via TMP_Text
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SumPlaceholder : MonoBehaviour
{
    // ---------- Rules ----------
    [Header("Rule")]
    [Min(0)] public int targetSum = 5;
    [Tooltip("If ON and count > 0, requires exactly this many boxes be on the placeholder.")]
    public bool requireExactCount = true;
    [Min(0)] public int requiredBoxCount = 0;

    // ---------- Detection ----------
    [Header("Detection")]
    [Tooltip("Desired inset (world units). Clamped per box so 1×1 areas auto use 0.")]
    [Range(0f, 0.2f)] public float fullyInsideMargin = 0.03f;

    // ---------- Visuals / UI (optional) ----------
    [Header("UI (optional)")]
    public TMP_Text targetLabel;        // assign a TMP label on the placeholder (or leave empty for no text)
    public bool showLiveSum = true;     // true: show current/target, false: show only target
    public Color unsatisfiedTint = new Color(1f, 0.4f, 0.4f, 0.25f);
    public Color satisfiedTint   = new Color(0.3f, 1f, 0.4f, 0.35f);

    // ---------- Live state ----------
    public int CurrentSum   { get; private set; }
    public int CurrentCount { get; private set; }
    public bool IsSatisfied { get; private set; }

    // ---------- Internals ----------
    private readonly HashSet<BoxNumber> overlapping = new();
    private SpriteRenderer sr;
    private BoxCollider2D triggerCol;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;

        if (!targetLabel) targetLabel = GetComponentInChildren<TMP_Text>();
        RefreshVisual();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
        if (bn != null) { overlapping.Add(bn); Recalc(); }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var bn = other.GetComponent<BoxNumber>() ?? other.GetComponentInParent<BoxNumber>();
        if (bn != null && overlapping.Remove(bn)) Recalc();
    }

    void OnTriggerStay2D(Collider2D other) => Recalc();

    // Count only if the box's ENTIRE collider fits inside the trigger
    // Margin is CLAMPED so a 1×1 box in a 1×1 placeholder uses 0 automatically.
    bool IsBoxFullyInside(BoxNumber b)
    {
        if (b == null) return false;

        var boxCol = b.GetComponent<Collider2D>() ?? b.GetComponentInChildren<Collider2D>();
        if (!boxCol) return false;

        Bounds tr = triggerCol.bounds;
        Bounds bx = boxCol.bounds;

        // Max margin that still allows the box to fit (per axis)
        float maxMx = 0.5f * (tr.size.x - bx.size.x);
        float maxMy = 0.5f * (tr.size.y - bx.size.y);
        float effective = Mathf.Clamp(fullyInsideMargin, 0f, Mathf.Max(0f, Mathf.Min(maxMx, maxMy) - 1e-4f));

        tr.Expand(-2f * effective);
        return tr.Contains(bx.min) && tr.Contains(bx.max);
    }

    void Recalc()
    {
        overlapping.RemoveWhere(b => b == null);

        int sum = 0, count = 0;
        foreach (var b in overlapping)
        {
            if (IsBoxFullyInside(b))
            {
                count++;
                sum += b.Value;
            }
        }

        CurrentSum = sum;
        CurrentCount = count;

        bool countOK = !requireExactCount || requiredBoxCount == 0 || CurrentCount == requiredBoxCount;
        bool satisfiedNow = (CurrentSum == targetSum) && countOK;

        if (satisfiedNow != IsSatisfied) IsSatisfied = satisfiedNow;

        RefreshVisual();
    }

    void RefreshVisual()
    {
        if (sr) sr.color = IsSatisfied ? satisfiedTint : unsatisfiedTint;

        if (targetLabel)
            targetLabel.text = showLiveSum ? $"{CurrentSum}/{targetSum}"
                                           : targetSum.ToString();
    }

#if UNITY_EDITOR
    // Visualize the effective "must fit" area in Scene view when selected
    void OnDrawGizmosSelected()
    {
        if (!triggerCol) triggerCol = GetComponent<BoxCollider2D>();
        if (!triggerCol) return;

        // Draw with the worst-case (largest) box size among current overlaps if available;
        // otherwise just show the raw trigger (good enough for placement).
        Bounds area = triggerCol.bounds;
        // We can't know future box sizes here, so show the raw trigger outline:
        Gizmos.color = IsSatisfied ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireCube(area.center, area.size);
    }
#endif
}

