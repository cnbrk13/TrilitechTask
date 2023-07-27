#region

using UnityEngine;

#endregion

namespace _Project.Scripts
{

	public class Coin : MonoBehaviour
	{
		#region Unity Methods

		private void Start()
		{
			GameManager.Instance.SpawnCoin();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<PlayerController>(out _))
			{
				HandleCoinCollection();
			}
		}

		#endregion

		#region Private Methods

		private void HandleCoinCollection()
		{
			GameManager.Instance.CollectCoin();
			Destroy(gameObject);
		}

		#endregion
	}

}