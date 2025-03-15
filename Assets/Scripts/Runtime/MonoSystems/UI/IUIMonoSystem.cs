using LoJam.Core;
using System.Collections.Generic;

namespace LoJam
{
    public interface IUIMonoSystem : IMonoSystem
    {
        public void PushView(View view);
        public void PopView();

        public void RegisterView(View view);
        public void UnregisterView(View view);

        public IReadOnlyDictionary<string, View> GetViews();
    }
}
