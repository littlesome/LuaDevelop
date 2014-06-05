using System;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using PluginCore.Localization;
using System.Windows.Forms;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;

namespace LuaDevelop.Settings
{
    public partial class SettingObject : ISettings
    {
        #region Folding

        [DefaultValue(false)]
        [DisplayName("Fold At Else")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.FoldAtElse")]
        public Boolean FoldAtElse
        {
            get { return this.foldAtElse; }
            set { this.foldAtElse = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Comment")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.FoldComment")]
        public Boolean FoldComment
        {
            get { return this.foldComment; }
            set { this.foldComment = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use Compact Folding")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.FoldCompact")]
        public Boolean FoldCompact
        {
            get { return this.foldCompact; }
            set { this.foldCompact = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Html")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.FoldHtml")]
        public Boolean FoldHtml
        {
            get { return this.foldHtml; }
            set { this.foldHtml = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Preprocessor")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.FoldPreprocessor")]
        public Boolean FoldPreprocessor
        {
            get { return this.foldPreprocessor; }
            set { this.foldPreprocessor = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Enable Folding")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [LocalizedDescription("LuaDevelop.Description.UseFolding")]
        public Boolean UseFolding
        {
            get { return this.useFolding; }
            set { this.useFolding = value; }
        }

        [DisplayName("Fold Flags")]
        [LocalizedCategory("LuaDevelop.Category.Folding")]
        [DefaultValue(ScintillaNet.Enums.FoldFlag.LineAfterContracted)]
        [LocalizedDescription("LuaDevelop.Description.FoldFlags")]
        public ScintillaNet.Enums.FoldFlag FoldFlags
        {
            get { return this.foldFlags; }
            set { this.foldFlags = value; }
        }

        #endregion

        #region Display

        [DefaultValue(false)]
        [DisplayName("View EOL Characters")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewEOL")]
        public Boolean ViewEOL
        {
            get { return this.viewEOL; }
            set { this.viewEOL = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Bookmarks")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewBookmarks")]
        public Boolean ViewBookmarks
        {
            get { return this.viewBookmarks; }
            set { this.viewBookmarks = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Line Numbers")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewLineNumbers")]
        public Boolean ViewLineNumbers
        {
            get { return this.viewLineNumbers; }
            set { this.viewLineNumbers = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Indentation Guides")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewIndentationGuides")]
        public Boolean ViewIndentationGuides
        {
            get { return this.viewIndentationGuides; }
            set { this.viewIndentationGuides = value; }
        }

        [DefaultValue(false)]
        [DisplayName("View Whitespace Characters")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewWhitespace")]
        public Boolean ViewWhitespace
        {
            get { return this.viewWhitespace; }
            set { this.viewWhitespace = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View ToolBar")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewToolBar")]
        public Boolean ViewToolBar
        {
            get { return this.viewToolBar; }
            set { this.viewToolBar = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View StatusBar")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewStatusBar")]
        public Boolean ViewStatusBar
        {
            get { return this.viewStatusBar; }
            set { this.viewStatusBar = value; }
        }

        [DefaultValue(false)]
        [DisplayName("View Modified Lines")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ViewModifiedLines")]
        public Boolean ViewModifiedLines
        {
            get { return this.viewModifiedLines; }
            set { this.viewModifiedLines = value; }
        }

        [DisplayName("ComboBox Flat Style")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ComboBoxFlatStyle")]
        [DefaultValue(FlatStyle.Popup)]
        public FlatStyle ComboBoxFlatStyle
        {
            get { return this.comboBoxFlatStyle; }
            set { this.comboBoxFlatStyle = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use List View Grouping")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.UseListViewGrouping")]
        public Boolean UseListViewGrouping
        {
            get { return this.useListViewGrouping; }
            set { this.useListViewGrouping = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use System UI Colors")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.UseSystemColors")]
        public Boolean UseSystemColors
        {
            get { return this.useSystemColors; }
            set { this.useSystemColors = value; }
        }

        [DisplayName("UI Render Mode")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.RenderMode")]
        [DefaultValue(UiRenderMode.Professional)]
        public UiRenderMode RenderMode
        {
            get { return this.uiRenderMode; }
            set { this.uiRenderMode = value; }
        }

        [XmlIgnore]
        [DisplayName("UI Console Font")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.ConsoleFont")]
        [DefaultValue(typeof(Font), "Courier New, 8.75pt")]
        public Font ConsoleFont
        {
            get { return this.consoleFont; }
            set { this.consoleFont = value; }
        }

        [XmlIgnore]
        [DisplayName("UI Default Font")]
        [LocalizedCategory("LuaDevelop.Category.Display")]
        [LocalizedDescription("LuaDevelop.Description.DefaultFont")]
        [DefaultValue(typeof(Font), "Tahoma, 8.25pt")]
        public Font DefaultFont
        {
            get { return this.defaultFont; }
            set { this.defaultFont = value; }
        }

        #endregion

        #region Editor

        [DefaultValue(false)]
        [DisplayName("Highlight Caret Line")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.CaretLineVisible")]
        public Boolean CaretLineVisible
        {
            get { return this.caretLineVisible; }
            set { this.caretLineVisible = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Disable Highlight Guide")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.HighlightGuide")]
        public Boolean HighlightGuide
        {
            get { return this.highlightGuide; }
            set { this.highlightGuide = value; }
        }

        [DefaultValue(0)]
        [DisplayName("Print Margin Column")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.PrintMarginColumn")]
        public Int32 PrintMarginColumn
        {
            get { return this.printMarginColumn; }
            set { this.printMarginColumn = value; }
        }

        [DisplayName("Virtual Space Mode")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.VirtualSpaceMode")]
        [DefaultValue(ScintillaNet.Enums.VirtualSpaceMode.RectangularSelection)]
        public ScintillaNet.Enums.VirtualSpaceMode VirtualSpaceMode
        {
            get { return this.virtualSpaceMode; }
            set { this.virtualSpaceMode = value; }
        }

        [DisplayName("End Of Line Mode")]
        [DefaultValue(ScintillaNet.Enums.EndOfLine.CRLF)]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.EOLMode")]
        public ScintillaNet.Enums.EndOfLine EOLMode
        {
            get { return this.eolMode; }
            set { this.eolMode = value; }
        }

        [DefaultValue(500)]
        [DisplayName("Caret Period")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.CaretPeriod")]
        public Int32 CaretPeriod
        {
            get { return this.caretPeriod; }
            set { this.caretPeriod = value; }
        }

        [DefaultValue(2)]
        [DisplayName("Caret Width")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.CaretWidth")]
        public Int32 CaretWidth
        {
            get { return this.caretWidth; }
            set { this.caretWidth = value; }
        }

        [DefaultValue(3000)]
        [DisplayName("Scroll Area Width")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.ScrollWidth")]
        public Int32 ScrollWidth
        {
            get { return this.scrollWidth; }
            set { this.scrollWidth = value; }
        }

        [DefaultValue("as")]
        [DisplayName("Default File Extension")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.DefaultFileExtension")]
        public String DefaultFileExtension
        {
            get { return this.defaultFileExtension; }
            set { this.defaultFileExtension = value; }
        }

        [DefaultValue(15000)]
        [DisplayName("Backup Interval")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.BackupInterval")]
        public Int32 BackupInterval
        {
            get { return this.backupInterval; }
            set { this.backupInterval = value; }
        }

        [DefaultValue(3000)]
        [DisplayName("File Poll Interval")]
        [LocalizedCategory("LuaDevelop.Category.Editor")]
        [LocalizedDescription("LuaDevelop.Description.FilePollInterval")]
        public Int32 FilePollInterval
        {
            get { return this.filePollInterval; }
            set { this.filePollInterval = value; }
        }

        #endregion

        #region Locale

        [DisplayName("Selected Locale")]
        [DefaultValue(LocaleVersion.en_US)]
        [LocalizedCategory("LuaDevelop.Category.Locale")]
        [LocalizedDescription("LuaDevelop.Description.LocaleVersion")]
        public LocaleVersion LocaleVersion
        {
            get { return this.localeVersion; }
            set { this.localeVersion = value; }
        }

        [DefaultValue(CodePage.UTF8)]
        [DisplayName("Default CodePage")]
        [LocalizedCategory("LuaDevelop.Category.Locale")]
        [LocalizedDescription("LuaDevelop.Description.DefaultCodePage")]
        public CodePage DefaultCodePage
        {
            get { return this.defaultCodePage; }
            set { this.defaultCodePage = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Create Unicode With BOM")]
        [LocalizedCategory("LuaDevelop.Category.Locale")]
        [LocalizedDescription("LuaDevelop.Description.SaveUnicodeWithBOM")]
        public Boolean SaveUnicodeWithBOM
        {
            get { return this.saveUnicodeWithBOM; }
            set { this.saveUnicodeWithBOM = value; }
        }

        #endregion

        #region Indenting

        [DefaultValue(false)]
        [DisplayName("Use Backspace Unindents")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.BackSpaceUnIndents")]
        public Boolean BackSpaceUnIndents
        {
            get { return this.backSpaceUnIndents; }
            set { this.backSpaceUnIndents = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Indents")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.TabIndents")]
        public Boolean TabIndents
        {
            get { return this.tabIndents; }
            set { this.tabIndents = value; }
        }

        [DefaultValue(ScintillaNet.Enums.IndentView.Real)]
        [DisplayName("Indent Guide Type")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.IndentView")]
        public ScintillaNet.Enums.IndentView IndentView
        {
            get { return this.indentView; }
            set { this.indentView = value; }
        }

        [DisplayName("Smart Indent Type")]
        [DefaultValue(ScintillaNet.Enums.SmartIndent.CPP)]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.SmartIndentType")]
        public ScintillaNet.Enums.SmartIndent SmartIndentType
        {
            get { return this.smartIndentType; }
            set { this.smartIndentType = value; }
        }

        [DisplayName("Coding Style Type")]
        [DefaultValue(CodingStyle.BracesAfterLine)]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.CodingStyle")]
        public CodingStyle CodingStyle
        {
            get { return this.codingStyle; }
            set { this.codingStyle = value; }
        }

        [DisplayName("Comment Block Indenting")]
        [DefaultValue(CommentBlockStyle.Indented)]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.CommentBlockStyle")]
        public CommentBlockStyle CommentBlockStyle
        {
            get { return this.commentBlockStyle; }
            set { this.commentBlockStyle = value; }
        }

        [DefaultValue(4)]
        [DisplayName("Indenting Size")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.IndentSize")]
        public Int32 IndentSize
        {
            get { return this.indentSize; }
            set { this.indentSize = value; }
        }

        [DefaultValue(4)]
        [DisplayName("Width Of Tab")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.TabWidth")]
        public Int32 TabWidth
        {
            get { return this.tabWidth; }
            set { this.tabWidth = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Characters")]
        [LocalizedCategory("LuaDevelop.Category.Indenting")]
        [LocalizedDescription("LuaDevelop.Description.UseTabs")]
        public Boolean UseTabs
        {
            get { return this.useTabs; }
            set { this.useTabs = value; }
        }

        #endregion

        #region Features

        [DefaultValue(true)]
        [DisplayName("Apply File Extension")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.ApplyFileExtension")]
        public Boolean ApplyFileExtension
        {
            get { return this.applyFileExtension; }
            set { this.applyFileExtension = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Automaticly Reload Modified Files")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.AutoReloadModifiedFiles")]
        public Boolean AutoReloadModifiedFiles
        {
            get { return this.autoReloadModifiedFiles; }
            set { this.autoReloadModifiedFiles = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use Sequential Tabbing")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.SequentialTabbing")]
        public Boolean SequentialTabbing
        {
            get { return this.sequentialTabbing; }
            set { this.sequentialTabbing = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Wrap Editor Text")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.WrapText")]
        public Boolean WrapText
        {
            get { return this.wrapText; }
            set { this.wrapText = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Brace Matching")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.BraceMatchingEnabled")]
        public Boolean BraceMatchingEnabled
        {
            get { return this.braceMatchingEnabled; }
            set { this.braceMatchingEnabled = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Line Comments After Indent")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.LineCommentsAfterIndent")]
        public Boolean LineCommentsAfterIndent
        {
            get { return this.lineCommentsAfterIndent; }
            set { this.lineCommentsAfterIndent = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Move Cursor After Comment")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.MoveCursorAfterComment")]
        public Boolean MoveCursorAfterComment
        {
            get { return this.moveCursorAfterComment; }
            set { this.moveCursorAfterComment = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Restore File States")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.RestoreFileStates")]
        public Boolean RestoreFileStates
        {
            get { return this.restoreFileStates; }
            set { this.restoreFileStates = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Restore File Session")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.RestoreFileSession")]
        public Boolean RestoreFileSession
        {
            get { return this.restoreFileSession; }
            set { this.restoreFileSession = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Confirm On Exit")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.ConfirmOnExit")]
        public Boolean ConfirmOnExit
        {
            get { return this.confirmOnExit; }
            set { this.confirmOnExit = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Replace In Files Confirm")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.DisableReplaceFilesConfirm")]
        public Boolean DisableReplaceFilesConfirm
        {
            get { return this.disableReplaceFilesConfirm; }
            set { this.disableReplaceFilesConfirm = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Redirect Find In Files Results")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.RedirectFilesResults")]
        public Boolean RedirectFilesResults
        {
            get { return this.redirectFilesResults; }
            set { this.redirectFilesResults = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Option Sync")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.DisableFindOptionSync")]
        public Boolean DisableFindOptionSync
        {
            get { return this.disableFindOptionSync; }
            set { this.disableFindOptionSync = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Text Updating")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.DisableFindTextUpdating")]
        public Boolean DisableFindTextUpdating
        {
            get { return this.disableFindTextUpdating; }
            set { this.disableFindTextUpdating = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Simple Quick Find")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.DisableSimpleQuickFind")]
        public Boolean DisableSimpleQuickFind
        {
            get { return this.disableSimpleQuickFind; }
            set { this.disableSimpleQuickFind = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Tab Differentiation")]
        [LocalizedCategory("LuaDevelop.Category.Features")]
        [LocalizedDescription("LuaDevelop.Description.DisableTabDifferentiation")]
        public Boolean DisableTabDifferentiation
        {
            get { return this.disableTabDifferentiation; }
            set { this.disableTabDifferentiation = value; }
        }

        #endregion

        #region Formatting

        [DefaultValue(false)]
        [DisplayName("Strip Trailing Spaces")]
        [LocalizedCategory("LuaDevelop.Category.Formatting")]
        [LocalizedDescription("LuaDevelop.Description.StripTrailingSpaces")]
        public Boolean StripTrailingSpaces
        {
            get { return this.stripTrailingSpaces; }
            set { this.stripTrailingSpaces = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Ensure Consistent Line Ends")]
        [LocalizedCategory("LuaDevelop.Category.Formatting")]
        [LocalizedDescription("LuaDevelop.Description.EnsureConsistentLineEnds")]
        public Boolean EnsureConsistentLineEnds
        {
            get { return this.ensureConsistentLineEnds; }
            set { this.ensureConsistentLineEnds = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Ensure Last Line End")]
        [LocalizedCategory("LuaDevelop.Category.Formatting")]
        [LocalizedDescription("LuaDevelop.Description.EnsureLastLineEnd")]
        public Boolean EnsureLastLineEnd
        {
            get { return this.ensureLastLineEnd; }
            set { this.ensureLastLineEnd = value; }
        }

        #endregion

        #region State

        [DisplayName("Latest Startup Command")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.LatestCommand")]
        public Int32 LatestCommand
        {
            get { return this.latestCommand; }
            set { this.latestCommand = value; }
        }

        [DisplayName("Last Active Path")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.LatestDialogPath")]
        public String LatestDialogPath
        {
            get { return this.latestDialogPath; }
            set { this.latestDialogPath = value; }
        }

        [DisplayName("Window Size")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.WindowSize")]
        public Size WindowSize
        {
            get { return this.windowSize; }
            set { this.windowSize = value; }
        }

        [DisplayName("Window State")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.WindowState")]
        public FormWindowState WindowState
        {
            get { return this.windowState; }
            set { this.windowState = value; }
        }

        [DisplayName("Window Position")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.WindowPosition")]
        public Point WindowPosition
        {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

        [DisplayName("Previous Documents")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.PreviousDocuments")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> PreviousDocuments
        {
            get { return this.previousDocuments; }
            set { this.previousDocuments = value; }
        }

        [DisplayName("Disabled Plugins")]
        [LocalizedCategory("LuaDevelop.Category.State")]
        [LocalizedDescription("LuaDevelop.Description.DisabledPlugins")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> DisabledPlugins
        {
            get { return this.disabledPlugins; }
            set { this.disabledPlugins = value; }
        }

        #endregion

        #region Controls

        [DefaultValue(500)]
        [DisplayName("Hover Delay")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.HoverDelay")]
        public Int32 HoverDelay
        {
            get { return this.uiHoverDelay; }
            set
            {
                this.uiHoverDelay = value;
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    if (doc.IsEditable) doc.SciControl.MouseDwellTime = uiHoverDelay;
                }
            }
        }

        [DefaultValue(100)]
        [DisplayName("Display Delay")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.DisplayDelay")]
        public Int32 DisplayDelay
        {
            get { return this.uiDisplayDelay; }
            set { this.uiDisplayDelay = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Show Details")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.ShowDetails")]
        public Boolean ShowDetails
        {
            get { return this.uiShowDetails; }
            set { this.uiShowDetails = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Auto Filter List")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.AutoFilterList")]
        public Boolean AutoFilterList
        {
            get { return this.uiAutoFilterList; }
            set { this.uiAutoFilterList = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Enable AutoHide")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.EnableAutoHide")]
        public Boolean EnableAutoHide
        {
            get { return this.uiEnableAutoHide; }
            set { this.uiEnableAutoHide = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Wrap List")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.WrapList")]
        public Boolean WrapList
        {
            get { return this.uiWrapList; }
            set { this.uiWrapList = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Smart Matching")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.DisableSmartMatch")]
        public Boolean DisableSmartMatch
        {
            get { return this.uiDisableSmartMatch; }
            set { this.uiDisableSmartMatch = value; }
        }

        [DefaultValue("")]
        [DisplayName("Completion List Insertion Triggers")]
        [LocalizedCategory("LuaDevelop.Category.Controls")]
        [LocalizedDescription("LuaDevelop.Description.InsertionTriggers")]
        public String InsertionTriggers
        {
            get { return this.uiInsertionTriggers; }
            set { this.uiInsertionTriggers = value; }
        }

        #endregion

        #region Paths

        [DefaultValue("")]
        [DisplayName("Custom Snippet Directory")]
        [LocalizedCategory("LuaDevelop.Category.Paths")]
        [LocalizedDescription("LuaDevelop.Description.CustomSnippetDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomSnippetDir
        {
            get { return this.customSnippetDir; }
            set { this.customSnippetDir = value; }
        }

        [DefaultValue("")]
        [DisplayName("Custom Template Directory")]
        [LocalizedCategory("LuaDevelop.Category.Paths")]
        [LocalizedDescription("LuaDevelop.Description.CustomTemplateDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomTemplateDir
        {
            get { return this.customTemplateDir; }
            set { this.customTemplateDir = value; }
        }

        [DefaultValue("")]
        [DisplayName("Custom Projects Directory")]
        [LocalizedCategory("LuaDevelop.Category.Paths")]
        [LocalizedDescription("LuaDevelop.Description.CustomProjectsDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomProjectsDir
        {
            get { return this.customProjectsDir; }
            set { this.customProjectsDir = value; }
        }

        #endregion

    }

}
