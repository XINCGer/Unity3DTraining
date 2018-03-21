using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace AssetBundleDemo
{
    public enum LoadType
    {
        None,
        Loading,    //加载
        LoadOver    //加载完成
    }

    /// <summary>
    /// 加载模块
    /// </summary>
    public class LoadingProcess : MonoBehaviour
    {

        private static LoadingProcess instance = null;

        public static LoadingProcess _Instance
        {
            get
            {
                return instance;
            }
        }

        private Text loadInfo;
        private GameObject background = null;
        private Slider loadingSlider = null;


        void Start()
        {
            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            instance = this;

            loadInfo = transform.FindChild("info").GetComponent<Text>();
            background = transform.FindChild("background").gameObject;
            loadingSlider = transform.FindChild("LoadingSlider").GetComponent<Slider>();

            //transform.SetAsLastSibling();

            StartCoroutine(LoadBegin(
                UIID.Menu,
                LoadType.Loading,
                () =>
                {
                    //SourceManager._Instance.SetTrack(false);
                    ShowBackground(true);
                    ShowInfo("正在载入...", 50, Color.red, 0);  //显示加载
                },
                (UIid) =>
                {
                    ShowInfo("正在载入...", 50, Color.red, 0.1f);   //开始消失
                    ShowBackground(false);
                    UIManager._Instance.ShowUI(UIid);
                })
            );
        }

        /// <summary>
        /// 开始执行加载
        /// </summary>
        /// <param name="load">委托->加载内容</param>
        /// <param name="ID">加载的状态ID</param>
        /// <param name="loadOver">加载完成后执行</param>
        /// <returns></returns>
        public IEnumerator LoadBegin( UIID ID, LoadType loadType, Action load = null, Action<UIID> loadOver = null)
        {
            //transform.SetAsLastSibling();   //设置为最前的层

            switch (loadType)
            {
                case LoadType.Loading:
                    if (load != null)
                        load.Invoke();
                    yield return StartCoroutine(UIManager._Instance.CreateUI(ID));
                    if (loadOver != null)
                        loadOver.Invoke(ID);
                    break;
                case LoadType.LoadOver:
                    if (loadOver != null)
                        loadOver.Invoke(ID);
                    break;
                default:
                    break;
            }

            //load.Invoke();

            //yield return new WaitForEndOfFrame();

        }

        /// <summary>
        /// 显示提示，在duration时间后消失。若duration=0，表示永不消失
        /// </summary>
        /// <param name="duration">持续时间</param>
        public void ShowInfo(string info, int fontSize, Color fontColor, float duration)
        {
            this.gameObject.SetActive(true);
            loadInfo.DOKill();
            loadInfo.text = info;
            loadInfo.fontSize = fontSize;
            loadInfo.color = fontColor;
            if (duration > 0)
            {
                Tweener tweener = loadInfo.DOFade(0, 0.5f);  //0.5s后变为透明
                tweener.SetDelay(duration);

                tweener.OnComplete(delegate ()
                {
                    tweener.Kill();
                    this.gameObject.SetActive(false);
                });
            }
        }

        /// <summary>
        /// 显示背景框
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowBackground(bool isShow = false)
        {
            background.SetActive(isShow);
        }

        /// <summary>
        /// 显示liading条
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSlider(bool isShow = false)
        {
            loadingSlider.gameObject.SetActive(isShow);
        }

        /// <summary>
        /// 设置滑动条value
        /// </summary>
        /// <param name="value"></param>
        public void SetSliderValue(float value)
        {
            loadingSlider.value = value;
        }

    }
}