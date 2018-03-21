using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace AssetBundleDemo
{

    public enum UIID
    {
        None,
        Loading,
        Menu
    }

    /// <summary>
    /// 资源信息（资源包名字、资源名字、脚本名称）
    /// </summary>
    public class UISourceInfo
    {
        /// <summary>
        /// 加载方式，默认无加载
        /// </summary>
        public LoadType loadType = LoadType.None;
        /// <summary>
        /// UI状态，默认无
        /// </summary>
        public UIID UI_id = UIID.None;
        /// <summary>
        /// 资源包名称 -> 小写
        /// </summary>
        public string bundleName;
        /// <summary>
        /// 资源对象名称
        /// </summary>
        public string sourceName;
        /// <summary>
        /// 保存当前UI的界面
        /// </summary>
        public GameObject UIObj = null;
        /// <summary>
        /// 挂载的脚本
        /// </summary>
        public string[] scripts = null;
        /// <summary>
        /// UI组件的深度
        /// </summary>
        public int SiblingIndex = 0;
    }

    public class UIManager : MonoBehaviour
    {

        private static UIManager instance = null;
        public static UIManager _Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// UICanvas根路径
        /// </summary>
        public RectTransform UICanvas = null;

        /// <summary>
        /// 所有的UI信息
        /// </summary>
        public Dictionary<UIID, UISourceInfo> DictUISource = new Dictionary<UIID, UISourceInfo>(); //当前已加载的

        public void InitUIData()
        {
            instance = this;

            UICanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();

            SaveUIAssetBundleInfo();

        }

        /// <summary>
        /// 将所有UI资源的信息保存在内存中，方便以后加载
        /// </summary>
        void SaveUIAssetBundleInfo()
        {
            UISourceInfo UIInfo;

            //Loading
            UIInfo = new UISourceInfo();
            UIInfo.UI_id = UIID.Loading;
            UIInfo.bundleName = "uiasset/loading";
            UIInfo.sourceName = "Loading";
            UIInfo.scripts = new string[] { "LoadingProcess" };
            UIInfo.SiblingIndex = 30;
            DictUISource.Add(UIID.Loading, UIInfo);
            //MenuUI
            UIInfo = new UISourceInfo();
            UIInfo.UI_id = UIID.Menu;
            UIInfo.bundleName = "uiasset/menuui";
            UIInfo.sourceName = "MenuUI";
            UIInfo.scripts = new string[] { "MenuUI" };
            UIInfo.SiblingIndex = 1;
            DictUISource.Add(UIID.Menu, UIInfo);

        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ID"></param>
        public void ShowUI(UIID ID)
        {
            switch (ID)
            {
                case UIID.Menu:
                    DictUISource[ID].UIObj.SetActive(true);
                    SourceManager._Instance.currentID = ID;
                    break;
                case UIID.None:
                    break;
            }
        }
        
        /// <summary>
        /// 根据UIID创建UI预设
        /// </summary>
        /// <param name="id"></param>
        public IEnumerator CreateUI(UIID id)
        {
            Debug.Log("创建UI：" + id);
            UISourceInfo uiSourceInfo;  //获取需要的UISourceInfo
            if (DictUISource.TryGetValue(id, out uiSourceInfo)) //先从内存中获取
            {
                
                if (uiSourceInfo.UIObj != null) //如果场景中存在这个UI，则不会创建，直接显示
                {
                    //ShowUI(id);
                    yield return null;
                }
                else
                {
                    //加载UI资源
                    yield return StartCoroutine(SourceManager._Instance.DownloadAssetBundle(uiSourceInfo.bundleName, (bundle, dependBundles) => {
                        GameObject obj = bundle.LoadAsset<GameObject>(uiSourceInfo.sourceName);
                        uiSourceInfo.UIObj = Instantiate(obj);  //保存创建的对象
                        UIReset(ref uiSourceInfo.UIObj);    //重置transform
                        uiSourceInfo.UIObj.transform.SetSiblingIndex(uiSourceInfo.SiblingIndex);    //设置深度

                        //添加需要的脚本
                        for (int i = 0; i < uiSourceInfo.scripts.Length; i++)
                        {
                            uiSourceInfo.UIObj.AddComponent(SourceManager._Instance.getType(uiSourceInfo.scripts[i]));
                        }

                        if (id != UIID.Loading) //初始化UI
                        {
                            UIParam param = new UIParam(id);
                            uiSourceInfo.UIObj.GetComponent<UIBase>().InitUI(param);

                        }
                        //UI信息重新保存到内存中
                        DictUISource[id] = uiSourceInfo;

                        //释放主资源包
                        bundle.Unload(false);
                        //释放依赖资源包
                        if(dependBundles != null)
                            for (int i = 0; i < dependBundles.Length; i++)
                            {
                                dependBundles[i].Unload(false);
                            }

                    }));
                }
                    
            }
            else
            {
                Debug.Log("UI资源不存在，未加载成功！" + id);
            }

        }


        /// <summary>
        /// 将加载UI资源放置在Canvas下，并重置大小与Canvas一致
        /// </summary>
        /// <param name="obj"></param>
        public void UIReset(ref GameObject obj)
        {
            RectTransform trans = obj.GetComponent<RectTransform>();
            trans.SetParent(UICanvas);
            trans.sizeDelta = Vector2.zero;
            trans.localScale = Vector3.one;
        }

    }
}