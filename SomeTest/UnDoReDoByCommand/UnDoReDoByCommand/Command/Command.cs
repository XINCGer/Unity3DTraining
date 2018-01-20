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
        private EventHandler textEvent;

        public TextChangedCommand(TextBox textBox, string newStr, string oldStr,EventHandler textEvent)
        {
            this.textbox = textBox;
            this.newStr = newStr;
            this.oldStr = oldStr;
            this.textEvent = textEvent;
        }

        public void Execute()
        {
            SetText(newStr);
        }

        public void Undo()
        {
            SetText(oldStr);
        }

        private void SetText(string text)
        {
            this.textbox.TextChanged -= textEvent;
            this.textbox.Text = text;
            this.textbox.TextChanged += textEvent;
            this.textbox.SelectionStart = this.textbox.TextLength;
        }
    }
}
