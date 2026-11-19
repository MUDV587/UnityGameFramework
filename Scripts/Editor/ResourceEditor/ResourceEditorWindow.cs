//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// GF_X 补丁：对外公开的 ResourceEditor 入口（feature-upm）
// 上游无此文件，git 不会冲突，直接保留。

namespace UnityGameFramework.Editor.ResourceTools
{
    /// <summary>
    /// 资源编辑器公开入口。
    /// </summary>
    public static class ResourceEditorWindow
    {
        /// <summary>
        /// 打开资源编辑器窗口。
        /// </summary>
        public static void Open()
        {
            ResourceEditor.Open();
        }
    }
}
