using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField] protected GameObject _explosion;
    [SerializeField] protected AudioSource _explosionSound;

    protected virtual void PlayExplosion()
    {
        _explosion = null;
        _explosionSound = null;

        _explosion = PoolManager.Instance.RequestExplosion(this.gameObject);

        if (_explosion == null)
            Debug.LogError(this.gameObject.name + " _explosion is NULL.");

        _explosion.transform.position = this.transform.position;

        _explosionSound = _explosion.GetComponent<AudioSource>();
        if (_explosionSound == null)
            Debug.LogError(this.gameObject.name + " _explosionSound is NULL.");

        _explosion.SetActive(true); //Turn explosion visual effects on
        _explosionSound.Play();
    }
}
