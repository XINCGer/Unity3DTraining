using UnityEngine;
using UnityEngine.UI;

public class LoadNextLevel : MonoBehaviour 
{
	public int levelToLoad = 0;
	public int incrementOnClick = 1;

	void Start()
	{
		if (levelToLoad > 0 && Application.levelCount > 1)
			Application.LoadLevel(levelToLoad);
	}

	void OnClickEvent()
	{
		int nextLevel = (Application.loadedLevel - 1) + incrementOnClick;
		//nextLevel = Math.RepeatIndex(nextLevel, Application.levelCount - 1);

		if (nextLevel < 0)
		{
			Application.LoadLevel(Application.levelCount - 1);
		}
		else
		{
			++nextLevel;

			if (nextLevel >= Application.levelCount)
			{
				Application.LoadLevel(1);
			}
			else
			{
				Application.LoadLevel(nextLevel);
			}
		}
	}
}
