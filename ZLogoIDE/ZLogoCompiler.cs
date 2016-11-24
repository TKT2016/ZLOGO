using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZCompileCore.Builders;
using ZCompileCore.Reports;
using ZLangRT;
using ZLangRT.Utils;
using ZLogoEngine;

namespace ZLogoIDE
{
    public class ZLogoCompiler
    {
        public const string ZLogoExt = ".zlogo";
        private ZCompileProjectModel projectModel;
        private ZCompileClassModel classModel;
        private FileInfo srcFileInfo;

        public ProjectCompileResult CompileResult { get; private set;}

        public ZLogoCompiler(FileInfo zlogoFileInfo)
        {
            srcFileInfo = zlogoFileInfo;
            /*if (!srcFileInfo.EndsWith(ZLogoExt, StringComparison.OrdinalIgnoreCase))
            {
                srcFileInfo +=ZLogoExt;
            }*/
            projectModel = new ZCompileProjectModel();
            classModel = new ZCompileClassModel();

            projectModel.ProjectRootDirectoryInfo = srcFileInfo.Directory;// AppDomain.CurrentDomain.BaseDirectory;
            projectModel.BinaryFileKind = PEFileKinds.Dll;
            projectModel.BinarySaveDirectoryInfo = srcFileInfo.Directory;
            projectModel.ProjectPackageName = "ZLogoIDE";
            projectModel.EntryClassName = Path.GetFileNameWithoutExtension(srcFileInfo.FullName);
            projectModel.BinaryFileNameNoEx = Path.GetFileNameWithoutExtension(srcFileInfo.FullName);
            projectModel.NeedSave = false;
            projectModel.AddRefPackage("Z语言系统");
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZLogoEngine.exe")));

            classModel.SourceFileInfo = srcFileInfo;
            classModel.PreSourceCode =
@"

使用包:ZLogoEngine;
简略使用:颜色,补语控制;

属于:绘图窗体;

";
            projectModel.AddClass(classModel);
        }

        public ProjectCompileResult Compile()
        {
            ZCompileBuilder builder = new ZCompileBuilder();
            ProjectCompileResult result = builder.CompileProject(projectModel);
            CompileResult= result;
            return result;
        }

        public void Run()
        {
            if (CompileResult == null)
            {
                Compile();
            }
            if (CompileResult == null)
            {
                return;
            }
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(turtleForm);

            if (CompileResult.CompiledTypes.Count > 0)
            {
                Type type = CompileResult.CompiledTypes[0];
                TurtleForm turtleForm = ReflectionUtil.NewInstance(type) as TurtleForm;
                turtleForm.Show();
                
            }
        }
    }
}
