using UnityEngine;


namespace SweetEngine.Extensions
{
    public static class TransformExtensions
    {
	    public static void ResetLocal(this Transform transform)
	    {
			RectTransform rt = transform as RectTransform;

			if (rt != null)
			{
				rt.anchoredPosition3D = Vector3.zero;
			}
			else
			{
		    	transform.localPosition = Vector3.zero;
			}

		    transform.localScale = Vector3.one;
		    transform.localRotation = Quaternion.identity;
	    }


	    public static void SetParentAndResetLocal(this Transform transform, Transform parent)
	    {
		    transform.SetParent(parent, true);
			transform.ResetLocal();
	    }
    }
}
