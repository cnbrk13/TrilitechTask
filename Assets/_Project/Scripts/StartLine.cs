using System;
using UnityEngine;

namespace _Project.Scripts
{

	public class StartLine : MonoBehaviour
	{
		private void Start()
		{
			GameManager.Instance.Player.transform.position = transform.position;
		}
	}

}