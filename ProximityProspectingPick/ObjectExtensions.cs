using System.Runtime.CompilerServices;
using Vintagestory.API.Common;

namespace ProximityProspectingPick;

static class ObjectExtensions {
    // Kotlin: fun <T, R> T.let(block: (T) -> R): R
    public static R Let<T, R>(this T self, System.Func<T, R> block) {
        return block(self);
    }

    // Kotlin: fun <T> T.also(block: (T) -> Unit): T
    public static T Also<T>(this T self, Action<T> block) {
        block(self);
        return self;
    }

    public static bool IsPropickable(this Block self) {
        return self.Attributes?["propickable"]?.AsBool() == true;
    }
}