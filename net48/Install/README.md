## ��װ�����ɲ���

- ��Ҫ��װ�ĳ����ļ��� (Debug �� Release) ѹ���� zip �ļ�����Ϊ���������
- �½�һ������̨����������Ŀ `Install.Engine`
- ��ѹ�������Ƶ���Ŀ�У��������� "���ɲ���" ����Ϊ "Ƕ�����Դ"
- ������ͬһĿ¼�����һ��������:

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

- ����������������µ���
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
