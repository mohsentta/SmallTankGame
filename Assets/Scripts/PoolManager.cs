using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject Bullet;
    public int BulletAmountToPool;

    public GameObject Tank;
    public int TankAmountToPool;


    [HideInInspector]
    public static PoolManager Instance;

    [HideInInspector]
    public Queue<GameObject> PooledBullets = new Queue<GameObject>();

    [HideInInspector]
    public Queue<GameObject> PooledTanks = new Queue<GameObject>();


    // Use this for initialization
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < BulletAmountToPool; i++)
        {
            GameObject newBullet = GameObject.Instantiate(Bullet, transform.position, Quaternion.identity);
            newBullet.SetActive(false);
            PooledBullets.Enqueue(newBullet);

        }
        for (int i = 0; i < TankAmountToPool; i++)
        {
            GameObject newTank = GameObject.Instantiate(Tank, transform.position, Quaternion.identity);
            newTank.SetActive(false);
            PooledTanks.Enqueue(newTank);

        }
    }

    public GameObject GetABullet()
    {
        GameObject bullet = PooledBullets.Dequeue();
        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.SetActive(true);
        return bullet;
    }

    public GameObject GetATank()
    {
        GameObject tank = PooledTanks.Dequeue();
        tank.GetComponent<Rigidbody>().velocity = Vector3.zero;
        tank.SetActive(true);
        return tank;
    }

    public void StoreTank(GameObject tank)
    {
        tank.gameObject.SetActive(false);
        tank.transform.position = transform.position;
        PooledTanks.Enqueue(tank);
    }



    public void StoreBullet(GameObject bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.position = transform.position;
        PooledBullets.Enqueue(bullet);
    }

}
