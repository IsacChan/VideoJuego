using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtaqueRaicesCircular : MonoBehaviour
{
    [Header("Prefab de raíces")]
    public GameObject prefabRaices;

    [Header("Distribución")]
    public int cantidadRaices = 8;
    public float radio = 1.5f;

    [Header("Animación")]
    public float duracion = 1f;
    public float retrasoEntreRaices = 0.05f;

    private List<GameObject> raicesActivas = new List<GameObject>();
    private Coroutine ataqueActual;

    public void EjecutarAtaque()
    {
        if (ataqueActual != null)
            return;

        ataqueActual = StartCoroutine(GenerarRaices());
    }

    private IEnumerator GenerarRaices()
    {
        for (int i = 0; i < cantidadRaices; i++)
        {
            float angulo = i * Mathf.PI * 2f / cantidadRaices;

            Vector3 posicion = new Vector3(
                Mathf.Cos(angulo) * radio,
                Mathf.Sin(angulo) * radio,
                0f
            );

            GameObject raiz = Instantiate(
                prefabRaices,
                transform.position + posicion,
                Quaternion.identity,
                transform
            );

            raiz.SetActive(true);

            SpriteRenderer sprite = raiz.GetComponent<SpriteRenderer>();

            if (sprite != null)
                sprite.enabled = true;

            Animator anim = raiz.GetComponent<Animator>();

            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }

            raicesActivas.Add(raiz);

            yield return new WaitForSeconds(retrasoEntreRaices);
        }

        yield return new WaitForSeconds(duracion);

        DetenerRaices();

        ataqueActual = null;
    }

    public void DetenerRaices()
    {
        foreach (GameObject raiz in raicesActivas)
        {
            if (raiz != null)
                Destroy(raiz);
        }

        raicesActivas.Clear();
    }

    public void CancelarAtaque()
    {
        if (ataqueActual != null)
        {
            StopCoroutine(ataqueActual);
            ataqueActual = null;
        }

        DetenerRaices();
    }
}