namespace SweetEngine.Pooling
{
	public interface IPool<T>
	{
		T Get();


		void Release(T obj);
	}
}
