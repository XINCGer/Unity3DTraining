using UnityEngine;
using System.Collections;

namespace AssetBundleDemo
{

    /// <summary>
    /// UI接口
    /// </summary>
    public interface UIBase
    {

        /// <summary>
        /// UI初始化
        /// </summary>
        void InitUI(UIParam param);

        /// <summary>
        /// 更新UI
        /// </summary>
        void CheckUI();

        /// <summary>
        /// 创建UI
        /// </summary>
        void CreateUI();

        /// <summary>
        /// 显示UI
        /// </summary>
        void ShowUI();

        /// <summary>
        /// 删除UI
        /// </summary>
        void DeleteUI();

        /// <summary>
        /// 清理内存
        /// </summary>
        void ClearMemory();

    }

    /// <summary>
    /// UI参数
    /// </summary>
    public struct UIParam
    {
        public UIID ID;

        public UIParam(UIID id)
        {
            ID = id;
        }
    }
}