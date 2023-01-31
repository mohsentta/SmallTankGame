using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float Damage;
    public float LifeSpan;
    [Range(0, 1)]
    public float EnergyCost;
    private float _lifeTimeLeft;


    public void Initialize(ColorContainer.Color color)
    {
        switch (color)
        {
            case ColorContainer.Color.blue:
                this.GetComponent<Renderer>().material.color = Color.blue;
                this.gameObject.layer = LayerMask.NameToLayer("Blue");
                break;
            case ColorContainer.Color.red:
                this.GetComponent<Renderer>().material.color = Color.red;
                this.gameObject.layer = LayerMask.NameToLayer("Red");
                break;
            default:
                throw new System.ArgumentException("Color is not defined. try adding the color into the ColorContainer.cs");

        }

        _lifeTimeLeft = LifeSpan;
    }


    void Update()
    {
        _lifeTimeLeft -= Time.deltaTime;
        if (_lifeTimeLeft <= 0)
        {
            GameControl.GameControlInstance.ResetCombo();
            PoolManager.Instance.StoreBullet(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<EnemyTank>().TakeDamage(Damage);
            PoolManager.Instance.StoreBullet(gameObject);
        }
    }
}
