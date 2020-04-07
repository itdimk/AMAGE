using AMAGE.Presentation.View.ImageEditor;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace AMAGE.UI.WPF.ImageEditor
{
    public partial class ImageEditor : Window, IImageEditor
    {
        public IMenuPanel MenuPanel { get; }
        public IPanelGroup<IImagePanel> ImagePanels { get; }
        public IPanelGroup<IToolPanel> ToolPanels { get; }

        public ImageEditor()
        {
            InitializeComponent();

            if (File.Exists("layout.xml"))
            {
                XmlLayoutSerializer serializer = new XmlLayoutSerializer(uiDockManager);
                serializer.Deserialize("layout.xml");

                LayoutAnchorable[] panels = uiDockManager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray();

                LayoutAnchorable menuPanel = panels.First(p => p.Content is IMenuPanel);
                menuPanel.Title = Properties.Resources.Menu;

                LayoutAnchorable imagePanels = panels.First(p => p.Content is IPanelGroup<IImagePanel>);
                imagePanels.Title = Properties.Resources.Image;

                LayoutAnchorable toolPanels = panels.First(p => p.Content is IPanelGroup<IToolPanel>);
                toolPanels.Title = Properties.Resources.Tools;

                MenuPanel = (IMenuPanel)menuPanel.Content;
                ImagePanels = (IPanelGroup<IImagePanel>)imagePanels.Content;
                ToolPanels = (IPanelGroup<IToolPanel>)toolPanels.Content;
            }
            else
            {
                MenuPanel = uiMenu;
                ImagePanels = uiImages;
                ToolPanels = uiTools;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            XmlLayoutSerializer serializer = new XmlLayoutSerializer(uiDockManager);
            serializer.Serialize("layout.xml");
        }
    }
}
