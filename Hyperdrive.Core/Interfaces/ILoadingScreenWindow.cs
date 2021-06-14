using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Interfaces
{
    public interface ILoadingScreenWindow
    {
        event EventHandler CurrentProgressChanged;
        event EventHandler MaxProgressChanged;
        event EventHandler StatusChanged;
        event EventHandler ProgressCompelete;

        void LoadingScreenCancel();
        int LoadingScreenCurrentProgress();
        int LoadingScreenMaxProgress();
        string LoadingScreenStatusText();
    }
}
