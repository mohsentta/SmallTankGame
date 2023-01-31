using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class TankController : MonoBehaviour
{
    private Camera _mainCamera;

    public LayerMask WhatIsGround;

    public Transform BulletSpawnPoint;

    public float BulletForce;

    private Animator _anim;

    public float rotationSpeed;

    private Vector3 targetDirection;

    private bool useController;

    // Use this for initialization
    void Start()
    {
        useController = false;
        _anim = this.GetComponent<Animator>();
        _mainCamera = Camera.main;
        targetDirection = transform.position + Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameControl.GameControlInstance.IsGameEnded)
        {
            return;
        }

        RotateTank();

        CheckShootInput();

        ResetOrExit();
    }

    private void RotateTank()
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition + new Vector3(0, -Screen.height / 40f, 0));

        CheckController();

        if (useController)
        {
            transform.LookAt(transform.position + targetDirection, Vector3.up);
        }
        else if (Physics.Raycast(ray, out hit, 100f, WhatIsGround))
        {
            transform.LookAt(hit.point);
        }
    }

    private void CheckController()
    {
        float horizontal = Input.GetAxis("JoystickHorizontal");

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            useController = false;
        }
        else if (horizontal != 0)
        {
            useController = true;
            targetDirection = Quaternion.AngleAxis(horizontal * rotationSpeed * Time.deltaTime, Vector3.up) * targetDirection;
        }
    }

    private void CheckShootInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var bullet = PoolManager.Instance.GetABullet().GetComponent<Bullet>();

            GameControl.GameControlInstance.ChangeEnergy(-bullet.EnergyCost);

            bullet.Initialize(ColorContainer.Color.red);

            ShootBullet(bullet);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            var bullet = PoolManager.Instance.GetABullet().GetComponent<Bullet>();

            GameControl.GameControlInstance.ChangeEnergy(-bullet.EnergyCost);

            bullet.Initialize(ColorContainer.Color.blue);

            ShootBullet(bullet);
        }

    }


    private void ShootBullet(Bullet bullet)
    {
        AudioManager.AudioManagerInstance.PlayAudio(0);
        _anim.SetInteger("ShootNumber", UnityEngine.Random.Range(1, 5));
        _anim.SetTrigger("Shoot");
        bullet.transform.position = BulletSpawnPoint.transform.position;
        bullet.transform.LookAt(BulletSpawnPoint.transform.forward * 100);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * BulletForce, ForceMode.Force);
    }

    private void ResetOrExit()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("View") || Input.GetButtonDown("Select"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            Application.Quit();
        }
    }
}
