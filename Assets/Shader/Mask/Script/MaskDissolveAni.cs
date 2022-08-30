using UnityEngine;
using System.Collections;

public class MaskDissolveAni : MonoBehaviour
{
    public float LifeTime = 2.0f;
    public float Speed = 1.0f;
    private Renderer _renderer = null;
    private Renderer Renderer
    { 
        get
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();
            return _renderer;
        }
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(animRoutine());
    }

    IEnumerator animRoutine()
    {
        yield return new WaitForSeconds(LifeTime);

        float dissolvePower = 0.65f;
        bool finished = false;
        while (finished == false)
        {
            dissolvePower -= Time.deltaTime * Speed;
            Renderer.material.SetFloat("_DissolvePower", dissolvePower);

            if (dissolvePower <= 0.2f)
                finished = true;

            yield return null;
        }
    }
}
