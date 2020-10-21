using System;
using Eto.Forms;

namespace QuoterEto.Wpf
{
     class MainClass
     {
          [STAThread]
          public static void Main (string[ ] args)
          {
               new Application (Eto.Platforms.Wpf).Run (new MainForm ());
          }
     }
}
