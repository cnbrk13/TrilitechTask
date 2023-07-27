using System;
using UnityEngine;

namespace _Project.Scripts
{

	public class FinishLine : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<PlayerController>(out _))
			{
				TriggerGameOver();
			}
		}

		private void TriggerGameOver()
		{
			GameManager.Instance.FinishLineCrossed();
		}
	}

}
