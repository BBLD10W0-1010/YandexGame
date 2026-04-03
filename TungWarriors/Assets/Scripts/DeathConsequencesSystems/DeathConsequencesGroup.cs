using Unity.Entities;

namespace Assets.Scripts.DeathConsequencesSystems
{
    public struct DeathEntityFlag : IEnableableComponent, IComponentData { }


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessDamageThisFrameSystem))]
    public partial class DeathConsequencesGroup : ComponentSystemGroup { }
}
