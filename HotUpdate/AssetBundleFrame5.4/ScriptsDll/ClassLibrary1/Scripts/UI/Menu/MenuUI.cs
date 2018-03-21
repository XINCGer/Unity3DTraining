using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace AssetBundleDemo
{
    public class MenuUI : MonoBehaviour, UIBase
    {

        public Button BT_Start = null;

        public void CheckUI()
        {
            throw new NotImplementedException();
        }

        public void ClearMemory()
        {
            throw new NotImplementedException();
        }

        public void CreateUI()
        {
            throw new NotImplementedException();
        }

        public void DeleteUI()
        {
            throw new NotImplementedException();
        }

        public void InitUI(UIParam param)
        {
            if (param.ID != UIID.Menu)
                return;
            
            //添加进入AR按钮的动画
            BT_Start = transform.FindChild("BT_Start").GetComponent<Button>();
            Tweener BT_StartTween = BT_Start.transform.DOScale(1.2f, 0.2f);
            BT_StartTween.Pause();
            BT_StartTween.SetLoops(2, LoopType.Yoyo);
            BT_StartTween.SetEase(Ease.InQuad);
            BT_StartTween.SetAutoKill(false);
            BT_StartTween.OnComplete(delegate ()
            {
                BT_StartTween.Rewind();
                StartCoroutine(LoadingProcess._Instance.LoadBegin(
                    UIID.Menu,
                    LoadType.LoadOver,
                    () =>
                    {
                        
                    },
                    (UIid) =>
                    {
                        LoadingProcess._Instance.ShowInfo("无下一步操作...", 50, Color.red, 1f);
                    })
                );
            });

            BT_Start.onClick.AddListener(delegate ()
            {
                BT_StartTween.PlayForward();
                SoundManager._Instacne.PlayAudio("Capture");
            });

        }

        public void ShowUI()
        {
            throw new NotImplementedException();
        }

    }
}