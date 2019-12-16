using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(CleanUpExplosionRoutine());
    }

    private IEnumerator CleanUpExplosionRoutine()
    {
        yield return new WaitForSeconds(4.90f);
        PoolManager.Instance.ResetExplosionPosition(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
