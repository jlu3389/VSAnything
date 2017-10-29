using EnvDTE;
using SCLCoreCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Company.VSAnything
{
    public class DTE
    {
        private EnvDTE.DTE m_DTE;

        public EnvDTE.DTE EnvDTE
        {
            get
            {
                return this.m_DTE;
            }
        }

        public DTE(EnvDTE.DTE dte)
        {
            this.m_DTE = dte;
        }

        public void OpenSolution(string filename)
        {
            try
            {
                this.m_DTE.Solution.Open(filename);
            }
            catch (Exception arg_13_0)
            {
                Utils.LogException(arg_13_0);
            }
        }

        public void OpenFile(string filename)
        {
            try
            {
                this.m_DTE.ItemOperations.OpenFile(filename, "{00000000-0000-0000-0000-000000000000}");
            }
            catch (Exception arg_19_0)
            {
                Utils.LogException(arg_19_0);
            }
        }

        public void SetActiveDocumentLine(int line)
        {
            try
            {
                TextDocument text_doc = this.m_DTE.ActiveDocument.Object("") as TextDocument;
                if (text_doc != null)
                {
                    centerLine(line + 1);
                    text_doc.Selection.GotoLine(line + 1, true);
                }
            }
            catch (Exception arg_2F_0)
            {
                Utils.LogException(arg_2F_0);
            }
        }

        public void ActivateActiveDocument()
        {
            try
            {
                if (this.m_DTE.ActiveDocument != null)
                {
                    this.m_DTE.ActiveDocument.Activate();
                }
            }
            catch (Exception arg_1F_0)
            {
                Utils.LogExceptionQuiet(arg_1F_0);
            }
        }

        public string GetSelectedText()
        {
            try
            {
                if (this.m_DTE.ActiveDocument != null)
                {
                    TextSelection text_selection = this.m_DTE.ActiveDocument.Selection as TextSelection;
                    if (text_selection != null)
                    {
                        int start_line = text_selection.AnchorPoint.Line;
                        int end_line = text_selection.ActivePoint.Line;
                        if (start_line == end_line)
                        {
                            TextDocument text_doc = this.m_DTE.ActiveDocument.Object("") as TextDocument;
                            if (text_doc != null)
                            {
                                string line = text_doc.CreateEditPoint(null).GetLines(start_line, start_line + 1);
                                SettingsDialogPage settings_page = VSAnythingPackage.Inst.GetSettingsDialogPage();
                                string result;
                                if (line.Contains("#include"))
                                {
                                    int start = line.IndexOf('"');
                                    if (start != -1)
                                    {
                                        int end = line.IndexOf('"', start + 1);
                                        if (end != -1)
                                        {
                                            result = Path.GetFileName(line.Substring(start + 1, end - start - 1));
                                            return result;
                                        }
                                    }
                                }
                                else if (text_selection.Text.Length == 0 && settings_page.UseCurrentWordAsFindText && text_selection.TextRanges.Count > 0 && text_selection.AnchorPoint.LineCharOffset > 0)
                                {
                                    result = Utils.GetWord(line, text_selection.AnchorPoint.LineCharOffset - 1);
                                    return result;
                                }
                                result = text_selection.Text;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception arg_12D_0)
            {
                Utils.LogExceptionQuiet(arg_12D_0);
            }
            return null;
        }

        public string GetSolutionFilename()
        {
            try
            {
                if (this.m_DTE.Solution != null && this.m_DTE.Solution.IsOpen && this.m_DTE.Solution.FullName.Length != 0)
                {
                    return this.m_DTE.Solution.FullName;
                }
            }
            catch (Exception arg_4B_0)
            {
                Utils.LogExceptionQuiet(arg_4B_0);
            }
            return null;
        }

        public string GetActiveDocumentFilename()
        {
            try
            {
                if (this.m_DTE.ActiveDocument != null)
                {
                    return this.m_DTE.ActiveDocument.FullName;
                }
            }
            catch (Exception arg_22_0)
            {
                Utils.LogExceptionQuiet(arg_22_0);
            }
            return null;
        }

        private static void GetProjects(Project project, List<string> projects)
        {
            string project_kind = project.Kind;
            if (project_kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
            {
                IEnumerator enumerator = project.ProjectItems.GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        Project sub_project = ((ProjectItem)enumerator.Current).SubProject;
                        if (sub_project != null)
                        {
                            DTE.GetProjects(sub_project, projects);
                        }
                    }
                    return;
                }
            }
            if (project_kind != "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}")
            {
                string project_full_name = null;
                try
                {
                    project_full_name = project.FullName;
                }
                catch (Exception arg_72_0)
                {
                    Utils.LogExceptionQuiet(arg_72_0);
                }
                if (!string.IsNullOrEmpty(project_full_name))
                {
                    projects.Add(project_full_name);
                    Log.WriteLine("GetProjects found project " + project_full_name);
                }
            }
        }

        public List<string> GetProjects()
        {
            ProfileTimer timer = new ProfileTimer("GetProjects");
            List<string> projects = new List<string>();
            Log.WriteLine("GetProjects ---------");
            try
            {
                if (this.m_DTE.Solution != null)
                {
                    foreach (Project project in this.m_DTE.Solution.Projects)
                    {
                        try
                        {
                            DTE.GetProjects(project, projects);
                        }
                        catch (Exception arg_55_0)
                        {
                            Utils.LogExceptionQuiet(arg_55_0);
                        }
                    }
                }
            }
            catch (Exception arg_7C_0)
            {
                Utils.LogExceptionQuiet(arg_7C_0);
            }
            timer.Stop();
            return projects;
        }

        public void centerLine(int nNum)
        {
            ///VSAnything 窗口是否为autoHIde
            bool bIsAutoHide = false;

            ToolWindowPane window = VSAnythingPackage.Inst.FindToolWindow(typeof(FastFindToolWindowPane), 0, true);

            if (window != null && window.Frame != null)
            {
                Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame frame = (Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame)window.Frame;
                if (frame != null && frame.IsVisible() == 0) // 0 is visible !!!
                {
                    object currentFrameMode;

                    frame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out currentFrameMode);
                    if ((VsFrameMode)currentFrameMode == VsFrameMode.AutoHide)
                    {
                        bIsAutoHide = true;
                    }
                }

                IVsTextView textViewCurrent;
                var txtMngr = (IVsTextManager)VSAnythingPackage.Inst.GetService(typeof(SVsTextManager));
                txtMngr.GetActiveView(1, null, out textViewCurrent);

                /// 如果VSAnything窗口不是autoHide，简单地把改行滚动到Editor中间即可
                /// 但是在autoHide情况下，即使滚动到Editor中间，也可能被挡住。得计算出Editor的可视区域，将line滚到中间
                if (!bIsAutoHide)
                {
                    textViewCurrent.CenterLines(nNum, 1);
                }
                else
                {
                    int nMin;       // 当前页滚动条最小值
                    int nMax;       // 当前页滚动条最大值
                    int nPerPage;   // 当前页的高度等于多少滚动值
                    int nCurPos;    // 当前滚动条所在位置，能展示的区域为 [nCurPos,nCurPos + nPerPage]，即一页的内容

                    var scrollInfo = textViewCurrent.GetScrollInfo(1, out nMin, out nMax, out nPerPage, out nCurPos);

                    /// 获取VSAnything窗口大小，还需要加上toolbar高度
                    VSSETFRAMEPOS[] vars = new VSSETFRAMEPOS[20];
                    Guid guid;
                    int xToLeft, yToTop, width, height;

                    frame.GetFramePos(vars, out guid, out xToLeft, out yToTop, out width, out height);

                    /// todo ~~~
                    TextDocument text_doc = this.m_DTE.ActiveDocument.Object("") as TextDocument;
                    if (text_doc != null)
                    {

                    }
                    ////// todo ~~~
                    textViewCurrent.CenterLines(nNum, 1);
                    ////// 
                }
            }
        }
    }
}
