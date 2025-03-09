using UnityEngine;

namespace System.Runtime.CompilerServices {
    // C# is dumb
    public class IsExternalInit {

    }
}

namespace LoJam.Core
{
    public class GameEvents
    {
        public record TestEvent(string test = "YO");
    }
}
