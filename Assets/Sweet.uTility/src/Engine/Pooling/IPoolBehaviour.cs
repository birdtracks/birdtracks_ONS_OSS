namespace SweetEngine.Pooling
{
	public interface IPoolBehaviour
	{
		void OnSpawn(PoolContainer pool);


		void OnRecycle(PoolContainer pool);
	}
}
