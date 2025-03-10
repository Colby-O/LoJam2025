using LoJam.Core;

namespace LoJam
{
    public interface IUIMonoSystem : IMonoSystem
    {
        public void PushView(View view);
        public void PopView();
    }
}
