## 安装包做成步骤

- 将要安装的程序文件夹 (Debug 或 Release) 压缩成 zip 文件并改为程序的名字
- 新建一个控制台程序，引用项目 `Install.Engine`
- 将压缩包复制到项目中，并将属性 "生成操作" 设置为 "嵌入的资源"
- 如下在同一目录中添加一个类如下:

```c#
using A.Install.Engine;

namespace A.InstallMain
{
    [Install(rootDirectory: @"C:\Lisa", name: "Main", AutoStartup = "Main.exe")]
    internal sealed class Installer : InstallerBase
    {
    }
}
```

- 在主程序中添加如下调用
```c#
using A.InstallMain;

namespace InstallMain
{
    internal class Program
    {
        static void Main()
        {
            using (Installer installer = new Installer())
            {
                installer.Install();
            }
        }
    }
}
```
