using System;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using TestRecorder.Core;

namespace TestRecorder
{
    public partial class frmMain : Form
    {
        private const string Textnode = "#text";
        private const string Commentnode = "#comment";
        private const string Framenode = "FRAME";
        private const string Iframenode = "IFRAME";
        private const string Basenode = "BASE";
        private const string Bodynode = "BODY";
        private const string Valueseperator = " \"";
        private const string Valueseperator1 = "\"";

        /// <summary>
        /// Starting point to walk the DOM
        /// </summary>
        /// <param name="documentObject">Webbrowser.Document object</param>
        private void LoadDOM(object documentObject)
        {
            treeDOM.Nodes.Clear();
            arrSelectItems.Clear();

            if (documentObject == null) return;

            try
            {
                var doc = documentObject as IHTMLDocument3;
                if (doc != null)
                {
                    var domnode = (IHTMLDOMNode)doc.documentElement;
                    //Start walking
                    if (domnode != null) ParseNodes(domnode, null);
                }
                if (treeDOM.Nodes.Count > 0) treeDOM.Nodes[0].Expand();
            }
            catch (Exception ex)
            {
                FormHelper.FrmLog.AppendToLog("Load DOM failed: " + ex.Message);
            }
        }
        /// <summary>
        /// Recursive method to walk the DOM, acounts for frames
        /// </summary>
        /// <param name="nd">Parent DOM node to walk</param>
        /// <param name="node">Parent tree node to populate</param>
        /// <returns></returns>
        private void ParseNodes(IHTMLDOMNode nd, TreeNode node)
        {
            if (nd == null) return;

            string str = nd.nodeName;
            TreeNode nextnode;

            //Add a new node to tree
            if (node != null)
            {
                nextnode = node.Nodes.Add(str);
                nextnode.Tag = nd as IHTMLElement;

                var elem = nd as IHTMLElement;
                if (str.ToLower() == "select")
                {
                    var sel = elem as mshtml.HTMLSelectElementClass;
                    if (sel != null)
                        if (arrSelectItems.IndexOf(sel.GetHashCode()) == -1)
                        {
                            arrSelectItems.Add(sel.GetHashCode());
                            sel.HTMLSelectElementEvents2_Event_onchange += SelHtmlSelectElementEvents2EventOnchange;
                        }
                }
            }

            else
            {
                nextnode = treeDOM.Nodes.Add(str);
                nextnode.Tag = nd as IHTMLElement;
            }

            //For each child, get children collection
            //And continue walking up and down the DOM
            try
            {
                //Frame?
                if (str == Framenode || str == Iframenode)
                {
                    //Get the nd.IWebBrowser2.IHTMLDocument3.documentelement and recurse
                    var wb = (IWebBrowser2)nd;
                    var doc3 = (IHTMLDocument3)wb.Document;

                    var tempnode = (IHTMLDOMNode)doc3.documentElement;
                    //get the comments for this node, if any
                    var framends = (IHTMLDOMChildrenCollection)doc3.childNodes;
                    foreach (IHTMLDOMNode tmpnd in framends)
                    {
                        str = tmpnd.nodeName;
                        if (Commentnode == str)
                        {
                            if (tmpnd.nodeValue != null)
                                str += Valueseperator + tmpnd.nodeValue + Valueseperator1;

                            TreeNode newnode = nextnode.Nodes.Add(str);
                            newnode.Tag = tmpnd as IHTMLElement;
                        }
                    }
                    //parse document
                    ParseNodes(tempnode, nextnode);
                    return;
                }

                //Get the DOM collection
                var nds = nd.childNodes;
                foreach (IHTMLDOMNode childnd in nds)
                {
                    string strdom = childnd.nodeName;
                    //Attempt to extract text and comments
                    if ((Commentnode == strdom) || (Textnode == strdom))
                    {
                        if (childnd.nodeValue != null)
                            strdom += Valueseperator + childnd.nodeValue + Valueseperator1;
                        //Add a new node to tree
                        TreeNode newnode = nextnode.Nodes.Add(strdom);
                        newnode.Tag = childnd as IHTMLElement;
                    }
                    else
                    {
                        if ((Bodynode == strdom) &&
                            (str == Basenode))
                        {
                            //In MSDN, one of the inner FRAMEs BASE element
                            //contains the BODY element???
                            //Do nothing
                        }
                        else
                        {
                            ParseNodes(childnd, nextnode);
                        }
                    }
                }
            }
            catch (InvalidCastException icee)
            {
                Console.Write("\r\n InvalidCastException =" +
                    icee + "\r\nName =" +
                    str + " \r\n");
            }
            return;
        }

        private void SelHtmlSelectElementEvents2EventOnchange(mshtml.IHTMLEventObj pEvtObj)
        {
            if (pEvtObj.srcElement.getAttribute("value", 0) == null)
            {
                wsManager.AddSelectListItem(watinIE, (pEvtObj.srcElement as IHTMLElement), false);
            }
            else
            {
                wsManager.AddSelectListItem(watinIE, (pEvtObj.srcElement as IHTMLElement), true);
            }

        }

        private void HighlightElement(IHTMLElement element)
        {
            try
            {
                if (lastelement != null)
                {
                    lastelement.style.setAttribute("backgroundColor", originalColor, 0);
                }

                if (element == null)
                {
                    return;
                }

                lastelement = element;
                Object objColor = lastelement.style.getAttribute("backgroundColor", 0);
                originalColor = objColor != null ? objColor.ToString() : "";
                lastelement.style.setAttribute("backgroundColor", wsManager.Settings.DOMHighlightColor.ToKnownColor(), 0);
            }
            catch (System.UnauthorizedAccessException)
            {
                this.lastelement = element;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot highlight this element.", "Highlight Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeDOM_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                var element = e.Node.Tag as IHTMLElement;
                HighlightElement(element);
            }
            catch (Exception ex)
            {
                FormHelper.FrmLog.AppendToLog("DOM Node Mouse Click Failed: " + ex.Message);
                tsStatus.Text = Properties.Resources.DOMErrorHighlighting;
                return;
            }
        }
    }
}
