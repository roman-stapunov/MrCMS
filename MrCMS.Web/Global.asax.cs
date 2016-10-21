using MrCMS.Website;

namespace MrCMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : MrCMSApplication
    {
        protected override void OnApplicationStart()
        {
#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
            var log = log4net.LogManager.GetLogger(typeof(MvcApplication));
            log.Info("Startup Application");
#endif
        }
    }
}