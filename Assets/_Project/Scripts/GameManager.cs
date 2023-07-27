#region

using System;

#endregion

namespace _Project.Scripts
{

	public class GameManager : Singleton<GameManager>
	{
		#region Serialized Fields

		public PlayerController Player;

		#endregion

		#region Constants and Fields

		private int coinScore;
		private int totalCoinsInLevel;

		#endregion

		#region Public Events and Delegates

		public event Action<int> CoinCollected;
		public event Action<int> CoinSpawned;

		#endregion

		#region Public Methods

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

		#endregion
	}

}