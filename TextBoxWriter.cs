using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQLDBCompare
{
    public class TextBoxWriter : System.IO.TextWriter
    {
        RichTextBox rtBox;
        delegate void VoidAction();

        public TextBoxWriter(RichTextBox box)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            rtBox = box;
        }
        public override void WriteLine(string value)
        {
            VoidAction action = delegate
            {
                try
                {
                    //保存信息到文件夹
                    //DbLogger.LogWrite(value);
                    //string[] strLines = rtBox.Text.Split('\n');

                    /*
                    if (strLines.Length > 1000)
                    {
                        rtBox.Clear();
                    }
                    */
                    if (!string.IsNullOrEmpty(value))
                    {
                        //让文本框获取焦点  
                        rtBox.Focus();
                        //设置光标的位置到文本尾  
                        rtBox.Select(rtBox.TextLength, 0);
                        //滚动到控件光标处  
                        rtBox.ScrollToCaret();
                        //rtBox.AppendText(string.Format("#[{0:HH:mm:ss}]{1}\r\n", DateTime.Now, value));
                        rtBox.AppendText(string.Format("{1}\r\n", DateTime.Now, value));
                        //更改颜色
                        //rtBox.ForeColor = Color.FromArgb(51, 255, 102);
                    }
                    else
                    {
                        rtBox.AppendText("暂无日志");
                    }


                }
                catch (Exception ex)
                {
                    //DbLogger.LogException(ex, ex.Message);
                }

            };
            if (rtBox.IsHandleCreated)
            {
                try
                {
                    rtBox.BeginInvoke(action);
                }
                catch { }
            }

        }
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

    }
}
