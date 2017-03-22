using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UwpDroneController
{
    class Helpers
    {

        public static async Task RunOnCoreDispatcherIfPossible(Action action, bool runAnyway = true)
        {
            CoreDispatcher dispatcher = null;

            try
            {
                dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            }
            catch { }

            if (dispatcher != null)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { action.Invoke(); });
            }
            else if (runAnyway)
            {
                action.Invoke();
            }
        }
    }
}
