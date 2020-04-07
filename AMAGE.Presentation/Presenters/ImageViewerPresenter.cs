using AMAGE.Common.Imaging;
using AMAGE.Imaging;
using AMAGE.Presentation.View.ImageEditor;
using AMAGE.Services;
using AMAGE.Services.Imaging;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace AMAGE.Presentation.Presenters
{
    public class ImageViewerPresenter : BasePresenter<IImageEditor>
    {
        protected IRepository<IImageList> Repository;
        protected ISlideshowService SlideshowService;

        protected const string OpenFileFilter = "Images|*.jpg;*.png;*.gif;*.bmp|All|*.*";
        protected const string SaveFileFilter = "JPEG|*.jpg|PNG|*.png|GIF|*.gif|BMP|*.bmp";

        public ImageViewerPresenter(IAppController appController, IImageEditor view, ILogService logService,
            IMessageService messageService, IRepository<IImageList> repository, ISlideshowService slideshowService)
            : base(appController, view, logService, messageService)
        {
            Repository = repository;
            SlideshowService = slideshowService;

            SlideshowService.Repository = Repository;

            AppController.EventController
                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.OpenFile), MenuPanel_OpenFile)
                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.SaveFile), MenuPanel_SaveFile)
                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.CloseFile), MenuPanel_CloseFile)

                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.PlayAnimation), MenuPanel_PlayAnimation)
                .Subscribe(View.MenuPanel, nameof(View.MenuPanel.StopAnimation), MenuPanel_StopAnimation)

                .Subscribe<string>(Repository, nameof(Repository.ItemAdded), Repository_ItemAdded)
                .Subscribe<string>(Repository, nameof(Repository.ItemChanged), Repository_ItemChanged)
                .Subscribe<string>(Repository, nameof(Repository.ItemRemoved), Repository_ItemRemoved)

                .AddCondition(View.MenuPanel, nameof(View.MenuPanel.SaveFile), SaveFileCondition)
                .AddCondition(View.MenuPanel, nameof(View.MenuPanel.CloseFile), CloseFileCondition)
                .AddCondition(View.MenuPanel, nameof(View.MenuPanel.PlayAnimation), PlayAnimationCondition)
                .AddCondition(View.MenuPanel, nameof(View.MenuPanel.StopAnimation), StopAnimationCondition);
        }

        #region Conditions

        protected virtual bool PlayAnimationCondition() => View.ImagePanels.SelectedPanel != null;
        protected virtual bool StopAnimationCondition() => View.ImagePanels.SelectedPanel != null;
        protected virtual bool SaveFileCondition() => View.ImagePanels.SelectedPanel != null;
        protected virtual bool CloseFileCondition() => View.ImagePanels.SelectedPanel != null;

        #endregion

        #region Repository

        private void Repository_ItemAdded(object sender, string e)
        {
            IImageList image = Repository[e];
            IImagePanel imagePanel = View.ImagePanels.AddPanel(e, Path.GetFileName(e));

            imagePanel.SetImage(null);
            imagePanel.SetIcons(image.ToBitmapSources());

            AppController.EventController
                .Subscribe(imagePanel, nameof(imagePanel.SelectedIconsChanged), ImagePanel_SelectedIconsChanged);
        }

        private void ImagePanel_StopSlideshow(object sender, EventArgs e)
        {
            IImagePanel imagePanel = (IImagePanel)sender;
            string imageKey = View.ImagePanels[imagePanel];


            SlideshowService.StopSlideshow(imageKey);
        }

        private void ImagePanel_StartSlideshow(object sender, EventArgs e)
        {
            IImagePanel imagePanel = (IImagePanel)sender;
            string imageKey = View.ImagePanels[imagePanel];

            SlideshowService.StartSlideshow(imageKey, (o) => imagePanel.SetImage(o.ToBitmapSource()), 0);
        }

        private void Repository_ItemChanged(object sender, string e)
        {
            IImageList image = Repository[e];
            IImagePanel imagePanel = View.ImagePanels[e];

            int[] icons = imagePanel.SelectedIcons;
            int iconIndex = icons.Length > 0 ? icons[0] : -1;

            if (!SlideshowService.IsRunning(e))
                imagePanel.SetImage(image.ElementAtOrDefault(iconIndex)?.ToBitmapSource());

            imagePanel.SetIcons(image.ToBitmapSources());
        }

        private void Repository_ItemRemoved(object sender, string e)
        {
            View.ImagePanels.RemovePanel(e);
        }

        #endregion

        #region MenuPanel

        private void MenuPanel_OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = OpenFileFilter,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true && !Repository.ContainsKey(dialog.FileName))
            {
                try
                {
                    IImageList imageList = ImageList.Create();
                    imageList.FromFile(dialog.FileName);

                    Repository.Add(dialog.FileName, imageList);
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex.Message);
                    LogService.LogError(ex);
                }
            }
        }

        private void MenuPanel_SaveFile(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = SaveFileFilter
            };

            if (dialog.ShowDialog() == true)
            {
                string imageKey = View.ImagePanels.SelectedPanelKey;
                IImageList imageList = Repository[imageKey];
                imageList.ToFile(dialog.FileName, Path.GetExtension(dialog.FileName));
            }
        }

        private void MenuPanel_CloseFile(object sender, EventArgs e)
        {
            Repository.Remove(View.ImagePanels.SelectedPanelKey);
        }

        private void MenuPanel_StopAnimation(object sender, EventArgs e)
        {
            string imageKey = View.ImagePanels.SelectedPanelKey;

            SlideshowService.StopSlideshow(imageKey);
        }

        private void MenuPanel_PlayAnimation(object sender, EventArgs e)
        {
            IImagePanel imagePanel = View.ImagePanels.SelectedPanel;
            string imageKey = View.ImagePanels.SelectedPanelKey;

            SlideshowService.StartSlideshow(imageKey, (o) => imagePanel.SetImage(o.ToBitmapSource()), 0);
        }

        #endregion

        #region Image Panels

        private void ImagePanel_SelectedIconsChanged(object sender, EventArgs e)
        {
            IImagePanel imagePanel = (IImagePanel)sender;
            IImageList image = Repository[View.ImagePanels[imagePanel]];

            int[] icons = imagePanel.SelectedIcons;
            int iconIndex = icons.Length > 0 ? icons[0] : -1;

            imagePanel.SetImage(image.ElementAtOrDefault(iconIndex)?.ToBitmapSource());
        }

        #endregion
    }
}
