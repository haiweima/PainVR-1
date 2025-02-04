﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public Camera cam;
	public Canvas healthBar;
	public Text scoreText;

	private int score;

	void Start()
	{
		score = 0;

		if (isLocalPlayer)
			return;

		cam.enabled = false;
		healthBar.enabled = false;
		scoreText.enabled = false;
	}

	// Use this for initialization
	public override void OnStartLocalPlayer()
	{
		GetComponent<MeshRenderer> ().material.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		var x = Input.GetAxis ("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate (0, x, 0);
		transform.Translate (0, 0, z);

		if (Input.GetKeyDown (KeyCode.Space))
		{
			CmdFire ();
		}
	}

	[Command] //this code is called on the client but runs on the server!
	void CmdFire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate (
			             bulletPrefab,
			             bulletSpawn.position,
			             bulletSpawn.rotation,
						 gameObject.transform);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

//		if (isServer)
//		{
			//Spawn the bullet on the clients
			NetworkServer.Spawn (bullet);
//		}

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

	public void IncrementScore()
	{
		score++;
		scoreText.text = "Score: " + score.ToString ();
	}
}
