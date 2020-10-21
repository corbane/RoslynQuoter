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
          readonly CheckMenuItem openNL;
          readonly CheckMenuItem closeNL;
          readonly CheckMenuItem preserveWS;
          readonly CheckMenuItem keepApi;
          readonly CheckMenuItem staticUsing;
          readonly ButtonMenuItem subMenu;
          readonly RadioMenuItem kindFile;
          readonly RadioMenuItem kindMember;
          readonly RadioMenuItem kindStatement;
          readonly RadioMenuItem kindExpression;

          public MainForm ()
          {
               Title = "My Eto Form";
               ClientSize = new Size (800, 600);

               quoter = new Quoter ();
               lBox = new TextArea ();
               rBox = new TextArea ();

               openNL = new CheckMenuItem
               {
                    Text = "'(' + new line",
                    ToolTip = "Open parenthesis on a new line",
               };
               closeNL = new CheckMenuItem
               {
                    Text = "new line + ')'",
                    ToolTip = "Closing parenthesis on a new line",
               };
               preserveWS = new CheckMenuItem
               {
                    Text = "Whitespace",
                    ToolTip = "Preserve original whitespace"
               };
               keepApi = new CheckMenuItem
               {
                    Text = "Keep API",
                    ToolTip = "Keep redundant API calls",
                    Checked = true
               };
               staticUsing = new CheckMenuItem
               {
                    Text = "using static",
                    ToolTip = "Do require 'using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;'",
                    Checked = true
               };

               //var nodeKind = new MenuItem ();

               var layout =  new DynamicLayout
               {
                    Padding = 10,
               };
               
               layout.BeginHorizontal(yscale: true);
               layout.Add(lBox, xscale: true);
               layout.Add(rBox, xscale: true);
               layout.EndHorizontal();

               Content = layout;

               kindFile = new RadioMenuItem { Text = "File", Checked = true };
               kindMember = new RadioMenuItem (kindFile) { Text = "Member" };
               kindStatement = new RadioMenuItem (kindFile) { Text = "Statement" };
               kindExpression = new RadioMenuItem (kindFile) { Text = "Expression" };
               subMenu = new ButtonMenuItem
               {
                    Text = "Parse as File",
                    Items = { kindFile, kindMember, kindStatement, kindExpression }
               };

               Menu = new MenuBar (
                    openNL, closeNL, preserveWS, keepApi, staticUsing, new SeparatorMenuItem (), subMenu
               );

               lBox.TextChanged += OnUpdate;
               openNL.CheckedChanged += OnUpdate;
               closeNL.CheckedChanged += OnUpdate;
               preserveWS.CheckedChanged += OnUpdate;
               keepApi.CheckedChanged += OnUpdate;
               staticUsing.CheckedChanged += OnUpdate;
               kindFile.CheckedChanged += OnKindUpdate;
               kindMember.CheckedChanged += OnKindUpdate;
               kindStatement.CheckedChanged += OnKindUpdate;
               kindExpression.CheckedChanged += OnKindUpdate;
          }
          private void OnKindUpdate (object sender, EventArgs e)
          {
               subMenu.Text = "Parse as " + ((RadioMenuItem)sender).Text;
               OnUpdate (sender, e);
          }

          private void OnUpdate (object sender, EventArgs e)
          {
               var sourceNode = CSharpSyntaxTree.ParseText(lBox.Text).GetRoot() as CSharpSyntaxNode;

               quoter.OpenParenthesisOnNewLine = openNL.Checked;
               quoter.ClosingParenthesisOnNewLine = closeNL.Checked;
               quoter.UseDefaultFormatting = !preserveWS.Checked;
               quoter.RemoveRedundantModifyingCalls = keepApi.Checked;
               quoter.ShortenCodeWithUsingStatic = staticUsing.Checked;

               var kind
                    = kindFile.Checked ? NodeKind.CompilationUnit
                    : kindMember.Checked ? NodeKind.MemberDeclaration
                    : kindStatement.Checked ? NodeKind.Statement
                    : NodeKind.Expression;


               var generatedNode = quoter.Quote(sourceNode.ToFullString (), kind);
               var generatedCode = quoter.Print(generatedNode);

               rBox.Text = generatedCode;
          }
     }
}
