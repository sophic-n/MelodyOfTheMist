using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptUI : MonoBehaviour
{
    public Transform followTarget;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    public GameObject noteImagePrefab;
    public Transform sequenceContainer;

    public Sprite noteA;
    public Sprite noteS;
    public Sprite noteD;
    public Sprite noteF;
    public Sprite noteG;
    public Sprite noteH;
    public Sprite noteJ;
    public Sprite noteK;
    public Sprite noteL;

    public CanvasGroup canvasGroup;
    public Color activeColor = Color.cyan;
    public Color inactiveColor = Color.white;

    private Coroutine pulseRoutine;

    private Dictionary<KeyCode, Sprite> spriteMap;
    private List<GameObject> noteObjects = new List<GameObject>();

    void Awake()
    {
        spriteMap = new Dictionary<KeyCode, Sprite>
        {
            { KeyCode.A, noteA },
            { KeyCode.S, noteS },
            { KeyCode.D, noteD },
            { KeyCode.F, noteF },
            { KeyCode.G, noteG },
            { KeyCode.H, noteH },
            { KeyCode.J, noteJ },
            { KeyCode.K, noteK },
            { KeyCode.L, noteL }
        };
    }

    void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offset;
        }
    }

    public void SetSequence(List<KeyCode> sequence)
    {
        if (sequenceContainer == null)
        {
            Debug.LogWarning("[PromptUI] Sequence container is missing!");
            return;
        }

        // Disable existing note objects instead of destroying immediately
        foreach (GameObject note in noteObjects)
        {
            if (note != null)
            {
                note.SetActive(false);
            }
        }
        noteObjects.Clear();

        // Create new note sprites
        foreach (KeyCode key in sequence)
        {
            GameObject noteGO = Instantiate(noteImagePrefab, sequenceContainer);
            Image image = noteGO.GetComponent<Image>();
            if (noteGO == null || image == null)
            {
                Debug.LogError("[PromptUI] Note prefab or image missing!");
                continue;
            }

            if (spriteMap.ContainsKey(key))
            {
                image.sprite = spriteMap[key];
            }

            noteObjects.Add(noteGO);
        }

        Debug.Log($"[PromptUI] New Sequence Set: {string.Join(", ", sequence)}");
    }


    public void RemoveFirstNote()
    {
        if (noteObjects.Count > 0)
        {
            GameObject firstNote = noteObjects[0];
            noteObjects.RemoveAt(0);

            if (firstNote != null)
            {
                firstNote.SetActive(false);
            }
        }
    }
    
    public void Highlight(bool isActive)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isActive ? 1f : 0.7f;
        }

        foreach (GameObject noteGO in noteObjects)
        {
            Image image = noteGO?.GetComponent<Image>();
            if (image != null)
                image.color = isActive ? activeColor : inactiveColor;
        }

        if (isActive)
        {
            if (pulseRoutine == null)
                pulseRoutine = StartCoroutine(Pulse());
        }
        else
        {
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }
        }
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * 2f, 0.3f) + 0.7f;
            if (canvasGroup != null)
                canvasGroup.alpha = pulse;
            yield return null;
        }
    }
}

