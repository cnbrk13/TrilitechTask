using System;
using UnityEngine;

namespace _Project.Scripts.UI
{

	public class EndScreenUI : MonoBehaviour
	{
		[SerializeField] private GameObject endScreenGroup;
		
		private void Start()
		{
			GameManager.Instance.GameEnded += OnGameEnd;
		}

		private void OnGameEnd()
		{
			endScreenGroup.SetActive(true);
		}
	}

}