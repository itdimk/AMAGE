using AMAGE.Common.Imaging;
using AMAGE.Imaging.Tools;
using AMAGE.Presentation.Properties;
using AMAGE.Presentation.View.ImageEditor;
using AMAGE.Services;
using AMAGE.Services.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMAGE.Presentation.Presenters
{
    public class ImageEditorPresenter : ImageViewerPresenter
    {
        protected readonly Dictionary<string, IImageList> Originals
            = new Dictionary<string, IImageList>();

        protected readonly Dictionary<string, int[]> LastSelectedFrames
            = new Dictionary<string, int[]>();

        protected readonly IImageListSupport[] ImageListTools;
        protected readonly IImageSupport[] ImageTools;

        public ImageEditorPresenter(IAppController appController, IImageEditor view, ILogService logService,
            IMessageService messageService, IRepository<IImageList> repository, ISlideshowService slideshowService,
            IImageListSupport[] imageListTools, IImageSupport[] imageTools)
            : base(appController, view, logService, messageService, repository, slideshowService)
        {
            ImageListTools = imageListTools;
            ImageTools = imageTools;

            if (imageListTools.Length > 0)
                View.ToolPanels.AddPanel(nameof(IImageListSupport), Resources.AnimatedTools).SetTools(ImageListTools);

            if (imageTools.Length > 0)
                View.ToolPanels.AddPanel(nameof(IImageSupport), Resources.StaticTools).SetTools(ImageTools);

            AppController.EventController
                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.ApplyTool), MenuPanel_ApplyTool)
                .Subscribe(View.ImagePanels, nameof(View.ImagePanels.SelectedPanelChanged), OnLoadDefaultSettings)
                .Subscribe(View.ToolPanels, nameof(View.ToolPanels.SelectedPanelChanged))
                .Subscribe<string>(Repository, nameof(Repository.ItemAdded), Repository_ItemAdded)
                .Subscribe<string>(Repository, nameof(Repository.ItemRemoved), Repository_ItemRemoved);

            foreach (IToolPanel toolPanel in View.ToolPanels)
            {
                AppController.EventController
                    .Subscribe(toolPanel, nameof(toolPanel.Tuning), ToolPanel_Tuning)
                    .AddCondition(toolPanel, nameof(toolPanel.Tuning), TuningCondition)

                    .Subscribe(toolPanel, nameof(toolPanel.SelectedToolChanged), ToolPanel_SelectedToolChanged,
                        OnLoadDefaultSettings);
            }

            AppController.EventController
                .AddCondition(View.MenuPanel, nameof(View.MenuPanel.ApplyTool), ApplyToolCondition);
        }

        #region Conditions

        protected virtual bool ApplyToolCondition() => View.ImagePanels.SelectedPanel != null
            && View.ToolPanels.SelectedPanel?.SelectedTool != null;

        protected virtual bool TuningCondition() => ApplyToolCondition();

        #endregion

        #region MenuPanel

        private void MenuPanel_ApplyTool(object sender, EventArgs e)
        {
            string imageKey = View.ImagePanels.SelectedPanelKey;

            if (!string.IsNullOrEmpty(imageKey))
                Originals.Remove(imageKey);
        }

        #endregion

        #region Repository

        private void Repository_ItemAdded(object sender, string e)
        {
            IImagePanel imagePanel = View.ImagePanels[e];

            object selectedTool = View.ToolPanels.SelectedPanel.SelectedTool;
            imagePanel.AreaSelection = selectedTool is ICustomAreaSupport;
            imagePanel.MultiSelection = selectedTool is ICustomFramesSupport;

            AppController.EventController
                .Subscribe(imagePanel, nameof(imagePanel.SelectedAreaChanged), ImagePanel_SelectedAreaChanged)
                .Subscribe(imagePanel, nameof(imagePanel.SelectedIconsChanged), ImagePanel_SelectedIconsChanged);
        }

        private void Repository_ItemRemoved(object sender, string e)
        {
            Originals.Remove(e);
            LastSelectedFrames.Remove(e);
        }

        #endregion

        #region Tool Panels

        private void ToolPanel_Tuning(object sender, EventArgs e)
        {
            IImagePanel imagePanel = View.ImagePanels.SelectedPanel;
            string imageKey = View.ImagePanels.SelectedPanelKey;
            object tool = ((IToolPanel)sender).SelectedTool;

            ICustomFramesSupport framesSupport = tool as ICustomFramesSupport;
            ICustomAreaSupport areaSupport = tool as ICustomAreaSupport;
            IAsyncApplyingSupport asyncSupport = tool as IAsyncApplyingSupport;

            if (framesSupport != null)
                framesSupport.CustomFrames = imagePanel.SelectedIcons;

            if (areaSupport != null)
                areaSupport.CustomArea = imagePanel.SelectedArea;

            if (asyncSupport != null)
            {
                asyncSupport.AllowMultipleTasks = false;
                asyncSupport.ApplyAsync = Settings.Default.UseAsyncMode;
                AppController.EventController.UnsubscribeAll(asyncSupport, nameof(asyncSupport.AsyncApplyingCompleted));
                AppController.EventController.Subscribe(asyncSupport, nameof(asyncSupport.AsyncApplyingCompleted),
                    delegate { Repository.OnItemChanged(imageKey); });
            }

            IImageListSupport imageListSupport = tool as IImageListSupport;
            IImageSupport imageSupport = tool as IImageSupport;

            try
            {
                if (!Originals.ContainsKey(imageKey))
                    Originals.Add(imageKey, Repository[imageKey].Clone());

                if (imageListSupport != null)
                {
                    imageListSupport.Apply(Originals[imageKey], Repository[imageKey]);

                    if (!Settings.Default.UseAsyncMode)
                        Repository.OnItemChanged(imageKey);
                }
                else if (imageSupport != null)
                {
                    int frame = imagePanel.SelectedIcons.ElementAtOrDefault(0);

                    imageSupport.Apply(Originals[imageKey][frame], Repository[imageKey][frame]);

                    if (!Settings.Default.UseAsyncMode)
                        Repository.OnItemChanged(imageKey);
                }
                else
                    throw new Exception("Unsupported tool type");
            }
            catch (OutOfMemoryException ex)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                MessageService.ShowError(ex.Message);
            }
