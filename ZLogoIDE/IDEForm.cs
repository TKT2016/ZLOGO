using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZCompileCore.Reports;

namespace ZLogoIDE
{
    public partial class IDEForm : Form
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private String FileFullPath;

        Boolean textChangedFlag = false;
        private const string EditorName = "ZLogo";


        private CompileMsgForm compileMsgForm;
        public IDEForm()
        {
            InitializeComponent();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.Text = "无标题 - " + EditorName;
            compileMsgForm = new CompileMsgForm();
        }

        private void runStripButton_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                compileMsgForm.Hide();
                ZLogoCompiler compiler = new ZLogoCompiler(new FileInfo( FileFullPath));
                compiler.Compile();
                if (compiler.CompileResult.HasError())
                {
                    StringBuilder buffBuilder = new StringBuilder();
                    buffBuilder.AppendFormat("文件'{0}{1}'有以下错误:\n", FileName,ZLogoCompiler.ZLogoExt);
                    foreach (CompileMessage compileMessage in compiler.CompileResult.Errors)
                    {
                        if (compileMessage.Line > 0 || compileMessage.Col > 0)
                        {
                            buffBuilder.AppendFormat("第{0}行,第{1}列", compileMessage.Line, compileMessage.Col);
                        }
                        buffBuilder.AppendFormat("错误:{0}\n", compileMessage.Text);
                        //string str = string.Format("第{0}行,第{1}列:{2}\n",,compileMessage.Text);
                        //buffBuilder.Append(str);
                    }
                    compileMsgForm.ShowMessage(buffBuilder.ToString());
                    compileMsgForm.Show();
                }
                else
                {
                    compileMsgForm.Hide();
                    compiler.Run();
                }
                
            }
        }

        private void newStripButton_Click(object sender, EventArgs e)
        {
            Save();
            this.textBoxEditor.Text = "";
            this.FileFullPath = null;
            textChangedFlag = false;
        }

        private String FileName
        {
            get
            {
                 if (string.IsNullOrEmpty(FileFullPath)) return null;
                if (!File.Exists(FileFullPath)) return null;
                return Path.GetFileNameWithoutExtension(FileFullPath);
            }
        }

        private void openStripButton_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            if (FileFullPath == null)
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                openFileDialog.InitialDirectory = (new FileInfo(FileFullPath)).DirectoryName;
            }
            DialogResult dialogResult = openFileDialog.ShowDialog();
            saveFileDialog.FileName = openFileDialog.FileName;
            if (dialogResult == DialogResult.OK)
            {
                FileFullPath = openFileDialog.FileName;
                StreamReader streamReader = new StreamReader(FileFullPath, System.Text.Encoding.Default);
                SetFormTitle();//this.Text = FileName+ openFileDialog.FileName.Substring( /*fileNameStatiIndex, fileNameLength)*/ + " - ") + EditorName;
                textBoxEditor.Text = streamReader.ReadToEnd();
                streamReader.Dispose();
            }              
        }

        private void SetFormTitle()
        {
            if (FileName == null)
            {
                this.Text = "无标题 - " + EditorName;
            }
            else
            {
                this.Text = FileName +" - " + EditorName;
            }
        }

        private void saveStripButton_Click(object sender, EventArgs e)
        {      
            Save(); 
        }

        private bool Save()
        {
            if (textChangedFlag)
            {
                if (FileFullPath == null)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileFullPath = saveFileDialog.FileName;
                        if (!FileFullPath.EndsWith(ZLogoCompiler.ZLogoExt, StringComparison.CurrentCultureIgnoreCase))
                        {
                            FileFullPath += ZLogoCompiler.ZLogoExt;
                        }
                        textBoxEditor.SaveFile(FileFullPath, RichTextBoxStreamType.PlainText);
                        SetFormTitle();
                        textChangedFlag = false;
                        return true;
                    }
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(FileFullPath);
                    fileInfo.Delete();
                    textBoxEditor.SaveFile(FileFullPath, RichTextBoxStreamType.PlainText);
                    textChangedFlag = false;
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            } 
        }

        private void saveAsStripButton_Click(object sender, EventArgs e)
        {
            if (FileName != null)
            {
                saveFileDialog.FileName = FileName + ZLogoCompiler.ZLogoExt;
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxEditor.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                textChangedFlag = false;
            }
        }

        private void aboutStripButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void IDEForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save();
        }

        private void textBoxEditor_TextChanged(object sender, EventArgs e)
        {
            textChangedFlag = true;
        }
    }
}
