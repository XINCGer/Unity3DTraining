using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        public Form1()
        {
            InitializeComponent();
        }
    }
}
