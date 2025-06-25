using UnityEngine;
using UnityEngine.UI;

public class PromptNoteFade : MonoBehaviour
{
    public float lifespan = 3f; // how long the prompt is active
    private float timer = 0f;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        timer = 0f;
        SetAlpha(0f); // start transparent
    }

    void Update()
    {
        timer += Time.deltaTime;

        float alpha = 1f;

        if (timer < 0.3f) // quick fade in
        {
            alpha = Mathf.Lerp(0f, 1f, timer / 0.3f);
        }
        else if (timer > lifespan - 0.5f) // fade out at end
        {
            alpha = Mathf.Lerp(1f, 0f, (timer - (lifespan - 0.5f)) / 0.5f);
        }

        SetAlpha(alpha);

        if (timer >= lifespan)
        {
            Destroy(gameObject); // clean up prompt when done
        }
    }

    void SetAlpha(float a)
    {
        if (image != null)
        {
            Color c = image.color;
            c.a = a;
            image.color = c;
        }
    }
}

