using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour 
{
	CharacterGenerator generator;
	GameObject character;
	bool usingLatestConfig;
	bool newCharacterRequested = true;
	bool firstCharacter = true;
	string nonLoopingAnimationToPlay;

	const int typeWidth = 80;
	const int buttonWidth = 20;
	const string prefName = "Character Customization Pref";

	IEnumerator Start()
	{
		while (!CharacterGenerator.ReadyToUse) yield return 0;
		if (PlayerPrefs.HasKey(prefName))
			generator = CharacterGenerator.CreateWithConfig(PlayerPrefs.GetString(prefName));
		else
			generator = CharacterGenerator.CreateWithRandomConfig("Female");
	}

	void Update()
	{
		if(generator == null) return;
		if(usingLatestConfig) return;
		if(!generator.ConfigReady) return;
		
		usingLatestConfig = true;
		
		if (newCharacterRequested)
		{
			//新角色的加载过程
			Destroy(character);
			character = generator.Generate();
			Animation a = character.GetComponent<Animation>();
			a.Play("idle1");
			a["idle1"].wrapMode = WrapMode.Loop;
			newCharacterRequested = false;
			
			if (!firstCharacter) return;
			firstCharacter = false;
			if(a["walkin"] == null) return;
			a["walkin"].layer = 1 ;
			a["walkin"].weight = 1;
			a.CrossFade("walkin", 0.8f);
			character.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
		}
		else
		{
			//不加载新角色，更新角色部件的过程
			character = generator.Generate(character);
			if (nonLoopingAnimationToPlay == null) return;
			Animation a = character.GetComponent<Animation>();
			a[nonLoopingAnimationToPlay].layer = 1;
			a[nonLoopingAnimationToPlay].weight = 1;
			a.CrossFade(nonLoopingAnimationToPlay, 0.8f);
			nonLoopingAnimationToPlay = null;
		}
	}

	void ChangeCharacter(bool next)
	{
		generator.ChangeCharacter(next);
		usingLatestConfig = false;
		newCharacterRequested = true;
	}

	void ChangeElement(string catagory, bool next, string anim)
	{
		generator.ChangeElement(catagory, next);
		usingLatestConfig = false;
		
		if (!character.GetComponent<Animation>().IsPlaying(anim))
			nonLoopingAnimationToPlay = anim;
	}

	void AddCategory(string category, string displayName, string anim)
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("<", GUILayout.Width(buttonWidth)))
			ChangeElement(category, false, anim);
		
		GUILayout.Box(displayName, GUILayout.Width(typeWidth));
		
		if (GUILayout.Button(">", GUILayout.Width(buttonWidth)))
			ChangeElement(category, true, anim);
		GUILayout.EndHorizontal();
	}

	void OnGUI()
	{

		if (generator == null) return;
		GUI.enabled = usingLatestConfig && !character.GetComponent<Animation>().IsPlaying("walkin");
		
		GUILayout.BeginArea(new Rect(10, 10, typeWidth + 2 * buttonWidth + 8, 500));
		// 1. 添加切换角色按钮；
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("<", GUILayout.Width(buttonWidth)))
			ChangeCharacter(false);
		GUILayout.Box("角色", GUILayout.Width(typeWidth));
		if (GUILayout.Button(">", GUILayout.Width(buttonWidth)))
			ChangeCharacter(true);
		GUILayout.EndHorizontal();

		// 2. 添加切换角色身体部件按钮；
		AddCategory("face", "头", null);
		AddCategory("hair", "头发", null);
		AddCategory("eyes", "眼睛", null);
		AddCategory("top", "身体", "item_shirt");
		AddCategory("pants", "腿", "item_pants");
		AddCategory("shoes", "脚", "item_boots");

		// 3. 添加保存和删除设置按钮；
		if (GUILayout.Button("保存设置"))
			PlayerPrefs.SetString(prefName, generator.GetConfig());
		
		if (GUILayout.Button("删除设置"))
			PlayerPrefs.DeleteKey(prefName);

		GUI.enabled = true;
		if (!usingLatestConfig)
		{
			float progress = generator.CurrentConfigProgress;
			string status = "加载中";
			if (progress != 1) status = "下载中" + (int)(progress * 100) + "%";
			GUILayout.Box(status);
		}
		
		GUILayout.EndArea();
	}
}
