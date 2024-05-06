using System.Reflection;
using System.Runtime.InteropServices;
using Vintagestory.API.Common;

[assembly: AssemblyTitle("Proximity Prospecting Pick Mode")]
[assembly: AssemblyDescription("Adds a third prospecting pick mode allowing you to see the distance to closest ore block.")]
[assembly: AssemblyCompany("Shindou Jeikobu")]

[assembly: ComVisible(false)]
[assembly: Guid("55d051b3-13e7-41d2-833d-b02a501b8050")]

[assembly: AssemblyVersion("1.0.1")]
[assembly: AssemblyFileVersion("1.0.1")]

[assembly: ModInfo("Proximity Prospecting Pick Mode", "proximityprospectingpick",
    Version = "1.0.1",
    Description = "Adds a third prospecting pick mode allowing you to see the distance to closest ore block.",
    Authors = new[] { "Shindou Jeikobu" }
)]

[assembly: ModDependency("survival")]