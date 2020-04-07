namespace AMAGE.Presentation.View.ImageEditor
{
    public interface IImageEditor : IView
    {
        IPanelGroup<IImagePanel> ImagePanels { get; }
        IPanelGroup<IToolPanel> ToolPanels { get; }
        IMenuPanel MenuPanel { get; }
    }
}
