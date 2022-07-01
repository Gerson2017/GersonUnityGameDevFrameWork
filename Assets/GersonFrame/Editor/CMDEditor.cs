using UnityEngine;
using UnityEditor;

namespace DragonRun
{
    public class CMDEditor 
    {

        /// <summary>
        /// 构建Process对象，并执行
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>Process对象</returns>
        private static System.Diagnostics.Process CreateCmdProcess(string cmd, string args, string workingDir = "")
        {
            var en = System.Text.UTF8Encoding.UTF8;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                en = System.Text.Encoding.GetEncoding("gb2312");

            var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = false;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.StandardErrorEncoding = en;
            pStartInfo.StandardOutputEncoding = en;
            if (!string.IsNullOrEmpty(workingDir))
                pStartInfo.WorkingDirectory = workingDir;
            return System.Diagnostics.Process.Start(pStartInfo);
        }

        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="input">StandardInput</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string[] input, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            if (input != null && input.Length > 0)
            {
                for (int i = 0; i < input.Length; i++)
                    p.StandardInput.WriteLine(input[i]);
            }
            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        /// <summary>
        /// 运行命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <returns>string[] res[0]命令的stdout输出, res[1]命令的stderr输出</returns>
        public static string[] RunCmd(string cmd, string args, string workingDir = "")
        {
            string[] res = new string[2];
            var p = CreateCmdProcess(cmd, args, workingDir);
            res[0] = p.StandardOutput.ReadToEnd();
            res[1] = p.StandardError.ReadToEnd();
#if !UNITY_IOS
            res[2] = p.ExitCode.ToString();
#endif
            p.Close();
            return res;
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="absPath">文件夹的绝对路径</param>
        public static void OpenFolderInExplorer(string absPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                RunCmdNoErr("explorer.exe", absPath);
            else if (Application.platform == RuntimePlatform.OSXEditor)
                RunCmdNoErr("open", absPath.Replace("\\", "/"));
        }
    }
}
