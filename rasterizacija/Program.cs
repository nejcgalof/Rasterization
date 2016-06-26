using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace rasterization
{
    static class Program
    {
        /// <summary>
        /// //author: NEJC GALOF; galof.nejc@gmail.com
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
