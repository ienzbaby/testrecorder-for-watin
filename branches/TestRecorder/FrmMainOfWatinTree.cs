using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using IfacesEnumsStructsClasses;
using SourceGrid;
using TestRecorder.Core;
using TestRecorder.Core.Actions;
using TestRecorder.UserControls;
using WatiN.Core.Native.InternetExplorer;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;

namespace TestRecorder
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// Load the elements tree
        /// </summary>
        /// <param name="internetExplorer">Internet Explorer instance, we are attached to</param>
        private void LoadWatinTree(WatiN.Core.IE ie)
        {
            return ;/*hao*///====为提高开始速度，暂时屏蔽======

            if (ie != null)
            {
                treeWatin.Nodes.Clear();
                m_RootNode = new TreeNode("Web Page");
                treeWatin.Nodes.Add(m_RootNode);

                WatiN.Core.FrameCollection frames = ie.Frames;

                AddIEElements(ie);//=====添加根元素===
                //foreach (WatiN.Core.Frame frame in frames) {  AddFrameElements(frame, m_RootNode); }
                
                m_RootNode.Expand();
            }
        }
        /// <summary>
        /// Add an new element node to a parent treenode
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="parentNode">The parent node</param>
        private void AddElementToNode(WatiN.Core.Element element, TreeNode parentNode)
        {
            var newNode = new TreeNode();
            string elementName = element.GetAttributeValue("name");
            string elementValue = element.GetAttributeValue("value");

            bool isRadioButton = element.GetType() == typeof(WatiN.Core.RadioButton);
            bool isImage = element.GetType() == typeof(WatiN.Core.Image);

            if ((elementName != null) || (isRadioButton))
            {
                newNode.ForeColor = Color.Green;
                if (isRadioButton)
                {
                    newNode.Text = element.NextSibling.Text;
                }
                else
                    newNode.Text = element.Text + "Name: '" + elementName + "'";
            }
            else
            {
                if (elementValue != null)
                {
                    newNode.ForeColor = Color.DarkOrange;
                    newNode.Text = element.Text + ", Value: '" + elementValue + "'";
                }
                else if (element.Text != null)
                {
                    newNode.ForeColor = Color.Orange;
                    newNode.Text = element.Text + "(Only text)";
                }
                else if ((element.TagName != null) && (isImage))
                {
                    newNode.ForeColor = Color.Orange;
                    newNode.Text = element.GetAttributeValue("src") + "(Image)";
                }
                else
                {
                    newNode.ForeColor = Color.Red;
                    newNode.Text = element.Text + ", (No name, value or text!)";
                }
            }

            if (!string.IsNullOrEmpty(newNode.Text) || isRadioButton || isImage)
            {
                newNode.Tag = ((IEElement)element.NativeElement).AsHtmlElement;
                parentNode.Nodes.Add(newNode);
            }
        }
        /// <summary>
        /// Add all elements to a specific frame
        /// </summary>
        /// <param name="frame">The frame containing all elements to add</param>
        /// <param name="parentNode"></param>
        private void AddFrameElements(WatiN.Core.Frame frame, TreeNode parentNode)
        {
            var frameNode = new TreeNode("Frame");

            //sometime frame has no name
            if (frame.Name!=null && frame.Name.Length > 0)
            {
                frameNode.Text = "Frame Name: " + frame.Name;
            }
            else if (frame.Title.Length > 0)
            {
                frameNode.Text = "Frame Title: " + frame.Title;
            }
            else if (frame.Id.Length > 0)
            {
                frameNode.Text = "Frame Id: " + frame.Id;
            }
            else if (frame.Url.Length > 0)
            {
                frameNode.Text = "Frame URL: " + frame.Url;
            }

            parentNode.Nodes.Add(frameNode);
            CreateControlTypeNodes(frame,frameNode);
        }
        /// <summary>
        /// Add all elements from the root of IE
        /// </summary>
        private void AddIEElements(WatiN.Core.Document doc)
        {
            var frameNode = new TreeNode(doc.Title) { Text = "No name" };
            if (doc.Title.Length > 0) frameNode.Text = doc.Title;
            m_RootNode.Nodes.Add(frameNode);
            CreateControlTypeNodes(doc,frameNode);
        }

        /// <summary>
        /// Add a controltype node to the parent node
        /// </summary>
        /// <param name="parentNode">The parent node</param>
        /// <param name="controlTypeNode">The node that was created as root controltype node</param>
        /// <param name="text">The controltype text</param>
        private void AddControlTypeNode(TreeNode parentNode, out TreeNode controlTypeNode, string text)
        {
            controlTypeNode = new TreeNode { Text = text };
            parentNode.Nodes.Add(controlTypeNode);
        }
        /// <summary>
        /// Creates all the controltype nodes
        /// </summary>
        /// <param name="parentNode">The parent node to attach all the controltype nodes to</param>
        private void CreateControlTypeNodes(WatiN.Core.Document doc, TreeNode parentNode)
        {
            AddControlTypeNode(parentNode, out m_ButtonRootNode, "Buttons");
            foreach (WatiN.Core.Button item in doc.Buttons) { AddElementToNode(item, m_ButtonRootNode); }
            AddControlTypeNode(parentNode, out m_CheckBoxRootNode, "Checkboxes");
            foreach (WatiN.Core.CheckBox item in doc.CheckBoxes) { AddElementToNode(item, m_CheckBoxRootNode); }
            AddControlTypeNode(parentNode, out m_FrameRootNode, "Frames");
            foreach (WatiN.Core.Frame item in doc.Frames) { AddFrameElements(item, m_FrameRootNode); }
            AddControlTypeNode(parentNode, out m_ButtonRootNode, "Divs");
            foreach (WatiN.Core.Div item in doc.Divs) { AddElementToNode(item, m_ButtonRootNode); }
            AddControlTypeNode(parentNode, out m_HtmlDialogRootNode, "HTML Dialogs");
            //foreach (WatiN.Core.HtmlDialog item in doc.HtmlDialogs) { AddElementToNode(m_HtmlDialogRootNode, item); }
            AddControlTypeNode(parentNode, out m_ImageRootNode, "Images");
            foreach (WatiN.Core.Image item in doc.Images) { AddElementToNode(item, m_ImageRootNode); }
            AddControlTypeNode(parentNode, out m_LabelRootNode, "Labels");
            foreach (WatiN.Core.Label item in doc.Labels) { AddElementToNode(item, m_LabelRootNode); }
            AddControlTypeNode(parentNode, out m_LinkRootNode, "Links");
            foreach (WatiN.Core.Link item in doc.Links) { AddElementToNode(item, m_LinkRootNode); }
            AddControlTypeNode(parentNode, out m_RadioButtonRootNode, "Radio Buttons");
            foreach (WatiN.Core.RadioButton item in doc.RadioButtons) { AddElementToNode(item, m_RadioButtonRootNode); }
            AddControlTypeNode(parentNode, out m_SelectListRootNode, "Select Lists");
            foreach (WatiN.Core.SelectList item in doc.SelectLists) { AddElementToNode(item, m_SelectListRootNode); }
            AddControlTypeNode(parentNode, out m_TableCellRootNode, "Table Cells");
            foreach (WatiN.Core.TableCell item in doc.TableCells) { AddElementToNode(item, m_TableCellRootNode); }
            AddControlTypeNode(parentNode, out m_TableRowRootNode, "Table Rows");
            foreach (WatiN.Core.TableRow item in doc.TableRows) { AddElementToNode(item, m_TableRowRootNode); }
            AddControlTypeNode(parentNode, out m_TableRootNode, "Tables");
            foreach (WatiN.Core.Table item in doc.Tables) { AddElementToNode(item, m_TableRootNode); }
            AddControlTypeNode(parentNode, out m_TextFieldRootNode, "Text Fields");
            foreach (WatiN.Core.TextField item in doc.TextFields) { AddElementToNode(item, m_TextFieldRootNode); }

            parentNode.Expand();
        }

        private void treeWatin_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var clickedPoint = new Point(e.X, e.Y);
                TreeNode selectedNode = treeWatin.GetNodeAt(clickedPoint);
                if (selectedNode == null) return;
                var element = (IHTMLElement)selectedNode.Tag;
                if (element != null)
                {
                    try
                    {
                        HighlightElement(element);
                        //ShowPropertyWindow(element, clickPoint);
                    }
                    catch (Exception ex)
                    {
                        FormHelper.FrmLog.AppendToLog("treeWatin_AfterSelect Error: " + ex.Message);
                    }
                }
            }
        }

        private void treeWatin_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                try
                {
                    var element = (IHTMLElement)e.Node.Tag;
                    HighlightElement(element);
                }
                catch (Exception ex)
                {
                    FormHelper.FrmLog.AppendToLog("treeWatin_AfterSelect Error: " + ex.Message);
                }
            }
        }
    }
}