#if !DEBUG
            catch(Exception ex)
            {
                MessageService.ShowError(Resources.ApplyToolError);
                LogService.LogError(ex);
            }
#endif
        }

        private void ToolPanel_SelectedToolChanged(object sender, EventArgs e)
        {
            IToolPanel toolPanel = (IToolPanel)sender;
            object tool = toolPanel.SelectedTool;

            ICustomFramesSupport framesSupport = tool as ICustomFramesSupport;
            ICustomAreaSupport areaSupport = tool as ICustomAreaSupport;
            ITunerSupport tunerSupport = tool as ITunerSupport;

            toolPanel.SetTuner(tunerSupport?.Tuner);

            foreach (IImagePanel imagePanel in View.ImagePanels)
            {
                imagePanel.MultiSelection = framesSupport != null;
                imagePanel.AreaSelection = areaSupport != null;
            }

            foreach (string imageKey in Originals.Keys)
            {
                Originals[imageKey].CloneTo(Repository[imageKey]);
                Repository.OnItemChanged(imageKey);
            }
        }

        #endregion

        #region Image Panels

        private void ImagePanel_SelectedAreaChanged(object sender, EventArgs e)
        {
            IToolPanel toolPanel = View.ToolPanels.SelectedPanel;
            object tool = toolPanel?.SelectedTool;

            if (tool is ICustomAreaSupport && TuningCondition())
                ToolPanel_Tuning(toolPanel, e);
        }

        // TODO: FIX
        private void ImagePanel_SelectedIconsChanged(object sender, EventArgs e)
        {
            IImagePanel imagePanel = (IImagePanel)sender;
            string imageKey = View.ImagePanels[imagePanel];

            if (LastSelectedFrames.ContainsKey(imageKey) && Originals.ContainsKey(imageKey))
            {
                foreach (int frame in LastSelectedFrames[imageKey].Except(imagePanel.SelectedIcons))
                {
                    if (frame < Repository[imageKey].Count && frame < Originals[imageKey].Count)
                    {
                        Originals[imageKey][frame].EndPixelWorking();
                        Originals[imageKey][frame].CloneTo(Repository[imageKey][frame]);
                    }
                }
            }

            if (LastSelectedFrames.ContainsKey(imageKey))
                LastSelectedFrames[imageKey] = imagePanel.SelectedIcons;
            else
                LastSelectedFrames.Add(imageKey, imagePanel.SelectedIcons);

            IToolPanel toolPanel = View.ToolPanels.SelectedPanel;
            object selectedTool = toolPanel?.SelectedTool;

            if (selectedTool is ICustomFramesSupport && TuningCondition())
                ToolPanel_Tuning(toolPanel, e);
        }

        #endregion

        private void OnLoadDefaultSettings(object sender, EventArgs e)
        {
            string imageKey = View.ImagePanels.SelectedPanelKey;

            IImageList imageList = null;

            if (!string.IsNullOrEmpty(imageKey))
            {
                imageList = Originals.ContainsKey(imageKey)
                    ? Originals[imageKey]
                    : Repository[imageKey];
            }

            foreach (IImageListSupport tool in ImageListTools)
                tool.LoadSettings(imageList);

            foreach (IImageSupport tool in ImageTools)
                tool.LoadSettings(imageList?.FirstOrDefault());
        }
    }
}

