﻿using System;
using Eto.Forms;

namespace QuoterEto.Mac
{
     class MainClass
     {
          [STAThread]
          public static void Main (string[ ] args)
          {
               new Application (Eto.Platforms.Mac64).Run (new MainForm ());
          }
     }
}
