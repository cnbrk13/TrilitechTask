#region

using TMPro;
using UnityEngine;

#endregion

namespace _Project.Scripts.UI
{

	public class CoinScoreUI : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private TextMeshProUGUI scoreTMP;

		#endregion

		#region Constants and Fields

		private int score;

		private int total;

		#endregion

		#region Unity Methods

		private void Start()
		{
			GameManager.Instance.CoinCollected += OnCoinCollected;
			GameManager.Instance.CoinSpawned += OnCoinSpawned;
		}

		#endregion

		#region Event Handlers

		private void OnCoinCollected(int new_score)
		{
			score = new_score;
			UpdateText();
		}

		private void OnCoinSpawned(int new_total)
		{
			total = new_total;
			UpdateText();
		}

		#endregion

		#region Private Methods

		private void UpdateText()
		{
			scoreTMP.text = $"{score}/{total}";
		}

		#endregion
	}

}