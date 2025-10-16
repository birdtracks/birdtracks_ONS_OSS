using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;


namespace SweetEngine.Pooling
{
	public class PoolContainer : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField] private PoolInfo[] m_PoolInfos = default (PoolInfo[]);
		private Dictionary<GameObject, PoolInfo> _instanceToPoolInfos;
		private Dictionary<GameObject, PoolInfo> _prefabToPoolInfos;
		private Dictionary<GameObject, Component[]> _instanceToPoolBehaviours; 




		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			int capacity = 0;

			for (int i = 0; i < m_PoolInfos.Length; i++)
			{
				PoolInfo poolInfo = m_PoolInfos[i];
				capacity += poolInfo.PrePoolCount;
			}
			
			_instanceToPoolInfos = new Dictionary<GameObject, PoolInfo>(capacity);
			_instanceToPoolBehaviours = new Dictionary<GameObject, Component[]>(capacity);
			_prefabToPoolInfos = new Dictionary<GameObject, PoolInfo>(m_PoolInfos.Length);

			for (int i = 0; i < m_PoolInfos.Length; i++)
			{
				PoolInfo poolInfo = m_PoolInfos[i];

				if (poolInfo.Prefab == null ||
					_prefabToPoolInfos.ContainsKey(poolInfo.Prefab))
				{
					continue;
				}

				for (int j = 0; j < poolInfo.Instances.Count; j++)
				{
					PoolInstance poolInstance = poolInfo.Instances[j];
					_instanceToPoolInfos[poolInstance.GameObject] = poolInfo;
					_instanceToPoolBehaviours[poolInstance.GameObject] = poolInstance.PoolBehaviours;
				}

				_prefabToPoolInfos[poolInfo.Prefab] = poolInfo;
			}
		}


		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}




		private PoolInfo AddPoolInfoForPrefab(GameObject prefab)
		{
			var container = new GameObject(prefab.name).GetComponent<Transform>();
			container.SetParent(transform);

			var ret = new PoolInfo(prefab, 0, container);
			_prefabToPoolInfos.Add(prefab, ret);

			return ret;
		}


		public GameObject Spawn(GameObject prefab)
		{
			Assert.IsNotNull(prefab);

			PoolInfo poolInfo;

			if (!_prefabToPoolInfos.TryGetValue(prefab, out poolInfo))
			{
				poolInfo = AddPoolInfoForPrefab(prefab);
			}

			GameObject obj;
			Component[] poolables;

			if (poolInfo.Instances.Count > 0)
			{
				obj = poolInfo.Instances[poolInfo.Instances.Count - 1].GameObject;
				poolInfo.Instances.RemoveAt(poolInfo.Instances.Count - 1);
				poolables = _instanceToPoolBehaviours[obj];
			}
			else
			{
				obj = Instantiate(prefab);
				obj.name = string.Format("{0} (Pooled)", poolInfo.Prefab.name);
				poolables = obj.GetComponentsInChildren(typeof(IPoolBehaviour), true);
				_instanceToPoolBehaviours.Add(obj, poolables);
				_instanceToPoolInfos.Add(obj, _prefabToPoolInfos[prefab]);
			}

			obj.transform.SetParent(null);
			obj.SetActive(true);

			for (int i = 0; i < poolables.Length; i++)
			{
				Component component = poolables[i];
				((IPoolBehaviour)component).OnSpawn(this);
			}

			return obj;
		}


		public T Spawn<T>(T prefab)
			where T : Component
		{
			Assert.IsNotNull(prefab);

			return Spawn(prefab.gameObject).GetComponent<T>();
		}


		public void SpawnGroup(GameObject prefab, int count, List<GameObject> outCollection)
		{
			Assert.IsNotNull(prefab);
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);

			PoolInfo poolInfo;

			if (!_prefabToPoolInfos.TryGetValue(prefab, out poolInfo))
			{
				poolInfo = AddPoolInfoForPrefab(prefab);
			}

			int pooledInstanceCount = Mathf.Min(count, poolInfo.Instances.Count);
			int remainingCount = count - pooledInstanceCount;

			for (int c = 1; c <= pooledInstanceCount; ++c)
			{
				GameObject obj = poolInfo.Instances[poolInfo.Instances.Count - c].GameObject;
				Component[] poolables = _instanceToPoolBehaviours[obj];

				obj.transform.SetParent(null);
				obj.SetActive(true);

				for (int i = 0; i < poolables.Length; i++)
				{
					Component component = poolables[i];
					((IPoolBehaviour)component).OnSpawn(this);
				}

				outCollection.Add(obj);
			}

			poolInfo.Instances.RemoveRange(poolInfo.Instances.Count - pooledInstanceCount, pooledInstanceCount);

			while (remainingCount > 0)
			{
				var obj = Instantiate(prefab);
				obj.name = string.Format("{0} (Pooled)", poolInfo.Prefab.name);
				Component[] poolables = obj.GetComponentsInChildren(typeof(IPoolBehaviour), true);
				_instanceToPoolBehaviours.Add(obj, poolables);
				_instanceToPoolInfos.Add(obj, _prefabToPoolInfos[prefab]);

				obj.SetActive(true); 
				
				for (int i = 0; i < poolables.Length; i++)
				{
					Component component = poolables[i];
					((IPoolBehaviour)component).OnSpawn(this);
				}

				outCollection.Add(obj);
				remainingCount--;
			}
		}


		public void SpawnGroup<T>(T prefab, int count, List<T> outCollection)
			where T : Component
		{
			Assert.IsNotNull(prefab);
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);

			GameObject go = prefab.gameObject;
			PoolInfo poolInfo;

			if (!_prefabToPoolInfos.TryGetValue(go, out poolInfo))
			{
				poolInfo = AddPoolInfoForPrefab(go);
			}

			int pooledInstanceCount = Mathf.Min(count, poolInfo.Instances.Count);
			int remainingCount = count - pooledInstanceCount;

			for (int c = 1; c <= pooledInstanceCount; ++c)
			{
				GameObject obj = poolInfo.Instances[poolInfo.Instances.Count - c].GameObject;
				Component[] poolables = _instanceToPoolBehaviours[obj];

				obj.transform.SetParent(null);
				obj.SetActive(true);

				for (int i = 0; i < poolables.Length; i++)
				{
					Component component = poolables[i];
					((IPoolBehaviour)component).OnSpawn(this);
				}

				outCollection.Add(obj.GetComponent<T>());
			}

			poolInfo.Instances.RemoveRange(poolInfo.Instances.Count - pooledInstanceCount, pooledInstanceCount);

			while (remainingCount > 0)
			{
				var obj = Instantiate(go);
				obj.name = string.Format("{0} (Pooled)", poolInfo.Prefab.name);
				Component[] poolables = obj.GetComponentsInChildren(typeof(IPoolBehaviour), true);
				_instanceToPoolBehaviours.Add(obj, poolables);
				_instanceToPoolInfos.Add(obj, _prefabToPoolInfos[go]);

				obj.SetActive(true);

				for (int i = 0; i < poolables.Length; i++)
				{
					Component component = poolables[i];
					((IPoolBehaviour)component).OnSpawn(this);
				}

				outCollection.Add(obj.GetComponent<T>());
				remainingCount--;
			}
		}


		public Coroutine SpawnGroupAtRate(GameObject prefab, int count, List<GameObject> outCollection, int rate)
		{
			Assert.IsNotNull(prefab);
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(SpawnGroupAtRateRoutine(prefab, count, outCollection, rate, null));
		}


		public Coroutine SpawnGroupAtRate(GameObject prefab, int count, List<GameObject> outCollection, int rate,
			Action<GameObject> onSpawnTick)
		{
			Assert.IsNotNull(prefab);     
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(SpawnGroupAtRateRoutine(prefab, count, outCollection, rate, onSpawnTick));
		}


		private IEnumerator SpawnGroupAtRateRoutine(GameObject prefab, int count, List<GameObject> outCollection, int rate,
			Action<GameObject> onSpawnTick)
		{
			int index = 0;

			while (true)
			{
				SpawnGroup(prefab, Mathf.Min(count, rate), outCollection);
				count -= rate;

				if (onSpawnTick != null)
				{
					while (index < outCollection.Count)
					{
						onSpawnTick(outCollection[index]);
						index++;
					}
				}

				if (count == 0)
				{
					yield break;
				}

				yield return null;
			}
		}


		public Coroutine SpawnGroupAtRate<T>(T prefab, int count, List<T> outCollection, int rate)
			where T : Component
		{
			Assert.IsNotNull(prefab);
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(SpawnGroupAtRateRoutine(prefab, count, outCollection, rate, null));
		}


		public Coroutine SpawnGroupAtRate<T>(T prefab, int count, List<T> outCollection, int rate,
			Action<GameObject> onSpawnTick)
			where T : Component
		{
			Assert.IsNotNull(prefab);
			Assert.IsTrue(count > 0);
			Assert.IsNotNull(outCollection);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(SpawnGroupAtRateRoutine(prefab, count, outCollection, rate, onSpawnTick));
		}


		private IEnumerator SpawnGroupAtRateRoutine<T>(T prefab, int count, List<T> outCollection, int rate,
			Action<GameObject> onSpawnTick)
			where T : Component
		{
			int index = 0;

			while (true)
			{
				SpawnGroup(prefab, Mathf.Min(count, rate), outCollection);
				count -= rate;

				if (onSpawnTick != null)
				{
					while (index < outCollection.Count)
					{
						onSpawnTick(outCollection[index].gameObject);
						index++;
					}
				}

				if (count == 0)
				{
					yield break;
				}

				yield return null;
			}
		}


		public void Recycle(GameObject obj)
		{
			Assert.IsNotNull(obj);
			Assert.IsTrue(_instanceToPoolInfos.ContainsKey(obj));

			int limit = int.MaxValue;
			RecycleRecursive(obj, ref limit);
		}


		public void Recycle<T>(T obj)
			where T : Component
		{
			Recycle(obj.gameObject);
		}


		public Coroutine Recycle(GameObject obj, int rate)
		{
			Assert.IsNotNull(obj);
			Assert.IsTrue(_instanceToPoolInfos.ContainsKey(obj));
			Assert.IsTrue(rate > 0);

			return StartCoroutine(RecycleAtRateRoutine(obj, rate));
		}


		public Coroutine Recycle<T>(T obj, int rate)
			where T : Component
		{
			Assert.IsNotNull(obj);
			Assert.IsTrue(_instanceToPoolInfos.ContainsKey(obj.gameObject));
			Assert.IsTrue(rate > 0);

			return StartCoroutine(RecycleAtRateRoutine(obj.gameObject, rate));
		}


		public void RecycleGroup(List<GameObject> objs)
		{
			Assert.IsNotNull(objs);

			for (int i = 0; i < objs.Count; i++)
			{
				Recycle(objs[i]);
			}
		}


		public void RecycleGroup<T>(List<T> objs)
			where T : Component
		{
			Assert.IsNotNull(objs);

			for (int i = 0; i < objs.Count; i++)
			{
				Recycle(objs[i].gameObject);
			}
		}


		public Coroutine RecycleGroupAtRate(List<GameObject> objs, int rate)
		{
			Assert.IsNotNull(objs);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(RecycleGroupAtRateRoutine(objs, rate));
		}


		public Coroutine RecycleGroupAtRate<T>(List<T> objs, int rate)
			where T : Component
		{
			Assert.IsNotNull(objs);
			Assert.IsTrue(rate > 0);

			return StartCoroutine(RecycleGroupAtRateRoutine(objs, rate));
		}


		private bool RecycleRecursive(GameObject obj, ref int limit)
		{
			var t = obj.GetComponent<Transform>();

			for (int i = t.childCount - 1; i >= 0; i--)
			{
				GameObject g = t.GetChild(i).gameObject;

				if (!RecycleRecursive(g, ref limit) ||
				    limit == 0)
				{
					return false;
				}
			}

			PoolInfo poolInfo;

			if (!_instanceToPoolInfos.TryGetValue(obj, out poolInfo))
			{
				return true;
			}

			List<PoolInstance> instances = poolInfo.Instances;
			Component[] poolables = _instanceToPoolBehaviours[obj];

			for (int i = 0; i < poolables.Length; i++)
			{
				Component component = poolables[i];
				((IPoolBehaviour)component).OnRecycle(this);
			}

			t.SetParent(poolInfo.Container, false);
			obj.SetActive(false);
			instances.Add(new PoolInstance
			{
				GameObject = obj,
				PoolBehaviours = poolables
			});
			limit--;
			return true;
		}


		private IEnumerator RecycleAtRateRoutine(GameObject obj, int rate)
		{
			int limit = rate;

			while (!RecycleRecursive(obj, ref limit))
			{
				yield return null;
				limit = rate;
			}
		}


		private IEnumerator RecycleGroupAtRateRoutine(List<GameObject> objs, int rate)
		{
			int i = 0;

			while (i < objs.Count)
			{
				int limit = rate;

				while (i < objs.Count &&
				       RecycleRecursive(objs[i], ref limit) &&
				       limit > 0)
				{
					i++;
				}

				yield return null;
			}
		}


		private IEnumerator RecycleGroupAtRateRoutine<T>(List<T> objs, int rate)
			where T : Component
		{
			int i = 0;

			while (i < objs.Count)
			{
				int limit = rate;

				while (i < objs.Count &&
				       RecycleRecursive(objs[i].gameObject, ref limit) &&
				       limit > 0)
				{
					i++;
				}

				yield return null;
			}
		}
		


