using System;
using UnityEngine;

namespace _Project.Scripts
{

	public class Coin : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
         {
             if (other.TryGetComponent<PlayerController>(out _))
             {
                 HandleCoinCollection();
             }
         }

		private void Start()
		{
			GameManager.Instance.SpawnCoin();
		}

		private void HandleCoinCollection()
         {
             GameManager.Instance.CollectCoin();
             Destroy(gameObject);
         }
	}
}
