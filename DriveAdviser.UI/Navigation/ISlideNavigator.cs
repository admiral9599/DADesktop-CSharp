using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveAdviser.UI.Navigation
{
    interface ISlideNavigator
    {
        void GoTo(int slideIndex);
        void GoTo(int slideIndex, Action setupSlide);
        void GoBack();
        void GoForward();
        
    }
}
