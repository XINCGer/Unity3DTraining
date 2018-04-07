using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

public class CreateAssetBundles
{
	static string AssetbundlePath = "Assets" + Path.DirectorySeparatorChar + "assetbundles" + Path.DirectorySeparatorChar;

	[MenuItem("Character Generator/Create Assetbundles")]
	static void Execute()
	{
		bool createdBundle = false;
		foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
		{
			if (!(o is GameObject)) continue;
			if (o.name.Contains("@")) continue;
			if (!AssetDatabase.GetAssetPath(o).Contains("/characters/")) continue;

			// 将选中对象转为GameObject对象
			GameObject characterFBX = (GameObject)o;
			string name = characterFBX.name.ToLower();
			CreateCharacterBaseAssetBundle(characterFBX, name);	
			CreatePartAssetBundles(characterFBX, name);

			createdBundle = true;
		}
		
		if(! createdBundle)
		{
			EditorUtility.DisplayDialog("Character Generator", 
			                            "未生成Assetbundle.请选择Project视图中的characters文件夹来生成所有角色或者选择单个子目录生成选定角色", "OK");		
			return;
		}
		
		CreateElementDatabaseBundles ();
	}

	static void CreateCharacterBaseAssetBundle(GameObject fbx, string name)
	{
		// 若AssetBundle目录不存在则创建之
		if (!Directory.Exists(AssetbundlePath))
			Directory.CreateDirectory(AssetbundlePath);
		
		// 若AssetbundlePath下已包含assetbundle文件则删除之
		string[] existingAssetbundles = Directory.GetFiles(AssetbundlePath);
		foreach (string bundle in existingAssetbundles)
		{
			if (bundle.EndsWith(".assetbundle") && bundle.Contains("/assetbundles/" + name))
				File.Delete(bundle);
		}
		
		GameObject characterClone = (GameObject)Object.Instantiate(fbx);
		
		foreach (Animation a in characterClone.GetComponentsInChildren<Animation>())
			a.cullingType = AnimationCullingType.AlwaysAnimate;
		
		foreach (SkinnedMeshRenderer smr in characterClone.GetComponentsInChildren<SkinnedMeshRenderer>())
			Object.DestroyImmediate(smr.gameObject);
		
		//生成Male_characterbase.assetbundle和Female_characterbase.assetbundle
		characterClone.AddComponent<SkinnedMeshRenderer>();
		
		Object characterBasePrefab = GetPrefab(characterClone, "characterbase");
		string path = AssetbundlePath + name + "_characterbase.assetbundle";
		BuildPipeline.BuildAssetBundle(characterBasePrefab, null, path, BuildAssetBundleOptions.CollectDependencies,BuildTarget.StandaloneWindows);
		AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(characterBasePrefab));
	}

	static void CreatePartAssetBundles(GameObject fbx, string name)
	{
		List<Material> materials = EditorHelpers.CollectAll<Material>(MaterialsPath(fbx));
		
		foreach (SkinnedMeshRenderer smr in fbx.GetComponentsInChildren<SkinnedMeshRenderer>(true))
		{
			List<Object> toinclude = new List<Object>();
			
			GameObject rendererClone = (GameObject)PrefabUtility.InstantiatePrefab(smr.gameObject);
			GameObject rendererParent = rendererClone.transform.parent.gameObject;
			rendererClone.transform.parent = null;
			Object.DestroyImmediate(rendererParent);
			Object rendererPrefab = GetPrefab(rendererClone, "rendererobject");
			toinclude.Add(rendererPrefab);
			
			// 若材质对象名称中包含子对象的名称（如eyes、face-1、face-2等则
			// 视将材质对象加入列表。这里注意每个toinclude对象与一个
			// SkinnedMeshRenderer对象对应，即与FBX对象的子对象对应。
			foreach (Material m in materials)
				if (m.name.Contains(smr.name.ToLower())) 
					toinclude.Add(m);
			
			List<string> boneNames = new List<string>();
			foreach (Transform t in smr.bones)
				boneNames.Add(t.name);
			
			string stringholderpath = "Assets/bonenames.asset";
			StringHolder holder = ScriptableObject.CreateInstance<StringHolder> ();
			holder.content = boneNames.ToArray();
			AssetDatabase.CreateAsset(holder, stringholderpath);
			toinclude.Add(AssetDatabase.LoadAssetAtPath(stringholderpath, typeof (StringHolder)));
			
			string bundleName = name + "_" + smr.name.ToLower();
			string path = AssetbundlePath + bundleName + ".assetbundle";
			BuildPipeline.BuildAssetBundle(null, toinclude.ToArray(), path, BuildAssetBundleOptions.CollectDependencies,BuildTarget.StandaloneWindows);
			
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(rendererPrefab));
			AssetDatabase.DeleteAsset(stringholderpath);
		}
	}

	static Object GetPrefab(GameObject go, string name)
	{
		Object tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
		tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
		Object.DestroyImmediate(go);
		return tempPrefab;
	}

	public static string MaterialsPath(GameObject character)
	{
		string root = AssetDatabase.GetAssetPath(character);
		return root.Substring(0, root.LastIndexOf("/") + 1) + "Per Texture Materials";
	}

	static void CreateElementDatabaseBundles ()
	{
		List<CharacterElement> characterElements = new List<CharacterElement>();
		
		string[] assetbundles = Directory.GetFiles(AssetbundlePath);
		string[] materials = Directory.GetFiles("Assets/CharacterCustomization/characters", "*.mat", SearchOption.AllDirectories);
		foreach (string material in materials)
		{
			foreach (string bundle in assetbundles)
			{
				FileInfo bundleFI = new FileInfo(bundle);
				FileInfo materialFI = new FileInfo(material);
				string bundleName = bundleFI.Name.Replace(".assetbundle", "");
				if (!materialFI.Name.StartsWith(bundleName)) continue;
				if (!material.Contains("Per Texture Materials")) continue;
				characterElements.Add(new CharacterElement(materialFI.Name.Replace(".mat", ""), bundleFI.Name));
				break;
			}
		}

		CharacterElementHolder t = ScriptableObject.CreateInstance<CharacterElementHolder> ();
		t.content = characterElements;
		
		string p = "Assets/CharacterElementDatabase.asset";
		AssetDatabase.CreateAsset(t, p);
		Object o = AssetDatabase.LoadAssetAtPath(p, typeof(CharacterElementHolder));
		
		BuildPipeline.BuildAssetBundle(o, null, AssetbundlePath + "CharacterElementDatabase.assetbundle",BuildAssetBundleOptions.CollectDependencies,BuildTarget.StandaloneWindows);
		AssetDatabase.DeleteAsset(p);
	}
}
