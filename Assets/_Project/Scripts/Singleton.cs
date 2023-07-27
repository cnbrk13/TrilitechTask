#region

using UnityEngine;

#endregion

namespace _Project.Scripts
{

	public abstract class Singleton<T> : Singleton where T : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private bool isPersistent;

		#endregion

		#region Constants and Fields

		private static readonly object Lock = new();
		private static T instance;

		#endregion

		#region Public Properties

		public static T Instance
		{
			get
			{
				if (Exiting)
				{
					Debug.Log($"{nameof(Singleton)}<{typeof(T)}>: Cikis yapiliyor. Instance dondurulmeyecek.");
					return null;
				}

				lock (Lock)
				{
					if (instance != null)
					{
						return instance;
					}

					var instances = FindObjectsOfType<T>(true);
					var instance_count = instances.Length;

					if (instance_count > 0)
					{
						if (instance_count == 1)
						{
							return instance = instances[0];
						}

						for (var i = 1; i < instances.Length; i++)
						{
							Destroy(instances[i]);
						}

						return instance = instances[0];
					}

					return instance = new GameObject($"{typeof(T)} (Otomatik Olusturuldu)").AddComponent<T>();
				}
			}
		}

		public static bool IsNull
		{
			get
			{
				if (instance == null)
				{
					return true;
				}

				return false;
			}
		}

		#endregion

		#region Unity Methods

		private void Awake()
		{
			if (isPersistent)
			{
				var instances = FindObjectsOfType<T>(false);

				if (instances.Length > 1)
				{
					for (var i = 1; i < instances.Length; i++)
					{
						Destroy(instances[i].gameObject);
					}
				}
				else
				{
					DontDestroyOnLoad(gameObject);
				}
			}

			OnAwake();
		}

		#endregion

		#region Protected Methods

		protected virtual void OnAwake()
		{
		}

		#endregion
	}

	public abstract class Singleton : MonoBehaviour
	{
		#region Protected Properties

		protected static bool Exiting { get; private set; }

		#endregion

		#region Unity Methods

		private void OnApplicationQuit()
		{
			Exiting = true;
		}

		#endregion
	}

}