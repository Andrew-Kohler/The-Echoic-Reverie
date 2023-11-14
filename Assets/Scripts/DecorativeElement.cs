using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeElement : MonoBehaviour
{
    SpriteRenderer sr;
    BoxCollider2D trigger;
    SoundScaler soundScaler;
    //[SerializeField] Color regularColor;

    [SerializeField] protected Vector3 m_from = new Vector3(0.0F, 0.0F, 10.0F);
    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, 0.0F, -10.0F);
    [SerializeField] protected float m_frequency = 4F;

    [SerializeField] protected float rateOfFade = .7f;

    bool activeCoroutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        trigger = GetComponent<BoxCollider2D>();
        soundScaler = GetComponent<SoundScaler>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        activeCoroutine = false;
    }

    private void Update()
    {
        /*Quaternion from = Quaternion.Euler(this.m_from);
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);*/
    }

    private void OnParticleTrigger()
    {
        Debug.Log("hi");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activeCoroutine)
        {
            StopAllCoroutines();
            soundScaler.PlaySound();
            StartCoroutine(DoFadeEffect());
            StartCoroutine(DoShakeEffect());
            Debug.Log("??");
        }
        
    }

    private void OnParticleCollision(GameObject other)
    {

        if (other.CompareTag("External Particles") && !activeCoroutine) // !activeCoroutine && 
        {
            Debug.Log("??");
/*            if (!activeCoroutine)
            {*/
                StopAllCoroutines();
                soundScaler.PlaySound();
        
                StartCoroutine(DoFadeEffect());
                StartCoroutine(DoShakeEffect());
            //}
        }
    }

    

    IEnumerator DoFadeEffect()
    {
        //sr.color = regularColor;
        activeCoroutine = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        while (sr.color.a > 0)
        {
            float alpha = sr.color.a - rateOfFade * Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }
        activeCoroutine = false;
    }

    IEnumerator DoShakeEffect()
    {
        Vector3 n_from = m_from;
        Vector3 n_to = m_to;
        Quaternion from = Quaternion.Euler(n_from);
        Quaternion to = Quaternion.Euler(n_to);
        float increment = 0;

        while (n_from.z > 0 && n_to.z < 0)
        {
            from = Quaternion.Euler(n_from);
            to = Quaternion.Euler(n_to);
            float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
            this.transform.localRotation = Quaternion.Lerp(from, to, lerp);

            n_from = new Vector3(n_from.x, n_from.y, n_from.z - 7f*Time.deltaTime);
            n_to = new Vector3(n_to.x, n_to.y, n_to.z + 7f*Time.deltaTime);
            yield return null;
        }
  
    }
}
