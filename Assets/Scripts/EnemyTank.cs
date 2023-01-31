using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public float BaseHp = 1;
    [Range(0, 1)]
    public float EnergyReward;
    public float ScoreReward;


    public GameObject HealthyTank;
    public GameObject BustedTank;
    public GameObject TankHead;
    private float _currentHp;
    private float _maxHp;
    private Animator _anim;

    void Awake()
    {
        _anim = this.GetComponent<Animator>();
    }

    // Use this for initialization
    public void Initialize(ColorContainer.Color color)
    {

        _anim.SetTrigger("Reset");
        switch (color)
        {
            case ColorContainer.Color.blue:
                TankHead.GetComponent<Renderer>().material.color = Color.blue;
                this.gameObject.layer = LayerMask.NameToLayer("Blue");
                break;
            case ColorContainer.Color.red:
                TankHead.GetComponent<Renderer>().material.color = Color.red;
                this.gameObject.layer = LayerMask.NameToLayer("Red");
                break;
            default:
                throw new System.ArgumentException("Color is not defined. try adding the color into the ColorContainer.cs");

        }
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        if (_currentHp < 0)
        {
            _currentHp = 0;
            Explode();
        }
    }

    public void Explode()
    {

        _anim.SetTrigger("Explode");
        GameControl.GameControlInstance.ChangeEnergy(+EnergyReward);
        GameControl.GameControlInstance.AddScore(ScoreReward);
        GameControl.GameControlInstance.EnemiesLeft--;
        gameObject.layer = LayerMask.NameToLayer("Destroyed");
        StartCoroutine(StoreAfter(_anim.GetCurrentAnimatorStateInfo(0).length));

    }


    public IEnumerator StoreAfter(float time)
    {
        yield return new WaitForSeconds(time);

        _anim.SetTrigger("Reset");
        PoolManager.Instance.StoreTank(this.gameObject);


    }
}
