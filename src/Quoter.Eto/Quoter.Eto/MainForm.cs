using System;
using Eto.Forms;
using Eto.Drawing;
using RoslynQuoter;
using Microsoft.CodeAnalysis.CSharp;

namespace QuoterEto
{
     public partial class MainForm : Form
     {
          readonly Quoter quoter;
          readonly TextArea lBox;
          readonly TextArea rBox;
          readonly CheckToolItem openNL;
          readonly CheckToolItem closeNL;
          readonly CheckToolItem preserveWS;
          readonly CheckToolItem keepApi;
          readonly CheckToolItem staticUsing;

          public MainForm ()
          {
               Title = "My Eto Form";
               ClientSize = new Size (400, 350);

               quoter = new Quoter ();
               lBox = new TextArea ();
               rBox = new TextArea ();

               openNL = new CheckToolItem {
                    Text = "'(' + new line",
                    ToolTip = "Open parenthesis on a new line",
               };
               closeNL = new CheckToolItem
               {
                    Text = "new line + ')'",
                    ToolTip = "Closing parenthesis on a new line",
               };
               preserveWS = new CheckToolItem
               {
                    Text = "Whitespace",
                    ToolTip = "Preserve original whitespace"
               };
               keepApi = new CheckToolItem
               {
                    Text = "Keep API",
                    ToolTip = "Keep redundant API calls"
               };
               staticUsing = new CheckToolItem {
                    Text = "using static",
                    ToolTip = "Do require 'using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;'"
               };

               var layout =  new DynamicLayout
               {
                    Padding = 10,
               };
               
               layout.BeginHorizontal(yscale: true);
               layout.Add(lBox, xscale: true);
               layout.Add(rBox, xscale: true);
               layout.EndHorizontal();

               Content = layout;
               ToolBar = new ToolBar ()
               {
                    Items = { openNL, closeNL, preserveWS, keepApi, staticUsing }
               };

               lBox.TextChanged += OnUpdate;
               openNL.CheckedChanged += OnUpdate;
               closeNL.CheckedChanged += OnUpdate;
               preserveWS.CheckedChanged += OnUpdate;
               keepApi.CheckedChanged += OnUpdate;
               staticUsing.CheckedChanged += OnUpdate;
          }

          private void OnUpdate (object sender, EventArgs e)
          {
               var sourceNode = CSharpSyntaxTree.ParseText(lBox.Text).GetRoot() as CSharpSyntaxNode;

               quoter.OpenParenthesisOnNewLine = openNL.Checked;
               quoter.ClosingParenthesisOnNewLine = closeNL.Checked;
               quoter.UseDefaultFormatting = !preserveWS.Checked;
               quoter.RemoveRedundantModifyingCalls = keepApi.Checked;
               quoter.ShortenCodeWithUsingStatic = staticUsing.Checked;
               
               var generatedNode = quoter.Quote(sourceNode.ToFullString ());
               var generatedCode = quoter.Print(generatedNode);

               rBox.Text = generatedCode;
          }
     }
}
