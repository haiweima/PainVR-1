﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;
	public RectTransform healthBar;
	public bool destroyOnDeath;
	private NetworkStartPosition[] spawnPoints;

	void Start ()
	{
		if (isLocalPlayer)
		{
			spawnPoints = FindObjectsOfType<NetworkStartPosition>();
		}
	}

	public void TakeDamage(int amount, PlayerController shooter)
	{
//		if (isClient)
//		{
//			var tempHealth = currentHealth - amount;
//			if (tempHealth <= 0) 
//			{
//				shooter.IncrementScore ();
//			}
//		}

		if (!isServer)
		{
			return;
		}

		currentHealth -= amount;

		if (currentHealth <= 0)
		{
			if (destroyOnDeath)
			{
				shooter.IncrementScore ();
				Destroy (gameObject);
			}
			else
			{
				currentHealth = maxHealth;

				//called on the server but invoked on the clients
				RpcRespawn ();
			}
		}
	}

	void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2 (
			currentHealth,
			healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if (isLocalPlayer)
		{
			// Set the spawn point to origin as a default value
			Vector3 spawnPoint = Vector3.zero;

			// If there is a spawn point array and the array is not empty, pick a spawn point at random
			if (spawnPoints != null && spawnPoints.Length > 0)
			{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}

			// Set the player’s position to the chosen spawn point
			transform.position = spawnPoint;
		}
	}

}
