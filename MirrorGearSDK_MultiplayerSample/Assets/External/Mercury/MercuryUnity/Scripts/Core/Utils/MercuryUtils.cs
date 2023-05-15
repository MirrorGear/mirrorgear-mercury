using System.Text;
using UnityEngine;

public static class MercuryUtils
{
	public static T Clone<T>(this GameObject prefab, Transform parent) where T : Component
	{
		if (prefab == null)
			return default(T);

		return prefab.Clone<T>(parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static T Clone<T>(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, bool isLocal = true) where T : Component
	{
		if (prefab == null)
			return default(T);

		GameObject go;

		if (parent != null)
		{
			go = UnityEngine.Object.Instantiate(prefab, parent.position, parent.rotation) as GameObject;

			if (go != null)
				go.transform.SetParent(parent);
		}
		else
		{
			go = UnityEngine.Object.Instantiate(prefab) as GameObject;
		}

		go.name = go.name.Replace("(Clone)", "");

		if (isLocal)
		{
			go.transform.localPosition = position;
			go.transform.localRotation = rotation;
		}
		else
		{
			go.transform.position = position;
			go.transform.rotation = rotation;
		}
		go.transform.localScale = scale;

		if (go.activeSelf == false)
			go.SetActive(true);

		return go.GetComponent<T>();
	}

	/// <summary>
	/// 컬러값을 Hex로 바꿔준다.
	/// </summary>
	public static string ToHex(this Color color)
	{
		Color32 c = color;
		var hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
		return hex;
	}

	public static string ToColorString(this string message, Color color, bool isBold = false)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("<color=#");
		builder.Append($"{color.ToHex()}>");

		if (isBold)
			builder.Append("<b>");

		builder.Append(message);

		if (isBold)
			builder.Append("</b>");

		builder.Append("</color>");

		return builder.ToString();
	}

	public static RuntimeAnimatorController GetRuntimeAnimatorController(this Animator animator)
	{
		RuntimeAnimatorController controller = animator.runtimeAnimatorController;

		AnimatorOverrideController overrideController = controller as AnimatorOverrideController;
		while (overrideController != null)
		{
			controller = overrideController.runtimeAnimatorController;
			overrideController = controller as AnimatorOverrideController;
		}

		return controller;
	}

}

public class SingletonComponent<T> : MonoBehaviour where T : Component
{
	protected static T _instance;

	public static T Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<T>(true);
			}

			if (_instance == null)
			{
				GameObject newObject = new GameObject(typeof(T).Name);
				_instance = newObject.AddComponent<T>();
			}

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance == null)
			_instance = this as T;
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}
}