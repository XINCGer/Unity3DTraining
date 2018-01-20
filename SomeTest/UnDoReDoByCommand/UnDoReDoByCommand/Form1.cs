using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnDoReDoByCommand.Command;

namespace UnDoReDoByCommand
{
    public partial class Form1 : Form
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        private string oldStr = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ICommand command = new TextChangedCommand(this.textBox1, textBox1.Text, oldStr, textBox1_TextChanged);
            undoStack.Push(command);
            oldStr = this.textBox1.Text;
        }

        private void undoBtn_Click(object sender, EventArgs e)
        {
            if (undoStack.Count == 0) return;

            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }

        private void redoBtn_Click(object sender, EventArgs e)
        {
            if (redoStack.Count == 0) return;

            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}
