using System;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI
{

	public class CoinScoreUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI scoreTMP;

		private int total = 0;
		private int score = 0;
		
		private void Start()
		{
			GameManager.Instance.CoinCollected += OnCoinCollected;
			GameManager.Instance.CoinSpawned += OnCoinSpawned;
		}

		private void OnCoinSpawned(int new_total)
		{
			total = new_total;
			UpdateText();
		}

		private void OnCoinCollected(int new_score)
		{
			score = new_score;
			UpdateText();
		}
		
		private void UpdateText()
		{
			scoreTMP.text = $"{score}/{total}";
		}
	}

}