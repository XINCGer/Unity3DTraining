using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnDoReDoByCommand.Command
{
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();

        /// <summary>
        /// 撤销执行命令
        /// </summary>
        void Undo();
    }

    public class TextChangedCommand : ICommand
    {
        private TextBox textbox;
        private string newStr;
        private string oldStr;

        public TextChangedCommand(TextBox textBox, string newStr, string oldStr)
        {
            this.textbox = textBox;
            this.newStr = newStr;
            this.oldStr = oldStr;
        }

        public void Execute()
        {
            this.textbox.Text = newStr;
            this.textbox.SelectionStart = this.textbox.TextLength;
        }

        public void Undo()
        {
            this.textbox.Text = oldStr;
            this.textbox.SelectionStart = this.textbox.TextLength;
        }
    }
}
