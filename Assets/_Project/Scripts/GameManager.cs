using System;
using UnityEngine;

namespace _Project.Scripts
{
	public class GameManager : Singleton<GameManager>
	{
		
		public PlayerController Player;
		public event Action<int> CoinCollected;
		public event Action<int> CoinSpawned; 
		

		private int coinScore = 0;
		private int totalCoinsInLevel = 0;

		public void CollectCoin()
		{
			coinScore++;
			CoinCollected?.Invoke(coinScore);
		}

		public void SpawnCoin()
		{
			totalCoinsInLevel++;
			CoinSpawned?.Invoke(totalCoinsInLevel);
		}
	}
}