#if UNITY_EDITOR
		[ContextMenu("Rebuild Pool"), UsedImplicitly]
		private void RebuildPool()
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			for (int i = 0; i < m_PoolInfos.Length; i++)
			{
				PoolInfo info = m_PoolInfos[i];

				if (info.Prefab == null)
				{
					continue;
				}

				int index = Array.FindIndex(m_PoolInfos, pi => pi.Prefab == info.Prefab);
				if (index != i)
				{
					Debug.LogWarning(string.Format("Duplicate pool for prefab \"{0}\" between indexes {1} and {2}.",
						info.Prefab.gameObject.name, index, i), this);
					continue;
				}

				info.Instances = new List<PoolInstance>(info.PrePoolCount);
				info.Container = new GameObject(info.Prefab.name).GetComponent<Transform>();
				info.Container.SetParent(transform);

				while (info.Instances.Count < info.PrePoolCount)
				{
					var obj = (GameObject)PrefabUtility.InstantiatePrefab(info.Prefab);
					obj.SetActive(false);
					obj.transform.SetParent(info.Container, false);
					obj.name = string.Format("{0} (Pooled)", info.Prefab.name);
					info.Instances.Add(new PoolInstance
					{
						GameObject = obj,
						PoolBehaviours = obj.GetComponentsInChildren(typeof(IPoolBehaviour), true)
					});
				}
			}

			EditorUtility.SetDirty(this);
		}
#endif



		[Serializable]
		private struct PoolInfo
		{
			public GameObject Prefab;
			public int PrePoolCount;
			[HideInInspector] public Transform Container;
			[HideInInspector] public List<PoolInstance> Instances;




			public PoolInfo(GameObject prefab, int prePoolCount, Transform container)
			{
				Prefab = prefab;
				PrePoolCount = prePoolCount;
				Container = container;
				Instances = new List<PoolInstance>(prePoolCount);
			}
		}


		[Serializable]
		private struct PoolInstance
		{
			public GameObject GameObject;
			public Component[] PoolBehaviours;
		}
	}
}
