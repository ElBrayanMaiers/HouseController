namespace HouseController
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("controllerPage", typeof(Pages.ControllerPage));
        }
    }
}